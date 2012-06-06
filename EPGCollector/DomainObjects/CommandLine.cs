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

using System.IO;

namespace DomainObjects
{
    /// <summary>
    /// The class that processes and stores command line parameters.
    /// </summary>
    public sealed class CommandLine
    {
        /// <summary>
        /// Returns true if the run is to query the tuners; false otherwise.
        /// </summary>
        public static bool TunerQueryOnly { get { return (tunerQueryOnly); } }
        /// <summary>
        /// Get the initialization file name.
        /// </summary>
        public static string IniFileName { get { return (iniFileName); } }
        /// <summary>
        /// Get the background mode.
        /// </summary>
        public static bool BackgroundMode { get { return (backgroundMode); } }
        /// <summary>
        /// Get the plugin mode.
        /// </summary>
        public static bool PluginMode { get { return (pluginMode); } }
        /// <summary>
        /// Get the run reference number.
        /// </summary>
        public static string RunReference { get { return (runReference); } }

        private static bool tunerQueryOnly;
        private static string iniFileName = Path.Combine(RunParameters.DataDirectory, "EPG Collector.ini");
        private static bool backgroundMode;
        private static bool pluginMode;
        private static string runReference;

        private CommandLine() { }

        /// <summary>
        /// Process the command line.
        /// </summary>
        /// <param name="args">The command line parameters.</param>
        /// <returns>True if the command line parameters are valid; false otherwise.</returns>
        public static bool Process(string[] args)
        {
            if (args.Length == 0)
                return (true);

            foreach (string arg in args)
            {
                Logger.Instance.Write("Processing command line parameter: " + arg);

                string[] parts = arg.Split(new char[] { '=' });

                switch (parts[0].ToUpperInvariant())
                {
                    case "/TUNERS":
                        if (args.Length != 1 || parts.Length != 1)
                        {
                            Logger.Instance.Write("Command line parameter wrong");
                            return (false);
                        }
                        else
                        {
                            tunerQueryOnly = true;
                            return (true);
                        }
                    case "/INI":
                        if (parts.Length != 2 || parts[1].Trim().Length == 0)
                        {
                            Logger.Instance.Write("Command line parameter wrong");
                            return (false);
                        }
                        else
                            iniFileName = parts[1];
                        break;
                    case "/BACKGROUND":
                        if (args.Length != 2 || parts.Length != 2)
                        {
                            Logger.Instance.Write("Command line parameter wrong");
                            return (false);
                        }
                        else
                        {
                            backgroundMode = true;
                            runReference = parts[1];
                        }
                        break;
                    case "/PLUGIN":
                        if (args.Length != 2 || parts.Length != 2)
                        {
                            Logger.Instance.Write("Command line parameter wrong");
                            return (false);
                        }
                        else
                        {
                            pluginMode = true;
                            runReference = parts[1];
                        }
                        break;
                    default:
                        Logger.Instance.Write("Command line parameter not recognized: " + parts[0]);
                        return (false);
                }
            }

            return (true);
        }

        /// <summary>
        /// Get a description of the program exit code.
        /// </summary>
        /// <param name="code">The exit code.</param>
        /// <returns>A description of the exit code.</returns>
        public static string GetCompletionCodeDescription(int code)
        {
            switch (code)
            {
                case 0:
                    return ("The run completed successfully.");
                case 1:
                    return ("No DVB tuners installed.");
                case 2:
                    return ("The initialization file cannot be located.");
                case 3:
                    return ("The initialization file has a parameter error.");
                case 4:
                    return ("The command line is incorrect.");
                case 5:
                    return ("A program exception occurred.");
                case 6:
                    return ("The EPG data is incomplete.");
                case 7:
                    return ("The collection was abandoned by the user.");
                case 8:
                    return ("The initialization file parameters do not mact the tuner configuration.");
                case 9:
                    return ("The log file cannot be written.");
                case 10:
                    return ("Some frequencies could not be processed. See the log for details");
                case 11:
                    return ("The output file could not be created.");
                case 12:
                    return ("The simulation file could not be located.");
                case 13:
                    return ("The collection finished normally but no data was collected.");
                default:
                    return ("The exit code is not recognized.");
            }
        }
    }
}
