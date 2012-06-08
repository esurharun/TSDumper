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
    /// The class that describes a Cos naming name.
    /// </summary>
    public class CosNamingName
    {
        /// <summary>
        /// Get the collection of name components.
        /// </summary>
        public Collection<CosNameComponent> NameComponents { get { return (nameComponents); } }

        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the naming name.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The naming name has not been processed.
        /// </exception>
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("CosNamingName: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private int nameComponentsCount;
        private Collection<CosNameComponent> nameComponents;

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the CosNamingName class.
        /// </summary>
        public CosNamingName() { }

        /// <summary>
        /// Parse the naming name.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the naming name.</param>
        /// <param name="index">Index of the first byte of the naming name.</param>
        public void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                nameComponentsCount = Utils.Convert4BytesToInt(byteData, lastIndex);
                lastIndex += 4;

                if (nameComponentsCount != 0)
                {
                    nameComponents = new Collection<CosNameComponent>();

                    while (nameComponents.Count != nameComponentsCount)
                    {
                        CosNameComponent nameComponent = new CosNameComponent();
                        nameComponent.Process(byteData, lastIndex);
                        nameComponents.Add(nameComponent);

                        lastIndex = nameComponent.Index;
                    }
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The COS Naming Component message is short"));
            }
        }

        /// <summary>
        /// Validate the naming name fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A naming name field is not valid.
        /// </exception>
        public void Validate() { }

        /// <summary>
        /// Log the naming name fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;
            
            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "COS NAMING NAME: Name component ct: " + nameComponentsCount);

            if (nameComponents != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (CosNameComponent nameComponent in nameComponents)
                    nameComponent.LogMessage();

                Logger.DecrementProtocolIndent();
            }

            
        }
    }
}
