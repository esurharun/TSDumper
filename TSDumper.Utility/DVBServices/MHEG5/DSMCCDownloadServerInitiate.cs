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

using System;

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class the describes a DSMCC download servier initiate message.
    /// </summary>
    public class DSMCCDownloadServerInitiate : DSMCCMessage
    {
        /// <summary>
        /// The message ID of a download server initiate message.
        /// </summary>
        public const int MessageIDDownloadServerInitiate = 0x1006;

        /// <summary>
        /// Get or set the transaction ID.
        /// </summary>
        public DSMCCTransactionID TransactionID
        {
            get { return (transactionID); }
            set { transactionID = value; }
        }

        /// <summary>
        /// Get the service gateway information.
        /// </summary>
        public ServiceGatewayInfo ServiceGatewayInfo { get { return (serviceGateWayInfo); } }

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
                    throw (new InvalidOperationException("DSMCCDownloadServerInitiate: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private DSMCCTransactionID transactionID;

        private byte[] serverID = new byte[1] { 0x00 };
        private int compatabilityDescriptorLength;
        private byte[] compatabilityDescriptor = new byte[1] { 0x00 };
        private int privateDataLength;
        private ServiceGatewayInfo serviceGateWayInfo;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the DSMCCDownloadServerInitiate message.
        /// </summary>
        /// <param name="dsmccHeader">The DSMCC header that preceedes the message.</param>
        public DSMCCDownloadServerInitiate(DSMCCHeader dsmccHeader) : base(dsmccHeader) { }

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
                serverID = Utils.GetBytes(byteData, lastIndex, 20);
                lastIndex += 20;

                compatabilityDescriptorLength = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                if (compatabilityDescriptorLength != 0)
                {
                    compatabilityDescriptor = Utils.GetBytes(byteData, lastIndex, compatabilityDescriptorLength);
                    lastIndex += compatabilityDescriptorLength;
                }

                privateDataLength = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                if (privateDataLength != 0)
                {
                    serviceGateWayInfo = new ServiceGatewayInfo();
                    serviceGateWayInfo.Process(byteData, lastIndex);
                    lastIndex = serviceGateWayInfo.Index;                    
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The DSMCC Download Server Initiate message is short"));
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
            foreach (byte serverIDByte in serverID)
            {
                if (serverIDByte != 0xff)
                    throw (new ArgumentOutOfRangeException("The server ID is wrong"));
            }

            if (compatabilityDescriptorLength != 0)
                throw (new ArgumentOutOfRangeException("The compatability descriptor length is not zero"));
        }

        /// <summary>
        /// Log the message fields.
        /// </summary>
        public override void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            base.LogMessage();            

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DOWNLOAD SERVER INITIATE: Server ID: " + Utils.ConvertToHex(serverID) +
                " Compat descr lth: " + compatabilityDescriptorLength +
                " Compat descr: " + Utils.ConvertToHex(compatabilityDescriptor) +
                " Priv data lth: " + privateDataLength +
                " Priv data: " + Utils.ConvertToHex(privateDataLength));

            if (serviceGateWayInfo != null)
            {
                Logger.IncrementProtocolIndent();
                serviceGateWayInfo.LogMessage();
                Logger.DecrementProtocolIndent();
            }
        }
    }
}
