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

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// ATSC PSIP Rating Region alue entry.
    /// </summary>
    internal class RatingRegionValue
    {
        /// <summary>
        /// Get the abbreviated text.
        /// </summary>
        public MultipleString AbbreviatedText { get { return (abbreviatedText); } }
        /// <summary>
        /// Get the full text.
        /// </summary>
        public MultipleString FullText { get { return (fullText); } }

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
                    throw (new InvalidOperationException("RatingRegionValue: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private MultipleString abbreviatedText;
        private MultipleString fullText;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the RatingRegionValue class.
        /// </summary>
        internal RatingRegionValue() { }

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
                int abbreviatedTextLength  = (int)byteData[lastIndex];
                lastIndex++;

                if (abbreviatedTextLength != 0)
                {
                    abbreviatedText = new MultipleString();
                    abbreviatedText.Process(byteData, lastIndex);
                    abbreviatedText.LogMessage();

                    lastIndex = abbreviatedText.Index;
                }

                int fullTextLength = (int)byteData[lastIndex];
                lastIndex++;

                if (fullTextLength != 0)
                {
                    fullText = new MultipleString();
                    fullText.Process(byteData, lastIndex);
                    fullText.LogMessage();

                    lastIndex = fullText.Index;
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The PSIP Rating Region Value message is short"));
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

            string abbreviatedTextString;
            if (abbreviatedText != null)
                abbreviatedTextString = abbreviatedText.ToString();
            else
                abbreviatedTextString = "* Not Present *";

            string fullTextString;
            if (fullText != null)
                fullTextString = fullText.ToString();
            else
                fullTextString = "* Not Present *";

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "PSIP RATING REGION VALUE: Abbrev text: " + abbreviatedTextString +
                " Full text: " + fullTextString);
        }
    }
}
