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
    /// The base class for BIOP messages.
    /// </summary>
    public abstract class BIOPMessageDetail
    {
        /// <summary>
        /// Get the index of the byte following the message detail.
        /// </summary>
        public abstract int Index { get; }

        /// <summary>
        /// Initialize a new instance of the BIOPMessageDetail class.
        /// </summary>
        public BIOPMessageDetail() { }

        /// <summary>
        /// Process the message.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the message.</param>
        /// <param name="index">The index of the first byte of the message in the MPEG2 section.</param>
        /// <param name="objectInfoLength">The length of the object information field.</param>
        public abstract void Process(byte[] byteData, int index, int objectInfoLength);
        
        /// <summary>
        /// Validate the message.
        /// </summary>
        public abstract void Validate();
        
        /// <summary>
        /// Log the message fields.
        /// </summary>
        public abstract void LogMessage();
    }
}
