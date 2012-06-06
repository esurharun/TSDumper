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

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// DVB Cable Delivery System descriptor class.
    /// </summary>
    internal class DVBCableDeliverySystemDescriptor : DVBDeliverySystemDescriptor
    {
        /// <summary>
        /// Get the tuner type for this descriptor.
        /// </summary>
        public override TunerType TunerType { get { return (TunerType.Cable); } }

        /// <summary>
        /// Get the Outer FEC scheme.
        /// </summary>
        public int OuterFEC { get { return (outerFec); } }

        /// <summary>
        /// Get the modulation.
        /// </summary>
        public int Modulation { get { return (modulation); } }

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

        private int outerFec;
        private int modulation;
        private int symbolRate;
        private int innerFec;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the DVBCableDeliverySystemDescriptor class.
        /// </summary>
        internal DVBCableDeliverySystemDescriptor() { }

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
                lastIndex += 4;

                outerFec = byteData[lastIndex + 1] & 0x17;
                lastIndex+= 2;

                modulation = (int)byteData[lastIndex];
                lastIndex++;

                symbolRate = Utils.ConvertBCDToInt(byteData, lastIndex, 7);
                innerFec = byteData[lastIndex + 3] & 0x17;
                lastIndex += 4;

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The DVB Cable Delivery Descriptor message is short"));
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

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DVB CABLE DELIVERY DESCRIPTOR: Frequency: " + Frequency +
                " outer fec: " + outerFec +
                " modulation: " + modulation +
                " symbol rate: " + symbolRate +
                " inner fec: " + innerFec);
        }
    }
}
