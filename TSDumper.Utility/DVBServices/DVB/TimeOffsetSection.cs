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
    /// The class that describes a Time Offset section.
    /// </summary>
    public class TimeOffsetSection
    {
        internal Collection<DescriptorBase> Descriptors { get { return (descriptors); } }

        private Collection<DescriptorBase> descriptors;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the TimeOffsetSection class.
        /// </summary>
        internal TimeOffsetSection() { }

        /// <summary>
        /// Parse the section.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the section.</param>
        /// <param name="mpeg2Header">The MPEG2 header that preceedes the section.</param>
        internal void Process(byte[] byteData, Mpeg2ExtendedHeader mpeg2Header)
        {
            lastIndex = mpeg2Header.Index;

            int descriptorLength = ((byteData[lastIndex] & 0x0f) * 256) + byteData[lastIndex + 1];
            lastIndex += 2;

            if (descriptorLength != 0)
            {
                descriptors = new Collection<DescriptorBase>();

                while (descriptorLength > 0)
                {
                    DescriptorBase descriptor = DescriptorBase.Instance(byteData, lastIndex);

                    if (!descriptor.IsEmpty)
                    {
                        descriptors.Add(descriptor);

                        lastIndex = descriptor.Index;
                        descriptorLength -= descriptor.TotalLength;
                    }
                    else
                    {
                        lastIndex += DescriptorBase.MinimumDescriptorLength;
                        descriptorLength -= DescriptorBase.MinimumDescriptorLength;
                    }
                }
            }
        }

        private DateTime getCurrentDateTime(byte[] byteData, int index)
        {
            int startDate = Utils.Convert2BytesToInt(byteData, index);

            int year = (int)((startDate - 15078.2) / 365.25);
            int month = (int)(((startDate - 14956.1) - (int)(year * 365.25)) / 30.6001);
            int day = (startDate - 14956) - (int)(year * 365.25) - (int)(month * 30.6001);

            int adjust;

            if (month == 14 || month == 15)
                adjust = 1;
            else
                adjust = 0;

            year = year + 1900 + adjust;
            month = month - 1 - (adjust * 12);

            int hour1 = (int)byteData[index + 2] >> 4;
            int hour2 = (int)byteData[index + 2] & 0x0f;
            int hour = (hour1 * 10) + hour2;

            int minute1 = (int)byteData[index + 3] >> 4;
            int minute2 = (int)byteData[index + 3] & 0x0f;
            int minute = (minute1 * 10) + minute2;

            int second1 = (int)byteData[index + 4] >> 4;
            int second2 = (int)byteData[index + 4] & 0x0f;
            int second = (second1 * 10) + second2;

            try
            {
                DateTime utcStartTime = new DateTime(year, month, day, hour, minute, second);
                return (utcStartTime.ToLocalTime());
            }
            catch (ArgumentOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The start time element(s) are out of range"));
            }
            catch (ArgumentException)
            {
                throw (new ArgumentOutOfRangeException("The start time element(s) result in a start time that is out of range"));
            }
        }

        /// <summary>
        /// Process an MPEG2 section from the time offset table.
        /// </summary>
        /// <param name="byteData">The MPEG2 section.</param>
        /// <returns>A TimeOffsetSection instance.</returns>
        public static TimeOffsetSection ProcessTimeOffsetTable(byte[] byteData)
        {
            Mpeg2ExtendedHeader mpeg2Header = new Mpeg2ExtendedHeader();

            try
            {
                mpeg2Header.Process(byteData);

                TimeOffsetSection timeOffsetSection = new TimeOffsetSection();
                timeOffsetSection.Process(byteData, mpeg2Header);
                return (timeOffsetSection);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Logger.Instance.Write("<e> Error processing Time Offset Section message: " + e.Message);
            }

            return (null);
        }
    }
}
