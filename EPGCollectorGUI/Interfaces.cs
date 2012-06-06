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

using DomainObjects;

namespace EPGCentre
{
    /// <summary>
    /// The interface for an update control.
    /// </summary>
    public interface IUpdateControl
    {
        /// <summary>
        /// Get the general window heading for the data.
        /// </summary>
        string Heading { get; }

        /// <summary>
        /// Get the default directory for the data.
        /// </summary>
        string DefaultDirectory { get; }

        /// <summary>
        /// Get the default output name for the data.
        /// </summary>
        string DefaultFileName { get; }

        /// <summary>
        /// Get the save file dialog filter.
        /// </summary>
        string SaveFileFilter { get; }

        /// <summary>
        /// Get the save file title
        /// </summary>
        string SaveFileTitle { get; }

        /// <summary>
        /// Return the state of the data.
        /// </summary>
        DataState DataState { get; }

        /// <summary>
        /// Get the file suffix.
        /// </summary>
        string SaveFileSuffix { get; }
        
        /// <summary>
        /// Validate the data and prepare the data to be saved.
        /// </summary>
        /// <returns>True if the data can be saved; false otherwise.</returns>
        bool PrepareToSave();

        /// <summary>
        /// Save the data to the current file name.
        /// </summary>
        /// <returns>True if the data has been saved; false otherwise.</returns>
        bool Save();

        /// <summary>
        /// Save the data to a specified file name.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>True if the data has been saved; false otherwise.</returns>
        bool Save(string fileName);
    }

    /// <summary>
    /// The interface for a view control.
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// Locate a text string.
        /// </summary>
        void FindText();

        /// <summary>
        /// Filter the view.
        /// </summary>
        void FilterText();
    }
}
