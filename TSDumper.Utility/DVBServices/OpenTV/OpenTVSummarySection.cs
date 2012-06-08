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
    /// The class that describes an Open TV Summary section.
    /// </summary>
    public class OpenTVSummarySection
    {
        /// <summary>
        /// Get the collection of Open TV Title sections.
        /// </summary>
        public static Collection<OpenTVSummarySection> OpenTVSummarySections
        {
            get
            {
                if (openTVSummarySections == null)
                    openTVSummarySections = new Collection<OpenTVSummarySection>();
                return (openTVSummarySections);
            }
        }

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
        public OpenTVSummaryHeader SummaryHeader { get { return (summaryHeader); } }

        private int sectionNumber;
        private int lastSectionNumber;
        private OpenTVSummaryHeader summaryHeader;

        private static Collection<OpenTVSummarySection> openTVSummarySections;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the OpenTVSummarySection class.
        /// </summary>
        internal OpenTVSummarySection() { }

        /// <summary>
        /// Parse the section.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the section.</param>
        /// <param name="mpeg2Header">The MPEG2 header that preceedes the section.</param>
        internal void Process(byte[] byteData, Mpeg2ExtendedHeader mpeg2Header)
        {
            lastIndex = mpeg2Header.Index;
            sectionNumber = mpeg2Header.SectionNumber;
            lastSectionNumber = mpeg2Header.LastSectionNumber;

            summaryHeader = new OpenTVSummaryHeader();
            summaryHeader.Process(byteData, lastIndex, mpeg2Header);
        }

        /// <summary>
        /// Log the section fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            summaryHeader.LogMessage();            
        }

        /// <summary>
        /// Process an MPEG2 section from the Open TV Summary table.
        /// </summary>
        /// <param name="byteData">The MPEG2 section.</param>
        /// <returns>An Open TV Summary Section instance.</returns>
        public static OpenTVSummarySection ProcessOpenTVSummaryTable(byte[] byteData)
        {
            Mpeg2ExtendedHeader mpeg2Header = new Mpeg2ExtendedHeader();

            try
            {
                mpeg2Header.Process(byteData);

                if (mpeg2Header.Current)
                {
                    OpenTVSummarySection openTVSummarySection = new OpenTVSummarySection();
                    openTVSummarySection.Process(byteData, mpeg2Header);
                    openTVSummarySection.LogMessage();
                    return (openTVSummarySection);
                }
                else
                    return (null);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Logger.Instance.Write("<e> Error processing Summary Section: " + e.Message);
                return (null);
            }
        }

        /// <summary>
        /// Add a section to the collection.
        /// </summary>
        /// <param name="newSection">The section to be added.</param>
        public static void AddSection(OpenTVSummarySection newSection)
        {
            /*foreach (OpenTVSummarySection oldSection in OpenTVSummarySections)
            {
                if (oldSection.sectionNumber == newSection.sectionNumber)
                    return;

                if (oldSection.SectionNumber > newSection.SectionNumber)
                {
                    OpenTVSummarySections.Insert(OpenTVSummarySections.IndexOf(oldSection), newSection);
                    return;
                }
            }*/

            OpenTVSummarySections.Add(newSection);
        }
    }
}
