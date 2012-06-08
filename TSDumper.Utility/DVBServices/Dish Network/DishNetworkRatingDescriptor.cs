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
using System.Text;

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that describes a Dish Network rating descriptor.
    /// </summary>
    internal class DishNetworkRatingDescriptor : DescriptorBase
    {
        /// <summary>
        /// Get the star rating.
        /// </summary>
        public int StarRating { get { return (starRating); } }  
        /// <summary>
        /// Get the parental rating.
        /// </summary>
        public int ParentalRating { get { return (parentalRating); } }
        /// <summary>
        /// Get the advisory rating.
        /// </summary>
        public int AdvisoryRating { get { return (advisoryRating); } }

        /// <summary>
        /// Get the sexual content advisory setting.
        /// </summary>
        public bool HasSexualContent { get { return ((advisoryRating & 0x05) != 0); } }
        /// <summary>
        /// Get the strong language advisory setting.
        /// </summary>
        public bool HasStrongLanguage { get { return ((advisoryRating & 0x02) != 0); } }
        /// <summary>
        /// Get the violence advisory setting.
        /// </summary>
        public bool HasViolence { get { return ((advisoryRating & 0x18) != 0); } }
        /// <summary>
        /// Get the nudity advisory setting.
        /// </summary>
        public bool HasNudity { get { return ((advisoryRating & 0x40) != 0); } }

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
                    throw (new InvalidOperationException("DishNetworkRatingDescriptor: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int parentalRating = -1;
        private int starRating = -1;
        private int advisoryRating = -1;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the DishNetworkRatingDescriptor class.
        /// </summary>
        internal DishNetworkRatingDescriptor() { }

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

            starRating = byteData[lastIndex] >> 5;
            parentalRating = ((byteData[lastIndex] >> 2) & 0x07);
            advisoryRating = ((byteData[lastIndex] & 0x03) * 256) + byteData[lastIndex + 1];  

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

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DISH NETWORK RATING DESCRIPTOR: Star rating: " + starRating +
                " Parental rating: " + parentalRating +
                " Advisory rating: " + advisoryRating);
        }
    }
}
