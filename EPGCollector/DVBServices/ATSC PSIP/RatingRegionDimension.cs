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
    /// ATSC PSIP Rating Region Dimension class.
    /// </summary>
    internal class RatingRegionDimension
    {
        /// <summary>
        /// Get the dimension name.
        /// </summary>
        public MultipleString Name { get { return (name); } }
        /// <summary>
        /// Get the graduated scale flag.
        /// </summary>
        public bool GraduatedScale { get { return (graduatedScale); } }
        /// <summary>
        /// Get the value collection.
        /// </summary>
        public Collection<RatingRegionValue> Values { get { return (values); } }
                
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
                    throw (new InvalidOperationException("RatingRegionDimension: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private MultipleString name;
        private bool graduatedScale;
        private Collection<RatingRegionValue> values;
                
        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the RatingRegionDimension class.
        /// </summary>
        internal RatingRegionDimension() { }

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
                int nameLength = (int)byteData[lastIndex];
                lastIndex++;

                if (nameLength != 0)
                {
                    name = new MultipleString();
                    name.Process(byteData, lastIndex);
                    name.LogMessage();

                    lastIndex = name.Index;
                }

                graduatedScale = ((byteData[lastIndex] & 0x10) != 0);
                
                int valueCount = byteData[lastIndex] & 0x0f;
                lastIndex++;

                if (valueCount != 0)
                {
                    values = new Collection<RatingRegionValue>();

                    while (valueCount != 0)
                    {
                        RatingRegionValue value = new RatingRegionValue();
                        value.Process(byteData, lastIndex);
                        values.Add(value);

                        lastIndex = value.Index;
                        valueCount--;
                    }
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The PSIP Rating Region Dimension message is short"));
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

            string nameString;
            if (name != null)
                nameString = name.ToString();
            else
                nameString = "* Not present *";

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "PSIP RATING REGION DIMENSION: Name: " + nameString +
                " Graduated scale: " + graduatedScale);

            if (values != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (RatingRegionValue value in values)
                    value.LogMessage();

                Logger.DecrementProtocolIndent();
            }
        }
    }
}
