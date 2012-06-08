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
    /// Dish Network Extended Event descriptor class.
    /// </summary>
    internal class DishNetworkExtendedEventDescriptor : DescriptorBase
    {
        /// <summary>
        /// Get the event description.
        /// </summary>
        public string EventDescription { get { return (eventDescription); } }
        /// <summary>
        /// Get the original unedited event description.
        /// </summary>
        public string OriginalDescription { get { return (originalDescription); } }
        /// <summary>
        /// Get the subtitle.
        /// </summary>
        public string SubTitle { get { return (subTitle); } }
        /// <summary>
        /// Get the closed captions flag.
        /// </summary>
        public bool ClosedCaptions { get { return (closedCaptions); } }
        /// <summary>
        /// Get the high definition flag.
        /// </summary>
        public bool HighDefinition { get { return (highDefinition); } }
        /// <summary>
        /// Get the stereo flag.
        /// </summary>
        public bool Stereo { get { return (stereo); } }
        /// <summary>
        /// Get the date.
        /// </summary>
        public string Date { get { return (date); } }
        /// <summary>
        /// Get the cast.
        /// </summary>
        public Collection<string> Cast { get { return (cast); } }

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
                    throw (new InvalidOperationException("DishNetworkExtendedEventDescriptor: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private byte[] startBytes;
        private byte[] eventDescriptionBytes;
        private string originalDescription;
        private string eventDescription;
        private string subTitle;
        private bool closedCaptions;
        private bool highDefinition;
        private bool stereo;
        private string date;
        private Collection<string> cast;

        private int huffmanTable;
        private int compressedLength;
        private int decompressedLength;
        private int loggedStartIndex;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the DishNetworkExtendedEventDescriptor class.
        /// </summary>
        internal DishNetworkExtendedEventDescriptor() { }

        /// <summary>
        /// Parse the descriptor.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the descriptor.</param>
        /// <param name="index">Index of the byte in the MPEG2 section following the descriptor length.</param>
        internal override void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            startBytes = Utils.GetBytes(byteData, lastIndex, 2);

            int startIndex;

            if ((byteData[lastIndex + 1] & 0xf8) == 0x80)
            {
                compressedLength = Length;

                if ((byteData[lastIndex] & 0x40) != 0)
                    decompressedLength = (byteData[lastIndex] & 0x3f) | ((byteData[lastIndex + 1] << 6) & 0xff);
                else
                    decompressedLength = byteData[lastIndex] & 0x3f;

                startIndex = lastIndex + 2;
                loggedStartIndex = 2;
            }
            else
            {
                compressedLength = Length - 1;
                decompressedLength = byteData[lastIndex] & 0x7f;
                startIndex = lastIndex + 1;
                loggedStartIndex = 1;
            }

            if (compressedLength <= 0)
            {
                lastIndex = index + Length;
                return;
            }

            eventDescriptionBytes = Utils.GetBytes(byteData, startIndex, compressedLength);

            if (Table <= 0x80)
                huffmanTable = 1;
            else
                huffmanTable = 2;

            originalDescription = SingleTreeDictionaryEntry.DecodeData(huffmanTable, eventDescriptionBytes);

            int splitter = originalDescription.IndexOf("<0D>");
            if (splitter != -1)
            {
                subTitle = originalDescription.Substring(0, splitter);
                eventDescription = originalDescription.Substring(splitter + 5, decompressedLength - subTitle.Length - 2);                
            }
            else
                eventDescription = originalDescription.Substring(0, decompressedLength);

            int closedCaptionsIndex = eventDescription.IndexOf(" (CC)");
            closedCaptions = closedCaptionsIndex != -1;
            if (closedCaptions)
                eventDescription = eventDescription = eventDescription.Remove(closedCaptionsIndex, 5);

            int highDefinitionIndex = eventDescription.IndexOf(" (HD)");
            highDefinition = highDefinitionIndex != -1;
            if (highDefinition)
                eventDescription = eventDescription.Remove(highDefinitionIndex, 5);

            int stereoIndex = eventDescription.IndexOf(" (Stereo)");
            stereo = stereoIndex != -1;
            if (stereo)
                eventDescription = eventDescription.Remove(stereoIndex, 9);

            int dateIndex = eventDescription.IndexOf(" (19");
            if (dateIndex == -1)
                dateIndex = eventDescription.IndexOf(" (20");
            if (dateIndex != -1)
            {
                date = eventDescription.Substring(dateIndex + 2, 4);
                eventDescription = eventDescription.Remove(dateIndex, 7);
            }

            if (eventDescription.StartsWith("Movie."))
                eventDescription = eventDescription.Remove(0, 6);
            else
            {
                if (eventDescription.StartsWith("Sports."))
                    eventDescription = eventDescription.Remove(0, 7);
                else
                {
                    if (eventDescription.StartsWith("News/Business."))
                        eventDescription = eventDescription.Remove(0, 14);
                    else
                    {
                        if (eventDescription.StartsWith("Family/Children."))
                            eventDescription = eventDescription.Remove(0, 16);
                        else
                        {
                            if (eventDescription.StartsWith("Education."))
                                eventDescription = eventDescription.Remove(0, 10);
                            else
                            {
                                if (eventDescription.StartsWith("Series/Special."))
                                    eventDescription = eventDescription.Remove(0, 15);
                                else
                                {
                                    if (eventDescription.StartsWith("Music/Art."))
                                        eventDescription = eventDescription.Remove(0, 10);
                                    else
                                    {
                                        if (eventDescription.StartsWith("Religious."))
                                            eventDescription = eventDescription.Remove(0, 10);
                                    }
                                }
                            }
                        }
                    }
                }            
            }

            int newIndex = eventDescription.IndexOf(" New.");
            if (newIndex != -1)
                eventDescription = eventDescription.Remove(newIndex, 5);

            eventDescription = eventDescription.Trim();

            string[] castParts = eventDescription.Split(new char[] { '.' } );
            if (castParts.Length > 2)
            {
                if (!castParts[0].Trim().StartsWith("Scheduled: "))
                {
                    cast = getCast(castParts[0]);
                    if (cast != null)
                        eventDescription = eventDescription.Remove(0, castParts[0].Length + 1).Trim();
                }
            }

            lastIndex = index + Length;

            Validate();            
        }

        private Collection<string> getCast(string castList)
        {
            Collection<string> cast = new Collection<string>();

            string[] castParts = castList.Split(new char[] { ',' });

            foreach (string castMember in castParts)
            {
                string name = castMember.Trim();

                if (name.StartsWith("Voice of: "))
                    cast.Add(name.Substring(10));
                else
                {
                    if (name.Length > 2 && name.Length < 21)
                    {
                        if (name[0] >= 'A' && name[0] <= 'Z')
                            cast.Add(name);
                        else
                            return (null);
                    }
                    else
                        return (null);
                }
            }

            return (cast);
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

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DISH NETWORK EXTENDED EVENT DESCRIPTOR: Huffman table: " + huffmanTable +
                " Compressed lth: " + compressedLength +
                " Decompressed lth: " + decompressedLength +
                " Start bytes: " + Utils.ConvertToHex(startBytes) +
                " Start index: " + loggedStartIndex +
                " Description: " + eventDescription);
        }
    }
}
