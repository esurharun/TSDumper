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

namespace DVBServices
{
    /// <summary>
    /// The class that describes a Huffman dictionary entry.
    /// </summary>
    public class OpenTVHuffmanEntry
    {
        /// <summary>
        /// Get or set the zero bit link.
        /// </summary>
        public OpenTVHuffmanEntry P0 
        { 
            get { return (p0); }
            set { p0 = value; }
        }
        
        /// <summary>
        /// Get or set the one bit link.
        /// </summary>
        public OpenTVHuffmanEntry P1 
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
            set { this.value = value; }
        }

        private OpenTVHuffmanEntry p0;
        private OpenTVHuffmanEntry p1;
        private string value;

        /// <summary>
        /// Intialize a new instance of the OpenTVHuffmanEntry.
        /// </summary>
        public OpenTVHuffmanEntry() { }        
    }
}
