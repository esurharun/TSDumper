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
using System.Xml;
using System.Collections.ObjectModel;
using System.Globalization;

namespace DomainObjects
{
    /// <summary>
    /// The class that describes an area.
    /// </summary>
    public class Area
    {
        /// <summary>
        /// Get the name of the area.
        /// </summary>
        public string Name { get { return (name); } }

        /// <summary>
        /// Get the code of the area.
        /// </summary>
        public int Code { get { return (code); } }

        /// <summary>
        /// Get the collection of regions in the area.
        /// </summary>
        public Collection<Region> Regions
        {
            get
            {
                if (regions == null)
                    regions = new Collection<Region>();
                return (regions);
            }
        }

        private string name;
        private int code;
        private Collection<Region> regions;

        private Area() { }

        /// <summary>
        /// Initialize a new instance of the Area class.
        /// </summary>
        /// <param name="name">The name of the area.</param>
        /// <param name="code">The code of the area.</param>
        public Area(string name, int code)
        {
            this.name = name;
            this.code = code;
        }

        internal void Load(XmlReader reader)
        {
            switch (reader.Name)
            {
                case "Region":
                    AddRegion(new Region(reader.GetAttribute("name"), Int32.Parse(reader.GetAttribute("code"), CultureInfo.InvariantCulture)));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Add a region to the area.
        /// </summary>
        /// <param name="newRegion">The new region.</param>
        public void AddRegion(Region newRegion)
        {
            if (newRegion == null)
                throw (new ArgumentException("The region cannot be null", "newRegion"));

            foreach (Region oldRegion in Regions)
            {
                if (oldRegion.Code == newRegion.Code)
                    return;

                if (oldRegion.Code > newRegion.Code)
                {
                    regions.Insert(regions.IndexOf(oldRegion), newRegion);
                    return;
                }
            }

            regions.Add(newRegion);
        }

        /// <summary>
        /// Find an region defined for the area.
        /// </summary>
        /// <param name="regionCode">The region code.</param>
        /// <returns>The region or null if it cannot be located.</returns>
        public Region FindRegion(int regionCode)
        {
            foreach (Region region in Regions)
            {
                if (region.Code == regionCode)
                    return (region);
            }

            return (null);
        }

        /// <summary>
        /// Return a description of this instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return (name);
        }
    }
}
