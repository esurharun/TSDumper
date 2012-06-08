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
    /// The class that describes a Event Information Table section.
    /// </summary>
    public class EventInformationTable
    {
        /// <summary>
        /// Return true if all sections have been received; false otherwsie.
        /// </summary>
        public static bool Complete
        {
            get
            {
                return (false);
            }            
        }

        /// <summary>
        /// Get the source ID.
        /// </summary>
        public int SourceID { get { return (sourceID); } }
        /// <summary>
        /// Get the protocol version.
        /// </summary>
        public int ProtocolVersion { get { return (protocolVersion); } }        
        /// <summary>
        /// Get the collection of events.
        /// </summary>
        internal Collection<EventInformationTableEntry> Events { get { return (events); } }
        
        private static int lastSectionNumber = -1;

        private Collection<EventInformationTableEntry> events;

        private int protocolVersion;
        private int sourceID;

        private int lastIndex;

        /// <summary>
        /// Initialize a new instance of the VirtualChannelTable class.
        /// </summary>
        public EventInformationTable()
        {
            Logger.ProtocolIndent = "";
        }

        /// <summary>
        /// Parse the section.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the section.</param>
        /// <param name="mpeg2Header">The MPEG2 header that preceedes the section.</param>
        public void Process(byte[] byteData, Mpeg2ExtendedHeader mpeg2Header)
        {
            lastIndex = mpeg2Header.Index;
            sourceID = mpeg2Header.TableIDExtension;
            lastSectionNumber = mpeg2Header.LastSectionNumber;

            protocolVersion = (int)byteData[lastIndex];
            lastIndex++;

            int entryCount = (int)byteData[lastIndex];
            lastIndex++;

            if (entryCount != 0)
            {
                events = new Collection<EventInformationTableEntry>();

                while (entryCount != 0)
                {
                    EventInformationTableEntry eventEntry = new EventInformationTableEntry();
                    eventEntry.Process(byteData, lastIndex);

                    events.Add(eventEntry);

                    lastIndex += eventEntry.TotalLength;
                    entryCount--;
                }
            }

            Validate();
        }

        private void addSectionNumber(int newSectionNumber)
        {
            
        }

        /// <summary>
        /// Validate the entry fields.
        /// </summary>
        public void Validate() { }

        /// <summary>
        /// Log the entry fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "EVENT INFORMATION TABLE: Source ID: " + sourceID + 
                " Protocol version: " + protocolVersion);

            if (events != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (EventInformationTableEntry eventEntry in events)
                    eventEntry.LogMessage();

                Logger.DecrementProtocolIndent();
            }
        }
    }
}
