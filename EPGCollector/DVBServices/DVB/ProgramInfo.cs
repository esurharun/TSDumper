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
    /// The class that describes the program information.
    /// </summary>
    public class ProgramInfo
    {
        /// <summary>
        /// Get the program number.
        /// </summary>
        public int ProgramNumber { get { return (programNumber); } }
        /// <summary>
        /// Get the program identification (PID).
        /// </summary>
        public int ProgramID { get { return (programID); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the program information.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The program information has not been processed.
        /// </exception> 
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("ProgramInfo: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int programNumber;
        private int programID;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the ProgramInfo class.
        /// </summary>
        public ProgramInfo() { }

        /// <summary>
        /// Parse the program information.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the program information.</param>
        /// <param name="index">Index of the first byte of the program information in the MPEG2 section.</param>
        internal void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                programNumber = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                programID = (int)((byteData[lastIndex] & 0x1f) * 256) + (int)byteData[lastIndex + 1];
                lastIndex += 2;
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The Program Info message is short"));
            }
        }

        /// <summary>
        /// Validate the program information fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A program information field is not valid.
        /// </exception>
        public void Validate() { }

        /// <summary>
        /// Log the program information fields.
        /// </summary>
        public void LogMessage() { }
    }
}
