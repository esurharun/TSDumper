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

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// DVB Satellite Delivery System descriptor class.
    /// </summary>
    internal class DVBSatelliteDeliverySystemDescriptor : DVBDeliverySystemDescriptor
    {
        /// <summary>
        /// Get the tuner type for this descriptor.
        /// </summary>
        public override TunerType TunerType { get { return (TunerType.Satellite); } }

        /// <summary>
        /// Get the orbital position.
        /// </summary>
        public int OrbitalPosition { get { return (orbitalPosition); } }

        /// <summary>
        /// Get the east flag.
        /// </summary>
        public bool EastFlag { get { return (eastFlag); } }

        /// <summary>
        /// Get the polarization.
        /// </summary>
        public int Polarization { get { return (polarization); } }

        /// <summary>
        /// Get the roll off.
        /// </summary>
        public int RollOff { get { return (rollOff); } }

        /// <summary>
        /// Get the DVB-S2 flag.
        /// </summary>
        public bool S2Flag { get { return (s2Flag); } }

        /// <summary>
        /// Get the modulation type.
        /// </summary>
        public int ModulationType { get { return (modulationType); } }

        /// <summary>
        /// Get the symbol rate.
        /// </summary>
        public int SymbolRate { get { return (symbolRate); } }

        /// <summary>
        /// Get the Inner FEC scheme.
        /// </summary>
        public int InnerFEC { get { return (innerFec); } }

        /// <summary>
        /// Get the index of the next byte in the EIT section following this descriptor.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The descriptor has not been processed.
        /// </exception> 
        public override int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("ServiceDescriptor: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int orbitalPosition;
        private bool eastFlag;
        private int polarization;
        private int rollOff;
        private bool s2Flag;
        private int modulationType;
        private int symbolRate;
        private int innerFec;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the DVBSatelliteDeliverySystemDescriptor class.
        /// </summary>
        internal DVBSatelliteDeliverySystemDescriptor() { }

        /// <summary>
        /// Parse the descriptor.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the descriptor.</param>
        /// <param name="index">Index of the byte in the MPEG2 section following the descriptor length.</param>
        internal override void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                Frequency = Utils.ConvertBCDToInt(byteData, lastIndex, 8);
                lastIndex+= 4;

                orbitalPosition = Utils.ConvertBCDToInt(byteData, lastIndex, 4);
                lastIndex+= 2;

                eastFlag = ((byteData[lastIndex] & 0x80) != 0);
                polarization = (byteData[lastIndex] >> 5) & 0x03;
                rollOff = (byteData[lastIndex] >> 3) & 0x03;
                s2Flag = ((byteData[lastIndex] & 0x04) != 0);
                modulationType = byteData[lastIndex] & 0x03;

                lastIndex++;

                symbolRate = Utils.ConvertBCDToInt(byteData, lastIndex, 7);
                innerFec = byteData[lastIndex + 3] & 0x17;
                lastIndex+= 4;

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The DVB Satellite Delivery Descriptor message is short"));
            }
        }

        
        /// <summary>
        /// Validate the descriptor fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A descriptor field is not valid.
        /// </exception>
        internal override void Validate() { }

        /// <summary>
        /// Log the descriptor fields.
        /// </summary>
        internal override void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DVB SATELLITE DELIVERY DESCRIPTOR: Frequency: " + Frequency +
                " orbital posn: " + orbitalPosition +
                " east flag: " + eastFlag +
                " polarization: " + polarization +
                " roll off: " + rollOff +
                " s2 flag: " + s2Flag + 
                " mod type: " + modulationType + 
                " symbol rate: " + symbolRate +
                " inner fec: " + innerFec);
        }
    }
}
