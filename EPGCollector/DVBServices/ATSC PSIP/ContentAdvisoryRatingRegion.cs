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
    /// ATSC PSIP Content Advisory Rating Region.
    /// </summary>
    internal class ContentAdvisoryRatingRegion
    {
        /// <summary>
        /// Get the region.
        /// </summary>
        public int Region { get { return (region); } }
        /// <summary>
        /// Get the dimension collection.
        /// </summary>
        public Collection<ContentAdvisoryRatingDimension> Dimensions { get { return (dimensions); } }
        /// <summary>
        /// Get the description.
        /// </summary>
        public MultipleString Description { get { return (description); } }

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
                    throw (new InvalidOperationException("ContentAdvisoryRatingRegion: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int region;
        private Collection<ContentAdvisoryRatingDimension> dimensions;
        private MultipleString description;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the ContentAdvisoryRatingRegion class.
        /// </summary>
        internal ContentAdvisoryRatingRegion() { }

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
                region = (int)byteData[lastIndex];
                lastIndex++;

                int dimensionCount = (int)byteData[lastIndex];
                lastIndex++;

                if (dimensionCount != 0)
                {
                    dimensions = new Collection<ContentAdvisoryRatingDimension>();

                    while (dimensionCount != 0)
                    {
                        ContentAdvisoryRatingDimension dimension = new ContentAdvisoryRatingDimension();
                        dimension.Process(byteData, lastIndex);
                        dimensions.Add(dimension);

                        lastIndex = dimension.Index;
                        dimensionCount--;
                    }
                }

                int descriptionLength = (int)byteData[lastIndex];
                lastIndex++;

                if (descriptionLength != 0)
                {
                    description = new MultipleString();
                    description.Process(byteData, lastIndex);
                    lastIndex = description.Index;
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The PSIP Content Advisory Rating Region message is short"));
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

            string descriptionString;
            if (description != null)
                descriptionString = description.ToString();
            else
                descriptionString = "* Not present *";

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "PSIP CONTENT ADVISORY RATING REGION: Region: " + region +
                " Desc: " + descriptionString);

            if (dimensions != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (ContentAdvisoryRatingDimension dimension in dimensions)
                    dimension.LogMessage();

                Logger.DecrementProtocolIndent();
            }
        }
    }
}
