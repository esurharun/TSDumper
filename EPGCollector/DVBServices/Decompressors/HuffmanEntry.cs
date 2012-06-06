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

namespace DVBServices
{
    /// <summary>
    /// The class that describes a Huffman dictionary entry.
    /// </summary>
    public class HuffmanEntry
    {
        /// <summary>
        /// Get or set the zero bit link.
        /// </summary>
        public HuffmanEntry P0
        {
            get { return (p0); }
            set { p0 = value; }
        }

        /// <summary>
        /// Get or set the one bit link.
        /// </summary>
        public HuffmanEntry P1
        {
            get { return (p1); }
            set { p1 = value; }
        }

        /// <summary>
        /// Get or set the entry value.
        /// </summary>
        public string Value
        {
            get { return (value); }
            set 
            { 
                this.value = value;
                holdsValue = true;
            }
        }

        /// <summary>
        /// Returns true if the value has been set; false otherwise.
        /// </summary>
        public bool HoldsValue { get { return (holdsValue); } }

        private HuffmanEntry p0;
        private HuffmanEntry p1;
        private string value;
        private bool holdsValue;

        /// <summary>
        /// Intialize a new instance of the HuffmanEntry.
        /// </summary>
        public HuffmanEntry() { }
    }
}
