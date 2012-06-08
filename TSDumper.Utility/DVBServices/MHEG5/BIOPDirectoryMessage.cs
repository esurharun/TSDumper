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
    /// The class the describes a BIOP directory message.
    /// </summary>
    public class BIOPDirectoryMessage : BIOPMessageDetail
    {
        /// <summary>
        /// Get the object information for the directory.
        /// </summary>
        public byte[] ObjectInfoData { get { return (objectInfoData); } }
        /// <summary>
        /// Get the collection of service contexts for the directory.
        /// </summary>
        public Collection<BIOPServiceContext> ServiceContexts { get { return (serviceContexts); } }
        /// <summary>
        /// Get the length of the message body.
        /// </summary>
        public int MessageBodyLength { get { return (messageBodyLength); } }
        /// <summary>
        /// Get the collection of bindings for the directory.
        /// </summary>
        public Collection<BIOPBinding> Bindings { get { return (bindings); } }

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
                    throw (new InvalidOperationException("BIOPDirectoryMessage: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private byte[] objectInfoData = new byte[1] { 0x00 };
        private int serviceContextCount;
        private Collection<BIOPServiceContext> serviceContexts;
        private int messageBodyLength;
        private int bindingsCount;
        private Collection<BIOPBinding> bindings;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the BIOPDirectoryMessage class.
        /// </summary>
        public BIOPDirectoryMessage() { }

        /// <summary>
        /// Parse the message.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the message.</param>
        /// <param name="index">Index of the first byte of the message  in the MPEG2 section.</param>
        /// <param name="objectInfoLength">The length of the object information data.</param>
        public override void Process(byte[] byteData, int index, int objectInfoLength)
        {
            lastIndex = index;

            try
            {
                if (objectInfoLength > 0)
                {
                    objectInfoData = Utils.GetBytes(byteData, lastIndex, objectInfoLength);
                    lastIndex += objectInfoLength;
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

                bindingsCount = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                if (bindingsCount != 0)
                {
                    bindings = new Collection<BIOPBinding>();

                    while (bindings.Count != bindingsCount)
                    {
                        BIOPBinding binding = new BIOPBinding();
                        binding.Process(byteData, lastIndex);
                        bindings.Add(binding);

                        lastIndex = binding.Index;
                    }
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The BIOP Directory message is short"));
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

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "BIOP DIRECTORY MESSAGE: Obj info data: " + Utils.ConvertToHex(ObjectInfoData) +
                " Svc ctxt ct: " + serviceContextCount + 
                " Msg body lth: " + messageBodyLength +
                 " Bindings ct: " + bindingsCount);

            if (serviceContexts != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (BIOPServiceContext serviceContext in serviceContexts)
                    serviceContext.LogMessage();

                Logger.DecrementProtocolIndent();
            }

            if (bindings != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (BIOPBinding binding in bindings)
                    binding.LogMessage();

                Logger.DecrementProtocolIndent();
            }            
        }
    }
}
