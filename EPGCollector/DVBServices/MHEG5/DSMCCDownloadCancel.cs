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
    /// The class that describes a DSMCC download cancel message.
    /// </summary>
    public class DSMCCDownloadCancel : DSMCCMessage
    {
        /// <summary>
        /// The message ID of a download cancel message.
        /// </summary>
        public const int MessageIDDownloadCancel = 0x1005;

        /// <summary>
        /// Get the download ID.
        /// </summary>
        public int DownloadID { get { return(downloadID); } }
        /// <summary>
        /// Get the module ID..
        /// </summary>
        public int ModuleID { get { return (moduleID); } }
        /// <summary>
        /// Get the block number.
        /// </summary>
        public int BlockNumber { get { return (blockNumber); } }
        /// <summary>
        /// Get the download cancel reason code.
        /// </summary>
        public int DownloadCancelReason { get { return (downloadCancelReason); } }
        /// <summary>
        /// Get the length of private data.
        /// </summary>
        public int PrivateDataLength { get { return (privateDataLength); } }
        /// <summary>
        /// Get the private data.
        /// </summary>
        public byte[] PrivateData { get { return (privateData); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the message.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The message has not been processed.
        /// </exception> 
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("DSMCCDownloadCancel: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int downloadID;
        private int moduleID;
        private int blockNumber;
        private int downloadCancelReason;
        private int reserved1;
        private int privateDataLength;
        private byte[] privateData = new byte[1] { 0x00 };

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the DSMCCDownloadCancel message.
        /// </summary>
        /// <param name="dsmccHeader">The DSMCC header that preceedes the message.</param>
        public DSMCCDownloadCancel(DSMCCHeader dsmccHeader) : base(dsmccHeader) { }

        /// <summary>
        /// Parse the message.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the message.</param>
        /// <param name="index">Index of the first byte of the message following the header in the MPEG2 section.</param>
        public override void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                downloadID = Utils.Swap4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                moduleID = Utils.Swap2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                blockNumber = Utils.Swap2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                downloadCancelReason = (int)byteData[lastIndex];
                lastIndex++;

                reserved1 = (int)byteData[lastIndex];
                lastIndex++;

                privateDataLength = Utils.Swap2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                if (privateDataLength != 0)
                {
                    privateData = Utils.GetBytes(byteData, lastIndex, privateDataLength);
                    lastIndex += privateDataLength;
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The DSMCC Download Cancel message is short"));
            }
        }

        /// <summary>
        /// Validate the message.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The message information is not valid.
        /// </exception>
        public override void Validate()
        {
            if (reserved1 != 0xff)
                throw (new ArgumentOutOfRangeException("Reserved byte is not 0xff"));
        }

        /// <summary>
        /// Log the message fields.
        /// </summary>
        public override void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            base.LogMessage();

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DOWNLOAD CANCEL: Download ID: " + Utils.ConvertToHex(downloadID) +
                " Module ID: " + Utils.ConvertToHex(moduleID) +
                " Block no: " + blockNumber +
                " Cancel rsn: " + Utils.ConvertToHex(downloadCancelReason) +
                " Reserved1: " + Utils.ConvertToHex(reserved1) +
                " Priv data lth: " + privateDataLength +
                " Priv data: " + Utils.ConvertToHex(privateData));
        }
    }
}
