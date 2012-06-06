////////////////////////////////////////////////////////////////////////////////// 
//                                                                              //
//      Copyright (C) 2005-2011 nzsjb                                           //
//                                                                              //
//  This Program is free software; you can redistribute it and/or modify        //
//  it under the terms of the GNU General Public License as published by        //
//  the Free Software Foundation; either version 2, or (at your option)         //
//  any later version.                                                          //
//                                                                              //
//  This Program is distributed in the hope that it will be useful,             //
//  but WITHOUT ANY WARRANTY; without even the implied warranty of              //
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                //
//  GNU General Public License for more details.                                //
//                                                                              //
//  You should have received a copy of the GNU General Public License           //
//  along with GNU Make; see the file COPYING.  If not, write to                //
//  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.       //
//  http://www.gnu.org/copyleft/gpl.html                                        //
//                                                                              //  
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.ComponentModel;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Diagnostics;

using DomainObjects;

namespace DVBLogicPlugin
{
    public class PluginMonitor
    {
        internal int MonitorIdentity { get { return (monitorIdentity); } }
 
        private enum status
        {
            unknown = 0,
            inProgress,
            finishedError,
            finishSuccess,
            finishAborted
        };

        private int monitorIdentity;

        private string directory;
        private status pluginStatus = status.unknown;

        private string frequency;
        private Process collectionProcess;
        private string runReference;

        private Mutex cancelMutex;

        public PluginMonitor(int monitorIdentity, string directory)
        {
            this.monitorIdentity = monitorIdentity;
            this.directory = directory;

            pluginStatus = status.inProgress;            
        }

        public bool StartScan(int bufferAddress, IntPtr scanInfo)
        {
            byte[] scanData = new byte[256];

            byte scanByte = 0xff;
            int index = 0;

            while (scanByte != 0x00)
            {
                scanByte = Marshal.ReadByte(scanInfo, index);
                if (scanByte != 0x00)
                {
                    scanData[index] = scanByte;
                    index++;
                }
            }

            MemoryStream memoryStream = new MemoryStream(scanData, 0, index);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.CloseInput = true;
            settings.IgnoreWhitespace = true;
            XmlReader reader = XmlReader.Create(memoryStream, settings);

            try
            {
                while (!reader.EOF)
                {
                    reader.Read();
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "frequency":
                                frequency = reader.ReadString();
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (XmlException e)
            {
                Logger.Instance.Write("<E> Failed to parse scan info");
                Logger.Instance.Write("<E> Data exception: " + e.Message);
                return (false);
            }
            catch (IOException e)
            {
                Logger.Instance.Write("<E> Failed to parse scan info");
                Logger.Instance.Write("<E> I/O exception: " + e.Message);
                return (false);
            }

            reader.Close();

            string actualFileName = Path.Combine(directory, frequency.ToString()) + ".ini";
            Logger.Instance.Write("Running collection with parameters from " + actualFileName);

            if (!File.Exists(actualFileName))
            {
                Logger.Instance.Write("<e> The collection parameters do not exist - collection will be abandoned");
                pluginStatus = status.finishedError;
                return (false);
            }

            runReference = Process.GetCurrentProcess().Id + "-" + monitorIdentity;

            string cancellationName = "EPG Collector Cancellation Mutex " + runReference;
            Logger.Instance.Write("Cancellation mutex name is " + cancellationName);
            cancelMutex = new Mutex(true, cancellationName); 

            collectionProcess = new Process();

            collectionProcess.StartInfo.FileName = Path.Combine(RunParameters.BaseDirectory, "EPGCollector.exe");
            collectionProcess.StartInfo.WorkingDirectory = RunParameters.BaseDirectory;
            collectionProcess.StartInfo.Arguments = @"/ini=" + '"' + actualFileName + '"' + " /plugin=" + runReference;
            collectionProcess.StartInfo.UseShellExecute = false;
            collectionProcess.StartInfo.CreateNoWindow = true;
            collectionProcess.EnableRaisingEvents = true;
            collectionProcess.Exited += new EventHandler(collectionProcessExited);

            collectionProcess.Start();

            pluginStatus = status.inProgress;

            return (true);
        }

        private void collectionProcessExited(object sender, EventArgs e)
        {
            int exitCode = collectionProcess.ExitCode;
            Logger.Instance.Write("Plugin notified that collection has completed with code " + exitCode);

            collectionProcess.Close();
            collectionProcess = null;

            cancelMutex.Close();
            cancelMutex = null;

            if (exitCode == 0)
                pluginStatus = status.finishSuccess;
            else
                pluginStatus = status.finishedError;
        }

        public bool StopScan()
        {
            if (cancelMutex != null)
            {
                Logger.Instance.Write("Plugin is releasing cancellation mutex");
                cancelMutex.ReleaseMutex();
            }
            
            pluginStatus = status.finishAborted;

            return (true);
        }

        public int GetScanStatus()
        {
            return ((int)pluginStatus);
        }

        public int GetEPGData(IntPtr buffer, int bufferSize)
        {
            string fileName = Path.Combine(directory, "EPG Collector Plugin.xml");
            FileInfo fileInfo = new FileInfo(fileName);
            int fileLength = 0;
            if (fileInfo.Exists)
                fileLength = (int)fileInfo.Length;

            if (bufferSize == 0)
            {
                if (fileLength != 0)
                {
                    Logger.Instance.Write("Replying with buffer size = " + (fileLength + 1));
                    return (fileLength + 1);
                }
                else
                {
                    Logger.Instance.Write("Replying with buffer size 0 for non-buffered output");
                    fileInfo = null;
                    GC.Collect();
                    return (0);
                }
            }
            else
            {
                Logger.Instance.Write("Passing " + (fileLength + 1) + " bytes");

                FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                bool eof = false;
                byte[] readBuffer = new byte[1024 * 1024];

                do
                {
                    int readCount = fileStream.Read(readBuffer, 0, 1024 * 1024);
                    if (readCount != 0)
                    {
                        Marshal.Copy(readBuffer, 0, buffer, readCount);
                        buffer = new IntPtr(buffer.ToInt64() + readCount);
                    }
                    eof = (readCount != 1024 * 1024);
                }
                while (!eof);

                fileStream.Close();
                fileInfo.Delete();

                Marshal.WriteByte(buffer, 0x00);
                Logger.Instance.Write("Data transfer complete");

                return (fileLength + 1);
            }
        }
    }
}
