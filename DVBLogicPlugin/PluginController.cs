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
using System.Collections.ObjectModel;

using DomainObjects;

namespace DVBLogicPlugin
{
    public class PluginController
    {
        /// <summary>
        /// Get the full assembly version number.
        /// </summary>
        public static string AssemblyVersion
        {
            get
            {
                System.Version version = Assembly.GetExecutingAssembly().GetName().Version;
                return (version.Major + "." + version.Minor + "." + version.Build + "." + version.Revision);
            }
        }

        /// <summary>
        /// Get the plugin instance.
        /// </summary>
        public static PluginController Instance
        {
            get
            {
                if (instance == null)
                    instance = new PluginController();
                return (instance);
            }
        }        

        private static LockClass lockClass = new LockClass(); 
        private static PluginController instance;                
        
        private Collection<PluginMonitor> pluginMonitors;
        private int lastMonitorIdentity;

        public PluginController() 
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(unhandledException);

            Logger.Instance.WriteSeparator("DVB Logic Plugin (Version " + RunParameters.SystemVersion + ")");

            Logger.Instance.Write("Plugin build: " + AssemblyVersion);
            Logger.Instance.Write("");
            Logger.Instance.Write("Privilege level: " + RunParameters.Role);
            Logger.Instance.Write("");

            pluginMonitors = new Collection<PluginMonitor>();
        }

        public int Init(string workingDirectory, string baseDirectory)
        {
            lock (lockClass)
            {
                int endIndex = baseDirectory.LastIndexOf(Path.DirectorySeparatorChar);
                if (endIndex == -1)
                    return (-1);

                RunParameters.BaseDirectory = baseDirectory.Substring(0, endIndex);
                Logger.Instance.Write("Base directory: " + RunParameters.BaseDirectory);
                Logger.Instance.Write("Data directory: " + RunParameters.DataDirectory);
                Logger.Instance.Write("EPG directory: " + workingDirectory);
                Logger.Instance.Write("");

                if (lastMonitorIdentity == Int32.MaxValue)
                    lastMonitorIdentity = 1;
                else
                    lastMonitorIdentity++;

                pluginMonitors.Add(new PluginMonitor(lastMonitorIdentity, workingDirectory));
                Logger.Instance.Write("Created plugin monitor " + lastMonitorIdentity);

                return (lastMonitorIdentity);
            }
        }

        private void unhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception;

            if (exception != null)
            {
                while (exception.InnerException != null)
                    exception = exception.InnerException;

                Logger.Instance.Write("<E> ** The program has failed with an exception");
                Logger.Instance.Write("<E> ** Exception: " + exception.Message);
                Logger.Instance.Write("<E> ** Location: " + exception.StackTrace);
            }
            else
                Logger.Instance.Write("<E> An unhandled exception of type " + e.ExceptionObject + " has occurred");
        }

        public bool StartScan(int monitorIdentity, int bufferAddress, IntPtr scanInfo)
        {
            lock (lockClass)
            {
                Logger.Instance.Write("Start scan called for plugin monitor " + monitorIdentity);

                foreach (PluginMonitor pluginMonitor in pluginMonitors)
                {
                    if (pluginMonitor.MonitorIdentity == monitorIdentity)
                        return (pluginMonitor.StartScan(bufferAddress, scanInfo));
                }

                Logger.Instance.Write("Plugin monitor " + monitorIdentity + " does not exist for Start Scan");
                return (false);
            }
        }

        public bool StopScan(int monitorIdentity)
        {
            lock (lockClass)
            {
                Logger.Instance.Write("Stop scan called for plugin monitor " + monitorIdentity);

                foreach (PluginMonitor pluginMonitor in pluginMonitors)
                {
                    if (pluginMonitor.MonitorIdentity == monitorIdentity)
                    {
                        bool reply = pluginMonitor.StopScan();

                        pluginMonitors.Remove(pluginMonitor);
                        Logger.Instance.Write("Plugin monitor " + monitorIdentity + " has been deleted");

                        return (reply);
                    }
                }

                Logger.Instance.Write("Plugin monitor " + monitorIdentity + " does not exist for Stop Scan");
                return (false);
            }
        }

        public int GetScanStatus(int monitorIdentity)
        {
            lock (lockClass)
            {
                foreach (PluginMonitor pluginMonitor in pluginMonitors)
                {
                    if (pluginMonitor.MonitorIdentity == monitorIdentity)
                        return (pluginMonitor.GetScanStatus());
                }

                Logger.Instance.Write("Plugin monitor " + monitorIdentity + " does not exist for Get Scan Status");
                return (0);
            }
        }

        public int GetEPGData(int monitorIdentity, IntPtr buffer, int bufferSize)
        {
            lock (lockClass)
            {
                Logger.Instance.Write("Get EPG data called for plugin monitor " + monitorIdentity);
                Logger.Instance.Write("Get EPG data buffer address=" + buffer + " size=" + bufferSize);

                foreach (PluginMonitor pluginMonitor in pluginMonitors)
                {
                    if (pluginMonitor.MonitorIdentity == monitorIdentity)
                    {
                        int dataSize = pluginMonitor.GetEPGData(buffer, bufferSize);

                        if (dataSize == 0)
                        {
                            pluginMonitors.Remove(pluginMonitor);
                            Logger.Instance.Write("Plugin monitor " + monitorIdentity + " has been deleted");
                        }
                        else
                        {
                            if (buffer.ToInt64() != 0 && bufferSize != 0)
                            {
                                pluginMonitors.Remove(pluginMonitor);
                                Logger.Instance.Write("Plugin monitor " + monitorIdentity + " has been deleted");
                            }
                        }

                        return (dataSize);
                    }
                }

                Logger.Instance.Write("Plugin monitor " + monitorIdentity + " does not exist for Get EPG Data");
                return (0);
            }
        }

        private class LockClass { }
    }
}
