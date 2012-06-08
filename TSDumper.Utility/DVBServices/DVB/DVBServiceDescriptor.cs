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
    /// DVB Service descriptor class.
    /// </summary>
    internal class DVBServiceDescriptor : DescriptorBase
    {
        /// <summary>
        /// Get the service type.
        /// </summary>
        public int ServiceType { get { return (serviceType); } }
        
        /// <summary>
        /// Get the provider name.
        /// </summary>
        public string ProviderName { get { return (providerName); } }
        
        /// <summary>
        /// Get the service name.
        /// </summary>
        public string ServiceName { get { return (serviceName); } }

        /// <summary>
        /// Get the index of the next byte in the EIT section following this descriptor.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The descriptor has not been processed.
        /// </exception> 
        public override int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("ServiceDescriptor: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int serviceType = -1;
        private string providerName;
        private string serviceName;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the DVBServiceDescriptor class.
        /// </summary>
        internal DVBServiceDescriptor() { }

        /// <summary>
        /// Parse the descriptor.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the descriptor.</param>
        /// <param name="index">Index of the byte in the MPEG2 section following the descriptor length.</param>
        internal override void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                serviceType = (int)byteData[lastIndex];
                lastIndex++;

                int providerNameLength = (int)byteData[lastIndex];
                lastIndex++;

                if (providerNameLength != 0)
                {
                    providerName = Utils.GetString(byteData, lastIndex, providerNameLength);
                    lastIndex += providerNameLength;
                }

                int serviceNameLength = (int)byteData[lastIndex];
                lastIndex++;

                if (serviceNameLength != 0)
                {
                    serviceName = Utils.GetString(byteData, lastIndex, serviceNameLength);
                    lastIndex += serviceNameLength;
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The DVB Service Descriptor message is short"));
            }
        }

        /// <summary>
        /// Validate the descriptor fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A descriptor field is not valid.
        /// </exception>
        internal override void Validate() { }

        /// <summary>
        /// Log the descriptor fields.
        /// </summary>
        internal override void LogMessage() 
        {
            if (Logger.ProtocolLogger == null)
                return;

            string providerName;
            if (ProviderName != null)
                providerName = ProviderName;
            else
                providerName = "?";

            string serviceName;
            if (ServiceName != null)
                serviceName = ServiceName;
            else
                serviceName = "?";

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DVB SERVICE DESCRIPTOR: Svc type: " + serviceType +
                " Prov name: " + providerName +
                " Svc name: " + serviceName);
        }
    }
}
