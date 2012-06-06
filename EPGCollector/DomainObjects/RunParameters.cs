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
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.Security.Principal;

namespace DomainObjects
{
    /// <summary>
    /// The class that processes the initialization file.
    /// </summary>
    public class RunParameters
    {
        /// <summary>
        /// Return true if running under any version of Windows.
        /// </summary>
        public static bool IsWindows
        {
            get
            {
                return (Environment.OSVersion.Platform == PlatformID.Win32NT ||
                    Environment.OSVersion.Platform == PlatformID.Win32S ||
                    Environment.OSVersion.Platform == PlatformID.Win32Windows ||
                    Environment.OSVersion.Platform == PlatformID.WinCE);
                     
            }
        }

        /// <summary>
        /// Return true if running under 64-bit Windows
        /// </summary>
        public static bool Is64Bit { get { return (Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE").Contains("64")); } }

        /// <summary>
        /// Get the system version number.
        /// </summary>
        public static string SystemVersion 
        { 
            get 
            {
                System.Version version = Assembly.GetExecutingAssembly().GetName().Version;
                return (version.Major + "." + version.Minor); 
            } 
        }

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
        /// Get the privilege level.
        /// </summary>
        public static string Role
        {
            get
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                if (principal.IsInRole(WindowsBuiltInRole.Administrator))
                    return("Administrator");
                if (principal.IsInRole(WindowsBuiltInRole.User))
                    return("User");
                if (principal.IsInRole(WindowsBuiltInRole.Guest))
                    return("Guest");
                return ("Other");
            }
        }

        /// <summary>
        /// Get or set the current run parameters instance.
        /// </summary>
        public static RunParameters Instance 
        { 
            get 
            {
                if (instance == null)
                {
                    instance = new RunParameters(ParameterSet.Collector);
                    TuningFrequency.FrequencyCollection.Clear();
                    TVStation.StationCollection.Clear();
                }
                return (instance); 
            }
            set { instance = value; }
        }

        /// <summary>
        /// Get or set the application base directory.
        /// </summary>
        public static string BaseDirectory
        {
            get { return (baseDirectory); }
            set { baseDirectory = value; }
        }

        /// <summary>
        /// Get the application data directory.
        /// </summary>
        public static string DataDirectory 
        { 
            get 
            {
                if (applicationDirectory == null)
                {
                    WindowsIdentity identity = WindowsIdentity.GetCurrent();
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    if (principal.IsInRole(WindowsBuiltInRole.Administrator))

                        applicationDirectory = Environment.CurrentDirectory;// Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Path.Combine("Geekzone", "EPG Collector"));
                    else
                        applicationDirectory = Environment.CurrentDirectory; // Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Path.Combine("Geekzone", "EPG Collector"));
                    if (!Directory.Exists(applicationDirectory))
                        Directory.CreateDirectory(applicationDirectory);
                }
                return (applicationDirectory); 
            } 
        }

        /// <summary>
        /// Get the application configuration directory.
        /// </summary>
        public static string ConfigDirectory { get { return (Path.Combine(baseDirectory, "Configuration")); } }

        /// <summary>
        /// Get the selected tuners.
        /// </summary>
        public Collection<int> SelectedTuners 
        { 
            get 
            {
                if (selectedTuners == null)
                    selectedTuners = new Collection<int>();
                return (selectedTuners); 
            } 
        }
        
        /// <summary>
        /// Get or set the output file name.
        /// </summary>
        /// <remarks>
        /// The default name will be returned if it is not overridden by the initialization file.
        /// </remarks>
        public string OutputFileName 
        { 
            get { return (outputFileName); }
            set { outputFileName = value; }
        }

        /// <summary>
        /// Get the INI file name.
        /// </summary>
        /// <remarks>
        /// The default name will be returned if it is not overridden by the initialization file.
        /// </remarks>
        public string IniFileName { get { return (iniFileName); } }
        
        /// <summary>
        /// Get or set the timeout for acquiring data for a frequency.
        /// </summary>
        /// <remarks>
        /// The default of 300 seconds will be returned if it is not overridden by the initialization file.
        /// </remarks>
        public TimeSpan FrequencyTimeout 
        { 
            get { return (frequencyTimeout); }
            set { frequencyTimeout = value; }
        }
        
        /// <summary>
        /// Get or set the timeout for acquiring a signal lock and receiving station information.
        /// </summary>
        /// <remarks>
        /// The default of 10 seconds will be returned if it is not overridden by the initialization file.
        /// </remarks>
        public TimeSpan LockTimeout 
        { 
            get { return (lockTimeout); }
            set { lockTimeout = value; }
        }

        /// <summary>
        /// Get or set the number of repeats for collections that cannot determine data complete.
        /// </summary>
        /// <remarks>
        /// The default of 5 will be returned if it is not overridden by the initialization file.
        /// </remarks>
        public int Repeats 
        { 
            get { return (repeats); }
            set { repeats = value; }
        }

        /// <summary>
        /// Get or set the country code.
        /// </summary>
        public string CountryCode 
        { 
            get { return (countryCode); }
            set { countryCode = value; }
        }

        /// <summary>
        /// Get or set the region.
        /// </summary>
        public int Region 
        { 
            get { return (region); }
            set { region = value; }
        }

        /// <summary>
        /// Get or set the input language code.
        /// </summary>
        public string InputLanguage
        {
            get { return (inputLanguage); }
            set { inputLanguage = value; }
        }

        /// <summary>
        /// Get or set the output language code.
        /// </summary>
        public string OutputLanguage
        {
            get { return (outputLanguage); }
            set { outputLanguage = value; }
        }
        
        /// <summary>
        /// Get or set the timezone.
        /// </summary>
        public TimeSpan TimeZone 
        { 
            get { return (timeZone); }
            set { timeZone = value; }
        }

        /// <summary>
        /// Get or set the next timezone.
        /// </summary>
        public TimeSpan NextTimeZone
        {
            get { return (nextTimeZone); }
            set { nextTimeZone = value; }
        }

        /// <summary>
        /// Get or set the date/time of the next timezone change.
        /// </summary>
        public DateTime NextTimeZoneChange
        {
            get { return (nextTimeZoneChange); }
            set { nextTimeZoneChange = value; }
        }
        
        /// <summary>
        /// Get or set an indication of whether the timezone has been set.
        /// </summary>
        public bool TimeZoneSet 
        { 
            get { return (timeZoneSet); }
            set { timeZoneSet = value; }
        }

        /// <summary>
        /// Get the collection of options.
        /// </summary>
        public Collection<string> Options { get { return (options); } }
        
        /// <summary>
        /// Get the collection of trace ID's.
        /// </summary>
        public Collection<string> TraceIDs { get { return (traceIDs); } }
        
        /// <summary>
        /// Get the collection of debug ID's.
        /// </summary>
        public Collection<string> DebugIDs { get { return (debugIDs); } }
        
        /// <summary>
        /// Get or set the TS dump file name.
        /// </summary>
        public string TSFileName 
        { 
            get { return (tsFileName); }
            set { tsFileName = value; }
        }
        
        /// <summary>
        /// Get or set the channel bouquet.
        /// </summary>
        public int ChannelBouquet 
        { 
            get { return (channelBouquet); }
            set { channelBouquet = value; }
        }

        /// <summary>
        /// Get or set the channel region.
        /// </summary>
        public int ChannelRegion 
        { 
            get { return (channelRegion); }
            set { channelRegion = value; }
        }

        /// <summary>
        /// Get the diseqc string.
        /// </summary>
        public string DiseqcIdentity { get { return (diseqcIdentity); } }
        
        /// <summary>
        /// Get or set the character set.
        /// </summary>
        public string CharacterSet 
        { 
            get { return (characterSet); }
            set { characterSet = value; }
        }
        
        /// <summary>
        /// Get or set the EIT PID number.
        /// </summary>
        public int EITPid 
        { 
            get { return (eitPid); }
            set { eitPid = value; }
        }

        /// <summary>
        /// Get or set the MHW1 PID numbers.
        /// </summary>
        public int[] MHW1Pids 
        { 
            get { return (mhw1Pids); }
            set { mhw1Pids = value; }
        }

        /// <summary>
        /// Get or set the MHW2 PID numbers.
        /// </summary>
        public int[] MHW2Pids 
        { 
            get { return (mhw2Pids); }
            set { mhw2Pids = value; }
        }

        /// <summary>
        /// Get or set the maximum service ID.
        /// </summary>
        public int MaxService
        {
            get { return (maxService); }
            set { maxService = value; }
        }

        /// <summary>
        /// Get or set the WMC import name used in the MXF file.
        /// </summary>
        public string WMCImportName
        {
            get { return (wmcImportName); }
            set { wmcImportName = value; }
        }

        /// <summary>
        /// Get or set the plugin Frequency details.
        /// </summary>
        public TuningFrequency PluginFrequency 
        { 
            get { return (pluginFrequency); }
            set { pluginFrequency = value; }
        }
        
        /// <summary>
        /// Get an indication of whether the output file name has been set.
        /// </summary>
        public bool OutputFileSet { get { return (outputFileSet); } }

        private static RunParameters instance;
        
        private static string baseDirectory;
        private static string applicationDirectory;

        private ParameterSet parameterSet = ParameterSet.Collector;
        
        private Collection<int> selectedTuners;
        private string outputFileName = Path.Combine(DataDirectory, "TVGuide.xml");
        private bool outputFileSet;
        private string iniFileName = "";

        private TimeSpan frequencyTimeout = new TimeSpan(0, 5, 0);
        private TimeSpan lockTimeout = new TimeSpan(0, 0, 10);
        private int repeats = 5;

        private string countryCode;
        private int region;
        private string inputLanguage;
        private string outputLanguage;
        private TimeSpan timeZone;
        private TimeSpan nextTimeZone;
        private DateTime nextTimeZoneChange;
        private bool timeZoneSet;

        private int channelBouquet = -1;
        private int channelRegion = -1;

        private string diseqcIdentity;

        private string characterSet;

        private int eitPid = -1;
        private int[] mhw1Pids;
        private int[] mhw2Pids;

        private string wmcImportName;

        private int maxService = -1;

        private TuningFrequency pluginFrequency;
        
        private Collection<string> options = new Collection<string>();
        private Collection<string> traceIDs = new Collection<string>();
        private Collection<string> debugIDs = new Collection<string>();

        private string tsFileName;

        private Collection<TVStation> originalStations;
        private Collection<TuningFrequency> originalFrequencies;
        private Collection<ChannelFilterEntry> originalFilters;
        private Collection<TimeOffsetChannel> originalTimeOffsets;
        private Collection<RepeatExclusion> originalRepeatExclusions;
        private Collection<string> originalPhrasesToIgnore;

        private Satellite currentSatellite;
        private SatelliteDish currentDish;
        private TuningFrequency currentFrequency;

        private const int errorCodeNoError = 0;
        private const int errorCodeNoFile = 2;
        private const int errorCodeFormatError = 3;

        private RunParameters()  { }

        /// <summary>
        /// Initialise a new instance of the RunParameters class.
        /// </summary>
        /// <param name="parameterSet">The type of parameters this instance will hold.</param>
        public RunParameters(ParameterSet parameterSet) 
        {
            this.parameterSet = parameterSet;
        }

        /// <summary>
        /// Process a parameter file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>True if the parameters are valid; false otherwise.</returns>
        public int Process(string fileName)
        {
            FileStream fileStream = null;
            
            try { fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read); }
            catch (IOException)
            {
                Logger.Instance.Write("Failed to open " + fileName);
                return (errorCodeNoFile);
            }

            int reply = errorCodeNoError;

            StreamReader streamReader = new StreamReader(fileStream);

            while (!streamReader.EndOfStream && reply == errorCodeNoError)
            {
                string line = streamReader.ReadLine();
                string processLine = line.Replace((char)0x09, ' ');
                processLine = processLine.Replace("\ufffd", "?");

                int commentIndex = processLine.LastIndexOf("//");
                if (commentIndex != -1)
                    processLine = processLine.Substring(0, commentIndex);

                char splitter = ':';
                if (processLine.IndexOf('=') != -1)
                    splitter = '=';

                string[] parts = processLine.Split(new char[] { splitter });
                if (parts.Length > 0)
                {
                    switch (parts[0].Trim().ToUpperInvariant())
                    {
                        case "OUTPUT":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            if (parts.Length > 2)
                                reply = processOutput(parts[1] + ":" + parts[2]);
                            else
                                reply = processOutput(parts[1]);
                            break;
                        case "TUNER":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processTuner(parts[1]);
                            break;
                        case "SATELLITE":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processSatellite(parts[1]);
                            break;
                        case "DISH":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processDish(parts[1]);
                            break;
                        case "FREQUENCY":
                            currentFrequency = null;
                            reply = processFrequency(parts[1]);
                            break;
                        case "SCANNINGFREQUENCY":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processFrequency(parts[1]);
                            break;
                        case "TIMEOUTS":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processTimeouts(parts[1]);
                            break;
                        case "STATION":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processStation(parts[1]);
                            break;
                        case "OPTION":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processOption(parts[1]);
                            break;
                        case "TRACE":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processTrace(parts[1]);
                            break;
                        case "DEBUG":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processDebug(parts[1]);
                            break;
                        case "LOCATION":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processLocation(parts[1]);
                            break;
                        case "LANGUAGE":
                        case "INPUTLANGUAGE":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processInputLanguage(parts[1]);
                            break;
                        case "OUTPUTLANGUAGE":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processOutputLanguage(parts[1]);
                            break;
                        case "TIMEZONE":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processTimeZone(parts[1]);
                            break;
                        case "TSFILE":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            if (parts.Length > 2)
                                reply = processTSFile(parts[1] + ":" + parts[2]);
                            else
                                reply = processTSFile(parts[1]);
                            break;
                        case "CHANNELS":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processChannel(parts[1]);
                            break;
                        case "DISEQC":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processDiseqc(parts[1]);
                            break;
                        case "CHARSET":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processCharSet(parts[1]);
                            break;
                        case "EITPID":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processEITPid(parts[1]);
                            break;
                        case "MHW1PIDS":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processMHW1Pids(parts[1]);
                            break;
                        case "MHW2PIDS":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processMHW2Pids(parts[1]);
                            break;
                        case "PLUGINFREQUENCY":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processPluginFrequency(parts[1]);
                            break;
                        case "[DVBS]":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            currentFrequency = new SatelliteFrequency();
                            break;
                        case "[DVBT]":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            currentFrequency = new TerrestrialFrequency();
                            break;
                        case "[DVBC]":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            currentFrequency = new CableFrequency();
                            break;
                        case "[ATSC]":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            currentFrequency = new AtscFrequency();
                            break;
                        case "[CLEARQAM]":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            currentFrequency = new ClearQamFrequency();
                            break;
                        case "[ISDBS]":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            currentFrequency = new ISDBSatelliteFrequency();
                            break;
                        case "[ISDBT]":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            currentFrequency = new ISDBTerrestrialFrequency();
                            break;
                        case "TUNINGFILE":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processTuningFile(parts[1]);                            
                            break;
                        case "SCANNED":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processScannedChannel(parts[1]);
                            break;
                        case "OFFSET":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processOffsetChannel(parts[1]);
                            break;
                        case "INCLUDESERVICE":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processIncludeService(parts[1]);
                            break;
                        case "MAXSERVICE":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processMaxService(parts[1]);
                            break;
                        case "REPEATEXCLUSION":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processRepeatExclusion(parts[1]);
                            break;
                        case "REPEATPHRASE":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processRepeatPhrase(parts[1]);
                            break;
                        case "WMCIMPORTNAME":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            reply = processWMCImportName(parts[1]);
                            break;
                        case "[GENERAL]":
                        case "[DIAGNOSTICS]":
                        case "[SCANLIST]":
                        case "[STATIONS]":
                        case "[OFFSETS]":
                        case "[SERVICEFILTERS]":
                        case "[REPEATEXCLUSIONS]":
                            Logger.Instance.Write("Processing ini parameter: " + line);
                            break;                            
                        default:
                            if (parts[0].Trim().ToUpperInvariant().StartsWith("SCANNED"))
                            {
                                Logger.Instance.Write("Processing ini parameter: " + line);
                                reply = processScannedChannel(parts[1]);   
                            }
                            break;
                    }                        
                }
            }

            streamReader.Close();
            fileStream.Close();

            return (reply);            
        }

        private int processOutput(string parts)
        {          
            if (parts == string.Empty)
            {
                Logger.Instance.Write("INI file format error: The Output file name is wrong.");
                return (errorCodeFormatError);
            }

            outputFileName = parts.Trim().Replace("<ApplicationData>", RunParameters.DataDirectory);
            outputFileSet = true;

            return (errorCodeNoError);
        }

        private int processTuner(string parts)
        {
            if (parts.ToUpperInvariant() == "F")
                return (errorCodeNoError);

            try
            {
                string[] parameters = parts.Split(new char[] { ',' });

                foreach (string parameter in parameters)
                {
                    int selectedTuner = Int32.Parse(parameter);

                    if (selectedTuner < 1)
                    {
                        Logger.Instance.Write("INI file format error: The Tuner number is out of range.");
                        return (errorCodeFormatError);
                    }
                    else
                        SelectedTuners.Add(selectedTuner);
                }
            }
            catch (FormatException)
            {
                Logger.Instance.Write("INI file format error: The Tuner line is wrong.");
                return (errorCodeFormatError);
            }
            catch (ArithmeticException)
            {
                Logger.Instance.Write("INI file format error: The Tuner line is wrong.");
                return (errorCodeFormatError);
            }

            return (errorCodeNoError);
        }

        private int processSatellite(string parts)
        {
            string[] parameters = parts.Split(new char[] { ',' });
            if (parameters.Length != 1)
            {
                Logger.Instance.Write("INI file format error: The Satellite line is wrong.");
                return (errorCodeFormatError);
            }

            try
            {
                int longitude = Int32.Parse(parameters[0].Trim());

                currentSatellite = Satellite.FindSatellite(longitude);
                if (currentSatellite == null)
                    currentSatellite = new Satellite(longitude);

                if (currentFrequency != null && currentFrequency.Provider == null)
                    currentFrequency.Provider = currentSatellite;
            }
            catch (FormatException)
            {
                Logger.Instance.Write("INI file format error: The Satellite line is wrong.");
                return (errorCodeFormatError);
            }
            catch (ArithmeticException)
            {
                Logger.Instance.Write("INI file format error: The Satellite line is wrong.");
                return (errorCodeFormatError);
            }

            return (errorCodeNoError);
        }

        private int processDish(string parts)
        {
            string[] parameters = parts.Split(new char[] { ',' });
            if (parameters.Length < 3 || parameters.Length > 4)
            {
                Logger.Instance.Write("INI file format error: The Dish line is wrong.");
                return (errorCodeFormatError);
            }

            SatelliteDish satelliteDish = new SatelliteDish();

            try
            {
                satelliteDish.LNBLowBandFrequency = Int32.Parse(parameters[0].Trim());
                satelliteDish.LNBHighBandFrequency = Int32.Parse(parameters[1].Trim());
                satelliteDish.LNBSwitchFrequency = Int32.Parse(parameters[2].Trim());
                if (parameters.Length == 4)
                    satelliteDish.DiseqcSwitch = parameters[3].ToUpperInvariant().Trim();                   

                currentDish = satelliteDish;

                if (currentFrequency as SatelliteFrequency != null)
                    (currentFrequency as SatelliteFrequency).SatelliteDish = currentDish;
            }
            catch (FormatException)
            {
                Logger.Instance.Write("INI file format error: The Dish line is wrong.");
                return (errorCodeFormatError);
            }
            catch (ArithmeticException)
            {
                Logger.Instance.Write("INI file format error: The Dish line is wrong.");
                return (errorCodeFormatError);
            }

            return (errorCodeNoError);
        }

        private int processFrequency(string parts)
        {
            string[] parameters = parts.Split(new char[] { ',' });

            if (parameters.Length == 2)
                return (processBasicFrequency(parameters));

            switch (currentFrequency.TunerType)
            {
                case TunerType.Satellite:
                    return (processSatelliteFrequency(parameters));
                case TunerType.Terrestrial:
                    return(processTerrestrialFrequency(parameters));
                case TunerType.Cable:
                    return (processCableFrequency(parameters));
                case TunerType.ATSC:
                case TunerType.ATSCCable:
                    return (processAtscFrequency(parameters));
                case TunerType.ClearQAM:
                    return (processClearQamFrequency(parameters));
                case TunerType.ISDBS:
                    return (processISDBSatelliteFrequency(parameters));
                case TunerType.ISDBT:
                    return (processISDBTerrestrialFrequency(parameters));
                default:
                    Logger.Instance.Write("INI file format error: A Frequency line is out of sequence.");
                    return (errorCodeFormatError);
            }
        }

        private int processBasicFrequency(string[] parameters)
        {
            try
            {
                int frequency = Int32.Parse(parameters[0].Trim());
                
                if (currentFrequency == null)
                {
                    currentFrequency = new TerrestrialFrequency();
                    currentFrequency.Provider = new TerrestrialProvider("Undefined");
                }

                TerrestrialFrequency terrestrialFrequency = currentFrequency as TerrestrialFrequency;
                terrestrialFrequency.Frequency = frequency;
                int errorCode = getCollectionType(parameters[1].Trim().ToUpperInvariant(), terrestrialFrequency);
                if (errorCode != errorCodeNoError)
                    return (errorCode);

                TuningFrequency.FrequencyCollection.Add(terrestrialFrequency);
            }
            catch (FormatException)
            {
                Logger.Instance.Write("INI file format error: A Frequency line is wrong.");
                return (errorCodeFormatError);
            }
            catch (ArithmeticException)
            {
                Logger.Instance.Write("INI file format error: A Frequency line is wrong.");
                return (errorCodeFormatError);
            }

            return (errorCodeNoError);
        }

        private int processTerrestrialFrequency(string[] parameters)
        {
            try
            {
                int frequency = Int32.Parse(parameters[0].Trim());
                int bandWidth = Int32.Parse(parameters[1].Trim());

                if (currentFrequency == null)
                {
                    currentFrequency = new TerrestrialFrequency();
                    currentFrequency.Provider = TerrestrialProvider.FindProvider(frequency, bandWidth);
                    if (currentFrequency.Provider == null)
                        currentFrequency.Provider = new TerrestrialProvider("Unknown");
                }

                TerrestrialFrequency terrestrialFrequency = currentFrequency as TerrestrialFrequency;
                terrestrialFrequency.Frequency = frequency;
                terrestrialFrequency.Bandwidth = bandWidth;
                int errorCode = getCollectionType(parameters[2].Trim().ToUpperInvariant(), terrestrialFrequency);
                if (errorCode != errorCodeNoError)
                    return (errorCode);
                
                TuningFrequency.FrequencyCollection.Add(terrestrialFrequency);                
            }
            catch (FormatException)
            {
                Logger.Instance.Write("INI file format error: A Frequency line is wrong.");
                return (errorCodeFormatError);
            }
            catch (ArithmeticException)
            {
                Logger.Instance.Write("INI file format error: A Frequency line is wrong.");
                return (errorCodeFormatError);
            }

            return (errorCodeNoError);
        }

        private int processSatelliteFrequency(string[] parameters)
        {
            if (parameters.Length != 5 && parameters.Length != 8)
            {
                Logger.Instance.Write("INI file format error: A Frequency line is wrong.");
                return (errorCodeFormatError);
            }

            try
            {
                int frequency = Int32.Parse(parameters[0].Trim());
                int symbolRate = Int32.Parse(parameters[1].Trim());
                FECRate fecRate = new FECRate(parameters[2]);
                SignalPolarization polarization = new SignalPolarization(parameters[3].Trim()[0]);

                Pilot pilot = Pilot.NotSet;
                RollOff rollOff = RollOff.NotSet;
                Modulation modulation = Modulation.QPSK;

                int nextParameter = 4;

                if (parameters.Length == 8)
                {
                    pilot = (Pilot)Enum.Parse(typeof(Pilot), parameters[4]);
                    rollOff = (RollOff)Enum.Parse(typeof(RollOff), parameters[5]);
                    modulation = (Modulation)Enum.Parse(typeof(Modulation), parameters[6]);
                    nextParameter = 7;
                }

                if (currentFrequency == null)
                    currentFrequency = new SatelliteFrequency();

                SatelliteFrequency satelliteFrequency = currentFrequency as SatelliteFrequency;

                satelliteFrequency.Frequency = frequency;
                satelliteFrequency.SymbolRate = symbolRate;
                satelliteFrequency.FEC = fecRate;
                satelliteFrequency.Polarization = polarization;
                satelliteFrequency.Pilot = pilot;
                satelliteFrequency.RollOff = rollOff;
                satelliteFrequency.Modulation = modulation;
                int errorCode = getCollectionType(parameters[nextParameter].Trim().ToUpperInvariant(), satelliteFrequency);
                if (errorCode != errorCodeNoError)
                    return (errorCode);
                
                satelliteFrequency.SatelliteDish = currentDish;

                if (currentFrequency.Provider == null)
                    currentFrequency.Provider = currentSatellite;

                TuningFrequency.FrequencyCollection.Add(satelliteFrequency);
            }
            catch (FormatException)
            {
                Logger.Instance.Write("INI file format error: A Frequency line is wrong.");
                return (errorCodeFormatError);
            }
            catch (ArithmeticException)
            {
                Logger.Instance.Write("INI file format error: A Frequency line is wrong.");
                return (errorCodeFormatError);
            }

            return (errorCodeNoError);
        }

        private int processCableFrequency(string[] parameters)
        {
            if (parameters.Length != 5)
            {
                Logger.Instance.Write("INI file format error: A Frequency line is wrong.");
                return (errorCodeFormatError);
            }

            try
            {
                int frequency = Int32.Parse(parameters[0].Trim());
                int symbolRate = Int32.Parse(parameters[1].Trim());
                FECRate fecRate = new FECRate(parameters[2]);
                Modulation modulation = (Modulation)Enum.Parse(typeof(Modulation), parameters[3].ToUpperInvariant(), true);
                
                if (currentFrequency == null)
                {
                    currentFrequency = new CableFrequency();
                    currentFrequency.Provider = CableProvider.FindProvider(frequency, symbolRate, fecRate, modulation);
                    if (currentFrequency.Provider == null)
                        currentFrequency.Provider = new CableProvider("Unknown");
                } 

                CableFrequency cableFrequency = currentFrequency as CableFrequency;
                cableFrequency.Frequency = frequency;
                cableFrequency.SymbolRate = symbolRate;
                cableFrequency.FEC = fecRate;
                cableFrequency.Modulation = modulation;
                int errorCode = getCollectionType(parameters[4].Trim().ToUpperInvariant(), cableFrequency);
                if (errorCode != errorCodeNoError)
                    return (errorCode);

                TuningFrequency.FrequencyCollection.Add(cableFrequency);                
            }
            catch (FormatException)
            {
                Logger.Instance.Write("INI file format error: A Frequency line is wrong.");
                return (errorCodeFormatError);
            }
            catch (ArithmeticException)
            {
                Logger.Instance.Write("INI file format error: A Frequency line is wrong.");
                return (errorCodeFormatError);
            }

            return (errorCodeNoError);
        }

        private int processAtscFrequency(string[] parameters)
        {
            if (parameters.Length != 6)
            {
                Logger.Instance.Write("INI file format error: A Frequency line is wrong.");
                return (errorCodeFormatError);
            }

            try
            {
                int frequency = Int32.Parse(parameters[0].Trim());
                int channelNumber = Int32.Parse(parameters[1].Trim());
                int symbolRate = Int32.Parse(parameters[2].Trim());
                FECRate fecRate = new FECRate(parameters[3]);
                Modulation modulation = (Modulation)Enum.Parse(typeof(Modulation), parameters[4].ToUpperInvariant(), true);

                AtscFrequency atscFrequency = currentFrequency as AtscFrequency;
                atscFrequency.Frequency = frequency;
                atscFrequency.ChannelNumber = channelNumber;
                atscFrequency.SymbolRate = symbolRate;
                atscFrequency.FEC = fecRate;
                atscFrequency.Modulation = modulation;
                int errorCode = getCollectionType(parameters[5].Trim().ToUpperInvariant(), atscFrequency);
                if (errorCode != errorCodeNoError)
                    return (errorCode);

                TuningFrequency.FrequencyCollection.Add(atscFrequency);
            }
            catch (FormatException)
            {
                Logger.Instance.Write("INI file format error: A Frequency line is wrong.");
                return (errorCodeFormatError);
            }
            catch (ArithmeticException)
            {
                Logger.Instance.Write("INI file format error: A Frequency line is wrong.");
                return (errorCodeFormatError);
            }

            return (errorCodeNoError);
        }

        private int processClearQamFrequency(string[] parameters)
        {
            if (parameters.Length != 6)
            {
                Logger.Instance.Write("INI file format error: A Frequency line is wrong.");
                return (errorCodeFormatError);
            }

            try
            {
                int frequency = Int32.Parse(parameters[0].Trim());
                int channelNumber = Int32.Parse(parameters[1].Trim());
                int symbolRate = Int32.Parse(parameters[2].Trim());
                FECRate fecRate = new FECRate(parameters[3]);
                Modulation modulation = (Modulation)Enum.Parse(typeof(Modulation), parameters[4].ToUpperInvariant(), true);

                ClearQamFrequency clearQamFrequency = currentFrequency as ClearQamFrequency;
                clearQamFrequency.Frequency = frequency;
                clearQamFrequency.ChannelNumber = channelNumber;
                clearQamFrequency.SymbolRate = symbolRate;
                clearQamFrequency.FEC = fecRate;
                clearQamFrequency.Modulation = modulation;
                int errorCode = getCollectionType(parameters[5].Trim().ToUpperInvariant(), clearQamFrequency);
                if (errorCode != errorCodeNoError)
                    return (errorCode);

                TuningFrequency.FrequencyCollection.Add(clearQamFrequency);
            }
            catch (FormatException)
            {
                Logger.Instance.Write("INI file format error: A Frequency line is wrong.");
                return (errorCodeFormatError);
            }
            catch (ArithmeticException)
            {
                Logger.Instance.Write("INI file format error: A Frequency line is wrong.");
                return (errorCodeFormatError);
            }

            return (errorCodeNoError);
        }

        private int processISDBTerrestrialFrequency(string[] parameters)
        {
            if (parameters.Length != 4)
            {
                Logger.Instance.Write("INI file format error: A Frequency line is wrong.");
                return (errorCodeFormatError);
            }

            try
            {
                int channelNumber = Int32.Parse(parameters[0].Trim());
                int frequency = Int32.Parse(parameters[1].Trim());
                int bandWidth = Int32.Parse(parameters[2].Trim());

                if (currentFrequency == null)
                {
                    currentFrequency = new ISDBTerrestrialFrequency();
                    currentFrequency.Provider = ISDBTerrestrialProvider.FindProvider(frequency, bandWidth);
                    if (currentFrequency.Provider == null)
                        currentFrequency.Provider = new ISDBTerrestrialProvider("Unknown");
                }

                ISDBTerrestrialFrequency terrestrialFrequency = currentFrequency as ISDBTerrestrialFrequency;
                terrestrialFrequency.ChannelNumber = channelNumber;
                terrestrialFrequency.Frequency = frequency;
                terrestrialFrequency.Bandwidth = bandWidth;
                int errorCode = getCollectionType(parameters[3].Trim().ToUpperInvariant(), terrestrialFrequency);
                if (errorCode != errorCodeNoError)
                    return (errorCode);

                TuningFrequency.FrequencyCollection.Add(terrestrialFrequency);
            }
            catch (FormatException)
            {
                Logger.Instance.Write("INI file format error: A Frequency line is wrong.");
                return (errorCodeFormatError);
            }
            catch (ArithmeticException)
            {
                Logger.Instance.Write("INI file format error: A Frequency line is wrong.");
                return (errorCodeFormatError);
            }

            return (errorCodeNoError);
        }

        private int processISDBSatelliteFrequency(string[] parameters)
        {
            if (parameters.Length != 5)
            {
                Logger.Instance.Write("INI file format error: A Frequency line is wrong.");
                return (errorCodeFormatError);
            }

            try
            {
                int frequency = Int32.Parse(parameters[0].Trim());
                int symbolRate = Int32.Parse(parameters[1].Trim());
                FECRate fecRate = new FECRate(parameters[2]);
                SignalPolarization polarization = new SignalPolarization(parameters[3].Trim()[0]);

                if (currentFrequency == null)
                    currentFrequency = new ISDBSatelliteFrequency();

                ISDBSatelliteFrequency satelliteFrequency = currentFrequency as ISDBSatelliteFrequency;

                satelliteFrequency.Frequency = frequency;
                satelliteFrequency.SymbolRate = symbolRate;
                satelliteFrequency.FEC = fecRate;
                satelliteFrequency.Polarization = polarization;
                int errorCode = getCollectionType(parameters[4].Trim().ToUpperInvariant(), satelliteFrequency);
                if (errorCode != errorCodeNoError)
                    return (errorCode);

                satelliteFrequency.SatelliteDish = currentDish;

                if (currentFrequency.Provider == null)
                    currentFrequency.Provider = currentSatellite;

                TuningFrequency.FrequencyCollection.Add(satelliteFrequency);
            }
            catch (FormatException)
            {
                Logger.Instance.Write("INI file format error: A Frequency line is wrong.");
                return (errorCodeFormatError);
            }
            catch (ArithmeticException)
            {
                Logger.Instance.Write("INI file format error: A Frequency line is wrong.");
                return (errorCodeFormatError);
            }

            return (errorCodeNoError);
        }

        private int getCollectionType(string parameter, TuningFrequency tuningFrequency)
        {
            switch (parameter)
            {
                case "N":
                case "EIT":
                    tuningFrequency.CollectionType = CollectionType.EIT;
                    break;
                case "Y":
                case "MHEG5":
                    tuningFrequency.CollectionType = CollectionType.MHEG5;
                    break;
                case "OPENTV":
                    tuningFrequency.CollectionType = CollectionType.OpenTV;
                    break;
                case "MHW1":
                case "MEDIAHIGHWAY1":
                    tuningFrequency.CollectionType = CollectionType.MediaHighway1;
                    break;
                case "MHW2":
                case "MEDIAHIGHWAY2":
                    tuningFrequency.CollectionType = CollectionType.MediaHighway2;
                    break;
                case "FREESAT":
                    tuningFrequency.CollectionType = CollectionType.FreeSat;
                    break;
                case "PSIP":
                    tuningFrequency.CollectionType = CollectionType.PSIP;
                    break;
                case "DISHNETWORK":
                    tuningFrequency.CollectionType = CollectionType.DishNetwork;
                    break;
                case "BELLTV":
                    tuningFrequency.CollectionType = CollectionType.BellTV;
                    break;
                case "SIEHFERNINFO":
                    tuningFrequency.CollectionType = CollectionType.SiehfernInfo;
                    break;                
                default:
                    Logger.Instance.Write("INI file format error: The collection type on a Frequency line is wrong.");
                    return (errorCodeFormatError);
            }

            return (errorCodeNoError);
        }

        private int processTimeouts(string parts)
        {
            string[] parameters = parts.Split(new char[] { ',' });
            if (parameters.Length < 2)
            {
                Logger.Instance.Write("INI file format error: The Timeouts line is wrong.");
                return (errorCodeFormatError);
            }

            try
            {
                int timeoutSeconds = Int32.Parse(parameters[0].Trim());
                lockTimeout = new TimeSpan(0, timeoutSeconds / 60, timeoutSeconds % 60);
                
                timeoutSeconds = Int32.Parse(parameters[1].Trim());
                frequencyTimeout = new TimeSpan(0, timeoutSeconds / 60, timeoutSeconds % 60);
                
                if (parameters.Length == 3)
                    repeats = Int32.Parse(parameters[2].Trim());

            }
            catch (FormatException)
            {
                Logger.Instance.Write("INI file format error: The Timeouts line is wrong.");
                return (errorCodeFormatError);
            }
            catch (ArithmeticException)
            {
                Logger.Instance.Write("INI file format error: The Timeouts line is wrong.");
                return (errorCodeFormatError);
            }

            return (errorCodeNoError);
        }

        private int processStation(string parts)
        {
            string[] parameters = parts.Split(new char[] { ',' });
            if (parameters.Length < 3 || parameters.Length > 5)
            {
                Logger.Instance.Write("INI file format error: A Station line is wrong.");
                return (errorCodeFormatError);
            }

            TVStation tvStation;
            if (parameters.Length == 3)
                tvStation = new TVStation("Excluded Station");
            else
                tvStation = new TVStation("Changed Station");

            try
            {
                tvStation.OriginalNetworkID = Int32.Parse(parameters[0].Trim());
                tvStation.TransportStreamID = Int32.Parse(parameters[1].Trim());
                tvStation.ServiceID = Int32.Parse(parameters[2].Trim());

                if (parameters.Length == 3)
                {
                    tvStation.Excluded = true;

                    TVStation oldStation = TVStation.FindStation(tvStation.OriginalNetworkID, tvStation.TransportStreamID, tvStation.ServiceID);
                    if (oldStation == null)
                        TVStation.AddStation(tvStation);
                    else
                        oldStation.Excluded = tvStation.Excluded;
                    return (errorCodeNoError);
                }

                try
                {
                    tvStation.LogicalChannelNumber = Int32.Parse(parameters[3].Trim());
                    if (parameters.Length == 5)
                        tvStation.NewName = parameters[4].Trim().Replace("%%", ",");

                    TVStation oldStation = TVStation.FindStation(tvStation.OriginalNetworkID, tvStation.TransportStreamID, tvStation.ServiceID);
                    if (oldStation == null)
                        TVStation.AddStation(tvStation);
                    else
                    {
                        oldStation.LogicalChannelNumber = tvStation.LogicalChannelNumber;
                        oldStation.NewName = tvStation.NewName;
                    }

                    return (errorCodeNoError);
                }
                catch (FormatException)
                {
                    if (parameters.Length == 5)
                    {
                        Logger.Instance.Write("INI file format error: A Station line is wrong.");
                        return (errorCodeFormatError);
                    }
                    else
                    {
                        tvStation.NewName = parameters[3].Trim().Replace("%%", ",");

                        TVStation oldStation = TVStation.FindStation(tvStation.OriginalNetworkID, tvStation.TransportStreamID, tvStation.ServiceID);
                        if (oldStation == null)
                            TVStation.AddStation(tvStation);
                        else
                            oldStation.NewName = tvStation.NewName;
                    }
                }
                catch (ArithmeticException)
                {
                    Logger.Instance.Write("INI file format error: A Station line is wrong.");
                    return (errorCodeFormatError);
                }
            }
            catch (FormatException)
            {
                Logger.Instance.Write("INI file format error: A Station line is wrong.");
                return (errorCodeFormatError);
            }
            catch (ArithmeticException)
            {
                Logger.Instance.Write("INI file format error: A Station line is wrong.");
                return (errorCodeFormatError);
            }

            return (errorCodeNoError);
        }

        private int processOption(string parts)
        {
            string[] parameters = parts.Split(new char[] { ',' });

            foreach (string parameter in parameters)
                options.Add(parameter.Trim().ToUpperInvariant());

            return (errorCodeNoError);
        }

        private int processTrace(string parts)
        {
            string[] parameters = parts.Split(new char[] { ',' });

            foreach (string parameter in parameters)
                traceIDs.Add(parameter.Trim().ToUpperInvariant());

            return (errorCodeNoError);
        }

        private int processDebug(string parts)
        {
            string[] parameters = parts.Split(new char[] { ',' });

            foreach (string parameter in parameters)
                debugIDs.Add(parameter.Trim().ToUpperInvariant());

            return (errorCodeNoError);
        }

        private int processLocation(string parts)
        {
            string[] parameters = parts.Split(new char[] { ',' });
            if (parameters.Length > 2)
            {
                Logger.Instance.Write("INI file format error: The Location line is wrong.");
                return (errorCodeFormatError);
            }

            try
            {
                countryCode = parameters[0].Trim().ToUpperInvariant();
                if (parameters.Length == 2)
                    region = Int32.Parse(parameters[1].Trim());
                else
                    region = 0;
            }
            catch (FormatException)
            {
                Logger.Instance.Write("INI file format error: The Location line is wrong.");
                return (errorCodeFormatError);
            }
            catch (ArithmeticException)
            {
                Logger.Instance.Write("INI file format error: The Location line is wrong.");
                return (errorCodeFormatError);
            }

            return (errorCodeNoError);
        }

        private int processInputLanguage(string parts)
        {
            bool reply = LanguageCode.Validate(parts.Trim().ToLower(), LanguageCode.Usage.Input);
            if (!reply)
            {
                Logger.Instance.Write("INI file format error: The input language code is undefined.");
                return (errorCodeFormatError);
            }

            inputLanguage = parts.Trim().ToLower();
            return (errorCodeNoError);
        }

        private int processOutputLanguage(string parts)
        {
            bool reply = LanguageCode.Validate(parts.Trim().ToLower(), LanguageCode.Usage.Output);
            if (!reply)
            {
                Logger.Instance.Write("INI file format error: The output language code is undefined.");
                return (errorCodeFormatError);
            }

            outputLanguage = parts.Trim().ToLower();
            return (errorCodeNoError);
        }

        private int processTimeZone(string parts)
        {
            string[] parameters = parts.Split(new char[] { ',' });
            if (parameters.Length != 1 && parameters.Length != 4)
            {
                Logger.Instance.Write("INI file format error: The Timezone line is wrong.");
                return (errorCodeFormatError);
            }

            try
            {
                string[] offsetParts = parameters[0].Split(new char[] { '.' });
                if (offsetParts.Length != 2)
                {
                    Logger.Instance.Write("INI file format error: The Timezone line is wrong.");
                    return (errorCodeFormatError);
                }
                int hours = Int32.Parse(offsetParts[0].Trim());
                int minutes = Int32.Parse(offsetParts[1].Trim());
                timeZone = new TimeSpan(hours, minutes, 0);

                if (parameters.Length == 4)
                {
                    string[] nextParts = parameters[1].Split(new char[] { '.' });
                    if (nextParts.Length != 2)
                    {
                        Logger.Instance.Write("INI file format error: The Timezone line is wrong.");
                        return (errorCodeFormatError);
                    }

                    int nextHours = Int32.Parse(nextParts[0].Trim());
                    int nextMinutes = Int32.Parse(nextParts[1].Trim());
                    nextTimeZone = new TimeSpan(nextHours, nextMinutes, 0);

                    try
                    {
                        nextTimeZoneChange = DateTime.ParseExact(parameters[2].Trim() + " " + parameters[3].Trim() + ".00", "dd/MM/yy HH.mm.ss", null);
                    }
                    catch (FormatException)
                    {
                        Logger.Instance.Write("INI file format error: The Timezone line is wrong.");
                        return (errorCodeFormatError);
                    }
                }
                else
                {
                    nextTimeZone = timeZone;
                    nextTimeZoneChange = DateTime.MaxValue;
                }
                
                timeZoneSet = true;
            }
            catch (FormatException)
            {
                Logger.Instance.Write("INI file format error: The Timezone line is wrong.");
                return (errorCodeFormatError);
            }
            catch (ArgumentOutOfRangeException)
            {
                Logger.Instance.Write("INI file format error: The Timezone line is wrong.");
                return (errorCodeFormatError);
            }

            return (errorCodeNoError);
        }

        private int processTSFile(string parts)
        {
            if (parts == string.Empty)
            {
                Logger.Instance.Write("INI file format error: The TS file name is wrong.");
                return (errorCodeFormatError);
            }
            
            tsFileName = parts.Trim();

            return (errorCodeNoError);
        }

        private int processChannel(string parts)
        {
            string[] parameters = parts.Split(new char[] { ',' });
            if (parameters.Length > 2)
            {
                Logger.Instance.Write("INI file format error: The Channel line is wrong.");
                return (errorCodeFormatError);
            }

            try
            {
                channelBouquet = Int32.Parse(parameters[0].Trim());
                if (parameters.Length > 1)
                    channelRegion = Int32.Parse(parameters[1].Trim());
                
            }
            catch (FormatException)
            {
                Logger.Instance.Write("INI file format error: The Channel line is wrong.");
                return (errorCodeFormatError);
            }
            catch (ArithmeticException)
            {
                Logger.Instance.Write("INI file format error: The Channel line is wrong.");
                return (errorCodeFormatError);
            }

            return (errorCodeNoError);
        }

        private int processDiseqc(string parts)
        {
            if (parts == string.Empty)
            {
                Logger.Instance.Write("INI file format error: The Diseqc identifier is wrong.");
                return (errorCodeFormatError);
            }

            diseqcIdentity = parts.Trim();

            return (errorCodeNoError);
        }

        private int processCharSet(string parts)
        {
            if (parts == string.Empty)
            {
                Logger.Instance.Write("INI file format error: The CharSet identifier is wrong.");
                return (errorCodeFormatError);
            }

            try
            {
                int characterSetSuffix = Int32.Parse(parts.Trim());
                characterSet = findCharacterSet(characterSetSuffix);
                if (characterSet == null)
                {
                    Logger.Instance.Write("INI file format error: The CharSet line is wrong.");
                    return (errorCodeFormatError);
                }
            }
            catch (FormatException)
            {
                characterSet = findCharacterSet(parts);
                if (characterSet == null)
                {
                    Logger.Instance.Write("INI file format error: The CharSet line is wrong.");
                    return (errorCodeFormatError);
                }
            }
            catch (ArithmeticException)
            {
                Logger.Instance.Write("INI file format error: The Channel line is wrong.");
                return (errorCodeFormatError);
            }

            return (errorCodeNoError);
        }

        private string findCharacterSet(int characterSetSuffix)
        {
            return (findCharacterSet("iso-8859-" + characterSetSuffix));
        }

        private string findCharacterSet(string characterSet)
        {
            foreach (EncodingInfo encoding in ASCIIEncoding.GetEncodings())
            {
                if (encoding.Name == characterSet)
                    return (characterSet);
            }

            return (null);
        }

        private int processEITPid(string parts)
        {
            try
            {
                eitPid = Int32.Parse(parts.Trim());
            }
            catch (FormatException)
            {
                Logger.Instance.Write("INI file format error: The EITPid line is wrong.");
                return (errorCodeFormatError);
            }
            catch (ArithmeticException)
            {
                Logger.Instance.Write("INI file format error: The EITPid line is wrong.");
                return (errorCodeFormatError);
            }

            return (errorCodeNoError);
        }

        private int processMHW1Pids(string parts)
        {
            string[] parameters = parts.Split(new char[] { ',' });
            if (parameters.Length != 2)
            {
                Logger.Instance.Write("INI file format error: The MHW1Pids line is wrong.");
                return (errorCodeFormatError);
            }

            try
            {
                mhw1Pids = new int[] { Int32.Parse(parameters[0].Trim()), Int32.Parse(parameters[1].Trim()) };
            }
            catch (FormatException)
            {
                Logger.Instance.Write("INI file format error: The MHW1Pids line is wrong.");
                return (errorCodeFormatError);
            }
            catch (ArithmeticException)
            {
                Logger.Instance.Write("INI file format error: The MHW1Pids line is wrong.");
                return (errorCodeFormatError);
            }

            return (errorCodeNoError);
        }

        private int processMHW2Pids(string parts)
        {
            string[] parameters = parts.Split(new char[] { ',' });
            if (parameters.Length != 3)
            {
                Logger.Instance.Write("INI file format error: The MHW1Pids line is wrong.");
                return (errorCodeFormatError);
            }

            try
            {
                mhw2Pids = new int[] { Int32.Parse(parameters[0].Trim()), Int32.Parse(parameters[1].Trim()), Int32.Parse(parameters[2].Trim()) };
            }
            catch (FormatException)
            {
                Logger.Instance.Write("INI file format error: The MHW1Pids line is wrong.");
                return (errorCodeFormatError);
            }
            catch (ArithmeticException)
            {
                Logger.Instance.Write("INI file format error: The MHW1Pids line is wrong.");
                return (errorCodeFormatError);
            }

            return (errorCodeNoError);
        }

        private int processPluginFrequency(string parts)
        {
            string[] parameters = parts.Split(new char[] { ',' });
            if (parameters.Length != 5)           
            {
                Logger.Instance.Write("INI file format error: A PluginFrequency line is wrong.");
                return (errorCodeFormatError);
            }

            switch (parameters[0])
            {
                case "Satellite":
                    pluginFrequency = new SatelliteFrequency();
                    break;
                case "Terrestrial":
                    pluginFrequency = new TerrestrialFrequency();
                    break;
                case "Cable":
                    pluginFrequency = new CableFrequency();
                    break;
                case "ATSC":
                    pluginFrequency = new AtscFrequency();
                    break;
                case "ISDBS":
                    pluginFrequency = new ISDBSatelliteFrequency();
                    break;
                case "ISDBT":
                    pluginFrequency = new ISDBTerrestrialFrequency();
                    break;
                default:
                    Logger.Instance.Write("INI file format error: A PluginFrequency line is wrong.");
                    return (errorCodeFormatError);
            }

            pluginFrequency.Provider = new Provider(parameters[1].Replace('+', ','));

            try
            {
                pluginFrequency.Frequency = Int32.Parse(parameters[2]);
                if (parameters[0] == "Satellite")
                    (pluginFrequency as SatelliteFrequency).Polarization = new SignalPolarization(parameters[3].Trim()[0]);
                return(getCollectionType(parameters[4].Trim().ToUpperInvariant(), pluginFrequency));
            }
            catch (FormatException)
            {
                Logger.Instance.Write("INI file format error: A PluginFrequency line is wrong.");
                return (errorCodeFormatError);
            }
            catch (ArithmeticException)
            {
                Logger.Instance.Write("INI file format error: A PluginFrequency line is wrong.");
                return (errorCodeFormatError);
            }
        }

        private int processTuningFile(string parts)
        {
            if (currentFrequency == null)
            {
                Logger.Instance.Write("INI file format error: Parameter sequence error.");
                return (errorCodeFormatError);
            }

            string fileName = parts.Trim().Substring(0, parts.Length - 4);
                            
            SatelliteFrequency satelliteFrequency = currentFrequency as SatelliteFrequency;
            if (satelliteFrequency != null)
            {
                satelliteFrequency.Provider = Satellite.FindSatellite(fileName);
                return(errorCodeNoError);
            }

            TerrestrialFrequency terrestrialFrequency = currentFrequency as TerrestrialFrequency;
            if (terrestrialFrequency != null)
            {
                terrestrialFrequency.Provider = TerrestrialProvider.FindProvider(fileName);
                return(errorCodeNoError);
            }

            CableFrequency cableFrequency = currentFrequency as CableFrequency;
            if (cableFrequency != null)
            {
                cableFrequency.Provider = CableProvider.FindProvider(fileName);
                return (errorCodeNoError);
            }

            AtscFrequency atscFrequency = currentFrequency as AtscFrequency;
            if (atscFrequency != null)
            {
                atscFrequency.Provider = AtscProvider.FindProvider(fileName);
                return (errorCodeNoError);
            }

            ClearQamFrequency clearQamFrequency = currentFrequency as ClearQamFrequency;
            if (clearQamFrequency != null)
            {
                clearQamFrequency.Provider = ClearQamProvider.FindProvider(fileName);
                return (errorCodeNoError);
            }

            ISDBSatelliteFrequency isdbSatelliteFrequency = currentFrequency as ISDBSatelliteFrequency;
            if (isdbSatelliteFrequency != null)
            {
                isdbSatelliteFrequency.Provider = ISDBSatelliteProvider.FindProvider(fileName);
                return (errorCodeNoError);
            }

            ISDBTerrestrialFrequency isdbTerrestrialFrequency = currentFrequency as ISDBTerrestrialFrequency;
            if (isdbTerrestrialFrequency != null)
            {
                isdbTerrestrialFrequency.Provider = ISDBTerrestrialProvider.FindProvider(fileName);
                return (errorCodeNoError);
            }

            Logger.Instance.Write("INI file format error: Internal error - Frequency not recognised.");
            return (errorCodeFormatError);
        }

        private int processScannedChannel(string parts)
        {
            string[] parameters = parts.Split(new char[] { ',' });            
            if (parameters.Length != 5)
            {
                Logger.Instance.Write("INI file format error: A Scanned line is wrong.");
                return (errorCodeFormatError);
            }

            try
            {
                int originalNetworkID = Int32.Parse(parameters[0].Trim());
                int transportStreamID = Int32.Parse(parameters[1].Trim());
                int serviceID = Int32.Parse(parameters[2].Trim());

                TVStation station = TVStation.FindStation(originalNetworkID, transportStreamID, serviceID);
                if (station != null)
                    station.Name = parameters[4].Trim().Replace("%%", ",");
                else
                {
                    TVStation newStation = new TVStation(parameters[4].Trim().Replace("%%", ","));
                    newStation.OriginalNetworkID = originalNetworkID;
                    newStation.TransportStreamID = transportStreamID;
                    newStation.ServiceID = serviceID;
                    TVStation.AddStation(newStation);
                }
            }
            catch (FormatException)
            {
                Logger.Instance.Write("INI file format error: A Scanned line is wrong.");
                return (errorCodeFormatError);
            }
            catch (ArithmeticException)
            {
                Logger.Instance.Write("INI file format error: A Scanned line is wrong.");
                return (errorCodeFormatError);
            }

            return (errorCodeNoError);
        }

        private int processOffsetChannel(string parts)
        {
            string[] parameters = parts.Split(new char[] { ',' });
            if (parameters.Length != 9)
            {
                Logger.Instance.Write("INI file format error: An Offset line is wrong.");
                return (errorCodeFormatError);
            }

            try
            {
                TVStation sourceChannel = new TVStation(parameters[0].Trim().Replace("%%", ","));
                sourceChannel.OriginalNetworkID = Int32.Parse(parameters[1].Trim());
                sourceChannel.TransportStreamID = Int32.Parse(parameters[2].Trim());
                sourceChannel.ServiceID = Int32.Parse(parameters[3].Trim());

                TVStation destinationChannel = new TVStation(parameters[4].Trim().Replace("%%", ","));
                destinationChannel.OriginalNetworkID = Int32.Parse(parameters[5].Trim());
                destinationChannel.TransportStreamID = Int32.Parse(parameters[6].Trim());
                destinationChannel.ServiceID = Int32.Parse(parameters[7].Trim());

                TimeOffsetChannel channel = new TimeOffsetChannel(sourceChannel, destinationChannel, Int32.Parse(parameters[8].Trim()));
                TimeOffsetChannel.Channels.Add(channel);
            }
            catch (FormatException)
            {
                Logger.Instance.Write("INI file format error: An Offset line is wrong.");
                return (errorCodeFormatError);
            }
            catch (ArithmeticException)
            {
                Logger.Instance.Write("INI file format error: An Offset line is wrong.");
                return (errorCodeFormatError);
            }

            return (errorCodeNoError);
        }

        private int processIncludeService(string parts)
        {
            string[] parameters = parts.Split(new char[] { ',' });
            if (parameters.Length > 4)
            {
                Logger.Instance.Write("INI file format error: An ExcludeService line is wrong.");
                return (errorCodeFormatError);
            }
            
            try
            {
                int originalNetworkID = Int32.Parse(parameters[0]);
                int transportStreamID = -1;
                int startServiceID = -1;
                int endServiceID = -1;

                if (ChannelFilterEntry.ChannelFilters == null)
                    ChannelFilterEntry.ChannelFilters = new Collection<ChannelFilterEntry>();

                if (parameters.Length > 1)
                    transportStreamID = Int32.Parse(parameters[1]);
                if (parameters.Length > 2)
                    startServiceID = Int32.Parse(parameters[2]);
                if (parameters.Length > 3)
                    endServiceID = Int32.Parse(parameters[3]);

                ChannelFilterEntry.ChannelFilters.Add(new ChannelFilterEntry(originalNetworkID, transportStreamID, startServiceID, endServiceID));                
            }
            catch (FormatException)
            {
                Logger.Instance.Write("INI file format error: An ExcludeService line is wrong.");
                return (errorCodeFormatError);
            }
            catch (ArithmeticException)
            {
                Logger.Instance.Write("INI file format error: An ExcludeService line is wrong.");
                return (errorCodeFormatError);
            }

            return (errorCodeNoError);
        }

        private int processMaxService(string parts)
        {
            try
            {
                maxService = Int32.Parse(parts.Trim());
            }
            catch (FormatException)
            {
                Logger.Instance.Write("INI file format error: The MaxService line is wrong.");
                return (errorCodeFormatError);
            }
            catch (ArithmeticException)
            {
                Logger.Instance.Write("INI file format error: The MaxService line is wrong.");
                return (errorCodeFormatError);
            }

            return (errorCodeNoError);
        }

        private int processRepeatExclusion(string parts)
        {
            string[] parameters = parts.Split(new char[] { ',' });
            if (parameters.Length != 2)
            {
                Logger.Instance.Write("INI file format error: A RepeatExclusion line is wrong.");
                return (errorCodeFormatError);
            }

            if (RepeatExclusion.Exclusions == null)
                RepeatExclusion.Exclusions = new Collection<RepeatExclusion>();

            RepeatExclusion.Exclusions.Add(new RepeatExclusion(parameters[0], parameters[1]));            

            return (errorCodeNoError);
        }

        private int processRepeatPhrase(string parts)
        {
            if (RepeatExclusion.PhrasesToIgnore == null)
                RepeatExclusion.PhrasesToIgnore = new Collection<string>();

            RepeatExclusion.PhrasesToIgnore.Add(parts.Trim());

            return (errorCodeNoError);
        }

        private int processWMCImportName(string parts)
        {
            if (parts.Trim() == string.Empty)
            {
                Logger.Instance.Write("INI file format error: The WMCImportName line is wrong.");
                return (errorCodeFormatError);
            }

            wmcImportName = parts.Trim();
            return (errorCodeNoError);
        }

        /// <summary>
        /// Write the current parameter set to a file.
        /// </summary>
        /// <param name="fileName">The full name of the file.</param>
        /// <returns>Null if output was successful; a message identifying the error otherwise.</returns>
        public  string Save(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    if (File.Exists(fileName + ".bak"))
                    {
                        File.SetAttributes(fileName + ".bak", FileAttributes.Normal);
                        File.Delete(fileName + ".bak");
                    }

                    File.Copy(fileName, fileName + ".bak");
                    File.SetAttributes(fileName + ".bak", FileAttributes.ReadOnly);

                    File.SetAttributes(fileName, FileAttributes.Normal);
                }

                FileStream fileStream = new FileStream(fileName, FileMode.Create);
                StreamWriter streamWriter = new StreamWriter(fileStream);

                outputGeneralParameters(streamWriter);
                outputDiagnosticParameters(streamWriter);

                if (parameterSet == ParameterSet.Collector)
                {
                    foreach (TuningFrequency tuningFrequency in TuningFrequency.FrequencyCollection)
                        outputFrequencyParameters(streamWriter, tuningFrequency);
                }

                outputStationParameters(streamWriter);
                outputScanListParameters(streamWriter);
                outputTimeOffsetParameters(streamWriter);
                outputServiceFilterParameters(streamWriter);
                outputRepeatExclusionParameters(streamWriter);

                streamWriter.Close();
                fileStream.Close();

                return (null);
            }
            catch (IOException e)
            {
                return (e.Message);
            }
        }

