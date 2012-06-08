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
    /// The class that describes a Cos name component.
    /// </summary>
    public class CosNameComponent
    {
        /// <summary>
        /// Get the identity length.
        /// </summary>
        public int IdentityLength { get { return(identityLength); } }
        /// <summary>
        /// Get the identity.
        /// </summary>
        public byte[] Identity { get { return(identity); } }
        /// <summary>
        /// Get the kind length.
        /// </summary>
        public int KindLength { get { return(kindLength); } }
        /// <summary>
        /// Get the kind of component.
        /// </summary>
        public byte[] Kind { get { return(kind); } }
        /// <summary>
        /// Get the initial context length.
        /// </summary>
        public int InitialContextLength { get { return(initialContextLength); } }
        /// <summary>
        /// Get the initial context.
        /// </summary>
        public byte[] InitialContext { get { return(initialContext); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the name component.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The name component has not been processed.
        /// </exception>
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("CosNameComponent: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int identityLength;
        private byte[] identity = new byte[1] { 0x00 };
        private int kindLength;
        private byte[] kind = new byte[1] { 0x00 };
        private int initialContextLength;
        private byte[] initialContext = new byte[1] { 0x00 };

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the CosNameComponent class.
        /// </summary>
        public CosNameComponent() { }

        /// <summary>
        /// Parse the name component.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the name component.</param>
        /// <param name="index">Index of the first byte of the name component in the MPEG2 section.</param>
        public void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                identityLength = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                if (identityLength != 0)
                {
                    identity = Utils.GetBytes(byteData, lastIndex, identityLength);
                    lastIndex += identityLength;
                }

                kindLength = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                if (kindLength != 0)
                {
                    kind = Utils.GetBytes(byteData, lastIndex, kindLength);
                    lastIndex += kindLength;
                }

                initialContextLength = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                if (initialContextLength != 0)
                {
                    initialContext = Utils.GetBytes(byteData, lastIndex, initialContextLength);
                    lastIndex += initialContextLength;
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The COS Naming Component message is short"));
            }
        }

        /// <summary>
        /// Validate the name component fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A name component field is not valid.
        /// </exception>
        public void Validate() { }

        /// <summary>
        /// Log the name component fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "COS NAME COMPONENT: Identity lth: " + identityLength +
                " Identity: " + Utils.ConvertToHex(identity) +
                " Kind lth: " + kindLength +
                " Kind: " + Utils.ConvertToHex(kind) +
                " Init ctxt lth: " + initialContextLength +
                " Init ctxt: " + Utils.ConvertToHex(initialContext));
        }
    }
}
