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
using System.Text;

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// DVB Content descriptor class.
    /// </summary>
    internal class DVBContentDescriptor : DescriptorBase
    {
        /// <summary>
        /// Get the content types.
        /// </summary>
        public Collection<int> ContentType { get { return (contentTypes); } }
        
        /// <summary>
        /// Get the content subtypes.
        /// </summary>
        public Collection<int> ContentSubType { get { return (contentSubTypes); } }

        /// <summary>
        /// Get the user types.
        /// </summary>
        public Collection<int> UserType { get { return (userTypes); } }

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
                    throw (new InvalidOperationException("ContentDescriptor: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private Collection<int> contentTypes;
        private Collection<int> contentSubTypes;
        private Collection<int> userTypes;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the DVBContentDescriptor class.
        /// </summary>
        internal DVBContentDescriptor() { }

        /// <summary>
        /// Parse the descriptor.
        /// </summary>
        /// <param name="byteData">The mpeg2 section containing the descriptor.</param>
        /// <param name="index">Index of the byte in the mpeg2 section following the descriptor length.</param>
        internal override void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                if (Length != 0)
                {
                    int dataLength = Length;

                    contentTypes = new Collection<int>();
                    contentSubTypes = new Collection<int>();
                    userTypes = new Collection<int>();

                    while (dataLength != 0)
                    {
                        int contentType = (int)(byteData[lastIndex] >> 4);
                        contentTypes.Add(contentType);
                        int contentSubType = (int)(byteData[lastIndex] & 0x0f);
                        contentSubTypes.Add(contentSubType);
                        lastIndex++;

                        userTypes.Add((int)byteData[lastIndex]);
                        lastIndex++;

                        dataLength -= 2;                        
                    }
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The DVB Content Descriptor message is short"));
            }
        }

        /// <summary>
        /// Validate the descriptor fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A descriptor field is not valid.
        /// </exception>
        internal override void Validate() 
        {
            if (contentTypes == null || contentTypes.Count == 0)
                throw (new ArgumentOutOfRangeException("There are no content types in the Content descriptor"));
        }

        /// <summary>
        /// Log the descriptor fields.
        /// </summary>
        internal override void LogMessage() 
        {
            if (Logger.ProtocolLogger == null)
                return;

            StringBuilder contentTypeString = new StringBuilder();

            foreach (int contentType in contentTypes)
            {
                if (contentTypeString.Length != 0)
                    contentTypeString.Append(", ");
                contentTypeString.Append(contentType.ToString());
            }

            StringBuilder contentSubTypeString = new StringBuilder();

            foreach (int contentSubType in contentSubTypes)
            {
                if (contentSubTypeString.Length != 0)
                    contentSubTypeString.Append(", ");
                contentSubTypeString.Append(contentSubType.ToString());
            }

            StringBuilder userTypeString = new StringBuilder();

            foreach (int userType in userTypes)
            {
                if (userTypeString.Length != 0)
                    userTypeString.Append(", ");
                userTypeString.Append(userType.ToString());
            }

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "DVB CONTENT DESCRIPTOR: Content types: " + contentTypeString +
                " Content subtypes: " + contentSubTypeString +
                " User types: " + userTypeString);
        }
    }
}
