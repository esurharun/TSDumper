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
    /// The class that describes a BIOP message.
    /// </summary>
    public class BIOPMessage
    {
        /// <summary>
        /// Get the major version.
        /// </summary>
        public int MajorVersion { get { return (majorVersion); } }        
        /// <summary>
        /// Get the minor version.
        /// </summary>
        public int MinorVersion { get { return (minorVersion); } }
        /// <summary>
        /// Get the byte ordering.
        /// </summary>
        public int ByteOrder { get { return (byteOrder); } }
        /// <summary>
        /// Get the message type.
        /// </summary>
        public int MessageType { get { return (messageType); } }
        /// <summary>
        /// Get the message size.
        /// </summary>
        public int MessageSize { get { return (messageSize); } }
        /// <summary>
        /// Get the object key length.
        /// </summary>
        public int ObjectKeyLength { get { return(objectKeyLength); } }
        /// <summary>
        /// Get the object key data.
        /// </summary>
        public byte[] ObjectKeyData { get { return (objectKeyData); } }
        /// <summary>
        /// Get the object kind length.
        /// </summary>
        public int ObjectKindLength { get { return (objectKindLength); } }
        /// <summary>
        /// Get te object kind data.
        /// </summary>
        public byte[] ObjectKindData { get { return (objectKindData); } }
        /// <summary>
        /// Get the object information length.
        /// </summary>
        public int ObjectInfoLength { get { return (objectInfoLength); } }
        /// <summary>
        /// Get the message detail.
        /// </summary>
        public BIOPMessageDetail MessageDetail { get { return (messageDetail); } }

        /// <summary>
        /// Return true if this is a file message.
        /// </summary>
        public bool IsKindFile{ get { return (matchKind(objectKindData, biopFile)); } }

        /// <summary>
        /// Return true if this is a directory message.
        /// </summary>
        public bool IsKindDirectory { get { return (matchKind(objectKindData, biopDirectory)); } }

        /// <summary>
        /// Return true if this is a service gateway message.
        /// </summary>
        public bool IsKindServiceGateway { get { return (matchKind(objectKindData, biopSRG)); } }

        /// <summary>
        /// Return true if this is a service transaction message.
        /// </summary>
        public bool IsKindServiceTransaction { get { return (matchKind(objectKindData, biopSTR)); } }

        /// <summary>
        /// Get a description of the kind of data this message represents.
        /// </summary>
        public string Kind
        {
            get
            {
                if (IsKindFile)
                    return ("File");
                if (IsKindDirectory)
                    return("Directory");
                if (IsKindServiceGateway)
                    return("Service Gateway");
                if (IsKindServiceTransaction)
                    return("Service Transaction");
                return (Utils.ConvertToHex(objectKindData));
            }
        }

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
                if (messageDetail == null)
                    throw (new InvalidOperationException("BIOPMessage: Index requested before block processed"));
                return(messageDetail.Index);
            }
        }

        private byte[] magic;
        private int majorVersion;
        private int minorVersion;
        private int byteOrder;
        private int messageType;
        private int messageSize;
        private int objectKeyLength;
        private byte[] objectKeyData = new byte[1] { 0x00 };
        private int objectKindLength;
        private byte[] objectKindData = new byte[1] { 0x00 };
        private int objectInfoLength;
        private BIOPMessageDetail messageDetail;

        private int lastIndex = -1;

        /// <summary>
        /// Get the BIOP magic identifier.
        /// </summary>
        public static byte[] BiopMagic = new byte[] { 0x42, 0x49, 0x4f, 0x50 };
        
        private byte[] biopFile = new byte[] { 0x66, 0x69, 0x6c, 0x00 };
        private byte[] biopDirectory = new byte[] { 0x64, 0x69, 0x72, 0x00 };
        private byte[] biopSRG = new byte[] { 0x73, 0x72, 0x67, 0x00 };
        private byte[] biopSTR = new byte[] { 0x73, 0x74, 0x72, 0x00 };

        /// <summary>
        /// Initialize a new instance of the BIOPMessage class.
        /// </summary>
        public BIOPMessage() { }

        /// <summary>
        /// Parse the message.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the message.</param>
        /// <param name="index">Index of the first byte of the message in the MPEG2 section.</param>
        public virtual void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                magic = Utils.GetBytes(byteData, lastIndex, 4);
                lastIndex += 4;

                majorVersion = (int)byteData[lastIndex];
                lastIndex++;

                minorVersion = (int)byteData[lastIndex];
                lastIndex++;

                byteOrder = (int)byteData[lastIndex];
                lastIndex++;

                messageType = (int)byteData[lastIndex];
                lastIndex++;

                messageSize = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                objectKeyLength = (int)byteData[lastIndex];
                lastIndex++;

                if (objectKeyLength != 0)
                {
                    objectKeyData = Utils.GetBytes(byteData, lastIndex, objectKeyLength);
                    lastIndex += objectKeyLength;
                }

                objectKindLength = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                if (objectKindLength != 0)
                {
                    objectKindData = Utils.GetBytes(byteData, lastIndex, objectKindLength);
                    lastIndex += objectKindLength;
                }

                objectInfoLength = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                if (objectKindLength != 4)
                    return;

                if (Utils.CompareBytes(objectKindData, biopFile))
                {
                    messageDetail = new BIOPFileMessage();
                    messageDetail.Process(byteData, lastIndex, objectInfoLength);
                    return;
                }

                if (Utils.CompareBytes(objectKindData, biopDirectory))
                {
                    messageDetail = new BIOPDirectoryMessage();
                    messageDetail.Process(byteData, lastIndex, objectInfoLength);
                    return;
                }

                if (Utils.CompareBytes(objectKindData, biopSRG))
                {
                    messageDetail = new BIOPServiceGatewayMessage();
                    messageDetail.Process(byteData, lastIndex, objectInfoLength);
                    return;
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The BIOP message is short"));
            }
        }

        /// <summary>
        /// Validate the message.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A message field is not valid.
        /// </exception>
        public virtual void Validate()
        {
            if (!Utils.CompareBytes(magic, BIOPMessage.BiopMagic))
                throw (new ArgumentOutOfRangeException("BIOPBaseMessage: Magic string is not BIOP"));

            if (majorVersion != 1)
                throw (new ArgumentOutOfRangeException("BIOPBaseMessage: Major version is not 1"));

            if (minorVersion != 1)
                throw (new ArgumentOutOfRangeException("BIOPBaseMessage: Minor version is not zero"));

            if (byteOrder != 0)
                throw (new ArgumentOutOfRangeException("BIOPBaseMessage: Byte order is not zero"));

            if (messageType != 0)
                throw (new ArgumentOutOfRangeException("BIOPBaseMessage: Message type is not zero"));
        }

        /// <summary>
        /// Log the message fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "BIOP MESSAGE: Magic: " + Utils.ConvertToHex(magic) +
                " Major vers: " + Utils.ConvertToHex(majorVersion) +
                " Minor vers: " + Utils.ConvertToHex(minorVersion) +
                " Byte order: " + Utils.ConvertToHex(byteOrder) +
                " Msg type: " + Utils.ConvertToHex(messageType) +
                " Msg size: " + messageSize +
                " Obj key lth: " + objectKeyLength +
                " Obj key : " + Utils.ConvertToHex(objectKeyData) +
                " Obj kind lth: " + objectKindLength +
                " Obj kind data: " + Utils.ConvertToHex(objectKindData) +
                " Obj info lth: " + objectInfoLength);

            if (messageDetail != null)
            {
                Logger.IncrementProtocolIndent();
                messageDetail.LogMessage();
                Logger.DecrementProtocolIndent();
            }
        }

        private bool matchKind(byte[] inputBytes, byte[] referenceBytes)
        {
            if (inputBytes.Length != referenceBytes.Length)
                return (false);

            for (int index = 0; index < inputBytes.Length; index++)
            {
                if (inputBytes[index] != referenceBytes[index])
                    return (false);
            }

            return (true);
        }
    }
}
