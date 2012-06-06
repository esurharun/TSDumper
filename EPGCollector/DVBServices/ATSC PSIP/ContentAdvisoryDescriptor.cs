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
    /// ATSC PSIP Content Advisory descriptor class.
    /// </summary>
    internal class ContentAdvisoryDescriptor : DescriptorBase
    {
        /// <summary>
        /// Get the collection of rating regions.
        /// </summary>
        public Collection<ContentAdvisoryRatingRegion> Regions { get { return (regions); } }

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
                    throw (new InvalidOperationException("PSIPContentAdvisoryDescriptor: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private Collection<ContentAdvisoryRatingRegion> regions;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the ContentAdvisoryDescriptor class.
        /// </summary>
        internal ContentAdvisoryDescriptor() { }

        /// <summary>
        /// Parse the descriptor.
        /// </summary>
        /// <param name="byteData">The mpeg2 section containing the descriptor.</param>
        /// <param name="index">Index of the byte in the mpeg2 section following the descriptor length.</param>
        internal override void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                int regionCount = byteData[lastIndex] & 0x1f;
                lastIndex++;

                if (regionCount != 0)
                {
                    regions = new Collection<ContentAdvisoryRatingRegion>();

                    while (regionCount != 0)
                    {
                        ContentAdvisoryRatingRegion region = new ContentAdvisoryRatingRegion();
                        region.Process(byteData, lastIndex);
                        regions.Add(region);

                        lastIndex = region.Index;
                        regionCount--;
                    }
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The PSIP Content Advisory Descriptor message is short"));
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

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "PSIP CONTENT ADVISORY DESCRIPTOR");

            if (regions != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (ContentAdvisoryRatingRegion region in regions)
                    region.LogMessage();

                Logger.DecrementProtocolIndent();
            }
        }
    }
}
