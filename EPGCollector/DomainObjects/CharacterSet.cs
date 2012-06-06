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

using System.Collections.ObjectModel;
using System.Text;

namespace DomainObjects
{
    /// <summary>
    ///  The class that describes a character set.
    /// </summary>
    public class CharacterSet
    {
        /// <summary>
        /// Get the collection of character sets.
        /// </summary>
        public static Collection<CharacterSet> CharacterSets
        {
            get
            {
                Collection<CharacterSet> characterSets = new Collection<CharacterSet>();

                foreach (EncodingInfo encodingInfo in ASCIIEncoding.GetEncodings())
                {
                    if (RunParameters.Instance.DebugIDs.Contains("LOGCODEPAGES"))
                    {
                        Logger.Instance.Write("Codepage " + encodingInfo.CodePage + " " +
                            encodingInfo.DisplayName + " " +
                            encodingInfo.Name);
                    }
                    characterSets.Add(new CharacterSet(encodingInfo.Name, encodingInfo.DisplayName));
                }

                characterSets.Insert(0, new CharacterSet(string.Empty, " -- Undefined --"));
                
                return (characterSets);
            }
        }

        /// <summary>
        /// Get the character set name.
        /// </summary>
        public string Name { get { return (name); } }
        /// <summary>
        /// Get the character set description.
        /// </summary>
        public string Description { get { return (description); } }

        private string name;
        private string description;

        private CharacterSet() { }

        /// <summary>
        /// Initialize a new instance of the CharacterSet class.
        /// </summary>
        /// <param name="name">The character set name.</param>
        /// <param name="description">The character set description.</param>
        public CharacterSet(string name, string description)
        {
            this.name = name;
            this.description = description;
        }

        /// <summary>
        /// Find a character set.
        /// </summary>
        /// <param name="name">The name of the character set.</param>
        /// <returns>The character set or null if it cannot be found.</returns>
        public static CharacterSet FindCharacterSet(string name)
        {
            foreach (CharacterSet characterSet in CharacterSets)
            {
                if (characterSet.Name == name)
                    return (characterSet);
            }

            return (null);
        }

        /// <summary>
        /// Get a description of this instance.
        /// </summary>
        /// <returns>A string describing this instance.</returns>
        public override string ToString()
        {
            return (description);
        }
    }
}
