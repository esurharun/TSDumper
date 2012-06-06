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
    /// The class that describes a BIOP profile.
    /// </summary>
    public class BIOPProfileBase
    {
        /// <summary>
        /// Get the profile body.
        /// </summary>
        public BIOPProfileBody ProfileBody { get { return (profileBody); } }
        /// <summary>
        /// Get the Lite Options profile body.
        /// </summary>
        public BIOPLiteOptionsProfileBody LiteOptionsProfileBosy { get { return (liteOptionsProfileBody); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the profile.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The profile has not been processed.
        /// </exception>
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("BIOPProfileBase: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private byte[] profileIDTag = new byte[1] { 0x00 };
        private int dataLength;
        private int byteOrder;
        private BIOPProfileBody profileBody;
        private BIOPLiteOptionsProfileBody liteOptionsProfileBody;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the BIOPProfileBase class.
        /// </summary>
        public BIOPProfileBase() { }

        /// <summary>
        /// Parse the profile.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the profile.</param>
        /// <param name="index">Index of the first byte of the profile in the MPEG2 section.</param>
        public void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                profileIDTag = Utils.GetBytes(byteData, lastIndex, 4);
                lastIndex += 4;

                dataLength = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                byteOrder = (int)byteData[lastIndex];
                lastIndex++;

                if (Utils.CompareBytes(profileIDTag, BIOPProfileBody.Tag))
                {
                    profileBody = new BIOPProfileBody();
                    profileBody.Process(byteData, lastIndex);
                    lastIndex = profileBody.Index;
                }
                else
                {
                    if (Utils.CompareBytes(profileIDTag, BIOPLiteOptionsProfileBody.Tag))
                    {
                        liteOptionsProfileBody = new BIOPLiteOptionsProfileBody();
                        liteOptionsProfileBody.Process(byteData, lastIndex);
                        lastIndex = liteOptionsProfileBody.Index;
                    }
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The BIOP Profile message is short"));
            }
        }

        /// <summary>
        /// Validate the profile fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A profile field is not valid.
        /// </exception>
        public void Validate()
        {
            if (byteOrder != 0)
                throw (new ArgumentOutOfRangeException("The byte order is wrong"));
        }

        /// <summary>
        /// Log the profile fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "BIOP PROFILE BASE: Profile ID: " + Utils.ConvertToHex(profileIDTag) +
                " Data lth: " + dataLength +
                " Byte order: " + Utils.ConvertToHex(byteOrder));

            if (profileBody != null)
            {
                Logger.IncrementProtocolIndent();
                profileBody.LogMessage();
                Logger.DecrementProtocolIndent();
            }

            if (liteOptionsProfileBody != null)
            {
                Logger.IncrementProtocolIndent();
                liteOptionsProfileBody.LogMessage();
                Logger.DecrementProtocolIndent();
            }
        }
    }
}