        private void outputGeneralParameters(StreamWriter streamWriter)
        {
            streamWriter.WriteLine("[GENERAL]");

            if (outputFileName != string.Empty)
                streamWriter.WriteLine("Output=" + outputFileName);

            if (parameterSet == ParameterSet.Plugin)
            {
                if (pluginFrequency != null)
                    outputPluginFrequencyParameters(streamWriter, pluginFrequency);
            }

            if (parameterSet == ParameterSet.Collector)
            {
                if (SelectedTuners.Count != 0)
                {
                    streamWriter.Write("Tuner=");

                    foreach (int tuner in SelectedTuners)
                    {
                        if (SelectedTuners.IndexOf(tuner) == 0)
                            streamWriter.Write(tuner.ToString());
                        else
                            streamWriter.Write("," + tuner);
                    }

                    streamWriter.WriteLine();
                }
            }

            streamWriter.WriteLine(("Timeouts=" + LockTimeout.TotalSeconds + "," + FrequencyTimeout.TotalSeconds + "," + Repeats).ToString(CultureInfo.InvariantCulture));

            if (countryCode != null)
                streamWriter.WriteLine("Location=" + countryCode + "," + region);

            if (inputLanguage != null)
                streamWriter.WriteLine("InputLanguage=" + inputLanguage);
            if (outputLanguage != null)
                streamWriter.WriteLine("OutputLanguage=" + outputLanguage);

            if (channelBouquet != -1)
            {
                if (channelRegion != -1)
                    streamWriter.WriteLine("Channels=" + channelBouquet + "," + channelRegion);
                else
                    streamWriter.WriteLine("Channels=" + channelBouquet);
            }

            if (characterSet != null)
                streamWriter.WriteLine("Charset=" + characterSet);

            if (timeZoneSet)
            {
                streamWriter.WriteLine("Timezone=" + timeZone.Hours.ToString("00") + "." + timeZone.Minutes.ToString("00") + "," +
                    nextTimeZone.Hours.ToString("00") + "." + nextTimeZone.Minutes.ToString("00") + "," +
                    nextTimeZoneChange.ToString("dd/MM/yy") + "," +
                    nextTimeZoneChange.TimeOfDay.Hours.ToString("00") + "." + nextTimeZoneChange.TimeOfDay.Minutes.ToString("00")); 
            }

            if (eitPid != -1)
                streamWriter.WriteLine("EITPid=" + eitPid);

            if (mhw1Pids != null)
                streamWriter.WriteLine("MHW1Pids=" + mhw1Pids[0] + "," + mhw1Pids[1]);

            if (mhw2Pids != null)
                streamWriter.WriteLine("MHW2Pids=" + mhw2Pids[0] + "," + mhw2Pids[1] + "," + mhw2Pids[2]);

            if (options.Count != 0)
            {
                streamWriter.Write("Option=");

                foreach (string option in options)
                {
                    if (options.IndexOf(option) == 0)
                        streamWriter.Write(option);
                    else
                        streamWriter.Write("," + option);
                }

                streamWriter.WriteLine();
            }

            if (wmcImportName != null)
                streamWriter.WriteLine("WMCImportName=" + wmcImportName);

        }

