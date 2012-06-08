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
using System.Collections.ObjectModel;

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// ATSC PSIP Service Location descriptor class.
    /// </summary>
    internal class ServiceLocationDescriptor : DescriptorBase
    {
        /// <summary>
        /// Get the PCR PID.
        /// </summary>
        public int PcrPid { get { return (pcrPid); } }
        /// <summary>
        /// Get the collection of locations.
        /// </summary>
        public Collection<ServiceLocationDescriptorEntry> Locations { get { return (locations); } }

        /// <summary>
        /// Get the index of the next byte in the section following this descriptor.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The descriptor has not been processed.
        /// </exception> 
        public override int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("PSIPServiceLocationDescriptor: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int pcrPid;
        private Collection<ServiceLocationDescriptorEntry> locations;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the ServiceLocationDescriptor class.
        /// </summary>
        internal ServiceLocationDescriptor() { }

        /// <summary>
        /// Parse the descriptor.
        /// </summary>
        /// <param name="byteData">The mpeg2 section containing the descriptor.</param>
        /// <param name="index">Index of the byte in the mpeg2 section following the descriptor length.</param>
        internal override void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                pcrPid = ((byteData[lastIndex] & 0x1f) * 256) + byteData[lastIndex];
                lastIndex += 2;

                int locationCount = byteData[lastIndex];
                lastIndex++;

                if (locationCount != 0)
                {
                    locations = new Collection<ServiceLocationDescriptorEntry>();

                    while (locationCount != 0)
                    {
                        ServiceLocationDescriptorEntry location = new ServiceLocationDescriptorEntry();
                        location.Process(byteData, lastIndex);
                        locations.Add(location);

                        lastIndex = location.Index;
                        locationCount--;
                    }
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The PSIP Service Location Descriptor message is short"));
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

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "PSIP SERVICE LOCATION DESCRIPTOR: PCR PID: " + pcrPid);

            if (locations != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (ServiceLocationDescriptorEntry location in locations)
                    location.LogMessage();

                Logger.DecrementProtocolIndent();
            }
        }
    }
}
