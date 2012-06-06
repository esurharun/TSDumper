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

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// ATSC PSIP Content Advisory Rating Dimension.
    /// </summary>
    internal class ContentAdvisoryRatingDimension
    {
        /// <summary>
        /// Get the dimension.
        /// </summary>
        public int Dimension { get { return (dimension); } }
        /// <summary>
        /// Get the rating value.
        /// </summary>
        public int RatingValue { get { return (ratingValue); } }
        
        /// <summary>
        /// Get the index of the next byte in the section following this entry.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The descriptor has not been processed.
        /// </exception> 
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("ContentAdvisoryRatingDimension: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int dimension;
        private int ratingValue;        

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the ContentAdvisoryRatingDimension class.
        /// </summary>
        internal ContentAdvisoryRatingDimension() { }

        /// <summary>
        /// Parse the descriptor.
        /// </summary>
        /// <param name="byteData">The mpeg2 section containing the descriptor.</param>
        /// <param name="index">Index of the byte in the mpeg2 section following the descriptor length.</param>
        internal void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                dimension = (int)byteData[lastIndex];
                lastIndex++;

                ratingValue = byteData[lastIndex] & 0x0f;
                lastIndex++;

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The PSIP Content Advisory Rating Dimension message is short"));
            }
        }

        /// <summary>
        /// Validate the descriptor fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A descriptor field is not valid.
        /// </exception>
        internal void Validate() { }

        /// <summary>
        /// Log the descriptor fields.
        /// </summary>
        internal void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "PSIP CONTENT ADVISORY RATING DIMENSION: Dimension: " + dimension +
                " Rating value: " + ratingValue);
        }
    }
}