        private void outputPluginFrequencyParameters(StreamWriter streamWriter, TuningFrequency tuningFrequency)
        {
            switch (tuningFrequency.TunerType)
            {
                case TunerType.Satellite:
                    streamWriter.WriteLine("PluginFrequency=Satellite," + tuningFrequency.Provider.ToString().Replace(',', '+') + "," +
                        tuningFrequency.Frequency + "," + (tuningFrequency as SatelliteFrequency).Polarization.PolarizationAbbreviation + "," + 
                        tuningFrequency.CollectionType);
                    break;
                case TunerType.Terrestrial:
                    streamWriter.WriteLine("PluginFrequency=Terrestrial," + (tuningFrequency.Provider) + "," +
                        tuningFrequency.Frequency + ",," + tuningFrequency.CollectionType);  
                    break;
                case TunerType.Cable:
                    streamWriter.WriteLine("PluginFrequency=Cable," + (tuningFrequency.Provider) + "," +
                        tuningFrequency.Frequency + ",," + tuningFrequency.CollectionType);
                    break;
                case TunerType.ATSC:
                    streamWriter.WriteLine("PluginFrequency=ATSC," + (tuningFrequency.Provider) + "," +
                        tuningFrequency.Frequency + ",," + tuningFrequency.CollectionType);
                    break;
                case TunerType.ClearQAM:
                    streamWriter.WriteLine("PluginFrequency=ClearQAM," + (tuningFrequency.Provider) + "," +
                        tuningFrequency.Frequency + ",," + tuningFrequency.CollectionType);
                    break;
                case TunerType.ISDBS:
                    streamWriter.WriteLine("PluginFrequency=ISDBS," + (tuningFrequency.Provider) + "," +
                        tuningFrequency.Frequency + ",," + tuningFrequency.CollectionType);
                    break;
                case TunerType.ISDBT:
                    streamWriter.WriteLine("PluginFrequency=ISDBT," + (tuningFrequency.Provider) + "," +
                        tuningFrequency.Frequency + ",," + tuningFrequency.CollectionType);
                    break;
                default:
                    break;
            }
        }

