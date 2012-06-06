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
    /// The class that describes an ATSC frequency.
    /// </summary>
    public class AtscFrequency : ChannelTuningFrequency, IComparable
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
        /// Get or set the forward error correction system.
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
        /// Get the tuner type for this type of frequency.
        /// </summary>
        public override TunerType TunerType 
        { 
            get 
            {
                if (modulation == DomainObjects.Modulation.VSB8)
                    return (TunerType.ATSC);
                else
                    return (TunerType.ATSCCable);
            } 
        }

        private int symbolRate = 6000;
        private FECRate fec = new FECRate();
        private Modulation modulation = Modulation.VSB8;

        /// <summary>
        /// Initialize a new instance of the AtscFrequency class.
        /// </summary>
        public AtscFrequency() { }

        internal void load(XmlReader reader)
        {
            switch (reader.Name)
            {
                case "ChannelNumber":
                    ChannelNumber = Int32.Parse(reader.ReadString(), CultureInfo.InvariantCulture);
                    break;
                case "CarrierFrequency":
                    Frequency = Int32.Parse(reader.ReadString(), CultureInfo.InvariantCulture);
                    break;
                case "SymbolRate":
                    symbolRate = Int32.Parse(reader.ReadString(), CultureInfo.InvariantCulture);
                    break;
                case "Modulation":
                    switch (reader.ReadString())
                    {
                        case "BPSK":
                            modulation = Modulation.BPSK;
                            break;
                        case "OQPSK":
                            modulation = Modulation.OQPSK;
                            break;
                        case "PSK8":
                            modulation = Modulation.PSK8;
                            break;
                        case "QAM1024":
                            modulation = Modulation.QAM1024;
                            break;
                        case "QAM112":
                            modulation = Modulation.QAM112;
                            break;
                        case "QAM128":
                            modulation = Modulation.QAM128;
                            break;
                        case "QAM16":
                            modulation = Modulation.QAM16;
                            break;
                        case "QAM160":
                            modulation = Modulation.QAM160;
                            break;
                        case "QAM192":
                            modulation = Modulation.QAM192;
                            break;
                        case "QAM224":
                            modulation = Modulation.QAM224;
                            break;
                        case "QAM256":
                            modulation = Modulation.QAM256;
                            break;
                        case "QAM32":
                            modulation = Modulation.QAM32;
                            break;
                        case "QAM320":
                            modulation = Modulation.QAM320;
                            break;
                        case "QAM384":
                            modulation = Modulation.QAM384;
                            break;
                        case "QAM448":
                            modulation = Modulation.QAM448;
                            break;
                        case "QAM512":
                            modulation = Modulation.QAM512;
                            break;
                        case "QAM64":
                            modulation = Modulation.QAM64;
                            break;
                        case "QAM640":
                            modulation = Modulation.QAM640;
                            break;
                        case "QAM768":
                            modulation = Modulation.QAM768;
                            break;
                        case "QAM80":
                            modulation = Modulation.QAM80;
                            break;
                        case "QAM896":
                            modulation = Modulation.QAM896;
                            break;
                        case "QAM96":
                            modulation = Modulation.QAM96;
                            break;
                        case "QPSK":
                            modulation = Modulation.QPSK;
                            break;
                        case "VSB16":
                            modulation = Modulation.VSB16;
                            break;
                        case "VSB8":
                            modulation = Modulation.VSB8;
                            break;
                    }
                    break;
                case "InnerFecRate":
                    switch (reader.ReadString())
                    {
                        case "Rate1_2":
                            fec = new FECRate(FECRate.FECRate12);
                            break;
                        case "Rate1_3":
                            fec = new FECRate(FECRate.FECRate13);
                            break;
                        case "Rate1_4":
                            fec = new FECRate(FECRate.FECRate14);
                            break;
                        case "Rate2_3":
                            fec = new FECRate(FECRate.FECRate23);
                            break;
                        case "Rate2_5":
                            fec = new FECRate(FECRate.FECRate25);
                            break;
                        case "Rate3_4":
                            fec = new FECRate(FECRate.FECRate34);
                            break;
                        case "Rate3_5":
                            fec = new FECRate(FECRate.FECRate35);
                            break;
                        case "Rate4_5":
                            fec = new FECRate(FECRate.FECRate45);
                            break;
                        case "Rate5_11":
                            fec = new FECRate(FECRate.FECRate511);
                            break;
                        case "Rate5_6":
                            fec = new FECRate(FECRate.FECRate56);
                            break;
                        case "Rate6_7":
                            fec = new FECRate(FECRate.FECRate67);
                            break;
                        case "Rate7_8":
                            fec = new FECRate(FECRate.FECRate78);
                            break;
                        case "Rate8_9":
                            fec = new FECRate(FECRate.FECRate89);
                            break;
                        case "Rate9_10":
                            fec = new FECRate(FECRate.FECRate910);
                            break;
                    }
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
            return ("Channel " + ChannelNumber + " (" + Frequency / 1000  + " MHz)");
        }

        /// <summary>
        /// Generate a copy of this frequency.
        /// </summary>
        /// <returns>A new instance with the same properties as the old instance.</returns>
        public override TuningFrequency Clone()
        {
            AtscFrequency newFrequency = new AtscFrequency();
            base.Clone(newFrequency);
            
            newFrequency.SymbolRate = symbolRate;
            newFrequency.FEC = fec;
            newFrequency.SymbolRate = symbolRate;
            newFrequency.Modulation = modulation;

            return (newFrequency);
        }

        /// <summary>
        /// Get a hash code for this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return (base.GetHashCode());
        }

        /// <summary>
        /// Compare another object with this one.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>True if the objects are equal; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            return (base.Equals(obj));
        }

        /// <summary>
        /// Compare two instances for equality.
        /// </summary>
        /// <param name="frequency1">The first frequency.</param>
        /// <param name="frequency2">The second frequency</param>
        /// <returns>True if the instances are equal; false otherwise.</returns>
        public static bool operator ==(AtscFrequency frequency1, AtscFrequency frequency2)
        {
            return (Equals(frequency1, frequency2));
        }

        /// <summary>
        /// Compare two instances for inequality.
        /// </summary>
        /// <param name="frequency1">The first frequency.</param>
        /// <param name="frequency2">The second frequency</param>
        /// <returns>True if the instances are not equal; false otherwise.</returns>
        public static bool operator !=(AtscFrequency frequency1, AtscFrequency frequency2)
        {
            return(!Equals(frequency1, frequency2));
        }

        /// <summary>
        /// Compare two instances for greater than.
        /// </summary>
        /// <param name="frequency1">The first frequency.</param>
        /// <param name="frequency2">The second frequency</param>
        /// <returns>True if the instance 1 is greater than instance 2; false otherwise.</returns>
        public static bool operator >(AtscFrequency frequency1, AtscFrequency frequency2)
        {
            if (frequency1 == null || frequency2 == null)
                return (false);

            return (frequency1.Frequency > frequency2.Frequency);
        }

        /// <summary>
        /// Compare two instances for less than.
        /// </summary>
        /// <param name="frequency1">The first frequency.</param>
        /// <param name="frequency2">The second frequency</param>
        /// <returns>True if the instance 1 is less than instance 2; false otherwise.</returns>
        public static bool operator <(AtscFrequency frequency1, AtscFrequency frequency2)
        {
            if (frequency1 == null || frequency2 == null)
                return (false);

            return (frequency1.Frequency < frequency2.Frequency);
        }
    }
}
