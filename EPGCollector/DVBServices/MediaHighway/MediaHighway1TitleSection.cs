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

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that describes a MediaHighway1 Title section.
    /// </summary>
    public class MediaHighway1TitleSection
    {
        /// <summary>
        /// Get the title data for this section.
        /// </summary>
        public MediaHighway1TitleData TitleData { get { return (titleData); } }

        private MediaHighway1TitleData titleData;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the MediaHighway1TitleSection class.
        /// </summary>
        internal MediaHighway1TitleSection() { }

        /// <summary>
        /// Parse the section.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the section.</param>
        /// <param name="index">Index of the first byte of the title section in the MPEG2 section.</param>
        internal void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            titleData = new MediaHighway1TitleData();
            titleData.Process(byteData, lastIndex);            
        }

        /// <summary>
        /// Log the section fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            titleData.LogMessage();
        }

        /// <summary>
        /// Process an MPEG2 section from the MediaHighway title table.
        /// </summary>
        /// <param name="byteData">The MPEG2 section.</param>
        /// <returns>A MediaHighway1TitleSection instance.</returns>
        public static MediaHighway1TitleSection ProcessMediaHighwayTitleTable(byte[] byteData)
        {
            Mpeg2BasicHeader mpeg2Header = new Mpeg2BasicHeader();

            try
            {
                mpeg2Header.Process(byteData);

                MediaHighway1TitleSection titleSection = new MediaHighway1TitleSection();
                titleSection.Process(byteData, mpeg2Header.Index);
                if (!titleSection.TitleData.IsEmpty)
                {
                    titleSection.LogMessage();
                    return (titleSection);
                }
                else
                    return (null);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Logger.Instance.Write("<e> Title section parsing failed: " + e.Message);
                return (null);
            }
        }
    }
}