        private void outputDiagnosticParameters(StreamWriter streamWriter)
        {
            if (traceIDs.Count == 0 && debugIDs.Count == 0 && tsFileName == null)
                return;

            streamWriter.WriteLine();
            streamWriter.WriteLine("[DIAGNOSTICS]");

            if (traceIDs.Count != 0)
            {
                streamWriter.Write("Trace=");

                foreach (string traceID in traceIDs)
                {
                    if (traceIDs.IndexOf(traceID) == 0)
                        streamWriter.Write(traceID);
                    else
                        streamWriter.Write("," + traceID);
                }

                streamWriter.WriteLine();
            }
            
            if (debugIDs.Count != 0)
            {
                streamWriter.Write("Debug=");

                foreach (string debugID in debugIDs)
                {
                    if (debugIDs.IndexOf(debugID) == 0)
                        streamWriter.Write(debugID);
                    else
                        streamWriter.Write("," + debugID);
                }

                streamWriter.WriteLine();
            }

            if (tsFileName != null)
                streamWriter.WriteLine("TSFile=" + tsFileName);
        }

        private  void outputFrequencyParameters(StreamWriter streamWriter, TuningFrequency tuningFrequency)
        {
            switch (tuningFrequency.TunerType)
            {
                case TunerType.Satellite:
                    outputSatelliteFrequency(streamWriter, tuningFrequency);
                    break;
                case TunerType.Terrestrial:
                    outputTerrestrialFrequency(streamWriter, tuningFrequency);
                    break;
                case TunerType.Cable:
                    outputCableFrequency(streamWriter, tuningFrequency);
                    break;
                case TunerType.ATSC:
                case TunerType.ATSCCable:
                    outputAtscFrequency(streamWriter, tuningFrequency);
                    break;
                case TunerType.ClearQAM:
                    outputClearQamFrequency(streamWriter, tuningFrequency);
                    break;
                case TunerType.ISDBS:
                    outputISDBSatelliteFrequency(streamWriter, tuningFrequency);
                    break;
                case TunerType.ISDBT:
                    outputISDBTerrestrialFrequency(streamWriter, tuningFrequency);
                    break;
                default:
                    break;
            }
        }

