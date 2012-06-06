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
using System.Xml;
using System.Collections.ObjectModel;

namespace DomainObjects
{
    /// <summary>
    /// The class that describes an ISDB terrestrial provider.
    /// </summary>
    public class ISDBTerrestrialProvider : Provider
    {
        /// <summary>
        /// Get the collection of providers.
        /// </summary>
        public static Collection<ISDBTerrestrialProvider> Providers
        {
            get
            {
                if (providers == null)
                    providers = new Collection<ISDBTerrestrialProvider>();
                return (providers);
            }
        }

        private static Collection<ISDBTerrestrialProvider> providers;

        /// <summary>
        /// Get the collection of channels from the provider.
        /// </summary>
        public Collection<TuningFrequency> Channels
        {
            get
            {
                Collection<TuningFrequency> channels = new Collection<TuningFrequency>();

                foreach (ISDBTerrestrialFrequency newChannel in Frequencies)
                {
                    bool inserted = false;

                    foreach (ISDBTerrestrialFrequency oldChannel in channels)
                    {
                        if (oldChannel.ChannelNumber > newChannel.ChannelNumber)
                        {
                            channels.Insert(channels.IndexOf(oldChannel), newChannel);
                            inserted = true;
                            break;
                        }
                    }

                    if (!inserted)
                        channels.Add(newChannel);
                }

                return (channels);
            }
        }

        /// <summary>
        /// Initialize a new instance of the ISDBTerrestrialProvider class.
        /// </summary>
        /// <param name="name"></param>
        public ISDBTerrestrialProvider(string name) : base(name) { }

        internal void load(FileInfo fileInfo)
        {
            ISDBTerrestrialFrequency isdbFrequency = null;
            XmlReader reader = null;

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            try
            {
                reader = XmlReader.Create(fileInfo.FullName, settings);
            }
            catch (IOException)
            {
                Logger.Instance.Write("Failed to open " + fileInfo.Name);
                return;
            }

            try
            {
                while (!reader.EOF)
                {
                    reader.Read();
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "Channel":
                                if (isdbFrequency != null)
                                    AddFrequency(isdbFrequency);

                                isdbFrequency = new ISDBTerrestrialFrequency();
                                isdbFrequency.Provider = this;                                

                                break;
                            default:
                                if (isdbFrequency != null)
                                    isdbFrequency.load(reader);
                                break;
                        }
                    }
                }

                if (isdbFrequency != null)
                    AddFrequency(isdbFrequency);
            }
            catch (XmlException e)
            {
                Logger.Instance.Write("Failed to load file " + fileInfo.Name);
                Logger.Instance.Write("Data exception: " + e.Message);
            }
            catch (IOException e)
            {
                Logger.Instance.Write("Failed to load file " + fileInfo.Name);
                Logger.Instance.Write("I/O exception: " + e.Message);
            }

            if (reader != null)
                reader.Close();
        }

        /// <summary>
        /// Load the ATSC collection from the tuning files.
        /// </summary>
        /// <param name="directoryName">The full path of the directory containing the tuning files.</param>
        public static void Load(string directoryName)
        {
            Providers.Clear();

            DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);

            foreach (FileInfo fileInfo in directoryInfo.GetFiles("*.xml"))
            {
                ISDBTerrestrialProvider atscProvider = new ISDBTerrestrialProvider(fileInfo.Name.Substring(0, fileInfo.Name.Length - 4));
                atscProvider.load(fileInfo);
                AddProvider(atscProvider);
            }
        }

        /// <summary>
        /// Add a provider to the list.
        /// </summary>
        /// <param name="newProvider">The provider to be added.</param>
        public static void AddProvider(ISDBTerrestrialProvider newProvider)
        {
            foreach (ISDBTerrestrialProvider oldProvider in Providers)
            {
                if (oldProvider.Name == newProvider.Name)
                    return;

                if (oldProvider.Name.CompareTo(newProvider.Name) > 0)
                {
                    Providers.Insert(Providers.IndexOf(oldProvider), newProvider);
                    return;
                }
            }

            Providers.Add(newProvider);
        }

        /// <summary>
        /// Find a provider.
        /// </summary>
        /// <param name="name">The name of the provider.</param>
        /// <returns>The provider or null if the name cannot be located.</returns>
        public static ISDBTerrestrialProvider FindProvider(string name)
        {
            foreach (ISDBTerrestrialProvider provider in Providers)
            {
                if (provider.Name == name)
                    return (provider);
            }

            return (null);
        }

        /// <summary>
        /// Find a provider given the broadcast parameters.
        /// </summary>
        /// <param name="frequency">The frequency of the provider.</param>
        /// <param name="bandWidth">The band width of the provider.</param>
        public static ISDBTerrestrialProvider FindProvider(int frequency, int bandWidth)
        {
            foreach (ISDBTerrestrialProvider provider in Providers)
            {
                foreach (ISDBTerrestrialFrequency atscFrequency in provider.Frequencies)
                {
                    if (atscFrequency.Frequency == frequency &&
                        atscFrequency.Bandwidth == bandWidth)
                        return (provider);
                }
            }

            return (null);
        }
    }
}
