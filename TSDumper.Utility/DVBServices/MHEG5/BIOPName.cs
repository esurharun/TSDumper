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
    /// The class that describes a BIOP name.
    /// </summary>
    public class BIOPName
    {
        /// <summary>
        /// Get the identify.
        /// </summary>
        public string Identity { get { return(identity); } }
        /// <summary>
        /// Get the kind of name.
        /// </summary>
        public string Kind { get { return(kind); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the name.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The name has not been processed.
        /// </exception> 
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("BIOPName: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private string identity = "?";
        private string kind = "?";

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the BIOPName class.
        /// </summary>
        public BIOPName() { }

        /// <summary>
        /// Parse the name.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the name.</param>
        /// <param name="index">Index of the first byte of the name in the MPEG2 section.</param>
        public void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                int identityLength = (int)byteData[lastIndex];
                lastIndex++;

                if (identityLength != 0)
                {
                    identity = Utils.GetString(byteData, lastIndex, identityLength);
                    lastIndex += identityLength;
                }

                int kindLength = (int)byteData[lastIndex];
                lastIndex++;

                if (kindLength != 0)
                {
                    kind = Utils.GetString(byteData, lastIndex, kindLength);
                    lastIndex += kindLength;
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The BIOP Name message is short"));
            }
        }

        /// <summary>
        /// Validate the name.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The name information is not valid.
        /// </exception>
        public void Validate()
        {
            if (identity == null)
                throw (new ArgumentOutOfRangeException("The component name is missing"));

            if (kind == null)
                throw (new ArgumentOutOfRangeException("The kind of name is missing"));
        }

        /// <summary>
        /// Log the name fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "BIOP NAME: Identity: " + identity +
                " Kind: " + kind);
        }
    }
}