        private void outputSatelliteFrequency(StreamWriter streamWriter, TuningFrequency tuningFrequency)
        {
            streamWriter.WriteLine();
            streamWriter.WriteLine("[DVBS]");

            streamWriter.WriteLine("Satellite=" + (tuningFrequency.Provider as Satellite).Longitude);

            SatelliteFrequency satelliteFrequency = tuningFrequency as SatelliteFrequency;

            if (satelliteFrequency.SatelliteDish.DiseqcSwitch != null)
                streamWriter.WriteLine("Dish=" + satelliteFrequency.SatelliteDish.LNBLowBandFrequency + "," +
                    satelliteFrequency.SatelliteDish.LNBHighBandFrequency + "," +
                    satelliteFrequency.SatelliteDish.LNBSwitchFrequency + "," +
                    satelliteFrequency.SatelliteDish.DiseqcSwitch);
            else
                streamWriter.WriteLine("Dish=" + satelliteFrequency.SatelliteDish.LNBLowBandFrequency + "," +
                    satelliteFrequency.SatelliteDish.LNBHighBandFrequency + "," +
                    satelliteFrequency.SatelliteDish.LNBSwitchFrequency);

            streamWriter.WriteLine("TuningFile=" + satelliteFrequency.Provider + ".xml");

            if (satelliteFrequency.Pilot == Pilot.NotSet && satelliteFrequency.RollOff == RollOff.NotSet)
            {
                streamWriter.WriteLine("ScanningFrequency=" + satelliteFrequency.Frequency + "," +
                    satelliteFrequency.SymbolRate + "," +
                    satelliteFrequency.FEC + "," +
                    satelliteFrequency.Polarization.PolarizationAbbreviation + "," +
                    satelliteFrequency.CollectionType);
            }
            else
            {
                streamWriter.WriteLine("ScanningFrequency=" + satelliteFrequency.Frequency + "," +
                    satelliteFrequency.SymbolRate + "," +
                    satelliteFrequency.FEC + "," +
                    satelliteFrequency.Polarization.PolarizationAbbreviation + "," +
                    satelliteFrequency.Pilot + "," +
                    satelliteFrequency.RollOff + "," +
                    satelliteFrequency.Modulation + "," +
                    satelliteFrequency.CollectionType);
            }
        }

