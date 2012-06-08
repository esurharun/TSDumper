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
    /// DVB Terrestrial Delivery System descriptor class.
    /// </summary>
    internal class DVBTerrestrialDeliverySystemDescriptor : DVBDeliverySystemDescriptor
    {
        /// <summary>
        /// Get the tuner type for this descriptor.
        /// </summary>
        public override TunerType TunerType { get { return (TunerType.Terrestrial); } }

        /// <summary>
        /// Get the bandwidth.
        /// </summary>
        public int Bandwidth { get { return (bandWidth); } }

        /// <summary>
        /// Get the priority.
        /// </summary>
        public bool PriorityFlag { get { return (priorityFlag); } }

        /// <summary>
        /// Get the time slice indicator.
        /// </summary>
        public bool TimeSliceIndicator { get { return (timeSliceIndicator); } }

        /// <summary>
        /// Get the MPE-FEC indicator.
        /// </summary>
        public bool FECIndicator { get { return (fecIndicator); } }

        /// <summary>
        /// Get the constellation.
        /// </summary>
        public int Constellation { get { return (constellation); } }

        /// <summary>
        /// Get the hierarchy information.
        /// </summary>
        public int HierarchyInformation { get { return (hierarchyInformation); } }

        /// <summary>
        /// Get the HP stream code rate.
        /// </summary>
        public int HPCodeRate { get { return (hpCodeRate); } }

        /// <summary>
        /// Get the LP stream code rate.
        /// </summary>
        public int LPCodeRate { get { return (lpCodeRate); } }

        /// <summary>
        /// Get the guard interval.
        /// </summary>
        public int GuardInterval { get { return (guardInterval); } }

        /// <summary>
        /// Get the transmission mode.
        /// </summary>
        public int TransmissionMode { get { return (transmissionMode); } }

        /// <summary>
        /// Get the other frequency flag.
        /// </summary>
        public bool OtherFrequencyFlag { get { return (otherFrequencyFlag); } }

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

        private int bandWidth;
        private bool priorityFlag;
        private bool timeSliceIndicator;
        private bool fecIndicator;
        private int constellation;
        private int hierarchyInformation;
        private int hpCodeRate;
        private int lpCodeRate;
        private int guardInterval;
        private int transmissionMode;
        private bool otherFrequencyFlag;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the DVBTerrestrialDeliverySystemDescriptor class.
        /// </summary>
        internal DVBTerrestrialDeliverySystemDescriptor() { }

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

                bandWidth = byteData[lastIndex + 1] >> 5;
                priorityFlag = ((byteData[lastIndex] & 0x10) != 0);
                timeSliceIndicator = ((byteData[lastIndex] & 0x08) != 0);
                fecIndicator = ((byteData[lastIndex] & 0x04) != 0);
                lastIndex++;

                constellation = byteData[lastIndex] >> 6;
                hierarchyInformation = (byteData[lastIndex] >> 3) & 0x07;
                hpCodeRate = byteData[lastIndex] & 0x07;
                lastIndex++;

                lpCodeRate = byteData[lastIndex] & 0xd0;
                guardInterval = (byteData[lastIndex] >> 3) & 0x03;
                transmissionMode = (byteData[lastIndex] >> 1) & 0x03;
                otherFrequencyFlag = ((byteData[lastIndex] & 0x01) != 0);
                lastIndex++;

                lastIndex += 4;

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The DVB Terrestrial Delivery Descriptor message is short"));
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

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DVB TERRESTRIAL DELIVERY DESCRIPTOR: Frequency: " + Frequency +
                " bandwidth: " + bandWidth +
                " priority: " + priorityFlag +
                " timeslice: " + timeSliceIndicator +
                " fec ind: " + fecIndicator +
                " constellation: " + constellation +
                " hierachy: " + hierarchyInformation +
                " hp rate: " + hpCodeRate +
                " lp rate: " + lpCodeRate +
                " guard: " + guardInterval +
                " trans mode: " + transmissionMode +
                " other freq: " + otherFrequencyFlag);
        }
    }
}
