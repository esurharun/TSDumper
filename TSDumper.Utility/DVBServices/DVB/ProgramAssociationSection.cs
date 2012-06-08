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
    /// The class that describes the program association section.
    /// </summary>
    public class ProgramAssociationSection
    {
        /// <summary>
        /// Get the transport stream identification (TSID).
        /// </summary>
        public int TransportStreamID { get { return (transportStreamID); } }
        /// <summary>
        /// Get the section number.
        /// </summary>
        public int SectionNumber { get { return (sectionNumber); } }
        /// <summary>
        /// Get the last section number.
        /// </summary>
        public int LastSectionNumber { get { return (lastSectionNumber); } }
        /// <summary>
        /// Get the collection of program information objects for the transport stream ID.
        /// </summary>
        public Collection<ProgramInfo> ProgramInfos { get { return (programInfos); } }

        private int transportStreamID;
        private int sectionNumber;
        private int lastSectionNumber;
        private Collection<ProgramInfo> programInfos;

        private int lastIndex = -1;

        private ProgramAssociationSection()
        {
            programInfos = new Collection<ProgramInfo>();
        }

        /// <summary>
        /// Parse the section.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the section.</param>
        /// <param name="mpeg2Header">The MPEG2 header that preceedes the section.</param>
        internal void Process(byte[] byteData, Mpeg2ExtendedHeader mpeg2Header)
        {
            lastIndex = mpeg2Header.Index;
            transportStreamID = mpeg2Header.TableIDExtension;
            sectionNumber = mpeg2Header.SectionNumber;
            lastSectionNumber = mpeg2Header.LastSectionNumber;

            while (lastIndex < byteData.Length - 4)
            {
                ProgramInfo programInfo = new ProgramInfo();
                programInfo.Process(byteData, lastIndex);
                programInfos.Add(programInfo);

                lastIndex = programInfo.Index;
            }
        }

        /// <summary>
        /// Process an MPEG2 section from the program association table.
        /// </summary>
        /// <param name="byteData">The MPEG2 section.</param>
        /// <returns>A ProgramAssociationSection instance.</returns>
        public static ProgramAssociationSection ProcessProgramAssociationTable(byte[] byteData)
        {
            Mpeg2ExtendedHeader mpeg2Header = new Mpeg2ExtendedHeader();

            try
            {
                mpeg2Header.Process(byteData);

                if (mpeg2Header.Current)
                {
                    int lastIndex = mpeg2Header.Index;

                    ProgramAssociationSection programAssociationSection = new ProgramAssociationSection();
                    programAssociationSection.Process(byteData, mpeg2Header);
                    return (programAssociationSection);
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                Logger.Instance.Write("<e> Error processing Program Association Section message: " + e.Message);
            }

            return (null);
        }
    }
}
