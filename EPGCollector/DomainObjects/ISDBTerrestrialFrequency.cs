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
using System.Globalization;

namespace DomainObjects
{
    /// <summary>
    /// The class that describes a terrestrial frequency.
    /// </summary>
    public class ISDBTerrestrialFrequency : ChannelTuningFrequency, IComparable
    {
        /// <summary>
        /// Get or set the bandwidth.
        /// </summary>
        public int Bandwidth
        {
            get { return (bandwidth); }
            set { bandwidth = value; }
        }

        /// <summary>
        /// Get the tuner type for this type of frequency.
        /// </summary>
        public override TunerType TunerType { get { return (TunerType.ISDBT); } }

        private int bandwidth;

        /// <summary>
        /// Initialize a new instance of the ISDBTerrestrialFrequency class.
        /// </summary>
        public ISDBTerrestrialFrequency() { }

        internal void load(XmlReader reader)
        {
            switch (reader.Name)
            {
                case "ChannelNumber":
                    ChannelNumber = Int32.Parse(reader.ReadString(), CultureInfo.InvariantCulture);
                    break;
                case "CarrierFrequency":
                    Frequency = Int32.Parse(reader.ReadString());
                    break;
                case "BandWidth":
                    Bandwidth = Int32.Parse(reader.ReadString());
                    break;
                case "CollectionType":
                    switch (reader.ReadString())
                    {
                        case "EIT":
                            CollectionType = CollectionType.EIT;
                            break;
                        case "MHEG5":
                            CollectionType = CollectionType.MHEG5;
                            break;
                        case "OPENTV":
                            CollectionType = CollectionType.OpenTV;
                            break;
                        case "MHW1":
                            CollectionType = CollectionType.MediaHighway1;
                            break;
                        case "MHW2":
                            CollectionType = CollectionType.MediaHighway2;
                            break;
                        case "FREESAT":
                            CollectionType = CollectionType.FreeSat;
                            break;
                        case "PSIP":
                            CollectionType = CollectionType.PSIP;
                            break;
                        case "DISHNETWORK":
                            CollectionType = CollectionType.DishNetwork;
                            break;
                        case "BELLTV":
                            CollectionType = CollectionType.BellTV;
                            break;
                        case "SIEHFERNINFO":
                            CollectionType = CollectionType.SiehfernInfo;
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Get a description of this instance.
        /// </summary>
        /// <returns>A string describing this instance.</returns>
        public override string ToString()
        {
            return ("Channel " + ChannelNumber + " (" + Frequency / 1000 + " MHz)");
        }

        /// <summary>
        /// Generate a copy of this frequency.
        /// </summary>
        /// <returns>A new instance with the same properties as the old instance.</returns>
        public override TuningFrequency Clone()
        {
            ISDBTerrestrialFrequency newFrequency = new ISDBTerrestrialFrequency();
            base.Clone(newFrequency);

            newFrequency.Bandwidth = bandwidth;

            return (newFrequency);
        }
    }
}
