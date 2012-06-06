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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Reflection;

namespace DomainObjects
{
    /// <summary>
    /// The class that controls creation of the output file.
    /// </summary>
    public sealed class OutputFile
    {
        /// <summary>
        /// Get or set the flag that forces the output to use Unicode encoding. The default is UTF-8.
        /// </summary>
        public static bool UseUnicodeEncoding
        {
            get { return (useUnicodeEncoding); }
            set { useUnicodeEncoding = value; }
        }

        private static bool useUnicodeEncoding;
        
        private OutputFile() { }

        /// <summary>
        /// Create the file.
        /// </summary>
        /// <param name="fileName">The name of the file to be created.</param>
        /// <returns></returns>
        public static string Process(string fileName)
        {
            if (RunParameters.Instance.Options.Contains("WMCIMPORT"))
                return (OutputFileMXF.Process());

            if (RunParameters.Instance.Options.Contains("DVBVIEWERIMPORT") || RunParameters.Instance.Options.Contains("DVBVIEWERRECSVCIMPORT"))
                return (OutputFileDVBViewer.Process());
            
            return (OutputFileXML.Process(fileName));                       
        }
    }
}
