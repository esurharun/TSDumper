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

using DomainObjects;

namespace DirectShow
{
    /// <summary>
    /// The base class for transport stream readers.
    /// </summary>
    public abstract class TSReaderBase : ProtectedResource 
    {
        /// <summary>
        /// Get the collection of sections stored by the reader.
        /// </summary>
        public abstract Collection<Mpeg2Section> Sections { get; }

        /// <summary>
        /// Get the number of discontinuities.
        /// </summary>
        public virtual int Discontinuities { get { return (0); } }

        /// <summary>
        /// Start the reader.
        /// </summary>
        public abstract void Run();
        /// <summary>
        /// Stop the reader.
        /// </summary>
        public abstract void Stop();        
    }
}
