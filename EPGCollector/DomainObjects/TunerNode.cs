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

namespace DomainObjects
{
    /// <summary>
    /// The class that describes a tuner node.
    /// </summary>
    public class TunerNode
    {
        /// <summary>
        /// Get the native node type.
        /// </summary>
        public int NativeNodeType { get { return(nativeNodeType); } }
        /// <summary>
        /// Get the tuner node type.
        /// </summary>
        public TunerNodeType TunerNodeType { get { return(tunerNodeType); } }

        private int nativeNodeType;
        private TunerNodeType tunerNodeType;

        private TunerNode() { }

        /// <summary>
        /// Intialize a new instance of the TunerNode class.
        /// </summary>
        /// <param name="nativeNodeType">The native node type.</param>
        /// <param name="tunerNodeType">The tuner node type.</param>
        public TunerNode(int nativeNodeType, TunerNodeType tunerNodeType)
        {
            this.nativeNodeType = nativeNodeType;
            this.tunerNodeType = tunerNodeType;
        }
    }
}
