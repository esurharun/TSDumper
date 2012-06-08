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

using System.Globalization;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;

namespace DomainObjects
{
    /// <summary>
    /// Initialize a new instance of the ISDBSatelliteProvider class.
    /// </summary>
    public class ISDBSatelliteProvider : Provider
    {
        /// <summary>
        /// Get the collection of providers.
        /// </summary>
        public static Collection<ISDBSatelliteProvider> Providers
        {
            get
            {
                if (providers == null)
                    providers = new Collection<ISDBSatelliteProvider>();
                return (providers);
            }
        }

        /// <summary>
        /// Get or set the azimuth of the satellite.
        /// </summary>
        public int Azimuth
        {
            get { return (azimuth); }
            set { azimuth = value; }
        }

        /// <summary>
        /// Get or set the elevation of the satellite.
        /// </summary>
        public int Elevation
        {
            get { return (elevation); }
            set { elevation = value; }
        }

        /// <summary>
        /// Get or set the longitude of the satellite in tenths of a degree.
        /// </summary>
        public int Longitude
        {
            get { return (longitude); }
            set { longitude = value; }
        }

        /// <summary>
        /// Get or set the east/west setting.
        /// </summary>
        public string EastWest
        {
            get { return (eastWest); }
            set { eastWest = value; }
        }

        /// <summary>
        /// Get the sort key for the satellite.
        /// </summary>
        public decimal SortKey
        {
            get
            {
                string[] parts = Name.Split(new char[] { ' ' });

                decimal eastWest = 0;
                if (parts[0].EndsWith("W"))
                    eastWest = 10000;

                decimal longitude = decimal.Parse(parts[0].Substring(0, parts[0].Length - 2), CultureInfo.InvariantCulture);
                return ((longitude * 10) + eastWest);
            }
        }

        private int azimuth;
        private int elevation;
        private int longitude;
        private string eastWest;

        private static Collection<ISDBSatelliteProvider> providers;

        /// <summary>
        /// Initialize a new instance of the ISDBSatelliteProvider class.
        /// </summary>
        /// <param name="name">The name of the satellite.</param>
        public ISDBSatelliteProvider(string name) : base(name)
        {
            string[] parts = Name.Split(new char[] { ' ' });
            if (parts.Length == 1)
                return;

            if (parts[0].EndsWith("E"))
                eastWest = "east";
            else
                eastWest = "west";

            longitude = (int)(decimal.Parse(parts[0].Substring(0, parts[0].Length - 2), CultureInfo.InvariantCulture) * 10);
        }

        /// <summary>
        /// Initialize a new instance of the ISDBSatelliteProvider class.
        /// </summary>
        /// <param name="longitude">The longitude in 1/10th's of a degree. Negative for west.</param>
        public ISDBSatelliteProvider(int longitude)
        {
            string namePart1 = ((decimal)(longitude / 10)).ToString();

            string namePart2;
            if (longitude < 0)
                namePart2 = "W";
            else
                namePart2 = "E";

            ISDBSatelliteProvider satellite = new ISDBSatelliteProvider(namePart1 + "\u00b0" + namePart2 + " Satellite");

            if (longitude < 0)
            {
                eastWest = "west";
                this.longitude = longitude * -1;
            }
            else
            {
                eastWest = "east";
                this.longitude = longitude;
            }
        }

        /// <summary>
        /// Find a satellite.
        /// </summary>
        /// <param name="name">The name of the ISDBSatelliteProvider.</param>
        /// <returns>The satellite or null if the name cannot be located.</returns>
        public static ISDBSatelliteProvider FindISDBSatellite(string name)
        {
            foreach (ISDBSatelliteProvider satellite in Providers)
            {
                if (satellite.Name == name)
                    return (satellite);
            }

            return (null);
        }

        /// <summary>
        /// Find a satellite.
        /// </summary>
        /// <param name="longitude">The longitude of the ISDBSatelliteProvider.</param>
        /// <returns>The satellite or null if the name cannot be located.</returns>
        public static ISDBSatelliteProvider FindISDBSatellite(int longitude)
        {
            foreach (ISDBSatelliteProvider satellite in Providers)
            {
                if (satellite.Longitude == longitude)
                    return (satellite);
            }

            return (null);
        }

