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
using System.Xml;

namespace DomainObjects
{
    /// <summary>
    /// The class that describes a satellite frequency.
    /// </summary>
    public class ISDBSatelliteFrequency : TuningFrequency, IComparable
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
        /// Get or set the signal polarization.
        /// </summary>
        public SignalPolarization Polarization
        {
            get { return (polarization); }
            set { polarization = value; }
        }

        /// <summary>
        /// Get or set the satellite dish for this frequency.
        /// </summary>
        public SatelliteDish SatelliteDish
        {
            get { return (satelliteDish); }
            set { satelliteDish = value; }
        }

        /// <summary>
        /// Get the tuner type needed for this type of frequency.
        /// </summary>
        public override TunerType TunerType { get { return (TunerType.ISDBS); } }

        private int symbolRate = 30000;
        private FECRate fec = new FECRate();
        private SignalPolarization polarization = new SignalPolarization("Linear Horizontal");
        private SatelliteDish satelliteDish;

        /// <summary>
        /// Initialize a new instance of the ISDBSatelliteFrequency class.
        /// </summary>
        public ISDBSatelliteFrequency() { }

        internal void load(XmlReader reader)
        {
            switch (reader.Name)
            {
                case "CarrierFrequency":
                    Frequency = Int32.Parse(reader.ReadString());
                    break;
                case "Polarisation":
                    switch (reader.ReadString())
                    {
                        case "CircularL":
                            polarization = new SignalPolarization(SignalPolarization.CircularLeft);
                            break;
                        case "CircularR":
                            polarization = new SignalPolarization(SignalPolarization.CircularRight);
                            break;
                        case "LinearH":
                            polarization = new SignalPolarization(SignalPolarization.LinearHorizontal);
                            break;
                        case "LinearV":
                            polarization = new SignalPolarization(SignalPolarization.LinearVertical);
                            break;
                    }
                    break;
                case "SymbolRate":
                    symbolRate = Int32.Parse(reader.ReadString());
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
                case "OpenTVCode":
                    OpenTVCode = reader.ReadString();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Compare another satellite frequency with this one.
        /// </summary>
        /// <param name="compareFrequency">The tuning frequency to be compared to.</param>
        /// <returns>0 if the frequencies are equal, -1 if this instance is less, +1 otherwise.</returns>
        public override int CompareTo(object compareFrequency)
        {
            ISDBSatelliteFrequency satelliteFrequency = compareFrequency as ISDBSatelliteFrequency;
            if (satelliteFrequency == null)
                throw (new ArgumentException("Object is not a ISDBSatelliteFrequency"));

            if (satelliteFrequency.Frequency == Frequency)
                return (polarization.ToString().CompareTo(satelliteFrequency.Polarization.ToString()));

            return (Frequency.CompareTo(satelliteFrequency.Frequency));
        }

        /// <summary>
        /// Get a description of this satellite frequency.
        /// </summary>
        /// <returns>A string describing this frequency.</returns>
        public override string ToString()
        {
            string polarity = string.Empty;

            switch (polarization.Polarization)
            {
                case SignalPolarization.CircularLeft:
                    polarity = "L";
                    break;
                case SignalPolarization.CircularRight:
                    polarity = "R";
                    break;
                case SignalPolarization.LinearHorizontal:
                    polarity = "H";
                    break;
                case SignalPolarization.LinearVertical:
                    polarity = "V";
                    break;
                default:
                    polarity = "?";
                    break;
            }

            return (Frequency.ToString() + " - " + polarity);
        }

        /// <summary>
        /// Generate a copy of this frequency.
        /// </summary>
        /// <returns>A new instance with the same properties as the old instance.</returns>
        public override TuningFrequency Clone()
        {
            ISDBSatelliteFrequency newFrequency = new ISDBSatelliteFrequency();
            base.Clone(newFrequency);

            newFrequency.FEC = fec;
            newFrequency.Polarization = polarization;
            newFrequency.SymbolRate = symbolRate;

            if (satelliteDish != null)
                newFrequency.SatelliteDish = (SatelliteDish)satelliteDish.Clone();

            return (newFrequency);
        }

        /// <summary>
        /// Check if this instance is equal to another.
        /// </summary>
        /// <param name="frequency">The other instance.</param>
        /// <returns></returns>
        public override bool EqualTo(TuningFrequency frequency)
        {
            bool reply = base.EqualTo(frequency);
            if (!reply)
                return (false);

            return (((ISDBSatelliteProvider)Provider).EqualTo(((ISDBSatelliteProvider)(frequency.Provider))));
        }
    }
}