        private void outputTerrestrialFrequency(StreamWriter streamWriter, TuningFrequency tuningFrequency)
        {
            streamWriter.WriteLine();
            streamWriter.WriteLine("[DVBT]");

            TerrestrialFrequency terrestrialFrequency = tuningFrequency as TerrestrialFrequency;

            streamWriter.WriteLine("TuningFile=" + tuningFrequency.Provider + ".xml");

            streamWriter.WriteLine("ScanningFrequency=" + terrestrialFrequency.Frequency + "," +
                terrestrialFrequency.Bandwidth + "," +
                terrestrialFrequency.CollectionType);
        }

        private void outputCableFrequency(StreamWriter streamWriter, TuningFrequency tuningFrequency)
        {
            streamWriter.WriteLine();
            streamWriter.WriteLine("[DVBC]");

           CableFrequency cableFrequency = tuningFrequency as CableFrequency;

            streamWriter.WriteLine("TuningFile=" + tuningFrequency.Provider + ".xml");

            streamWriter.WriteLine("ScanningFrequency=" + cableFrequency.Frequency + "," +
                cableFrequency.SymbolRate + "," +
                cableFrequency.FEC + "," +
                cableFrequency.Modulation + "," +
                cableFrequency.CollectionType);
        }

        private void outputAtscFrequency(StreamWriter streamWriter, TuningFrequency tuningFrequency)
        {
            streamWriter.WriteLine();
            streamWriter.WriteLine("[ATSC]");

            AtscFrequency atscFrequency = tuningFrequency as AtscFrequency;

            streamWriter.WriteLine("TuningFile=" + tuningFrequency.Provider + ".xml");

            streamWriter.WriteLine("ScanningFrequency=" + atscFrequency.Frequency + "," +
                atscFrequency.ChannelNumber + "," +
                atscFrequency.SymbolRate + "," +
                atscFrequency.FEC + "," +
                atscFrequency.Modulation + "," +
                atscFrequency.CollectionType);
        }

