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
    /// The class that describes an MHP content type descriptor.
    /// </summary>
    public class MHPContentTypeDescriptor : BIOPDescriptor
    {
        /// <summary>
        /// The tag value for an MHP content type descriptor.
        /// </summary>
        public const int Tag = 0x72;

        /// <summary>
        /// Get the content length.
        /// </summary>
        public int ContentLength { get { return (contentLength); } }
        /// <summary>
        /// Get the content type.
        /// </summary>
        public byte[] ContentType { get { return (contentType); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the descriptor.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The descriptor has not been processed.
        /// </exception>
        public override int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("MHPContentTypeDescriptor: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int tag;
        private int contentLength;
        private byte[] contentType = new byte[1] { 0x00 };

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the MHPContentTypeDescriptor class.
        /// </summary>
        public MHPContentTypeDescriptor() { }

        /// <summary>
        /// Parse the descriptor.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the descriptor.</param>
        /// <param name="index">Index of the first byte of the descriptor in the MPEG2 section.</param>
        public override void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                tag = (int)byteData[lastIndex];
                lastIndex++;

                contentLength = (int)byteData[lastIndex];
                lastIndex++;

                if (contentLength != 0)
                {
                    contentType = Utils.GetBytes(byteData, lastIndex, contentLength);
                    lastIndex += contentLength;
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The MHP Content Type Descriptor message is short"));
            }
        }

        /// <summary>
        /// Validate the descriptor fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A descriptor field is not valid.
        /// </exception>
        public override void Validate() { }

        /// <summary>
        /// Log the descriptor fields.
        /// </summary>
        public override void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "MHP CONTENT TYPE DESCRIPTOR: Tag: " + tag +
                " Content length: " + contentLength +
                " Content type: " + Utils.ConvertToHex(contentType));
        }
    }
}
