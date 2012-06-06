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
using System.Collections.ObjectModel;

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// DVB Extended Event descriptor class.
    /// </summary>
    internal class DVBExtendedEventDescriptor : DescriptorBase
    {
        /// <summary>
        /// Get the item descriptions.
        /// </summary>
        public Collection<string> ItemDescriptions { get { return (itemDescriptions); } }
        
        /// <summary>
        /// Get the items.
        /// </summary>
        public Collection<string> Items { get { return (items); } }

        /// <summary>
        /// Get the descriptor number.
        /// </summary>
        public int DescriptorNumber { get { return (descriptorNumber); } }

        /// <summary>
        /// Get the last descriptor number.
        /// </summary>
        public int LastDescriptorNumber { get { return (lastDescriptorNumber); } }
        
        /// <summary>
        /// Get the non itemised text.
        /// </summary>
        public string Text { get { return (text); } }

        /// <summary>
        /// Get the language code.
        /// </summary>
        public string LanguageCode { get { return (languageCode); } }

        /// <summary>
        /// Get the list of cast members.
        /// </summary>
        public Collection<string> Cast 
        { 
            get 
            {
                if (itemDescriptions == null || items == null)
                    return (null);

                int itemIndex = itemDescriptions.IndexOf("ACTORS");
                if (itemIndex == -1 || itemIndex >= items.Count)
                    return (null);

                Collection<string> cast = new Collection<string>();

                string[] castMembers = items[itemIndex].Split(new char[] { ',' });
                foreach (string castMember in castMembers)
                    cast.Add(castMember.Trim());

                return (cast); 
            } 
        }

        /// <summary>
        /// Get the list of directors.
        /// </summary>
        public Collection<string> Directors
        {
            get
            {
                if (itemDescriptions == null || items == null)
                    return (null);

                string directorString;

                if (itemDescriptions.Contains("DIRECTOR"))
                    directorString = "DIRECTOR";
                else
                {
                    if (itemDescriptions.Contains("DIRECTORS"))
                        directorString = "DIRECTORS";
                    else
                        return (null);
                }

                int itemIndex = itemDescriptions.IndexOf(directorString);
                if (itemIndex >= items.Count)
                    return(null);

                Collection<string> directors = new Collection<string>();

                string[] directorNames = items[itemIndex].Split(new char[] { ',' });
                foreach (string directorName in directorNames)
                    directors.Add(directorName.Trim());

                return (directors);
            }
        }

        /// <summary>
        /// Get the year of production.
        /// </summary>
        public string Year
        {
            get
            {
                if (itemDescriptions == null || items == null)
                    return (null);

                string yearString;

                if (itemDescriptions.Contains("YEAR"))
                    yearString = "YEAR";
                else
                {
                    if (itemDescriptions.Contains("PRODUCTION YEAR"))
                        yearString = "PRODUCTION YEAR";
                    else
                        return (null);
                }

                int itemIndex = itemDescriptions.IndexOf(yearString);
                if (itemIndex >= items.Count)
                    return(null);

                string item = items[itemIndex].Trim();
                if (item == string.Empty)
                    return (null);

                return (item);
            }
        }

        /// <summary>
        /// Get the star rating.
        /// </summary>
        public string StarRating
        {
            get
            {
                if (itemDescriptions == null || items == null)
                    return (null);

                int itemIndex = itemDescriptions.IndexOf("STAR");
                if (itemIndex == -1 || itemIndex >= items.Count)
                    return(null);

                string item = items[itemIndex].Trim();
                if (item == string.Empty)
                    return (null);

                return (item);
            }
        }

        /// <summary>
        /// Get the series ID.
        /// </summary>
        public string SeriesID
        {
            get
            {
                if (itemDescriptions == null || items == null)
                    return (null);

                int itemIndex = itemDescriptions.IndexOf("SERIESID");
                if (itemIndex == -1 || itemIndex >= items.Count)
                    return(null);

                string item = items[itemIndex].Trim();
                if (item == string.Empty)
                    return (null);

                return (item);
            }
        }

        /// <summary>
        /// Get the season ID.
        /// </summary>
        public string SeasonID
        {
            get
            {
                if (itemDescriptions == null || items == null)
                    return (null);

                int itemIndex = itemDescriptions.IndexOf("SEASONID");
                if (itemIndex == -1 || itemIndex >= items.Count)
                    return (null);

                string item = items[itemIndex].Trim();
                if (item == string.Empty)
                    return (null);

                return (item);
            }
        }

        /// <summary>
        /// Get the episode ID.
        /// </summary>
        public string EpisodeID
        {
            get
            {
                if (itemDescriptions == null || items == null)
                    return (null);

                int itemIndex = itemDescriptions.IndexOf("EPISODEID");
                if (itemIndex == -1 || itemIndex >= items.Count)
                    return (null);

                string item = items[itemIndex].Trim();
                if (item == string.Empty)
                    return (null);

                return (item);
            }
        }

        /// <summary>
        /// Get the TV rating.
        /// </summary>
        public string TVRating
        {
            get
            {
                if (itemDescriptions == null || items == null)
                    return (null);

                int itemIndex = itemDescriptions.IndexOf("TV RATINGS");
                if (itemIndex == -1 || itemIndex >= items.Count)
                    return (null);

                string item = items[itemIndex].Trim();
                if (item == string.Empty)
                    return (null);

                return (item);
            }
        }

        /// <summary>
        /// Get the offset back to the previous play date.
        /// </summary>
        public string PreviousPlayDate
        {
            get
            {
                if (itemDescriptions == null || items == null)
                    return (null);

                int itemIndex = itemDescriptions.IndexOf("PPD");
                if (itemIndex == -1 || itemIndex >= items.Count)
                    return (null);

                string item = items[itemIndex].Trim();
                if (item == string.Empty)
                    return (null);

                return (item);
            }
        }

        /// <summary>
        /// Get the country of origin.
        /// </summary>
        public string Country
        {
            get
            {
                if (itemDescriptions == null || items == null)
                    return (null);

                int itemIndex = itemDescriptions.IndexOf("COUNTRY");
                if (itemIndex == -1 || itemIndex >= items.Count)
                    return (null);

                string item = items[itemIndex].Trim();
                if (item == string.Empty)
                    return (null);

                return (item);
            }
        }

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
                    throw (new InvalidOperationException("ExtendedEventDescriptor: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int descriptorNumber;
        private int lastDescriptorNumber;
        private string languageCode;
        private Collection<string> itemDescriptions;
        private Collection<string> items;
        private string text;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the DVBExtendedEventDescriptor class.
        /// </summary>
        internal DVBExtendedEventDescriptor() { }

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
                descriptorNumber = (int)byteData[lastIndex] >> 4;
                lastDescriptorNumber = (int)byteData[lastIndex] & 0x0f;
                lastIndex++;

                languageCode = Utils.GetString(byteData, lastIndex, 3);
                lastIndex += 3;

                int totalItemLength = (int)byteData[lastIndex];
                lastIndex++;

                if (totalItemLength != 0)
                {
                    itemDescriptions = new Collection<string>();
                    items = new Collection<string>();

                    while (totalItemLength != 0)
                    {
                        int itemDescriptionLength = (int)byteData[lastIndex];
                        lastIndex++;

                        if (itemDescriptionLength != 0)
                        {
                            string itemDescription = Utils.GetString(byteData, lastIndex, itemDescriptionLength);
                            itemDescriptions.Add(itemDescription.ToUpper());
                            lastIndex += itemDescriptionLength;
                        }

                        int itemLength = (int)byteData[lastIndex];
                        lastIndex++;

                        if (itemLength != 0)
                        {
                            string item = Utils.GetString(byteData, lastIndex, itemLength);
                            items.Add(item);
                            lastIndex += itemLength;
                        }
                        else
                            items.Add("");

                        totalItemLength -= (itemDescriptionLength + itemLength + 2);
                    }
                }

                int textLength = (int)byteData[lastIndex];
                lastIndex++;

                if (textLength != 0)
                {
                    text = Utils.GetString(byteData, lastIndex, textLength);
                    lastIndex += textLength;
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The DVB Extended Event Descriptor message is short"));
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

            string text;
            if (Text != null)
                text = Text;
            else
                text = "?";

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DVB EXTENDED EVENT DESCRIPTOR: Descr no.: " + descriptorNumber +
                " Last descr no.: " + lastDescriptorNumber +
                " Lang code: " + languageCode +
                " Text: " + text);

            if (itemDescriptions != null)
            {
                foreach (string itemDescription in itemDescriptions)
                {
                    Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "    Item description: " + itemDescription +
                        " Value: " + items[itemDescriptions.IndexOf(itemDescription)]);
                }
            }
        }
    }
}
