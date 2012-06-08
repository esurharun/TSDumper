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
    /// The class that describes an Extended Text Table section.
    /// </summary>
    public class ExtendedTextTable
    {
        /// <summary>
        /// Get the collection of Extended Text entries in the section.
        /// </summary>
        public static Collection<ExtendedTextTableEntry> TextEntries
        {
            get
            {
                if (textEntries == null)
                    textEntries = new Collection<ExtendedTextTableEntry>();
                return (textEntries);
            }
        }

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
        /// Get the protocol version.
        /// </summary>
        public int ProtocolVersion { get { return (protocolVersion); } }
        /// <summary>
        /// Get the text entry.
        /// </summary>
        public ExtendedTextTableEntry TextEntry { get { return (extendedTextEntry); } }

        private static Collection<ExtendedTextTableEntry> textEntries;
        
        private int protocolVersion;
        private ExtendedTextTableEntry extendedTextEntry;

        private int lastIndex;

        /// <summary>
        /// Initialize a new instance of the ExtendedTextTable class.
        /// </summary>
        public ExtendedTextTable()
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

            protocolVersion = (int)byteData[lastIndex];
            lastIndex++;

            extendedTextEntry = new ExtendedTextTableEntry();
            extendedTextEntry.Process(byteData, lastIndex);
            addEntry(extendedTextEntry);

            lastIndex+= extendedTextEntry.Index;

            Validate();
        }

        private void addEntry(ExtendedTextTableEntry newEntry)
        {
            foreach (ExtendedTextTableEntry oldEntry in TextEntries)
            {
                if (oldEntry.SourceID == newEntry.SourceID)
                {
                    if (oldEntry.EventID == newEntry.EventID)
                        return;

                    if (oldEntry.EventID > newEntry.EventID)
                    {
                        TextEntries.Insert(TextEntries.IndexOf(oldEntry), newEntry);
                        return;
                    }
                }
                else
                {
                    if (oldEntry.SourceID > newEntry.SourceID)
                    {
                        TextEntries.Insert(TextEntries.IndexOf(oldEntry), newEntry);
                        return;
                    }
                }
            }

            TextEntries.Add(newEntry);
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

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "EXTENDED TEXT TABLE: Version: " + protocolVersion);

            if (extendedTextEntry != null)
            {
                Logger.IncrementProtocolIndent();
                extendedTextEntry.LogMessage();
                Logger.DecrementProtocolIndent();
            }
        }
                
        internal static ExtendedTextTableEntry FindEntry(int sourceID, int eventID)
        {
            foreach (ExtendedTextTableEntry textEntry in TextEntries)
            {
                if (textEntry.SourceID == sourceID && textEntry.EventID == eventID)
                    return (textEntry);
            }

            return (null);
        }

        internal static void LogEntries()
        {
            Logger.Instance.WriteSeparator("Extended Text Entries");

            foreach (ExtendedTextTableEntry entry in TextEntries)
            {
                string textString;
                if (entry.Text != null)
                    textString = entry.Text.ToString();
                else
                    textString = "* Not present* ";

                Logger.Instance.Write("Source ID: " + entry.SourceID +
                    " Event ID: " + entry.EventID +
                    " Text: " + textString);
            }

            Logger.Instance.WriteSeparator("Extended Text Entries");
        }

        internal static void Clear()
        {
            textEntries = null;
        }
    }
}