        private void outputClearQamFrequency(StreamWriter streamWriter, TuningFrequency tuningFrequency)
        {
            streamWriter.WriteLine();
            streamWriter.WriteLine("[CLEARQAM]");

            ClearQamFrequency clearQamFrequency = tuningFrequency as ClearQamFrequency;

            streamWriter.WriteLine("TuningFile=" + tuningFrequency.Provider + ".xml");

            streamWriter.WriteLine("ScanningFrequency=" + clearQamFrequency.Frequency + "," +
                clearQamFrequency.ChannelNumber + "," +
                clearQamFrequency.SymbolRate + "," +
                clearQamFrequency.FEC + "," +
                clearQamFrequency.Modulation + "," +
                clearQamFrequency.CollectionType);
        }

        private void outputISDBSatelliteFrequency(StreamWriter streamWriter, TuningFrequency tuningFrequency)
        {
            streamWriter.WriteLine();
            streamWriter.WriteLine("[ISDBS]");

            streamWriter.WriteLine("Satellite=" + (tuningFrequency.Provider as Satellite).Longitude);

            ISDBSatelliteFrequency satelliteFrequency = tuningFrequency as ISDBSatelliteFrequency;

            if (satelliteFrequency.SatelliteDish.DiseqcSwitch != null)
                streamWriter.WriteLine("Dish=" + satelliteFrequency.SatelliteDish.LNBLowBandFrequency + "," +
                    satelliteFrequency.SatelliteDish.LNBHighBandFrequency + "," +
                    satelliteFrequency.SatelliteDish.LNBSwitchFrequency + "," +
                    satelliteFrequency.SatelliteDish.DiseqcSwitch);
            else
                streamWriter.WriteLine("Dish=" + satelliteFrequency.SatelliteDish.LNBLowBandFrequency + "," +
                    satelliteFrequency.SatelliteDish.LNBHighBandFrequency + "," +
                    satelliteFrequency.SatelliteDish.LNBSwitchFrequency);

            streamWriter.WriteLine("TuningFile=" + satelliteFrequency.Provider + ".xml");

            streamWriter.WriteLine("ScanningFrequency=" + satelliteFrequency.Frequency + "," +
                satelliteFrequency.SymbolRate + "," +
                satelliteFrequency.FEC + "," +
                satelliteFrequency.Polarization.PolarizationAbbreviation + "," +
                satelliteFrequency.CollectionType);

        }

        private void outputISDBTerrestrialFrequency(StreamWriter streamWriter, TuningFrequency tuningFrequency)
        {
            streamWriter.WriteLine();
            streamWriter.WriteLine("[ISDBT]");

            ISDBTerrestrialFrequency terrestrialFrequency = tuningFrequency as ISDBTerrestrialFrequency;

            streamWriter.WriteLine("TuningFile=" + tuningFrequency.Provider + ".xml");

            streamWriter.WriteLine("ScanningFrequency=" + terrestrialFrequency.ChannelNumber + "," +
                terrestrialFrequency.Frequency + "," +
                terrestrialFrequency.Bandwidth + "," +
                terrestrialFrequency.CollectionType);
        }

        private  void outputStationParameters(StreamWriter streamWriter)
        {
            if (TVStation.StationCollection.Count == 0)
                return;

            bool first = true;

            foreach (TVStation station in TVStation.StationCollection)
            {
                if (station.Excluded || (station.NewName != null && station.NewName.Trim() != string.Empty) || station.LogicalChannelNumber != -1)
                {
                    if (first)
                    {
                        streamWriter.WriteLine();
                        streamWriter.WriteLine("[STATIONS]");
                        first = false;
                    }

                    streamWriter.Write("Station=" + station.OriginalNetworkID + "," + station.TransportStreamID + "," + station.ServiceID);

                    if (!station.Excluded)
                    {
                        streamWriter.Write("," + station.LogicalChannelNumber);
                        if (station.NewName != null && station.NewName.Trim() != string.Empty)
                            streamWriter.Write("," + station.NewName.Replace(",", "%%"));
                    }

                    streamWriter.WriteLine();
                }
            }
        }

        private  void outputScanListParameters(StreamWriter streamWriter)
        {
            if (TVStation.StationCollection.Count == 0)
                return;

            streamWriter.WriteLine();
            streamWriter.WriteLine("[SCANLIST]");

            foreach (TVStation station in TVStation.StationCollection)
                streamWriter.WriteLine("Scanned=" + station.OriginalNetworkID + "," + 
                    station.TransportStreamID + "," + 
                    station.ServiceID + "," +
                    station.LogicalChannelNumber + "," + 
                    station.Name.Replace(",", "%%"));
        }

        private void outputTimeOffsetParameters(StreamWriter streamWriter)
        {
            if (TimeOffsetChannel.Channels.Count == 0)
                return;

            streamWriter.WriteLine();
            streamWriter.WriteLine("[OFFSETS]");

            foreach (TimeOffsetChannel timeOffsetChannel in TimeOffsetChannel.Channels)
                streamWriter.WriteLine("Offset=" + timeOffsetChannel.SourceChannel.Name.Replace(",", "%%") + "," +
                    timeOffsetChannel.SourceChannel.OriginalNetworkID + "," +
                    timeOffsetChannel.SourceChannel.TransportStreamID + "," +
                    timeOffsetChannel.SourceChannel.ServiceID + "," +
                    timeOffsetChannel.DestinationChannel.Name.Replace(",", "%%") + "," +
                    timeOffsetChannel.DestinationChannel.OriginalNetworkID + "," +
                    timeOffsetChannel.DestinationChannel.TransportStreamID + "," +
                    timeOffsetChannel.DestinationChannel.ServiceID + "," +
                    timeOffsetChannel.Offset
                    );
        }

        private void outputServiceFilterParameters(StreamWriter streamWriter)
        {
            if (ChannelFilterEntry.ChannelFilters == null && maxService == -1)
                return;

            streamWriter.WriteLine();
            streamWriter.WriteLine("[SERVICEFILTERS]");

            if (maxService != -1)
                streamWriter.WriteLine("MaxService=" + maxService);

            if (ChannelFilterEntry.ChannelFilters != null)
            {
                foreach (ChannelFilterEntry filterEntry in ChannelFilterEntry.ChannelFilters)
                {
                    if (filterEntry.EndServiceID != -1)
                        streamWriter.WriteLine("IncludeService=" + filterEntry.OriginalNetworkID + "," +
                            filterEntry.TransportStreamID + "," +
                            filterEntry.StartServiceID + "," +
                            filterEntry.EndServiceID);
                    else
                    {
                        if (filterEntry.StartServiceID != -1)
                            streamWriter.WriteLine("IncludeService=" + filterEntry.OriginalNetworkID + "," +
                                filterEntry.TransportStreamID + "," +
                                filterEntry.StartServiceID);
                        else
                        {
                            if (filterEntry.TransportStreamID != -1)
                                streamWriter.WriteLine("IncludeService=" + filterEntry.OriginalNetworkID + "," +
                                filterEntry.TransportStreamID);
                            else
                            {
                                if (filterEntry.OriginalNetworkID != -1)
                                    streamWriter.WriteLine("IncludeService=" + filterEntry.OriginalNetworkID);                                
                            }
                        }
                    }
                }
            }
        }

        private void outputRepeatExclusionParameters(StreamWriter streamWriter)
        {
            if (RepeatExclusion.Exclusions == null && RepeatExclusion.PhrasesToIgnore == null)
                return;

            streamWriter.WriteLine();
            streamWriter.WriteLine("[REPEATEXCLUSIONS]");

            if (RepeatExclusion.Exclusions != null && RepeatExclusion.Exclusions.Count != 0)
            {
                foreach (RepeatExclusion exclusion in RepeatExclusion.Exclusions)
                    streamWriter.WriteLine("RepeatExclusion=" + exclusion.Title + "," + exclusion.Description);
            }

            if (RepeatExclusion.PhrasesToIgnore != null && RepeatExclusion.PhrasesToIgnore.Count != 0)
            {
                foreach (string phrase in RepeatExclusion.PhrasesToIgnore)
                    streamWriter.WriteLine("RepeatPhrase=" + phrase);
            }
        }

