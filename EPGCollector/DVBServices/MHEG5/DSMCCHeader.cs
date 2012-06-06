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
    /// The class that describes a DSMCC message header.
    /// </summary>
    public class DSMCCHeader
    {
        /// <summary>
        /// Get the MPEG2 header for the message.
        /// </summary>
        public Mpeg2ExtendedHeader Mpeg2Header { get { return (mpeg2Header); } }

        /// <summary>
        /// Get the protocol discriminator.
        /// </summary>
        public int ProtocolDiscriminator { get { return (protocolDiscriminator); } }
        /// <summary>
        /// Get the DSMCC type.
        /// </summary>
        public int DSMCCType { get { return (dsmccType); } }
        /// <summary>
        /// Get the message ID.
        /// </summary>
        public int MessageID { get { return (messageID); } }
        /// <summary>
        /// Get the transactionID.
        /// </summary>
        public DSMCCTransactionID TransactionID { get { return (transactionID); } }
        /// <summary>
        /// Get the length of the adaption data.
        /// </summary>
        public int AdaptionLength { get { return (adaptionLength); } }
        /// <summary>
        /// Get the adaption data.
        /// </summary>
        public byte[] AdaptionData { get { return (adaptionData); } }
        /// <summary>
        /// Get the message length.
        /// </summary>
        public int MessageLength { get { return (messageLength); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the header.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The header has not been processed.
        /// </exception>
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("DSMCCHeader: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private Mpeg2ExtendedHeader mpeg2Header;

        private int protocolDiscriminator;
        private int dsmccType;
        private int messageID;
        private DSMCCTransactionID transactionID;
        private int reserved1;
        private int adaptionLength;
        private byte[] adaptionData = new byte[1] { 0x00 };
        private int messageLength;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the DSMCCHeader class.
        /// </summary>
        public DSMCCHeader() { }

        /// <summary>
        /// Parse the header.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the header.</param>
        /// <param name="mpeg2Header">Header of the MPEG2 section.</param>
        public void Process(byte[] byteData, Mpeg2ExtendedHeader mpeg2Header)
        {
            lastIndex = mpeg2Header.Index;

            this.mpeg2Header = mpeg2Header;

            try
            {
                protocolDiscriminator = (int)byteData[lastIndex];
                lastIndex++;

                dsmccType = (int)byteData[lastIndex];
                lastIndex++;

                messageID = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                transactionID = new DSMCCTransactionID(Utils.Convert4BytesToInt(byteData, lastIndex));
                lastIndex += 4;

                reserved1 = (int)byteData[lastIndex];
                lastIndex++;

                adaptionLength = (int)byteData[lastIndex];
                lastIndex++;

                messageLength = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                if (adaptionLength != 0)
                {
                    adaptionData = Utils.GetBytes(byteData, lastIndex, adaptionLength);
                    lastIndex += adaptionLength;
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The DSMCC header is short"));
            }
        }

        /// <summary>
        /// Validate the header.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The header information is not valid.
        /// </exception>
        public void Validate()
        {
            if (protocolDiscriminator != 0x11)
                throw (new ArgumentOutOfRangeException("Protocol discriminator is not 0x11"));

            if (dsmccType != 0x03)
                throw (new ArgumentOutOfRangeException("The DSMCC type is not 3"));

            if (messageID != 0x01002 &&
                messageID != 0x01003 &&
                messageID != 0x01006)
                throw (new ArgumentOutOfRangeException("The message ID is not a valid value"));

            if (reserved1 != 0xff)
                throw (new ArgumentOutOfRangeException("The reserved byte is not 0xff"));

            if (adaptionLength != 0)
                throw (new ArgumentOutOfRangeException("The adaption length is not zero"));
        }

        /// <summary>
        /// Log the header fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write("DSMCC HEADER: Prot. dis: " + Utils.ConvertToHex(protocolDiscriminator) +
                " Type: " + Utils.ConvertToHex(dsmccType) +
                " Message ID: " + Utils.ConvertToHex(messageID) +
                " Trans ID: " + Utils.ConvertToHex(transactionID.Value) +
                " Reserved: " + Utils.ConvertToHex(reserved1) +
                " Adapt lth: " + adaptionLength +
                " Msg lth: " + messageLength);
        }
    }
}
