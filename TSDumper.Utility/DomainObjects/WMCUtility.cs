////////////////////////////////////////////////////////////////////////////////// 
//                                                                              //
//      Copyright (C) 2005-2012 nzsjb, Harun Esur                                           //
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using DomainObjects;

namespace DomainObjects
{
    public sealed class WMCUtility
    {
        private static Process process;
        private static bool exited;

        public static string Run(string description, string arguments)
        {
            Logger.Instance.Write("Running Windows Media Centre Utility to " + description);

            process = new Process();

            process.StartInfo.FileName = "WMCUtility.exe";
            process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;

            if (arguments != null)
                process.StartInfo.Arguments = arguments;

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.EnableRaisingEvents = true;
            process.Exited += new EventHandler(processExited);
            process.OutputDataReceived += new DataReceivedEventHandler(processOutputDataReceived);
            process.ErrorDataReceived += new DataReceivedEventHandler(processErrorDataReceived);

            exited = false;

            try
            {
                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                while (!exited)
                    Thread.Sleep(500);

                Logger.Instance.Write("Windows Media Centre Utility has completed: exit code " + process.ExitCode);
                if (process.ExitCode == 0)
                    return (null);
                else
                    return ("Windows Media Centre failed: reply code " + process.ExitCode);
            }
            catch (Exception e)
            {
                Logger.Instance.Write("<e> Failed to run the Windows Media Centre Utility");
                Logger.Instance.Write("<e> " + e.Message);
                return ("Failed to run Windows Media Centre Utility due to an exception");
            }
        }

        private static void processOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;

            Logger.Instance.Write(e.Data);
        }

        private static void processErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;

            Logger.Instance.Write(e.Data);
        }

        private static void processExited(object sender, EventArgs e)
        {
            exited = true;
        }
    }
}
