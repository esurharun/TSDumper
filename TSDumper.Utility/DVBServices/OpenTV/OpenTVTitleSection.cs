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

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that describes an Open TV Title section.
    /// </summary>
    public class OpenTVTitleSection
    {
        /// <summary>
        /// Get the section number.
        /// </summary>
        public int SectionNumber { get { return (sectionNumber); } }
        /// <summary>
        /// Get the section number.
        /// </summary>
        public int LastSectionNumber { get { return (lastSectionNumber); } }
        /// <summary>
        /// Get the title header.
        /// </summary>
        public OpenTVTitleHeader TitleHeader { get { return (titleHeader); } }
        
        private int sectionNumber;
        private int lastSectionNumber;
        private OpenTVTitleHeader titleHeader;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the OpenTVTitleSection class.
        /// </summary>
        internal OpenTVTitleSection() { }

        /// <summary>
        /// Parse the section.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the section.</param>
        /// <param name="mpeg2Header">The MPEG2 header that preceedes the section.</param>
        /// <param name="pid">The PID containing the section.</param>
        /// <param name="tid">The table ID containing the section.</param>
        internal void Process(byte[] byteData, Mpeg2ExtendedHeader mpeg2Header, int pid, int tid)
        {
            lastIndex = mpeg2Header.Index;
            sectionNumber = mpeg2Header.SectionNumber;
            lastSectionNumber = mpeg2Header.LastSectionNumber;

            titleHeader = new OpenTVTitleHeader();
            titleHeader.Process(byteData, lastIndex, mpeg2Header, pid, tid);
        }

        /// <summary>
        /// Log the section fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            titleHeader.LogMessage();
        }

        /// <summary>
        /// Process an MPEG2 section from the Open TV Title table.
        /// </summary>
        /// <param name="byteData">The MPEG2 section.</param>
        /// <param name="pid">The PID containing the section.</param>
        /// <param name="table">The table ID containing the section.</param>
        /// <returns>An Open TV Title Section instance or null if a section is not created.</returns>
        public static OpenTVTitleSection ProcessOpenTVTitleTable(byte[] byteData, int pid, int table)
        {
            Mpeg2ExtendedHeader mpeg2Header = new Mpeg2ExtendedHeader();

            try
            {
                mpeg2Header.Process(byteData);

                if (mpeg2Header.Current)
                {
                    OpenTVTitleSection openTVTitleSection = new OpenTVTitleSection();
                    openTVTitleSection.Process(byteData, mpeg2Header, pid, table);
                    openTVTitleSection.LogMessage();
                    return (openTVTitleSection);
                }
                else
                    return (null);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Logger.Instance.Write("<e> Error processing Title Section: " + e.Message);

                if (RunParameters.Instance.DebugIDs.Contains("TITLESECTION"))
                {
                    Logger.Instance.Write(e.Message);
                    Logger.Instance.Write(e.StackTrace);
                    Logger.Instance.Dump("Title Section", byteData, byteData.Length);
                }

                return (null);
            }
        }
    }
}
