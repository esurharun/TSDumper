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
    /// The class that describes a DSM service location.
    /// </summary>
    public class DSMServiceLocation
    {
        /// <summary>
        /// Get the length of the service domain.
        /// </summary>
        public int ServiceDomainLength { get { return(serviceDomainLength); } }
        /// <summary>
        /// Get the service domain.
        /// </summary>
        public byte[] ServiceDomain { get { return (serviceDomain); } }
        /// <summary>
        /// Get the Cos naming name for the service location.
        /// </summary>
        public CosNamingName CosNamingName { get { return (cosNamingName); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the service location.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The service location has not been processed.
        /// </exception>
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("DSMServiceLocation: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private byte[] componentIDTag = new byte[1] { 0x00 };
        private int componentDataLength;
        private int serviceDomainLength;
        private byte[] serviceDomain = new byte[] { 0x00 };
        private CosNamingName cosNamingName;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the DSMServiceLocation class.
        /// </summary>
        public DSMServiceLocation() { }

        /// <summary>
        /// Parse the service location.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the service location.</param>
        /// <param name="index">Index of the first byte of the service location in the MPEG2 section.</param>
        public void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                componentIDTag = Utils.GetBytes(byteData, lastIndex, 4);
                lastIndex += 4;

                componentDataLength = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                serviceDomainLength = (int)byteData[lastIndex];
                lastIndex++;

                serviceDomain = Utils.GetBytes(byteData, lastIndex, serviceDomainLength);
                lastIndex += serviceDomainLength;

                cosNamingName = new CosNamingName();
                cosNamingName.Process(byteData, lastIndex);
                lastIndex += cosNamingName.Index;

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The DSM Service Location message is short"));
            }
        }

        /// <summary>
        /// Validate the service location fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A service location field is not valid.
        /// </exception>
        public void Validate() { }

        /// <summary>
        /// Log the service location fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DSM SERVICE LOCATION: Component ID tag: " + Utils.ConvertToHex(componentIDTag) +
                " Component data lth: " + componentDataLength +
                " Service domain lth: " + serviceDomainLength +
                " Service domain: " + Utils.ConvertToHex(serviceDomain));

            if (cosNamingName != null)
            {
                Logger.IncrementProtocolIndent();
                cosNamingName.LogMessage();
                Logger.DecrementProtocolIndent();
            }
        }
    }
}
