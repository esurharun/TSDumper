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
    /// The class that describes a BIOP Tap.
    /// </summary>
    public class BIOPTap
    {
        /// <summary>
        /// Get the identity of the tap.
        /// </summary>
        public int Identity { get { return (identity); } }
        /// <summary>
        /// Get the use of the tap.
        /// </summary>
        public int Use { get { return (use); } }
        /// <summary>
        /// Get the association tag.
        /// </summary>
        public int AssociationTag { get { return (associationTag); } }
        /// <summary>
        /// Get the selector length.
        /// </summary>
        public int SelectorLength { get { return (selectorLength); } }
        /// <summary>
        /// Get the selector.
        /// </summary>
        public BIOPTapSelector Selector { get { return (tapSelector); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the tap.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The tap has not been processed.
        /// </exception>
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("BIOPTap: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int identity;
        private int use;
        private int associationTag;
        private int selectorLength;
        private byte[] selectorData = new byte[1] { 0x00 };
        private BIOPTapSelector tapSelector;

        private int lastIndex = -1;

        private const int tapIDUserPrivate = 0;

        private const int tapUseNPT = 0x0b;
        private const int tapUseStreamStatusAndEvent = 0x0c;
        private const int tapUseStreamEvent = 0x0d;
        private const int tapUseStreamStatus = 0x0e;
        private const int tapUseBIOPDeliveryPara = 0x16;
        private const int tapUseBIOPObject = 0x17;
        private const int tapUseBIOPES = 0x18;
        private const int tapUseBIOPProgram = 0x19;

        // Only DeliveryPara and Object will occur. DeliveryPara will be first if more than one.

        /// <summary>
        /// Initialize a new instance of the BIOPTap class.
        /// </summary>
        public BIOPTap() { }

        /// <summary>
        /// Parse the tap.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the tap.</param>
        /// <param name="index">Index of the first byte of the tap in the MPEG2 section.</param>
        public void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                identity = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                use = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                associationTag = Utils.Convert2BytesToInt(byteData, lastIndex);
                lastIndex += 2;

                selectorLength = (int)byteData[lastIndex];
                lastIndex++;

                if (selectorLength != 0)
                    selectorData = Utils.GetBytes(byteData, lastIndex, selectorLength);

                if (selectorLength == BIOPTapSelector.TapSelectorLength)
                {
                    tapSelector = new BIOPTapSelector();
                    tapSelector.Process(byteData, lastIndex);
                }

                lastIndex += selectorLength;

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The BIOP Tap message is short"));
            }
        }

        /// <summary>
        /// Validate the tap fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A tap field is not valid.
        /// </exception>
        public void Validate() { }

        /// <summary>
        /// Log the tap fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "BIOP TAP: Identity: " + Utils.ConvertToHex(identity) +
                " Use: " + Utils.ConvertToHex(use) +
                " Assoc tag: " + Utils.ConvertToHex(associationTag) +
                " Selector lth: " + selectorLength + 
                " Selector data: " + Utils.ConvertToHex(selectorData));

            if (tapSelector != null)
            {
                Logger.IncrementProtocolIndent();
                tapSelector.LogMessage();
                Logger.DecrementProtocolIndent();
            }
        }
    }
}
