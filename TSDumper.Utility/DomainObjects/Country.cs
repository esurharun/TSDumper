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
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;

namespace DomainObjects
{
    /// <summary>
    /// The class that describes a country.
    /// </summary>
    public class Country
    {
        /// <summary>
        /// Get the name of the country.
        /// </summary>
        public string Name { get { return(name); } }

        /// <summary>
        /// Get the code of the country.
        /// </summary>
        public string Code { get { return (code); } }
        
        /// <summary>
        /// Get the collection of areas in the country.
        /// </summary>
        public Collection<Area> Areas
        {
            get
            {
                if (areas == null)
                    areas = new Collection<Area>();
                return(areas);
            }
        }

        /// <summary>
        /// Get the collection of time zones in the country.
        /// </summary>
        public Collection<Region> TimeZones
        {
            get
            {
                if (timeZones == null)
                    timeZones = new Collection<Region>();
                return (timeZones);
            }
        }

        private string name;
        private string code;
        private Collection<Area> areas;
        private Collection<Region> timeZones;

        private Area lastArea;

        private Country() { }

        /// <summary>
        /// Initialize a new instance of the Country class.
        /// </summary>
        /// <param name="name">The name of the country.</param>
        /// <param name="code">The code of the country.</param>
        public Country(string name, string code)
        {
            this.name = name;
            this.code = code;
        }

        internal void load(XmlReader reader)
        {
            switch (reader.Name)
            {
                case "TimeZone":
                    AddTimeZone(new Region(reader.GetAttribute("name"), Int32.Parse(reader.GetAttribute("code"))), true);
                    break;
                case "Area":
                    lastArea = new Area(reader.GetAttribute("name"), Int32.Parse(reader.GetAttribute("code")));
                    AddArea(lastArea, true);
                    break;
                default:
                    lastArea.Load(reader);
                    break;
            }
        }

        /// <summary>
        /// Add an area to the country.
        /// </summary>
        /// <param name="newArea">The area to be added.</param>
        /// <param name="addUndefined">True if an undefined entry is to be added to the start of the list; false otherwise.</param>
        public void AddArea(Area newArea, bool addUndefined)
        {
            if (addUndefined && Areas.Count == 0)
            {
                Area undefinedArea = new Area("-- Undefined --", 0);
                undefinedArea.AddRegion(new Region("-- Undefined --", 0));
                Areas.Add(undefinedArea);
            }

            foreach (Area oldArea in Areas)
            {
                if (oldArea.Name == newArea.Name)
                    return;

                if (oldArea.Name.CompareTo(newArea.Name) > 0)
                {
                    areas.Insert(areas.IndexOf(oldArea), newArea);
                    return;
                }
            }

            areas.Add(newArea);
        }

        /// <summary>
        /// Add a time zone region to the country.
        /// </summary>
        /// <param name="newRegion">The region to be added.</param>
        /// <param name="addUndefined">True if an undefined entry is to be added to the start of the list; false otherwise.</param>
        public void AddTimeZone(Region newRegion, bool addUndefined)
        {
            if (addUndefined && TimeZones.Count == 0)
                TimeZones.Add(new Region("-- Undefined --", 0));

            foreach (Region oldRegion in TimeZones)
            {
                if (oldRegion.Name == newRegion.Name)
                    return;

                if (oldRegion.Name.CompareTo(newRegion.Name) > 0)
                {
                    TimeZones.Insert(TimeZones.IndexOf(oldRegion), newRegion);
                    return;
                }
            }

            TimeZones.Add(newRegion);
        }

        /// <summary>
        /// Return a description of this instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return (name);
        }

        /// <summary>
        /// Load the country collection from the configuration file.
        /// </summary>
        public static Collection<Country> Load()
        {
            Collection<Country> countries = new Collection<Country>();

            Country country = null;
            XmlReader reader = null;

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            try
            {
                reader = XmlReader.Create(Path.Combine(RunParameters.ConfigDirectory, "Countries.cfg"), settings);
            }
            catch (IOException)
            {
                Logger.Instance.Write("Failed to open " + Path.Combine(RunParameters.ConfigDirectory, "Countries.cfg"));
                return(countries);
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
                            case "Country":
                                if (country != null)
                                    addCountry(country, countries);

                                country = new Country(reader.GetAttribute("name"), reader.GetAttribute("code"));

                                break;
                            default:
                                if (country != null)
                                    country.load(reader);
                                break;
                        }
                    }
                }

                if (country != null)
                    addCountry(country, countries);
            }
            catch (XmlException e)
            {
                Logger.Instance.Write("Failed to load file " + Path.Combine(RunParameters.ConfigDirectory, "Countries.cfg"));
                Logger.Instance.Write("Data exception: " + e.Message);
            }
            catch (IOException e)
            {
                Logger.Instance.Write("Failed to load file " + Path.Combine(RunParameters.ConfigDirectory, "Countries.cfg"));
                Logger.Instance.Write("I/O exception: " + e.Message);
            }

            if (reader != null)
                reader.Close();

            return (countries);
        }

        /// <summary>
        /// Add a country to a collection.
        /// </summary>
        /// <param name="newCountry">The country to be added.</param>
        /// <param name="countries">The collection of countries to be added to.</param>
        public static void addCountry(Country newCountry, Collection<Country> countries)
        {
            if (countries.Count == 0)
            {
                Country undefinedCountry = new Country("-- Undefined --", "");
                Area undefinedArea = new Area("-- Undefined --", 0);
                undefinedArea.Regions.Add(new Region("-- Undefined --", 0));
                undefinedCountry.Areas.Add(undefinedArea);
                countries.Add(undefinedCountry);
            }

            foreach (Country oldCountry in countries)
            {
                if (oldCountry.Code == newCountry.Code)
                    return;

                if (oldCountry.Code.CompareTo(newCountry.Code) > 0)
                {
                    countries.Insert(countries.IndexOf(oldCountry), newCountry);
                    return;
                }
            }

            countries.Add(newCountry);
        }

        /// <summary>
        /// Find a country given the country code.
        /// </summary>
        /// <param name="countryCode">The country code.</param>
        /// <param name="countries">The collection of countries to search.</param>
        /// <returns>The country or null if it cannot be located.</returns>
        public static Country FindCountryCode(string countryCode, Collection<Country> countries)
        {
            foreach (Country country in countries)
            {
                if (country.Code == countryCode)
                    return (country);
            }

            return (null);
        }

        /// <summary>
        /// Find an area defined for the country.
        /// </summary>
        /// <param name="areaCode">The area code.</param>
        /// <returns>The area or null if it cannot be located.</returns>
        public Area FindArea(int areaCode)
        {
            foreach (Area area in Areas)
            {
                if (area.Code == areaCode)
                {
                    return (area);
                }
            }

            return (null);
        }

        /// <summary>
        /// Find an area defined for the country that contains a specific region.
        /// </summary>
        /// <param name="areaCode">The area code.</param>
        /// <param name="regionCode">The region code.</param>
        /// <returns>The area or null if it cannot be located.</returns>
        public Area FindArea(int areaCode, int regionCode)
        {
            foreach (Area area in Areas)
            {
                if (area.Code == areaCode)
                {
                    foreach (Region region in area.Regions)
                    {
                        if (region.Code == regionCode)
                            return (area);
                    }
                }
            }

            return (null);
        }
    }
}
