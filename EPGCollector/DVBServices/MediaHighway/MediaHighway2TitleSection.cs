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
    /// The class that describes a MediaHighway2 Title section.
    /// </summary>
    public class MediaHighway2TitleSection
    {
        /// <summary>
        /// Get the title data for this section.
        /// </summary>
        public Collection<MediaHighway2TitleData> Titles { get { return (titles); } }

        private byte[] unknown;
        private Collection<MediaHighway2TitleData> titles;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the MediaHighway2TitleSection class.
        /// </summary>
        internal MediaHighway2TitleSection() 
        {
            titles = new Collection<MediaHighway2TitleData>();
        }

        /// <summary>
        /// Parse the section.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the section.</param>
        /// <param name="index">Index of the first byte of the title section in the MPEG2 section.</param>
        /// <returns>True if the section is an MHW2 title section; false otherwise.</returns>
        internal bool Process(byte[] byteData, int index)
        {
            lastIndex = index;

            /*int checkIndex = 18;
            bool process = false;

            while (checkIndex < byteData.Length)
            {
                process = false;
                checkIndex += 7;

                if (checkIndex < byteData.Length)
                {
                    checkIndex += 3;

                    if (checkIndex < byteData.Length)
                    {
                        if (byteData[checkIndex] > 0xc0)
                        {
                            checkIndex += (byteData[checkIndex] - 0xc0);
                            checkIndex += 4;

                            if (checkIndex < byteData.Length)
                            {
                                if (byteData[checkIndex] == 0xff)
                                {
                                    checkIndex += 1;
                                    process = true;
                                }
                            }
                        }
                    }
                }                
            }

            if (!process)
                return (false);*/

            /*Logger.Instance.Dump("Title Section", byteData, byteData.Length);*/

            unknown = Utils.GetBytes(byteData, lastIndex, 15);
            lastIndex += unknown.Length;

            while (lastIndex < byteData.Length)
            {
                MediaHighway2TitleData title = new MediaHighway2TitleData();
                title.Process(byteData, lastIndex);

                titles.Add(title);
                lastIndex = title.Index;
            }

            return (true);
        }

        /// <summary>
        /// Log the section fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            if (titles != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (MediaHighway2TitleData title in titles)
                    title.LogMessage();

                Logger.DecrementProtocolIndent();
            }
        }

        /// <summary>
        /// Process an MPEG2 section from the MediaHighway title table.
        /// </summary>
        /// <param name="byteData">The MPEG2 section.</param>
        /// <returns>A MediaHighway2TitleSection instance.</returns>
        public static MediaHighway2TitleSection ProcessMediaHighwayTitleTable(byte[] byteData)
        {
            Mpeg2BasicHeader mpeg2Header = new Mpeg2BasicHeader();

            try
            {
                mpeg2Header.Process(byteData);

                MediaHighway2TitleSection titleSection = new MediaHighway2TitleSection();
                bool process = titleSection.Process(byteData, mpeg2Header.Index);
                if (process)
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
