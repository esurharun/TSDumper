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
    /// The class that describes a cable frequency.
    /// </summary>
    public class CableFrequency : ChannelTuningFrequency, IComparable
    {
        /// <summary>
        /// Get or set the symbol rate.
        /// </summary>
        public int SymbolRate
        {
            get { return (symbolRate); }
            set { symbolRate = value; }
        }

        /// <summary>
        /// Get or set the FEC rate.
        /// </summary>
        public FECRate FEC
        {
            get { return (fec); }
            set { fec = value; }
        }

        /// <summary>
        /// Get or set the modulation.
        /// </summary>
        public Modulation Modulation
        {
            get { return (modulation); }
            set { modulation = value; }
        }

        /// <summary>
        /// Returns a tuner type of cable.
        /// </summary>
        public override TunerType TunerType { get { return (TunerType.Cable); } }

        private int symbolRate = 22500;
        private FECRate fec = new FECRate();
        private Modulation modulation = Modulation.QPSK;
        
        /// <summary>
        /// Initialize a new instance of the CableFrequency class.
        /// </summary>
        public CableFrequency() { }

        internal void load(XmlReader reader)
        {
            switch (reader.Name)
            {
                case "ChannelNumber":
                    ChannelNumber = Int32.Parse(reader.ReadString(), CultureInfo.InvariantCulture);
                    break;
                case "Frequency":
                    Frequency = Int32.Parse(reader.ReadString(), CultureInfo.InvariantCulture);
                    break;
                case "ModulationType":
                    switch (reader.ReadString())
                    {
                        case "ModBPSK":
                            modulation = Modulation.BPSK;
                            break;
                        case "ModOQPSK":
                            modulation = Modulation.OQPSK;
                            break;
                        case "ModPSK8":
                            modulation = Modulation.PSK8;
                            break;
                        case "Mod1024Qam":
                            modulation = Modulation.QAM1024;
                            break;
                        case "Mod112Qam":
                            modulation = Modulation.QAM112;
                            break;
                        case "Mod128Qam":
                            modulation = Modulation.QAM128;
                            break;                        
                        case "Mod16Qam":
                            modulation = Modulation.QAM16;
                            break;
                        case "Mod160Qam":
                            modulation = Modulation.QAM160;
                            break;
                        case "Mod192Qam":
                            modulation = Modulation.QAM192;
                            break;
                        case "Mod224Qam":
                            modulation = Modulation.QAM224;
                            break;
                        case "Mod256Qam":
                            modulation = Modulation.QAM256;
                            break;
                        case "Mod32Qam":
                            modulation = Modulation.QAM32;
                            break;
                        case "Mod320Qam":
                            modulation = Modulation.QAM320;
                            break;
                        case "Mod384Qam":
                            modulation = Modulation.QAM384;
                            break;
                        case "Mod448Qam":
                            modulation = Modulation.QAM448;
                            break;
                        case "Mod512Qam":
                            modulation = Modulation.QAM512;
                            break;
                        case "Mod64Qam":
                            modulation = Modulation.QAM64;
                            break;
                        case "Mod640Qam":
                            modulation = Modulation.QAM640;
                            break;
                        case "Mod768Qam":
                            modulation = Modulation.QAM768;
                            break;
                        case "Mod80Qam":
                            modulation = Modulation.QAM80;
                            break;
                        case "Mod896Qam":
                            modulation = Modulation.QAM896;
                            break;
                        case "Mod96Qam":
                            modulation = Modulation.QAM96;
                            break;
                        case "ModQPSK":
                            modulation = Modulation.QPSK;
                            break;
                    }
                    break;
                case "SymbolRate":
                    symbolRate = Int32.Parse(reader.ReadString(), CultureInfo.InvariantCulture);
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
                case "OpenTVCode":
                    OpenTVCode = reader.ReadString();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Return a string describing the frequency.
        /// </summary>
        /// <returns>A string describing the frequency.</returns>
        public override string ToString()
        {
            if (ChannelNumber == 0)
                return (Frequency / 1000 + " MHz");
            else
                return ("Channel " + ChannelNumber + " (" + Frequency / 1000 + " MHz)");

        }

        /// <summary>
        /// Generate a copy of this frequency.
        /// </summary>
        /// <returns>A new instance with the same properties as the old instance.</returns>
        public override TuningFrequency Clone()
        {
            CableFrequency newFrequency = new CableFrequency();
            base.Clone(newFrequency);

            newFrequency.FEC = fec;
            newFrequency.SymbolRate = symbolRate;
            newFrequency.Modulation = modulation;

            return (newFrequency);
        }
    }
}