        internal void load(FileInfo fileInfo)
        {
            ISDBSatelliteFrequency satelliteFrequency = null;
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
                            case "Transponder":
                                if (satelliteFrequency != null)
                                    AddFrequency(satelliteFrequency);

                                satelliteFrequency = new ISDBSatelliteFrequency();
                                satelliteFrequency.Provider = this;

                                break;
                            default:
                                if (satelliteFrequency != null)
                                    satelliteFrequency.load(reader);
                                break;
                        }
                    }
                }

                if (satelliteFrequency != null)
                    AddFrequency(satelliteFrequency);
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
        /// Load the satellite collection from the tuning files.
        /// </summary>
        /// <param name="directoryName">The full path of the directory containing the tuning files.</param>
        public static void Load(string directoryName)
        {
            Providers.Clear();

            DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);

            foreach (FileInfo fileInfo in directoryInfo.GetFiles("*.xml"))
            {
                ISDBSatelliteProvider satellite = new ISDBSatelliteProvider(fileInfo.Name.Substring(0, fileInfo.Name.Length - 4));
                satellite.load(fileInfo);
                AddProvider(satellite);
            }
        }

        /// <summary>
        /// Add a new frequency.
        /// </summary>
        /// <param name="newFrequency">The frequency to be added.</param>
        public void AddFrequency(ISDBSatelliteFrequency newFrequency)
        {
            foreach (ISDBSatelliteFrequency oldFrequency in Frequencies)
            {
                if (oldFrequency.Frequency == newFrequency.Frequency)
                {
                    if (oldFrequency.Polarization == newFrequency.Polarization)
                        return;
                    else
                    {
                        if (oldFrequency.Polarization.PolarizationAbbreviation.CompareTo(newFrequency.Polarization.PolarizationAbbreviation) > 0)
                        {
                            Frequencies.Insert(Frequencies.IndexOf(oldFrequency), newFrequency);
                            return;
                        }
                    }
                }

                if (oldFrequency.Frequency > newFrequency.Frequency)
                {
                    Frequencies.Insert(Frequencies.IndexOf(oldFrequency), newFrequency);
                    return;
                }
            }

            Frequencies.Add(newFrequency);
        }

        /// <summary>
        /// Add a provider to the list.
        /// </summary>
        /// <param name="newProvider">The provider to be added.</param>
        public static void AddProvider(ISDBSatelliteProvider newProvider)
        {
            foreach (ISDBSatelliteProvider oldProvider in Providers)
            {
                if (oldProvider.SortKey == newProvider.SortKey)
                    return;

                if (oldProvider.SortKey > newProvider.SortKey)
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
        public static ISDBSatelliteProvider FindProvider(string name)
        {
            foreach (ISDBSatelliteProvider provider in Providers)
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
        /// <param name="symbolRate">The symbol rate of the provider.</param>
        /// <param name="fecRate">The FEC rate of the provider.</param>
        /// <param name="polarization">The polarization of the provider.</param>
        /// <returns>The provider or null if it cannot be located.</returns>
        public static ISDBSatelliteProvider FindProvider(int frequency, int symbolRate, FECRate fecRate, SignalPolarization polarization)
        {
            foreach (ISDBSatelliteProvider provider in Providers)
            {
                foreach (ISDBSatelliteFrequency satelliteFrequency in provider.Frequencies)
                {
                    if (provider.Longitude == 1600)
                    {
                        int count = 0;
                        count++;
                    }

                    if (satelliteFrequency.Frequency == frequency &&
                        satelliteFrequency.SymbolRate == symbolRate &&
                        satelliteFrequency.FEC.Rate == fecRate.Rate &&
                        satelliteFrequency.Polarization.Polarization == polarization.Polarization)
                        return (provider);
                }
            }

            return (null);
        }

        /// <summary>
        /// Find a satelllite frequency.
        /// </summary>
        /// <param name="frequency">The frequency to be searched for.</param>
        /// <param name="polarization">The polariz\ation of the frequency to be searched for.</param>
        /// <returns>The tuning frequency or null if it cannot be located.</returns>
        public ISDBSatelliteFrequency FindFrequency(int frequency, SignalPolarization polarization)
        {
            foreach (TuningFrequency tuningFrequency in Frequencies)
            {
                if (tuningFrequency.Frequency == frequency)
                {
                    ISDBSatelliteFrequency satelliteFrequency = tuningFrequency as ISDBSatelliteFrequency;
                    if (satelliteFrequency != null && satelliteFrequency.Polarization.Polarization == polarization.Polarization)
                        return (satelliteFrequency);
                }
            }

            return (null);
        }

        /// <summary>
        /// Check if this instance is equal to another.
        /// </summary>
        /// <param name="satelliteProvider">The other instance.</param>
        /// <returns>True if the instances are equal; false otherwise.</returns>
        public bool EqualTo(ISDBSatelliteProvider satelliteProvider)
        {
            return (SortKey == satelliteProvider.SortKey);
        }
    }
}
