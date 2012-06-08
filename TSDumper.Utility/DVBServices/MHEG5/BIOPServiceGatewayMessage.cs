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
    /// The class that describes the BIOP Service Gateway message.
    /// </summary>
    public class BIOPServiceGatewayMessage : BIOPMessageDetail
    {
        /// <summary>
        /// Get the object information data.
        /// </summary>
        public byte[] ObjectInfoData { get { return (objectInfoData); } }
        /// <summary>
        /// Get the collection of service contexts.
        /// </summary>
        public Collection<BIOPServiceContext> ServiceContexts { get { return (serviceContexts); } }
        /// <summary>
        /// Get the message body length.
        /// </summary>
        public int MessageBodyLength { get { return (messageBodyLength); } }
        /// <summary>
        /// Get the collection of bindings.
        /// </summary>
        public Collection<BIOPBinding> Bindings { get { return (bindings); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the service gateway message.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The service gateway message has not been processed.
        /// </exception>
        public override int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("BIOPServiceGatewayMessage: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private byte[] objectInfoData;
        private int serviceContextCount;
        private Collection<BIOPServiceContext> serviceContexts;
        private int messageBodyLength;
        private int bindingsCount;
        private Collection<BIOPBinding> bindings;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the BIOPServiceGatewayMessage class.
        /// </summary>
        public BIOPServiceGatewayMessage() { }

        /// <summary>
        /// Parse the service gateway message.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the service gateway message.</param>
        /// <param name="index">Index of the first byte of the service gateway message in the MPEG2 section.</param>
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
                throw (new ArgumentOutOfRangeException("The BIOP Service Gateway message is short"));
            }
        }

        /// <summary>
        /// Validate the service gateway message fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A service gateway message field is not valid.
        /// </exception>
        public override void Validate() { }

        /// <summary>
        /// Log the service gateway message fields.
        /// </summary>
        public override void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "BIOP SERVICE GATEWAY MESSAGE: Svc ctxt ct: " + serviceContextCount +
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
