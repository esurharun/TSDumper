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
    /// DVB Compressed Module descriptor class.
    /// </summary>
    public class DVBCompressedModuleDescriptor : BIOPDescriptor
    {
        /// <summary>
        /// Get the tag value for the descriptor.
        /// </summary>
        public const int Tag = 0x09;

        /// <summary>
        /// Get the module compression method.
        /// </summary>
        public int CompressionMethod { get { return (compressionMethod); } }
        
        /// <summary>
        /// Get the original size of the module.
        /// </summary>
        public int OriginalSize { get { return (originalSize); } }

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
                    throw (new InvalidOperationException("DVBCompressedModuleDescriptor: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int tag;
        private int length;
        private int compressionMethod;
        private int originalSize;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the DVBCompressedModuleDescriptor class.
        /// </summary>
        public DVBCompressedModuleDescriptor() { }

        /// <summary>
        /// Parse the descriptor.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the descriptor.</param>
        /// <param name="index">Index of the byte in the MPEG2 section following the descriptor length.</param>
        public override void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                tag = (int)byteData[lastIndex];
                lastIndex++;

                length = (int)byteData[lastIndex];
                lastIndex++;

                compressionMethod = (int)byteData[lastIndex];
                lastIndex++;

                originalSize = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The DVB Compressed Module Descriptor message is short"));
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

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DVB COMPRESSED MODULE DESCRIPTOR: Tag: " + Utils.ConvertToHex(tag) +
                " Length: " + length +
                " Compression method: " + Utils.ConvertToHex(compressionMethod) +
                " Orig size: " + originalSize);
        }
    }
}
