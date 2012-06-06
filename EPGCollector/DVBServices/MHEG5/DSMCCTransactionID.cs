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

namespace DVBServices
{
    /// <summary>
    /// The class that describes a DSMCC transaction ID.
    /// </summary>
    public class DSMCCTransactionID
    {
        /// <summary>
        /// Get the update flag.
        /// </summary>
        public bool UpdateFlag { get { return (updatedFlag); } }
        /// <summary>
        /// Get the transaction identification.
        /// </summary>
        public int Identification { get { return (identification); } }
        /// <summary>
        /// Get the version number.
        /// </summary>
        public int Version { get { return (version); } }
        /// <summary>
        /// Get the transaction ID assignment.
        /// </summary>
        public int Assignment { get { return (assignment); } }

        /// <summary>
        /// Get the entire transaction ID field.
        /// </summary>
        public int Value { get { return (value); } }

        private bool updatedFlag;
        private int identification;
        private int version;
        private int assignment;

        private int value;

        private DSMCCTransactionID() { }

        /// <summary>
        /// Initialize a new instance of the DSMCCTransactionID class.
        /// </summary>
        /// <param name="transactionID">The entire DSMCC transaction ID.</param>
        public DSMCCTransactionID(int transactionID)
        {
            value = transactionID;

            updatedFlag = (transactionID & 0x00000001) != 0;
            identification = (transactionID & 0xfffe) >> 1;
            version = (transactionID & 0x3fff0000) >> 16;

            switch (transactionID & 0xc0000000)
            {
                case 0x00000000:
                    assignment = 0;
                    break;
                case 0x40000000:
                    assignment = 1;
                    break;
                case 0x80000000:
                    assignment = 2;
                    break;
                default:
                    assignment = 3;
                    break;
            }
        }

        /// <summary>
        /// Validate the transaction ID.
        /// </summary>
        /// <returns></returns>
        public string Validate()
        {
            if (assignment != 0x02)
                return ("The assignment field is not 0x02");

            return (null);
        }
    }
}
