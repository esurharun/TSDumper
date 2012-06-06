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
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;

namespace DomainObjects
{
    /// <summary>
    /// The class that describes an ATSC provider.
    /// </summary>
    public class AtscProvider : Provider
    {
        /// <summary>
        /// Get the collection of providers.
        /// </summary>
        public static Collection<AtscProvider> Providers
        {
            get
            {
                if (providers == null)
                    providers = new Collection<AtscProvider>();
                return (providers);
            }
        }

        private static Collection<AtscProvider> providers;

        /// <summary>
        /// Get the collection of channels from the provider.
        /// </summary>
        public Collection<TuningFrequency> Channels
        {
            get
            {
                Collection<TuningFrequency> channels = new Collection<TuningFrequency>();

                foreach (AtscFrequency newChannel in Frequencies)
                {
                    bool inserted = false;

                    foreach (AtscFrequency oldChannel in channels)
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
        /// Initialize a new instance of the AtscProvider class.
        /// </summary>
        /// <param name="name"></param>
        public AtscProvider(string name) : base(name) { }

        internal void load(FileInfo fileInfo)
        {
            AtscFrequency atscFrequency = null;
            XmlReader reader = null;

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            try
            {
                reader = XmlReader.Create(fileInfo.FullName, settings);

                while (!reader.EOF)
                {
                    reader.Read();
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "Channel":
                                if (atscFrequency != null)
                                    AddFrequency(atscFrequency);

                                atscFrequency = new AtscFrequency();
                                atscFrequency.Provider = this;

                                break;
                            default:
                                if (atscFrequency != null)
                                    atscFrequency.load(reader);
                                break;
                        }
                    }
                }

                if (atscFrequency != null)
                    AddFrequency(atscFrequency);
            }
            catch (IOException e)
            {
                Logger.Instance.Write("Failed to load file " + fileInfo.Name);
                Logger.Instance.Write("I/O exception: " + e.Message);
            }
            catch (XmlException e)
            {
                Logger.Instance.Write("Failed to load file " + fileInfo.Name);
                Logger.Instance.Write("Data exception: " + e.Message);
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
                AtscProvider atscProvider = new AtscProvider(fileInfo.Name.Substring(0, fileInfo.Name.Length - 4));
                atscProvider.load(fileInfo);
                AddProvider(atscProvider);
            }
        }

        /// <summary>
        /// Add a provider to the list.
        /// </summary>
        /// <param name="newProvider">The provider to be added.</param>
        public static void AddProvider(AtscProvider newProvider)
        {
            foreach (AtscProvider oldProvider in Providers)
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
        public static AtscProvider FindProvider(string name)
        {
            foreach (AtscProvider provider in Providers)
            {
                if (provider.Name == name)
                    return (provider);
            }

            return (null);
        }

        /// <summary>
        /// Find a provider given the broadcast parameters.
        /// </summary>
        /// <param name="channelNumber">The channel number of the provider.</param>
        /// <param name="frequency">The frequency of the provider.</param>
        /// <param name="symbolRate">The symbol rate of the provider.</param>
        /// <param name="fecRate">The FEC rate of the provider.</param>
        /// <param name="modulation">The modulation of the provider.</param>
        /// <returns>The provider or null if it cannot be located.</returns>
        public static AtscProvider FindProvider(int channelNumber, int frequency, int symbolRate, FECRate fecRate, Modulation modulation)
        {
            if (fecRate == null)
                throw (new ArgumentException("The FEC rate cannot be null", "fecRate"));

            foreach (AtscProvider provider in Providers)
            {
                foreach (AtscFrequency atscFrequency in provider.Frequencies)
                {
                    if (atscFrequency.ChannelNumber == channelNumber &&
                        atscFrequency.Frequency == frequency &&
                        atscFrequency.SymbolRate == symbolRate &&
                        atscFrequency.FEC.Rate == fecRate.Rate &&
                        atscFrequency.Modulation == modulation)
                        return (provider);
                }
            }

            return (null);
        }
    }
}
