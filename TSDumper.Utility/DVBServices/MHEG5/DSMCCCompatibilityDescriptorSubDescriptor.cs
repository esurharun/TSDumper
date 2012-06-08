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
    /// The class that describes the DSMCC compatibility descriptor sub-descriptor.
    /// </summary>
    public class DSMCCCompatibilityDescriptorSubDescriptor
    {
        /// <summary>
        /// Get the descriptor type.
        /// </summary>
        public int DescriptorType { get { return (descriptorType); } }
        /// <summary>
        /// Get the length of the additional information.
        /// </summary>
        public int AdditionalInfoLength { get { return (additionalInfoLength); } }
        /// <summary>
        /// Get the additional information.
        /// </summary>
        public byte[] AdditionalInfo { get { return (additionalInfo); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the sub-descriptor.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The sub-descriptor has not been processed.
        /// </exception> 
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("DSMCCCompatabilityDescriptorSubDescriptor: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int descriptorType;
        private int additionalInfoLength;
        private byte[] additionalInfo;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the DSMCCCompatibilityDescriptorSubDescriptor class.
        /// </summary>
        public DSMCCCompatibilityDescriptorSubDescriptor() { }

        /// <summary>
        /// Parse the sub-descriptor.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the sub-descriptor.</param>
        /// <param name="index">Index of the first byte of the sub-descriptor in the MPEG2 section.</param>
        public void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                descriptorType = (int)byteData[lastIndex];
                lastIndex++;

                additionalInfoLength = (int)byteData[lastIndex];
                lastIndex++;

                if (additionalInfoLength != 0)
                {
                    additionalInfo = Utils.GetBytes(byteData, lastIndex, additionalInfoLength);
                    lastIndex += additionalInfoLength;
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The DSMCC Compatability Descriptor Sub-Descriptor message is short"));
            }
        }

        /// <summary>
        /// Validate the sub-descriptor.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The sub-descriptor information is not valid.
        /// </exception>
        public void Validate() { }

        /// <summary>
        /// Log the sub-descriptor fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DSMCC COMPATIBILITY DESCRIPTOR SUB DESCRIPTOR: Descr TYPE: " + Utils.ConvertToHex(descriptorType) +
                " Add info lth: " + additionalInfoLength);
        }
    }
}
