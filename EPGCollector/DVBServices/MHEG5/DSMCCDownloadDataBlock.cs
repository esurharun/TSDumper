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
    /// The class that describes a DSMCC download data block.
    /// </summary>
    public class DSMCCDownloadDataBlock : DSMCCMessage
    {
        /// <summary>
        /// The message ID of a download data block.
        /// </summary>
        public const int MessageIDDownloadDataBlock = 0x1003;

        /// <summary>
        /// Get the module ID.
        /// </summary>
        public int ModuleID { get { return (moduleID); } }
        /// <summary>
        /// Get the module version.
        /// </summary>
        public int ModuleVersion { get { return (moduleVersion); } }
        /// <summary>
        /// Get the block number.
        /// </summary>
        public int BlockNumber { get { return (blockNumber); } }
        /// <summary>
        /// Get the data block.
        /// </summary>
        public byte[] Data { get { return (data); } }
        /// <summary>
        /// Get the size of the data block.
        /// </summary>
        public int DataSize { get { return (dataSize); } }

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
                    throw (new InvalidOperationException("DSMCCDownloadDataBlock: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int moduleID;
        private int moduleVersion;
        private int reserved1;
        private int blockNumber;
        private byte[] data = new byte[1] { 0x00 };
        private int dataSize;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the DSMCCDownloadDataBlock.
        /// </summary>
        /// <param name="dsmccHeader">The DSMCC header that preceedes the message.</param>
        public DSMCCDownloadDataBlock(DSMCCHeader dsmccHeader) : base(dsmccHeader) { }

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
                moduleID = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                moduleVersion = (int)byteData[lastIndex];
                lastIndex++;

                reserved1 = (int)byteData[lastIndex];
                lastIndex++;

                blockNumber = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                dataSize = (base.DSMCCHeader.MessageLength - base.DSMCCHeader.AdaptionLength) - (lastIndex - index);

                if (dataSize != 0)
                    data = Utils.GetBytes(byteData, lastIndex, dataSize);

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The DSMCC Download Data Block message is short"));
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
                throw (new ArgumentOutOfRangeException("Reserved 1 is not 0xff"));
        }

        /// <summary>
        /// Log the message fields.
        /// </summary>
        public override void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            base.LogMessage();

            int logDataSize;
            if (data.Length < 10)
                logDataSize = data.Length;
            else
                logDataSize = 10;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DOWNLOAD DATA BLOCK: Module ID: " + moduleID +
                " Module ver: " + Utils.ConvertToHex(moduleVersion) +
                " Reserved: " + Utils.ConvertToHex(reserved1) +
                " Block no: " + blockNumber +
                " Data size: " + dataSize +
                " Initial data: " + Utils.ConvertToHex(data, logDataSize));
        }
    }
}
