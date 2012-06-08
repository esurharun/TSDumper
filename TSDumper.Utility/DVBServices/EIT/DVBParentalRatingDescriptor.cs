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
    internal class DVBParentalRatingDescriptor : DescriptorBase
    {
        /// <summary>
        /// Get the list of parental ratings.
        /// </summary>
        public Collection<int> ParentalRatings { get { return (parentalRatings); } }

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
                    throw (new InvalidOperationException("ParentalRatingDescriptor: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private Collection<int> parentalRatings;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the DVBParentalRatingDescriptor class.
        /// </summary>
        internal DVBParentalRatingDescriptor() { }

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
                if (Length != 0)
                {
                    int dataLength = Length;

                    parentalRatings = new Collection<int>();

                    while (dataLength != 0)
                    {
                        byte[] countryCode = Utils.GetBytes(byteData, lastIndex, 3);
                        lastIndex += 3;

                        int parentalRating = (int)(byteData[lastIndex]);
                        lastIndex++;

                        parentalRatings.Add(parentalRating);

                        dataLength -= 4;                        
                    }
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The DVB Parental Rating Descriptor message is short"));
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

            StringBuilder ratings = new StringBuilder();

            if (parentalRatings == null || parentalRatings.Count == 0)
                ratings.Append("No parental ratings");
            else
            {
                foreach (int rating in parentalRatings)
                {
                    if (ratings.Length != 0)
                        ratings.Append(", ");
                    ratings.Append(rating.ToString());
                }
            }

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DVB PARENTAL RATING DESCRIPTOR: Parental ratings: " + ratings.ToString());
        }
    }
}
