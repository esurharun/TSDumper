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
    /// The class that describes a BIOP service context.
    /// </summary>
    public class BIOPServiceContext
    {
        /// <summary>
        /// Get the context ID.
        /// </summary>
        public int ContextID { get { return (contextID); } }
        /// <summary>
        /// Get the context data length.
        /// </summary>
        public int DataLength { get { return(dataLength); } }
        /// <summary>
        /// Get the context data.
        /// </summary>
        public byte[] ContextData { get { return (contextData); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the service context.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The service context has not been processed.
        /// </exception>
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("BIOPServiceContext: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int contextID;
        private int dataLength;
        private byte[] contextData;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the BIOPServiceContext class.
        /// </summary>
        public BIOPServiceContext() { }

        /// <summary>
        /// Parse the service context.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the service context.</param>
        /// <param name="index">Index of the first byte of the service context in the MPEG2 section.</param>
        public void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                contextID = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                dataLength = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                if (dataLength != 0)
                {
                    contextData = Utils.GetBytes(byteData, lastIndex, dataLength);
                    lastIndex += dataLength;
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The Service Context message is short"));
            }
        }

        /// <summary>
        /// Validate the service context fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A service context field is not valid.
        /// </exception>
        public void Validate() { }

        /// <summary>
        /// Log the service context fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "BIOP SERVICE CONTEXT: Context ID: " + contextID +
                " Data length: " + dataLength);
        }
    }
}
