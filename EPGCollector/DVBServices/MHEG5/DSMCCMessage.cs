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

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The base class for DSMCC messages.
    /// </summary>
    public abstract class DSMCCMessage
    {
        /// <summary>
        /// Create an instance of a DSMCC message.
        /// </summary>
        /// <param name="dsmccHeader">The header of the message.</param>
        /// <param name="byteData">The MPEG2 data block that contains the message.</param>
        /// <returns>An instance of the appropriate DSMCC message class.</returns>
        public static DSMCCMessage CreateInstance(DSMCCHeader dsmccHeader, byte[] byteData)
        {
            DSMCCMessage dsmccMessage;

            switch (dsmccHeader.MessageID)
            {
                case DSMCCDownloadDataBlock.MessageIDDownloadDataBlock:
                    dsmccMessage = new DSMCCDownloadDataBlock(dsmccHeader);                    
                    break;
                case DSMCCDownloadInfoIndication.MessageIDDownloadInfoIndication:
                    dsmccMessage = new DSMCCDownloadInfoIndication(dsmccHeader);
                    break;
                case DSMCCDownloadServerInitiate.MessageIDDownloadServerInitiate:
                    dsmccMessage = new DSMCCDownloadServerInitiate(dsmccHeader);
                    break;
                case DSMCCDownloadCancel.MessageIDDownloadCancel:
                    dsmccMessage = new DSMCCDownloadCancel(dsmccHeader);
                    break;
                default:
                    throw (new ArgumentException("The DSMCC message ID is out of range"));
            }

            dsmccMessage.Process(byteData, dsmccHeader.Index);

            return (dsmccMessage);        
        }

        /// <summary>
        /// Get the header of the message.
        /// </summary>
        public DSMCCHeader DSMCCHeader { get { return (dsmccHeader); } }

        private DSMCCHeader dsmccHeader;

        private DSMCCMessage() { }

        /// <summary>
        /// Initialize a new instance of the DSMCCMessage class.
        /// </summary>
        /// <param name="dsmccHeader">The header of the message.</param>
        public DSMCCMessage(DSMCCHeader dsmccHeader)
        {
            this.dsmccHeader = dsmccHeader;
        }

        /// <summary>
        /// Parse the message.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the message.</param>
        /// <param name="index">Index of the first byte of the message following the header in the MPEG2 section.</param>
        public abstract void Process(byte[] byteData, int index);
        /// <summary>
        /// Validate the message.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The message information is not valid.
        /// </exception>
        public abstract void Validate();
        /// <summary>
        /// Log the message fields.
        /// </summary>
        public virtual void LogMessage()
        {
            Logger.ProtocolIndent = "";

            dsmccHeader.LogMessage();

            Logger.IncrementProtocolIndent();
        }
    }
}
