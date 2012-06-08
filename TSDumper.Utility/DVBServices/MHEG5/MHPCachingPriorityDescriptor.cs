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
    /// The class that describes an MHP caching priority descriptor.
    /// </summary>
    public class MHPCachingPriorityDescriptor : BIOPDescriptor
    {
        /// <summary>
        /// The tag value for an MHP caching priority descriptor.
        /// </summary>
        public const int Tag = 0x71;

        /// <summary>
        /// Get the priority.
        /// </summary>
        public int Priority { get { return (priority); } }
        /// <summary>
        /// Get the transparency level.
        /// </summary>
        public int TransparencyLevel { get { return (transparencyLevel); } }

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
                    throw (new InvalidOperationException("MHPCachingPriorityDescriptor: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int tag;
        private int length;
        private int priority;
        private int transparencyLevel;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the MHPCachingPriorityDescriptor class.
        /// </summary>
        public MHPCachingPriorityDescriptor() { }

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

                length = (int)byteData[lastIndex];
                lastIndex++;

                priority = (int)byteData[lastIndex];
                lastIndex++;

                transparencyLevel = (int)byteData[lastIndex];
                lastIndex++;

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The MHP Caching Priority Descriptor message is short"));
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

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "MHP CACHING PRIORITY DESCRIPTOR: Tag: " + Utils.ConvertToHex(tag) +
                " Length: " + length +
                " Priority: " + priority +
                " Transparency lev: " + transparencyLevel);
        }
    }
}
