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
    /// The class that describes an extended text table entry.
    /// </summary>
    public class ExtendedTextTableEntry
    {
        /// <summary>
        /// Get the source ID.
        /// </summary>
        public int SourceID { get { return (sourceID); } }
        /// <summary>
        /// Get the event ID.
        /// </summary>
        public int EventID { get { return (eventID); } }
        /// <summary>
        /// Get the text.
        /// </summary>
        public MultipleString Text { get { return (text); } }        

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the transport stream.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The transport stream has not been processed.
        /// </exception> 
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("Extended Text Table Entry: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int sourceID;
        private int eventID;
        private MultipleString text;
        
        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the ExtendedTextTableEntry class.
        /// </summary>
        public ExtendedTextTableEntry() { }

        /// <summary>
        /// Parse the entry.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the transport stream.</param>
        /// <param name="index">Index of the first byte of the transport stream in the MPEG2 section.</param>
        internal void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                sourceID = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                eventID = Utils.Convert2BytesToInt(byteData, lastIndex) >> 2;
                lastIndex += 2;

                text = new MultipleString();
                text.Process(byteData, lastIndex);
                lastIndex = text.Index;

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The Extended Text Table entry message is short"));
            }
        }

        /// <summary>
        /// Validate the fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A field is not valid.
        /// </exception>
        public void Validate() { }

        /// <summary>
        /// Log the fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "EXTENDED TEXT TABLE ENTRY: Source ID: " + sourceID +
                " Event ID: " + eventID);

            Logger.IncrementProtocolIndent();
            text.LogMessage();
            Logger.DecrementProtocolIndent();            
        }
    }
}
