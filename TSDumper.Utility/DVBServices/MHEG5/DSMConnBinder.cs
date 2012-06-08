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
    /// The class that describes a DSM connection binder.
    /// </summary>
    public class DSMConnBinder
    {
        /// <summary>
        /// Get the collection of taps.
        /// </summary>
        public Collection<BIOPTap> Taps { get { return (taps); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the connection binder.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The connection binder has not been processed.
        /// </exception>
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("DSMConnBinder: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private byte[] componentIDTag = new byte[1] { 0x3f };
        private int dataLength;
        private int tapsCount;
        private Collection<BIOPTap> taps;

        private int lastIndex = -1;

        private static byte[] tag = new byte[4] { 0x49, 0x53, 0x4f, 0x40 };

        /// <summary>
        /// Initialize a new instance of the DSMConnBinder class.
        /// </summary>
        public DSMConnBinder() { }

        /// <summary>
        /// Parse the connection binder.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the connection binder.</param>
        /// <param name="index">Index of the first byte of the connection binder in the MPEG2 section.</param>
        public void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                componentIDTag = Utils.GetBytes(byteData, lastIndex, 4);
                lastIndex += 4;

                dataLength = (int)byteData[lastIndex];
                lastIndex++;

                tapsCount = (int)byteData[lastIndex];
                lastIndex++;

                if (tapsCount != 0)
                {
                    taps = new Collection<BIOPTap>();

                    while (taps.Count != tapsCount)
                    {
                        BIOPTap tap = new BIOPTap();
                        tap.Process(byteData, lastIndex);
                        taps.Add(tap);

                        lastIndex = tap.Index;
                    }
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The DSM Conn Binder message is short"));
            }
        }

        /// <summary>
        /// Validate the connection binder fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A connection binder field is not valid.
        /// </exception>
        public void Validate()
        {
            if (!Utils.CompareBytes(componentIDTag, tag))
                throw (new ArgumentOutOfRangeException("The component ID tag is wrong"));
        }

        /// <summary>
        /// Log the connection binder fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DSM CONN BINDER: Component ID tag: " + Utils.ConvertToHex(componentIDTag) +
                " Data lth: " + dataLength +
                " Taps ct: " + tapsCount);

            if (taps != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (BIOPTap tap in taps)
                    tap.LogMessage();

                Logger.DecrementProtocolIndent();
            }            
        }
    }
}
