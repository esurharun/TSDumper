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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DVBServices
{
    /// <summary>
    /// The reply codes from the protocol collectors.
    /// </summary>
    public enum CollectorReply
    {
        /// <summary>
        /// The collection was successful.
        /// </summary>
        OK,
        /// <summary>
        /// The collection failed.
        /// </summary>
        GeneralFailure,
        /// <summary>
        /// There was a format error in the received data.
        /// </summary>
        FatalFormatError,
        /// <summary>
        /// The was an error loading the reference data.
        /// </summary>
        ReferenceDataError,
        /// <summary>
        /// There was an error in the broadcast data.
        /// </summary>
        BroadcastDataError,
        /// <summary>
        /// The collection was cancelled.
        /// </summary>
        Cancelled
    }

    /// <summary>
    /// The current scope
    /// </summary>
    public enum Scope
    {
        /// <summary>
        /// No scope restrictions
        /// </summary>
        All,
        /// <summary>
        /// Bouquet sections are being processed
        /// </summary>
        Bouquet,
        /// <summary>
        /// Service description sections are being processed
        /// </summary>
        ServiceDescripton
    }
}
