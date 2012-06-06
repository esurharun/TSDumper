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

namespace DVBServices
{
    /// <summary>
    /// The class that describes an MPEG2 extended header.
    /// </summary>
    public class Mpeg2ExtendedHeader : Mpeg2BasicHeader
    {
        /// <summary>
        /// Get the table identification extension.
        /// </summary>
        public int TableIDExtension { get { return (tableIDExtension); } }
        /// <summary>
        /// Return true if the MPEG2 section is current; false otherwise.
        /// </summary>
        public bool CurrentNextIndicator { get { return (currentNextIndicator); } }
        /// <summary>
        /// Get the version number.
        /// </summary>
        public int VersionNumber { get { return (versionNumber); } }
        /// <summary>
        /// Get the section number.
        /// </summary>
        public int SectionNumber { get { return (sectionNumber); } }
        /// <summary>
        /// Get the last section number.
        /// </summary>
        public int LastSectionNumber { get { return (lastSectionNumber); } }

        /// <summary>
        /// Return true if the MPEG2 section is current; false otherwise.
        /// </summary>
        public bool Current { get { return (currentNextIndicator); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the header.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The header has not been processed.
        /// </exception> 
        public override int Index 
        { 
            get 
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("Mpeg2ExtendedHeader: Index requested before block processed"));
                return (lastIndex); 
            } 
        }

        private int tableIDExtension;
        private bool currentNextIndicator;
        private int versionNumber;
        private int sectionNumber;
        private int lastSectionNumber;

        private int lastIndex = -1;        

        // MPEG extended header layout is as follows
        //
        // table ID extension   word        uimsbf
        //                        
        // reserved             2 bits      bslbf
        // version no           5 bits      uimsbf
        // current/next ind     1 bit       bslbf
        //
        // section no           byte        uimsbf
        // last section no      byte        uimsbf

        /// <summary>
        /// Initialize a new instance of the Mpeg2ExtendedHeader class.
        /// </summary>
        public Mpeg2ExtendedHeader() { }

        /// <summary>
        /// Parse the header.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the header.</param>
        public override void Process(byte[] byteData)
        {
            base.Process(byteData);
            lastIndex = base.Index;

            try
            {
                tableIDExtension = (byteData[lastIndex] * 256) + (int)byteData[lastIndex + 1];
                lastIndex += 2;

                versionNumber = ((int)((byteData[lastIndex] >> 1) & 0x1f));
                currentNextIndicator = (byteData[lastIndex] & 0x01) != 0;
                lastIndex++;

                sectionNumber = (int)byteData[lastIndex];
                lastIndex++;

                lastSectionNumber = (int)byteData[lastIndex];
                lastIndex++;

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("MPEG2 extended header short"));
            }
        }

        /// <summary>
        /// Validate the header fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A header field is not valid.
        /// </exception>
        public override void Validate() 
        {
            base.Validate();
        }

        /// <summary>
        /// Log the header fields.
        /// </summary>
        public override void LogMessage() 
        {
            base.LogMessage();
        }
    }
}
