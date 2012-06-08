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
    /// DVB Content Identifier descriptor class.
    /// </summary>
    internal class DVBContentIdentifierDescriptor : DescriptorBase
    {
        /// <summary>
        /// Get the type of reference.
        /// </summary>
        public int ContentType { get { return (contentType); } }
        /// <summary>
        /// Get the type of reference.
        /// </summary>
        public int ContentLocation { get { return (contentLocation); } }
        /// <summary>
        /// Get the content reference.
        /// </summary>
        public string ContentReference { get { return (contentReference); } }

        /// <summary>
        /// Return true if the link is a FreeSat series link; false otherwise.
        /// </summary>
        public bool IsEpisodeLink { get { return (contentType == 0x31); } }
        /// <summary>
        /// Return true if the link is a FreeSat episode link; false otherwise.
        /// </summary>
        public bool IsSeriesLink { get { return (contentType == 0x32); } }

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
                    throw (new InvalidOperationException("DVBContentIdentifierDescriptor: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int contentType;
        private int contentLocation;
        private string contentReference;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the DVBContentIdentifierDescriptor class.
        /// </summary>
        internal DVBContentIdentifierDescriptor() { }

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
                contentType = (int)byteData[lastIndex] >> 2;
                contentLocation = (int)byteData[lastIndex] & 0x03;
                lastIndex++;

                int contentReferenceLength = (int)byteData[lastIndex];
                lastIndex++;

                if (contentReferenceLength != 0)
                {
                    contentReference = Utils.GetString(byteData, lastIndex, contentReferenceLength);
                    lastIndex += contentReferenceLength;
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The DVB Content Identifier Descriptor message is short"));
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

            string referenceString;

            if (contentReference != null)
                referenceString = contentReference;
            else
                referenceString = "** Not Available **";

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DVB CONTENT IDENTIFIER DESCRIPTOR: Type: " + contentType +
                " Location: " + contentLocation +
                " Reference: " + referenceString);
        }
    }
}
