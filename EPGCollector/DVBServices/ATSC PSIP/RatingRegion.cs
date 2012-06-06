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
    /// The class that describes a Rating Region.
    /// </summary>
    public class RatingRegion
    {
        /// <summary>
        /// Get the region.
        /// </summary>
        public int Region { get { return (region); } }
        /// <summary>
        /// Get the name.
        /// </summary>
        public MultipleString Name { get { return (name); } }
        /// <summary>
        /// Get the collection of dimensions.
        /// </summary>
        internal Collection<RatingRegionDimension> Dimensions { get { return (dimensions); } }
        /// <summary>
        /// Get the collection of descriptors.
        /// </summary>
        internal Collection<DescriptorBase> Descriptors { get { return (descriptors); } }

        private int region;
        private MultipleString name;
        private Collection<RatingRegionDimension> dimensions;
        private Collection<DescriptorBase> descriptors;

        private int lastIndex;

        /// <summary>
        /// Initialize a new instance of the RatingRegion class.
        /// </summary>
        public RatingRegion() {}

        /// <summary>
        /// Parse the descriptor.
        /// </summary>
        /// <param name="byteData">The mpeg2 section containing the descriptor.</param>
        /// <param name="index">Index of the byte in the mpeg2 section following the descriptor length.</param>
        /// <param name="region">The region being processed.</param>
        internal void Process(byte[] byteData, int index, int region)
        {
            lastIndex = index;
            this.region = region;

            int nameLength = (int)byteData[lastIndex];
            lastIndex++;

            if (nameLength != 0)
            {
                name = new MultipleString();
                name.Process(byteData, lastIndex);
                name.LogMessage();

                lastIndex = name.Index;
            }

            int dimensionCount = (int)byteData[lastIndex];
            lastIndex++;

            if (dimensionCount != 0)
            {
                dimensions = new Collection<RatingRegionDimension>();

                while (dimensionCount != 0)
                {
                    RatingRegionDimension dimension = new RatingRegionDimension();
                    dimension.Process(byteData, lastIndex);

                    dimensions.Add(dimension);

                    lastIndex = dimension.Index;
                    dimensionCount--;
                }
            }

            int descriptorLoopLength = ((byteData[lastIndex] & 0x03) * 256) + (int)byteData[lastIndex + 1];
            lastIndex += 2;

            if (descriptorLoopLength != 0)
            {
                descriptors = new Collection<DescriptorBase>();

                while (descriptorLoopLength != 0)
                {
                    while (descriptorLoopLength != 0)
                    {
                        DescriptorBase descriptor = DescriptorBase.AtscInstance(byteData, lastIndex);
                        descriptors.Add(descriptor);

                        lastIndex = descriptor.Index;
                        descriptorLoopLength -= descriptor.TotalLength;
                    }
                }
            }

            Validate();
        }

        /// <summary>
        /// Validate the entry fields.
        /// </summary>
        public void Validate() { }

        /// <summary>
        /// Log the entry fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            string nameString;
            if (name != null)
                nameString = name.ToString();
            else
                nameString = "* Not present *";

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "RATING REGION: Region: " + region +
                " Name: " + nameString);

            if (dimensions != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (RatingRegionDimension dimension in dimensions)
                    dimension.LogMessage();

                Logger.DecrementProtocolIndent();
            }

            if (descriptors != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (DescriptorBase descriptor in descriptors)
                    descriptor.LogMessage();

                Logger.DecrementProtocolIndent();
            }
        }
    }
}
