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
    /// The class that describes the Service Gateway information.
    /// </summary>
    public class ServiceGatewayInfo    
    {
        /// <summary>
        /// Get the IOP:IOR.
        /// </summary>
        public IOPIOR IOPIOR { get { return (iopIor); } }
        /// <summary>
        /// Get the collection of taps.
        /// </summary>
        public Collection<BIOPTap> Taps { get { return (taps); } }
        /// <summary>
        /// Get the collection of service contexts.
        /// </summary>
        public Collection<BIOPServiceContext> ServiceContexts { get { return (serviceContexts); } }
        /// <summary>
        /// Get the length of the user information.
        /// </summary>
        public int UserInfoLength { get { return (userInfoLength); } }
        /// <summary>
        /// Get the user information.
        /// </summary>
        public byte[] UserInfo { get { return (userInfo); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the service gateway information.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The information has not been processed.
        /// </exception> 
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("ServiceGatewayInfo: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private IOPIOR iopIor;
        private int tapsCount;
        private Collection<BIOPTap> taps;
        private int serviceContextCount;
        private Collection<BIOPServiceContext> serviceContexts;
        private int userInfoLength;
        private byte[] userInfo = new byte[1] { 0x00 };

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the ServiceGatewayInfo class.
        /// </summary>
        public ServiceGatewayInfo() { }

        /// <summary>
        /// Parse the gateway information.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the gateway information.</param>
        /// <param name="index">Index of the byte in the MPEG2 section following the gateway information.</param>
        public void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                iopIor = new IOPIOR();
                iopIor.Process(byteData, lastIndex);
                lastIndex = iopIor.Index;

                tapsCount = (int)byteData[lastIndex];
                lastIndex++;

                if (tapsCount != 0)
                {
                    taps = new Collection<BIOPTap>();

                    while (taps.Count < tapsCount)
                    {
                        BIOPTap tap = new BIOPTap();
                        tap.Process(byteData, lastIndex);
                        taps.Add(tap);

                        lastIndex = tap.Index;
                    }
                }

                serviceContextCount = (int)byteData[lastIndex];
                lastIndex++;

                if (serviceContextCount != 0)
                {
                    serviceContexts = new Collection<BIOPServiceContext>();

                    while (serviceContexts.Count < serviceContextCount)
                    {
                        BIOPServiceContext serviceContext = new BIOPServiceContext();
                        serviceContext.Process(byteData, lastIndex);
                        serviceContexts.Add(serviceContext);

                        lastIndex = serviceContext.Index;
                    }
                }

                userInfoLength = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                if (userInfoLength != 0)
                {
                    userInfo = Utils.GetBytes(byteData, lastIndex, userInfoLength);
                    lastIndex += userInfoLength;
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The Service Gateway Info message is short"));
            }
        }

        /// <summary>
        /// Validate the gateway information fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// An information field is not valid.
        /// </exception>
        public void Validate() { }

        /// <summary>
        /// Log the gateway information fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "SERVER GATEWAY INFO: Taps count: " + tapsCount +
                " Service context count: " + serviceContextCount +
                " UserInfo lth: " + userInfoLength +
                " User info: " + Utils.ConvertToHex(userInfo));

            if (iopIor != null)
            {
                Logger.IncrementProtocolIndent();
                iopIor.LogMessage();
                Logger.DecrementProtocolIndent();
            }

            if (taps != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (BIOPTap tap in taps)
                    tap.LogMessage();

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
