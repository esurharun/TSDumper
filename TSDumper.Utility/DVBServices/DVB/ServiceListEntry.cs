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
    /// The class that describes a service list entry.
    /// </summary>
    public class ServiceListEntry
    {
        /// <summary>
        /// Get the service identification.
        /// </summary>
        public int ServiceID { get { return (serviceID); } }
        /// <summary>
        /// Get the service type.
        /// </summary>
        public int ServiceType { get { return (serviceType); } }

        private int serviceID;
        private int serviceType;

        private ServiceListEntry() { }

        /// <summary>
        /// Initialize a new instance of the ServiceListEntry class.
        /// </summary>
        /// <param name="serviceID">The service identification.</param>
        /// <param name="serviceType">The type of service (EN 300 468 table 81).</param>
        public ServiceListEntry(int serviceID, int serviceType)
        {
            this.serviceID = serviceID;
            this.serviceType = serviceType;
        }

        /// <summary>
        /// Validate the entry fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A field is not valid.
        /// </exception>
        internal void Validate() { }

        /// <summary>
        /// Log the entry fields.
        /// </summary>
        internal void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DVB SERVICE LIST ENTRY: Service ID : " + serviceID +
                " Service type: " + serviceType);            
        }
    }
}
