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
    /// The class that describes a tap selector.
    /// </summary>
    public class BIOPTapSelector
    {
        /// <summary>
        /// Get the selector type.
        /// </summary>
        public int SelectorType { get { return (selectorType); } }
        /// <summary>
        /// Get the transaction ID.
        /// </summary>
        public DSMCCTransactionID TransactionID { get { return (transactionID); } }
        /// <summary>
        /// Get the timeout.
        /// </summary>
        public int Timeout { get { return (timeout); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the tap selector.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The tap selector has not been processed.
        /// </exception>
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("BIOPTapSelector: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int selectorType;
        private DSMCCTransactionID transactionID;
        private int timeout;

        private int lastIndex = -1;

        /// <summary>
        /// Get the length of a tap selector.
        /// </summary>
        public const int TapSelectorLength = 10;

        // Selector type is 0x01 - 'message'
        // Transaction ID is the transaction ID of the DII message
        // Timeout is in microseconds and is the acquisition timeout for the DII message

        /// <summary>
        /// Initialize a new instance of the BIOPTapSelector class.
        /// </summary>
        public BIOPTapSelector() { }

        /// <summary>
        /// Parse the tap selector.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the tap selector.</param>
        /// <param name="index">Index of the first byte of the tap selector in the MPEG2 section.</param>
        public void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                selectorType = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                transactionID = new DSMCCTransactionID(Utils.Convert4BytesToInt(byteData, lastIndex));
                lastIndex += 4;

                timeout = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The BIOP Tap Selector message is short"));
            }
        }

        /// <summary>
        /// Validate the tap selector fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A tap selector field is not valid.
        /// </exception>
        private void Validate()
        {
            if (selectorType != 0x01)
                throw (new ArgumentOutOfRangeException("BIOPTapSelector: The selector type is not 0x01"));
        }

        /// <summary>
        /// Log the tap selector fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "BIOP TAP SELECTOR: Sel type: " + Utils.ConvertToHex(selectorType) +
                " Trans ID: " + Utils.ConvertToHex(transactionID.Value) +
                " Timeout: " + timeout);
        }
    }
}