        /// <summary>
        /// Create a copy of this instance.
        /// </summary>
        /// <returns>A new instance with the same properties as the current instance.</returns>
        public RunParameters Clone()
        {
            RunParameters newParameters = new RunParameters(parameterSet);

            if (parameterSet == ParameterSet.Collector)
            {
                if (selectedTuners != null)
                {
                    newParameters.selectedTuners = new Collection<int>();
                    foreach (int tuner in selectedTuners)
                        newParameters.selectedTuners.Add(tuner);
                }
            }

            newParameters.outputFileName = outputFileName;
            newParameters.wmcImportName = wmcImportName;
            newParameters.frequencyTimeout = frequencyTimeout;
            newParameters.lockTimeout = lockTimeout;
            newParameters.repeats = repeats;
            newParameters.countryCode = countryCode;
            newParameters.region = region;
            newParameters.inputLanguage = inputLanguage;
            newParameters.outputLanguage = outputLanguage;
            newParameters.timeZone = timeZone;
            newParameters.nextTimeZone = nextTimeZone;
            newParameters.nextTimeZoneChange = nextTimeZoneChange;
            newParameters.timeZoneSet = timeZoneSet;

            if (options != null)
            {
                newParameters.options = new Collection<string>();
                foreach (string option in options)
                    newParameters.options.Add(option);
            }

            if (traceIDs != null)
            {
                newParameters.traceIDs = new Collection<string>();
                foreach (string traceID in traceIDs)
                    newParameters.traceIDs.Add(traceID);
            }

            if (debugIDs != null)
            {
                newParameters.debugIDs = new Collection<string>();
                foreach (string debugID in debugIDs)
                    newParameters.debugIDs.Add(debugID);
            }

            newParameters.tsFileName = tsFileName;
            newParameters.channelBouquet = channelBouquet;
            newParameters.channelRegion = channelRegion;
            newParameters.diseqcIdentity = diseqcIdentity;
            newParameters.eitPid = eitPid;

            if (mhw1Pids != null)
                newParameters.mhw1Pids = (int[])mhw1Pids.Clone();

            if (mhw2Pids != null)
                newParameters.mhw2Pids = (int[])mhw2Pids.Clone();

            if (parameterSet == ParameterSet.Plugin)
            {
                if (pluginFrequency != null)
                    newParameters.pluginFrequency = pluginFrequency.Clone();
            }
            else
            {
                originalStations = new Collection<TVStation>();
                foreach (TVStation station in TVStation.StationCollection)
                    originalStations.Add(station.Clone());

                originalFrequencies = new Collection<TuningFrequency>();
                foreach (TuningFrequency frequency in TuningFrequency.FrequencyCollection)
                    originalFrequencies.Add(frequency.Clone());
            }

            originalTimeOffsets = new Collection<TimeOffsetChannel>();

            foreach (TimeOffsetChannel oldOffset in TimeOffsetChannel.Channels)
            {
                TimeOffsetChannel newOffset = new TimeOffsetChannel(oldOffset.SourceChannel, oldOffset.DestinationChannel, oldOffset.Offset);
                originalTimeOffsets.Add(newOffset);
            }

            if (ChannelFilterEntry.ChannelFilters == null)
                originalFilters = null;
            else
            {
                originalFilters = new Collection<ChannelFilterEntry>();

                foreach (ChannelFilterEntry oldFilterEntry in ChannelFilterEntry.ChannelFilters)
                {
                    ChannelFilterEntry newFilterEntry = new ChannelFilterEntry(oldFilterEntry.OriginalNetworkID,
                        oldFilterEntry.TransportStreamID, oldFilterEntry.StartServiceID, oldFilterEntry.EndServiceID);

                    originalFilters.Add(newFilterEntry);
                }
            }

            if (RepeatExclusion.Exclusions == null)
                originalRepeatExclusions = null;
            else
            {
                originalRepeatExclusions = new Collection<RepeatExclusion>();

                foreach (RepeatExclusion oldRepeatExclusion in RepeatExclusion.Exclusions)
                {
                    RepeatExclusion newRepeatExclusion = new RepeatExclusion(oldRepeatExclusion.Title, oldRepeatExclusion.Description);
                    originalRepeatExclusions.Add(newRepeatExclusion);
                }
            }

            if (RepeatExclusion.PhrasesToIgnore == null)
                originalPhrasesToIgnore = null;
            else
            {
                originalPhrasesToIgnore = new Collection<string>();

                foreach (string oldPhrase in RepeatExclusion.PhrasesToIgnore)
                    originalPhrasesToIgnore.Add(oldPhrase);
            }

            newParameters.MaxService = maxService;

            return (newParameters);
        }

        /// <summary>
        /// Check if there have been and data changes.
        /// </summary>
        /// <param name="oldParameters">The original parameter values.</param>
        /// <returns>HasChanged if the data has changed; NotChanged otherwise.</returns>
        public DataState HasDataChanged(RunParameters oldParameters)
        {
            if (parameterSet == ParameterSet.Collector)
            {
                if (SelectedTuners.Count != oldParameters.SelectedTuners.Count)
                    return (DataState.Changed);

                foreach (int tuner in SelectedTuners)
                {
                    if (!oldParameters.SelectedTuners.Contains(tuner))
                        return (DataState.Changed);
                }
            }

            if (outputFileName != oldParameters.OutputFileName)
                return (DataState.Changed);
            if (wmcImportName != oldParameters.WMCImportName)
                return(DataState.Changed);
            if (maxService != oldParameters.MaxService)
                return(DataState.Changed);
            if (frequencyTimeout != oldParameters.FrequencyTimeout)
                return (DataState.Changed);
            if (lockTimeout != oldParameters.LockTimeout)
                return (DataState.Changed);
            if (repeats != oldParameters.Repeats)
                return (DataState.Changed);           
            if (countryCode != oldParameters.CountryCode)
                return (DataState.Changed);
            if (region != oldParameters.Region)
                return (DataState.Changed);
            if (inputLanguage != oldParameters.InputLanguage)
                return (DataState.Changed);
            if (outputLanguage != oldParameters.OutputLanguage)
                return (DataState.Changed);
            if (timeZoneSet != oldParameters.TimeZoneSet)
                return (DataState.Changed);
            if (timeZone != oldParameters.TimeZone)
                return (DataState.Changed);
            if (nextTimeZone != oldParameters.NextTimeZone)
                return (DataState.Changed);
            if (nextTimeZoneChange != oldParameters.NextTimeZoneChange)
                return (DataState.Changed);

            if ((options == null && oldParameters.Options != null) || (options != null && oldParameters.Options == null))
                return (DataState.Changed);

            if (options != null)
            {
                if (options.Count != oldParameters.Options.Count)
                    return (DataState.Changed);

                foreach (string option in options)
                {
                    if (!oldParameters.Options.Contains(option))
                        return (DataState.Changed);
                }
            }

            if ((traceIDs == null && oldParameters.TraceIDs != null) || (traceIDs != null && oldParameters.TraceIDs == null))
                return (DataState.Changed);

            if (traceIDs != null)
            {
                if (traceIDs.Count != oldParameters.TraceIDs.Count)
                    return (DataState.Changed);

                foreach (string traceID in traceIDs)
                {
                    if (!oldParameters.TraceIDs.Contains(traceID))
                        return (DataState.Changed);
                }
            }

            if ((debugIDs == null && oldParameters.DebugIDs != null) || (debugIDs != null && oldParameters.DebugIDs == null))
                return (DataState.Changed);

            if (debugIDs != null)
            {
                if (debugIDs.Count != oldParameters.DebugIDs.Count)
                    return (DataState.Changed);

                foreach (string debugID in debugIDs)
                {
                    if (!oldParameters.DebugIDs.Contains(debugID))
                        return (DataState.Changed);
                }
            }

            if (tsFileName != oldParameters.TSFileName)
                return (DataState.Changed);
            if (channelBouquet != oldParameters.ChannelBouquet)
                return (DataState.Changed);
            if (channelRegion != oldParameters.ChannelRegion)
                return (DataState.Changed);
            if (diseqcIdentity != oldParameters.DiseqcIdentity)
                return (DataState.Changed);
            if (eitPid != oldParameters.EITPid)
                return (DataState.Changed);

            if ((mhw1Pids == null && oldParameters.MHW1Pids != null) || (mhw1Pids != null && oldParameters.MHW1Pids == null))
                return (DataState.Changed);

            if (mhw1Pids != null)
            {
                if (mhw1Pids.Length != oldParameters.MHW1Pids.Length)
                    return (DataState.Changed);

                foreach (int newPid in mhw1Pids)
                {
                    bool found = false;
                    foreach (int oldPid in oldParameters.MHW1Pids)
                    {
                        if (oldPid == newPid)
                            found = true;
                    }
                    if (!found)
                        return (DataState.Changed);
                }
            }

            if ((mhw2Pids == null && oldParameters.MHW2Pids != null) || (mhw2Pids != null && oldParameters.MHW2Pids == null))
                return (DataState.Changed);

            if (mhw2Pids != null)
            {
                if (mhw2Pids.Length != oldParameters.MHW2Pids.Length)
                    return (DataState.Changed);

                foreach (int newPid in mhw2Pids)
                {
                    bool found = false;
                    foreach (int oldPid in oldParameters.MHW2Pids)
                    {
                        if (oldPid == newPid)
                            found = true;
                    }
                    if (!found)
                        return (DataState.Changed);
                }
            }

            if (parameterSet == ParameterSet.Plugin)
            {
                if (pluginFrequency != null)
                {
                    if (pluginFrequency.GetType().FullName != oldParameters.pluginFrequency.GetType().FullName)
                        return (DataState.Changed);
                    else
                    {
                        if (pluginFrequency != null)
                        {
                            if (pluginFrequency.Provider.Name != oldParameters.pluginFrequency.Provider.Name)
                                return (DataState.Changed);
                            if (pluginFrequency.Frequency != oldParameters.pluginFrequency.Frequency)
                                return (DataState.Changed);
                            if (pluginFrequency.CollectionType != oldParameters.pluginFrequency.CollectionType)
                                return (DataState.Changed);
                        }
                    }
                }
            }
            else
            {
                if (originalStations.Count != TVStation.StationCollection.Count)
                    return (DataState.Changed);

                foreach (TVStation originalStation in originalStations)
                {
                    TVStation newStation = TVStation.FindStation(originalStation.OriginalNetworkID, originalStation.TransportStreamID, originalStation.ServiceID);
                    if (newStation == null)
                        return (DataState.Changed);
                    else
                    {
                        if (!originalStation.EqualTo(newStation))
                            return (DataState.Changed);
                    }
                }

                if (originalFrequencies.Count != TuningFrequency.FrequencyCollection.Count)
                    return (DataState.Changed);

                for (int index = 0; index < originalFrequencies.Count; index++)
                {
                    if (!originalFrequencies[index].EqualTo(TuningFrequency.FrequencyCollection[index]))
                        return (DataState.Changed);
                }
            }

            if (originalTimeOffsets.Count != TimeOffsetChannel.Channels.Count)
                return (DataState.Changed);

            foreach (TimeOffsetChannel oldOffset in originalTimeOffsets)
            {
                TimeOffsetChannel newOffset = TimeOffsetChannel.Channels[originalTimeOffsets.IndexOf(oldOffset)];

                if (oldOffset.SourceChannel.Name != newOffset.SourceChannel.Name ||
                    oldOffset.DestinationChannel.Name != newOffset.DestinationChannel.Name ||
                    oldOffset.Offset != newOffset.Offset)
                    return (DataState.Changed);
            }

            if ((originalFilters == null && ChannelFilterEntry.ChannelFilters != null) ||
                originalFilters != null && ChannelFilterEntry.ChannelFilters == null)
                return(DataState.Changed);

            if (originalFilters != null)
            {
                if (originalFilters.Count != ChannelFilterEntry.ChannelFilters.Count)
                    return(DataState.Changed);

                foreach (ChannelFilterEntry oldFilterEntry in originalFilters)
                {
                    ChannelFilterEntry newFilterEntry = ChannelFilterEntry.ChannelFilters[originalFilters.IndexOf(oldFilterEntry)];
                    if (oldFilterEntry.OriginalNetworkID != newFilterEntry.OriginalNetworkID ||
                        oldFilterEntry.TransportStreamID != newFilterEntry.TransportStreamID ||
                        oldFilterEntry.StartServiceID != newFilterEntry.StartServiceID ||
                        oldFilterEntry.EndServiceID != newFilterEntry.EndServiceID)
                        return(DataState.Changed);
                }
            }

            if (originalRepeatExclusions != null)
            {
                if (originalRepeatExclusions.Count != RepeatExclusion.Exclusions.Count)
                    return (DataState.Changed);

                foreach (RepeatExclusion oldRepeatExclusion in originalRepeatExclusions)
                {
                    RepeatExclusion newRepeatExclusion = RepeatExclusion.Exclusions[originalRepeatExclusions.IndexOf(oldRepeatExclusion)];
                    if (oldRepeatExclusion.Title != newRepeatExclusion.Title ||
                        oldRepeatExclusion.Description != newRepeatExclusion.Description)
                        return (DataState.Changed);
                }
            }

            if (originalPhrasesToIgnore != null)
            {
                if (originalPhrasesToIgnore.Count != RepeatExclusion.PhrasesToIgnore.Count)
                    return (DataState.Changed);

                foreach (string oldPhrase in originalPhrasesToIgnore)
                {
                    if (RepeatExclusion.PhrasesToIgnore[originalPhrasesToIgnore.IndexOf(oldPhrase)] != oldPhrase)
                        return (DataState.Changed);
                }
            }

            return (DataState.NotChanged);
        }
    }
}
