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
    /// Bell TV Extended Event descriptor class.
    /// </summary>
    internal class BellTVExtendedEventDescriptor : DescriptorBase
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
        /// Get the language code.
        /// </summary>
        public string LanguageCode { get { return (languageCode); } }
        /// <summary>
        /// Get the closed captions flag.
        /// </summary>
        public bool ClosedCaptions { get { return (closedCaptions); } }        
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
                    throw (new InvalidOperationException("BellTVExtendedEventDescriptor: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int descriptorNumber;
        private string languageCode;
        private int itemLength;
        private string originalDescription;
        private string eventDescription;
        private string subTitle;
        private bool closedCaptions;
        private bool stereo;
        private string date;
        private Collection<string> cast;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the BellTVExtendedEventDescriptor class.
        /// </summary>
        internal BellTVExtendedEventDescriptor() { }

        /// <summary>
        /// Parse the descriptor.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the descriptor.</param>
        /// <param name="index">Index of the byte in the MPEG2 section following the descriptor length.</param>
        internal override void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            descriptorNumber = (int)byteData[lastIndex];
            lastIndex++;

            languageCode = Utils.GetString(byteData, lastIndex, 3);
            lastIndex += 3;

            itemLength = (int)byteData[lastIndex];
            lastIndex++;

            int textLength = (int)byteData[lastIndex];
            lastIndex++;

            if (textLength != 0)
            {
                originalDescription = Utils.GetString(byteData, lastIndex, textLength);
                processEventDescription(originalDescription);
            }            

            lastIndex = index + Length;

            Validate();
        }

        private void processEventDescription(string originalDescription)
        {
            if (originalDescription.Contains("Girls Next Door"))
            {
                int ct = 0;
                ct++;
            }

            eventDescription = originalDescription;

            int closedCaptionsIndex = eventDescription.IndexOf(" (CC)");
            closedCaptions = closedCaptionsIndex != -1;
            if (closedCaptions)
                eventDescription = eventDescription = eventDescription.Remove(closedCaptionsIndex, 5);            

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

            extractCategory();

            int newIndex = eventDescription.IndexOf(" New.");
            if (newIndex != -1)
                eventDescription = eventDescription.Remove(newIndex, 5);

            eventDescription = eventDescription.Trim();
            if (eventDescription.Length != 0 && !eventDescription.EndsWith(".") && !eventDescription.EndsWith(".'"))
                eventDescription = eventDescription + ".";            

            string[] castParts = eventDescription.Split(new char[] { '.' });

            if (castParts.Length > 2)
            {
                cast = getCast(castParts[0]);
                eventDescription = eventDescription.Remove(0, castParts[0].Length + 1).Trim();
            }
        }

        private void extractCategory()
        {
            int categoryIndex = -1;

            categoryIndex = eventDescription.IndexOf("Movie.");
            if (categoryIndex != -1)
            {
                processCategory(categoryIndex, 6);
                return;
            }
            
            categoryIndex = eventDescription.IndexOf("Sports.");
            if (categoryIndex != -1)
            {
                processCategory(categoryIndex, 7);
                return;
            }
                
            categoryIndex = eventDescription.IndexOf("News.");
            if (categoryIndex != -1)
            {
                processCategory(categoryIndex, 5);
                return;
            }
             
            categoryIndex = eventDescription.IndexOf("Children.");
            if (categoryIndex != -1)
            {
                processCategory(categoryIndex, 9);
                return;
            }
                        
            categoryIndex = eventDescription.IndexOf("Education.");
            if (categoryIndex != -1)
            {
                processCategory(categoryIndex, 10);
                return;
            }
            
            categoryIndex = eventDescription.IndexOf("Series.");
            if (categoryIndex != -1)
            {
                processCategory(categoryIndex, 7);
                return;
            }
            
            categoryIndex = eventDescription.IndexOf("Music.");
            if (categoryIndex != -1)
            {
                processCategory(categoryIndex, 6);
                return;
            }
            
            categoryIndex = eventDescription.IndexOf("Religious.");
            if (categoryIndex != -1)
            {
                processCategory(categoryIndex, 10);
                return;              
            }

            categoryIndex = eventDescription.IndexOf("Enfants.");
            if (categoryIndex != -1)
            {
                processCategory(categoryIndex, 8);
                return;
            }

            categoryIndex = eventDescription.IndexOf("Film.");
            if (categoryIndex != -1)
            {
                processCategory(categoryIndex, 5);
                return;
            }

            categoryIndex = eventDescription.IndexOf("Séries.");
            if (categoryIndex != -1)
            {
                processCategory(categoryIndex, 7);
                return;
            }

            categoryIndex = eventDescription.IndexOf("-.");
            if (categoryIndex != -1)
            {
                processCategory(categoryIndex, 2);
                return;
            }
        }

        private void processCategory(int index, int length)
        {
            if (index == 0)
            {
                eventDescription = eventDescription.Remove(0, length).Trim();
            }
            else
            {
                eventDescription = eventDescription.Remove(index, length);
                subTitle = eventDescription.Substring(0, index - 1).Trim();
                eventDescription = eventDescription.Remove(0, index).Trim();
            }
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

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "BELL TV EXTENDED EVENT DESCRIPTOR: Descriptor no: " + descriptorNumber + 
                " Language code: " + languageCode +
                " Item lth: " + itemLength +
                " Description: " + originalDescription);
        }
    }
}
