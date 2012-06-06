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
using System.Collections.ObjectModel;
using System.Text;

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that describes a Dish Network VCHIP descriptor.
    /// </summary>
    internal class DishNetworkVCHIPDescriptor : DescriptorBase
    {
        /// <summary>
        /// Get the VCHIP rating.
        /// </summary>
        public int VCHIPRating { get { return (vchipRating); } }
        /// <summary>
        /// Get the content advisory.
        /// </summary>
        public int ContentAdvisory { get { return (contentAdvisory); } }

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
                    throw (new InvalidOperationException("DishNetworkVCHIPDescriptor: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int vchipRating = -1;

        // 0x01 = TV-Y
        // 0x02 = TV-Y7
        // 0x03 = TV-G
        // 0x04 = TV-PG
        // 0x05 = TV-14
        // 0x06 = TV-MA

        private int contentAdvisory = -1;

        // 0x01 = sexual content
        // 0x02 = language
        // 0x04 = mild sensuality
        // 0x08 = fantasy violence
        // 0x10 = violence
        // 0x20 = mild peril(?)
        // 0x40 = nudity

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the DishNetworkVCHIPDescriptor class.
        /// </summary>
        internal DishNetworkVCHIPDescriptor() { }

        /// <summary>
        /// Parse the descriptor.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the descriptor.</param>
        /// <param name="index">Index of the byte in the MPEG2 section following the descriptor length.</param>
        internal override void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            if (Length < 2)
                return;

            vchipRating = (int)byteData[lastIndex];
            lastIndex++;

            contentAdvisory = (int)byteData[lastIndex];

            lastIndex = index + Length;
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

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DISH NETWORK VCHIP DESCRIPTOR: VCHIP rating: " + vchipRating +
                " Content advisory: " + contentAdvisory);
        }
    }
}
