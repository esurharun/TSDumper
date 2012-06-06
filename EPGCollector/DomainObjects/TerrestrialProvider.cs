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
    /// The class that describes a terrestrial provider.
    /// </summary>
    public class TerrestrialProvider : Provider
    {
        /// <summary>
        /// Get the collection of providers.
        /// </summary>
        public static Collection<TerrestrialProvider> Providers
        {
            get
            {
                if (providers == null)
                    providers = new Collection<TerrestrialProvider>();
                return (providers);
            }
        }

        /// <summary>
        /// Get the country the provider is part of.
        /// </summary>
        public Country Country { get { return (country); } }

        /// <summary>
        /// Get the area the provider is part of.
        /// </summary>
        public Area Area { get { return (area); } }
        
        /// <summary>
        /// Get the collection of provider countries.
        /// </summary>
        public static Collection<Country> Countries
        {
            get
            {
                if (countries == null)
                    countries = new Collection<Country>();
                return (countries);
            }
        }

        private static Collection<TerrestrialProvider> providers;
        private static Collection<Country> countries;

        private Country country;
        private Area area;

        /// <summary>
        /// Initialize a new instance of the TerrestrialProvider class.
        /// </summary>
        /// <param name="name"></param>
        public TerrestrialProvider(string name) : base(name) 
        {
            if (!name.Contains("."))
            {
                country = new Country(name, null);
                area = null;
                return;
            }

            string[] parts = name.Split(new char[] { '.' });
            country = new Country(parts[0], null);
            area = new Area(parts[1], 0); 
        }

        internal void load(FileInfo fileInfo)
        {
            TerrestrialFrequency terrestrialFrequency = null;
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
                            case "DVBTTuning":
                                if (terrestrialFrequency != null)
                                    AddFrequency(terrestrialFrequency);

                                terrestrialFrequency = new TerrestrialFrequency();
                                terrestrialFrequency.Provider = this;

                                break;
                            default:
                                if (terrestrialFrequency != null)
                                    terrestrialFrequency.load(reader);
                                break;
                        }
                    }
                }

                if (terrestrialFrequency != null)
                    AddFrequency(terrestrialFrequency);
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
        /// Load the terrestrial collection from the tuning files.
        /// </summary>
        /// <param name="directoryName">The full path of the directory containing the tuning files.</param>
        public static void Load(string directoryName)
        {
            Providers.Clear();

            DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);

            foreach (FileInfo fileInfo in directoryInfo.GetFiles("*.xml"))
            {
                TerrestrialProvider terrestrialProvider = new TerrestrialProvider(fileInfo.Name.Substring(0, fileInfo.Name.Length - 4));
                terrestrialProvider.load(fileInfo);
                AddProvider(terrestrialProvider);
            }
        }

        /// <summary>
        /// Add a new provider.
        /// </summary>
        /// <param name="newProvider">The new provider instance.</param>
        public static void AddProvider(TerrestrialProvider newProvider)
        {
            addProvider(newProvider);

            Country newCountry = newProvider.Country;

            foreach (Country oldCountry in Countries)
            {
                if (oldCountry.Name == newCountry.Name)
                {
                    oldCountry.AddArea(newProvider.Area, false);
                    return;
                }

                if (oldCountry.Name.CompareTo(newCountry.Name) > 0)
                {
                    newCountry.AddArea(newProvider.Area, false);
                    countries.Insert(countries.IndexOf(oldCountry), newCountry);
                    return;
                }
            }

            newCountry.AddArea(newProvider.Area, false);
            countries.Add(newCountry);
        }

        private static void addProvider(TerrestrialProvider newProvider)
        {
            foreach (TerrestrialProvider oldProvider in Providers)
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
        /// Find the provider for a name.
        /// </summary>
        /// <param name="name">The provider name.</param>
        /// <returns>The provider or null if it cannot be located.</returns>
        public static TerrestrialProvider FindProvider(string name)
        {
            foreach (TerrestrialProvider provider in Providers)
            {
                if (provider.Name == name)
                    return (provider);
            }

            return (null);
        }

        /// <summary>
        /// Find the provider for a country and area.
        /// </summary>
        /// <param name="country">The country.</param>
        /// <param name="area">The area.</param>
        /// <returns>The provider or null if it cannot be located.</returns>
        public static TerrestrialProvider FindProvider(string country, string area)
        {
            foreach (TerrestrialProvider provider in Providers)
            {
                if (provider.Country.Name == country && provider.Area.Name == area)
                    return (provider);
            }

            return (null);
        }

        /// <summary>
        /// Find a provider given the broadcast parameters.
        /// </summary>
        /// <param name="frequency">The frequency of the provider.</param>
        /// <param name="bandWidth">The band width of the provider.</param>
        /// <returns>The provider or null if it cannot be located.</returns>
        public static TerrestrialProvider FindProvider(int frequency, int bandWidth)
        {
            foreach (TerrestrialProvider provider in Providers)
            {
                foreach (TerrestrialFrequency terrestrialFrequency in provider.Frequencies)
                {
                    if (terrestrialFrequency.Frequency == frequency &&
                        terrestrialFrequency.Bandwidth == bandWidth)
                        return (provider);
                }
            }

            return (null);
        }
    }
}
