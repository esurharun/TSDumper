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

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that describes a DSMCC MPEG2 section.
    /// </summary>
    public class DSMCCSection
    {
        /// <summary>
        /// Get the DSMCCmessage for this section.
        /// </summary>
        public DSMCCMessage DSMCCMessage { get { return(dsmccMessage); } }

        private DSMCCMessage dsmccMessage;

        /// <summary>
        /// Initialize a new instance of the DSMCCSection class.
        /// </summary>
        public DSMCCSection() { }

        /// <summary>
        /// Parse the section.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containg the DSMCC data.</param>
        /// <param name="mpeg2Header">The MPEG2 header for the section.</param>
        public void Process(byte[] byteData, Mpeg2ExtendedHeader mpeg2Header)
        {
            DSMCCHeader dsmccHeader = new DSMCCHeader();
            dsmccHeader.Process(byteData, mpeg2Header);
            
            dsmccMessage = DSMCCMessage.CreateInstance(dsmccHeader, byteData);
        }
    }
}
