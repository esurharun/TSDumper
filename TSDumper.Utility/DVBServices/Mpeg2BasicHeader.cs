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

namespace DVBServices
{
    /// <summary>
    /// The class that describes a basic MPEG2 section header.
    /// </summary>
    public class Mpeg2BasicHeader
    {
        /// <summary>
        /// Get the table identification.
        /// </summary>
        public int TableID { get { return (tableID); } }
        /// <summary>
        /// Get the section length.
        /// </summary>
        public int SectionLength { get { return (sectionLength); } }
        /// <summary>
        /// Return true if the section is private data; false otherwise.
        /// </summary>
        public bool PrivateIndicator { get { return (privateIndicator); } }
        /// <summary>
        /// Return true if the sysntax indicator is set; false otherwise.
        /// </summary>
        public bool SyntaxIndicator { get { return (syntaxIndicator); } }
        
        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the header.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The header has not been processed.
        /// </exception> 
        public virtual int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("Mpeg2BasicHeader: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int tableID;
        private int sectionLength;
        private bool privateIndicator;
        private bool syntaxIndicator;
        
        private int lastIndex = -1;
        private int dataLength;

        // MPEG  basic header layout is as follows
        //
        // tableID              byte        uimsbf
        //
        // section syntax ind   1 bit       bslbf
        // private indicator    1 bit       bslbf
        // reserved             2 bits      bslbf
        // section length       12 bits     uimsbf
         
        /// <summary>
        /// Initialize a new instance of the Mpeg2BasicHeader class.
        /// </summary>
        public Mpeg2BasicHeader() { }

        /// <summary>
        /// Parse the header.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the header.</param>
        public virtual void Process(byte[] byteData)
        {
            lastIndex = 0;
            dataLength = byteData.Length;

            try
            {
                tableID = (int)byteData[lastIndex];
                lastIndex++;

                syntaxIndicator = (byteData[lastIndex] & 0x80) != 0;
                privateIndicator = (byteData[lastIndex] & 0x40) != 0;
                sectionLength = ((byteData[lastIndex] & 0x0f) * 256) + (int)byteData[lastIndex + 1];
                lastIndex += 2;

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("MPEG2 basic header is short"));
            }
        }

        /// <summary>
        /// Validate the header fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A header field is not valid.
        /// </exception>
        public virtual void Validate()
        {
            if (dataLength > 4096)
                throw (new ArgumentOutOfRangeException("MPEG2 Section data length wrong"));

            if (sectionLength < 1 || sectionLength > dataLength - 3)
                throw (new ArgumentOutOfRangeException("MPEG2 Section length wrong"));
        }

        /// <summary>
        /// Log the header fields.
        /// </summary>
        public virtual void LogMessage() { }
    }
}
