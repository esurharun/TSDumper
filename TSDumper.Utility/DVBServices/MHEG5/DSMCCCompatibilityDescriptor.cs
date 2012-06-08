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
    /// The class that describes the DSMCC compatibility descriptor.
    /// </summary>
    public class DSMCCCompatibilityDescriptor
    {
        /// <summary>
        /// Get the collection of descriptor entries.
        /// </summary>
        public Collection<DSMCCCompatibilityDescriptorEntry> DescriptorEntries { get { return (descriptorEntries); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the descriptor.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The descriptor has not been processed.
        /// </exception>
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("DSMCCCompatabilityDescriptor: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int descriptorLength;
        private int descriptorCount;
        private Collection<DSMCCCompatibilityDescriptorEntry> descriptorEntries;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the DSMCCCompatabilityDescriptor class.
        /// </summary>
        public DSMCCCompatibilityDescriptor() { }

        /// <summary>
        /// Parse the descriptor.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the descriptor.</param>
        /// <param name="index">Index of the first byte of the descriptor in the MPEG2 section.</param>
        public void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                descriptorLength = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                descriptorCount = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                if (descriptorCount != 0)
                {
                    descriptorEntries = new Collection<DSMCCCompatibilityDescriptorEntry>();

                    while (descriptorEntries.Count != descriptorCount)
                    {
                        DSMCCCompatibilityDescriptorEntry descriptorEntry = new DSMCCCompatibilityDescriptorEntry();
                        descriptorEntry.Process(byteData, lastIndex);
                        descriptorEntries.Add(descriptorEntry);

                        lastIndex = descriptorEntry.Index;
                    }
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The DSMCC Compatability Descriptor message is short"));
            }
        }

        /// <summary>
        /// Validate the descriptor.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The descriptor information is not valid.
        /// </exception>
        public void Validate() { }

        /// <summary>
        /// Log the descriptor fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DSMCC COMPATIBILITY DESCRIPTOR: Descr lth: " + descriptorLength +
                " Descr count: " + descriptorCount);

            if (descriptorEntries != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (DSMCCCompatibilityDescriptorEntry descriptorEntry in descriptorEntries)
                    descriptorEntry.LogMessage();

                Logger.DecrementProtocolIndent();
            }
        }
    }
}
