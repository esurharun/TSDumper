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

namespace DomainObjects
{
    /// <summary>
    /// The class that describes an EIT section map entry.
    /// </summary>
    public class SectionMapEntry
    {
        internal int TableID { get { return (tableID); } }
        internal int SectionNumber { get { return (sectionNumber); } }

        internal int LastTableID { get { return (lastTableID); } }
        internal int LastSectionNumber { get { return (lastSectionNumber); } }
        internal int SegmentLastSectionNumber { get { return (segmentLastSectionNumber); } }

        private int tableID;
        private int sectionNumber;

        private int lastTableID;
        private int lastSectionNumber;
        private int segmentLastSectionNumber;

        private SectionMapEntry() { }

        /// <summary>
        /// Initialize a new instance of the SectionMapEntry class.
        /// </summary>
        /// <param name="tableID">The table ID.</param>
        /// <param name="sectionNumber">The section number.</param>
        /// <param name="lastTableID">The last table ID.</param>
        /// <param name="lastSectionNumber">The last section number.</param>
        /// <param name="segmentLastSectionNumber">The last section number of the segment.</param>
        public SectionMapEntry(int tableID, int sectionNumber, int lastTableID, int lastSectionNumber, int segmentLastSectionNumber)
        {
            this.tableID = tableID;
            this.sectionNumber = sectionNumber;

            this.lastTableID = lastTableID;
            this.lastSectionNumber = lastSectionNumber;
            this.segmentLastSectionNumber = segmentLastSectionNumber;
        }
    }
}
