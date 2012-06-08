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

namespace DomainObjects
{
    /// <summary>
    /// The class that describes a log record.
    /// </summary>
    public class LogRecord
    {
        /// <summary>
        /// Get the timestamp for a record.
        /// </summary>
        public DateTime Time { get { return (time); } }
        /// <summary>
        /// Get the text of the record.
        /// </summary>
        public string Text { get { return (text); } }

        private DateTime time;
        private string text;

        private LogRecord() { }

        /// <summary>
        /// Intialize a new instance of the LogRecord class.
        /// </summary>
        /// <param name="logLine">The log record it describes.</param>
        public LogRecord(string logLine)
        {
            if (logLine.Length < 14)
            {
                text = logLine;
                return;
            }

            try
            {
                time = new DateTime(1, 1, 1,
                    Int32.Parse(logLine.Substring(0, 2)),
                    Int32.Parse(logLine.Substring(3, 2)),
                    Int32.Parse(logLine.Substring(6, 2)),
                    Int32.Parse(logLine.Substring(9, 3)));

                text = logLine.Substring(13);
            }
            catch (FormatException)
            {
                text = logLine;
            }
        }
    }
}
