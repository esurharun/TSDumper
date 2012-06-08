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
using System.Collections.ObjectModel;

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class the describes a BIOP file message.
    /// </summary>
    public class BIOPFileMessage : BIOPMessageDetail
    {
        /// <summary>
        /// Get the length in bytes of the file contents.
        /// </summary>
        public long FileContentSize { get { return (fileContentSize); } }
        /// <summary>
        /// Get the collection of MHP Content Type descriptors.
        /// </summary>
        public Collection<MHPContentTypeDescriptor> ContentTypeDescriptors { get { return (contentTypeDescriptors); } }
        /// <summary>
        /// Get the collection of BIOP Service Contexts.
        /// </summary>
        public Collection<BIOPServiceContext> ServiceContexts { get { return (serviceContexts); } }
        /// <summary>
        /// Get the length of the message body.
        /// </summary>
        public int MessageBodyLength { get { return (messageBodyLength); } }
        /// <summary>
        /// Get the content length.
        /// </summary>
        public int ContentLength { get { return (contentLength); } }
        /// <summary>
        /// Get the content data.
        /// </summary>
        public byte[] ContentData { get { return (contentData); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the message.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The message has not been processed.
        /// </exception> 
        public override int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("BIOPFileMessage: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private long fileContentSize;
        private int serviceContextCount;
        private Collection<MHPContentTypeDescriptor> contentTypeDescriptors;
        private Collection<BIOPServiceContext> serviceContexts;
        private int messageBodyLength;
        private int contentLength;
        private byte[] contentData = new byte[1] { 0x00 };
        
        private int lastIndex = -1;
        
        /// <summary>
        /// Initialize a new instance of the BIOPFileMessage class.
        /// </summary>
        public BIOPFileMessage() { }

        /// <summary>
        /// Parse the message.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the message.</param>
        /// <param name="index">Index of the first byte of the message in the MPEG2 section.</param>
        /// <param name="objectInfoLength">The length of the object information data.</param>
        public override void Process(byte[] byteData, int index, int objectInfoLength)
        {
            lastIndex = index;

            try
            {
                if (objectInfoLength != 0)
                {
                    fileContentSize = Utils.Convert8BytesToLong(byteData, lastIndex);
                    lastIndex += 8;

                    if (objectInfoLength > 8)
                    {
                        contentTypeDescriptors = new Collection<MHPContentTypeDescriptor>();

                        int workingLength = objectInfoLength - 8;

                        while (workingLength != 0)
                        {
                            MHPContentTypeDescriptor descriptor = new MHPContentTypeDescriptor();
                            descriptor.Process(byteData, lastIndex);
                            contentTypeDescriptors.Add(descriptor);

                            workingLength = descriptor.Index - lastIndex;
                            lastIndex = descriptor.Index;

                        }
                    }
                }

                serviceContextCount = (int)byteData[lastIndex];
                lastIndex++;

                if (serviceContextCount != 0)
                {
                    serviceContexts = new Collection<BIOPServiceContext>();

                    while (serviceContexts.Count != serviceContextCount)
                    {
                        BIOPServiceContext serviceContext = new BIOPServiceContext();
                        serviceContext.Process(byteData, lastIndex);
                        serviceContexts.Add(serviceContext);

                        lastIndex = serviceContext.Index;
                    }
                }

                messageBodyLength = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                contentLength = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                if (contentLength != 0)
                {
                    contentData = Utils.GetBytes(byteData, lastIndex, contentLength);
                    lastIndex += contentLength;
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The BIOP File message is short"));
            }
        }

        /// <summary>
        /// Validate the message fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A message field is not valid.
        /// </exception>
        public override void Validate() { }

        /// <summary>
        /// Log the message fields.
        /// </summary>
        public override void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            int contentLogSize = 0;
            if (contentData.Length < 16)
                contentLogSize = contentData.Length;
            else
                contentLogSize = 16;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "BIOP FILE MESSAGE: File content size: " + fileContentSize + 
                " Service ctxt ct: " + serviceContextCount +
                " Msg body lth: " + messageBodyLength +
                " Content lth: " + contentLength +
                " Content data: " + Utils.ConvertToHex(contentData, contentLogSize));

            if (contentTypeDescriptors != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (MHPContentTypeDescriptor contentTypeDescriptor in contentTypeDescriptors)
                    contentTypeDescriptor.LogMessage();

                Logger.DecrementProtocolIndent();
            }

            if (serviceContexts != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (BIOPServiceContext serviceContext in serviceContexts)
                    serviceContext.LogMessage();

                Logger.DecrementProtocolIndent();
            }
        }
    }
}
