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

namespace DVBServices
{
    /// <summary>
    /// The class that describes a BIOP descriptor.
    /// </summary>
    public class BIOPDescriptor
    {
        /// <summary>
        /// Get the descriptor tag identifying the descriptor.
        /// </summary>
        public int DescriptorTag { get { return (descriptorTag); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following this descriptor.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The descriptor has not been processed.
        /// </exception> 
        public virtual int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("BIOPDescriptor: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int descriptorTag;

        private int lastIndex = -1;
        
        /// <summary>
        /// Create a new instance of a BIOP descriptor.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the descriptor.</param>
        /// <param name="index">Index of the descriptor tag in the MPEG2 section.</param>
        /// <returns>A BIOP descriptor instance.</returns>
        public static BIOPDescriptor Create(byte[] byteData, int index)
        {
            BIOPDescriptor descriptor = null;

            switch (byteData[index])
            {
                case DVBCompressedModuleDescriptor.Tag:
                    descriptor = new DVBCompressedModuleDescriptor();                    
                    break;
                case MHPLabelDescriptor.Tag:
                    descriptor = new MHPLabelDescriptor();
                    break;
                case MHPCachingPriorityDescriptor.Tag:
                    descriptor = new MHPCachingPriorityDescriptor();
                    break;
                case MHPContentTypeDescriptor.Tag:
                    descriptor = new MHPContentTypeDescriptor();
                    break;
                default:
                    throw (new InvalidOperationException("BIOPDescriptor: Tag not recognized - " + byteData[index]));
            }

            descriptor.Process(byteData, index);

            return (descriptor);
        }

        /// <summary>
        /// Initialize a new instance of the BIOPDescriptor class.
        /// </summary>
        public BIOPDescriptor() { }

        /// <summary>
        /// Parse the descriptor.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the descriptor.</param>
        /// <param name="index">Index of the byte in the MPEG2 section following the descriptor length.</param>
        public virtual void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                descriptorTag = (int)byteData[lastIndex];
                lastIndex++;

                int length = (int)byteData[lastIndex];
                lastIndex++;

                lastIndex += length;

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The BIOP Descriptor message is short"));
            }
        }

        /// <summary>
        /// Validate the descriptor fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A descriptor field is not valid.
        /// </exception>
        public virtual void Validate() { }

        /// <summary>
        /// Log the descriptor fields.
        /// </summary>
        public virtual void LogMessage() { }
    }
}
