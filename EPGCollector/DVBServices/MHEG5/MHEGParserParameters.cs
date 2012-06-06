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
using System.Collections.ObjectModel;
using System.IO;

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that processes the MHEG parser format file.
    /// </summary>
    /// <remarks>
    /// This class cannot be instantiated. All methods are static.
    /// </remarks>
    public sealed class MHEGParserParameters
    {
        /// <summary>
        /// The names of the fields extracted by the MHEG parser.
        /// </summary>
        public enum FieldName
        {
            /// <summary>
            /// The root CRID from the station header.
            /// </summary>
            RootCRID,
            /// <summary>
            /// The number of programs from the station header.
            /// </summary>
            ProgramCount,
            /// <summary>
            /// The ID of the event.
            /// </summary>
            EventID,
            /// <summary>
            /// The name of the event.
            /// </summary>
            EventName,
            /// <summary>
            /// The start time of the event.
            /// </summary>
            StartTime,
            /// <summary>
            /// The end time of the event.
            /// </summary>
            EndTime,
            /// <summary>
            /// The short description of the event.
            /// </summary>
            ShortDescription,
            /// <summary>
            /// The count of 'jpg' file names for the event. 
            /// </summary>
            ImageCount,
            /// <summary>
            /// The CRID of the series containing the event.
            /// </summary>
            SeriesCRID,
            /// <summary>
            /// The CRID of the event.
            /// </summary>
            ProgramCRID,
            /// <summary>
            /// The parental rating of the event.
            /// </summary>
            ParentalRating,
            /// <summary>
            /// The picture quality of the event.
            /// </summary>
            HighDefinition,
            /// <summary>
            /// The closed captions status of the event.
            /// </summary>
            ClosedCaptions
        }

        /// <summary>
        /// Get the maximum number of fixed fields the parser should expect in the header of each EPG record.
        /// </summary>
        public static int HeaderFields { get { return (headerFields); } }

        /// <summary>
        /// Get the maximum number of fixed fields the parser should expect in the detail of each EPG record.
        /// </summary>
        /// <remarks>
        /// Note that this is a base number because there are a variable number of fields that
        /// can be received in an MHEG EPG record.
        /// </remarks>
        public static int DetailFields { get { return (detailFields); } }

        private static int headerFields = -1;
        private static int detailFields = -1;
        private static Collection<ParserParameter> parserParameters;

        /// <summary>
        /// Initialize a new instance of the MHEGParserParameters class with a specified parameter file name.
        /// </summary>
        /// <param name="fileName">The full name of the parameter file.</param>
        /// <returns>True if the contents of the parameter file are valid; false otherwise.</returns>
        /// <remarks>
        /// Note that at present errors in the parameter parameters cause the application to exit so a
        /// reply of false is not applicable.
        /// </remarks>
        public static bool Process(string fileName)
        {
            FileStream fileStream = null;

            string actualName = Path.Combine(RunParameters.ConfigDirectory, fileName);
            Logger.Instance.Write("Loading MHEG5 Parser Parameters from " + actualName);

            try { fileStream = new FileStream(actualName, FileMode.Open, FileAccess.Read); }
            catch (IOException)
            {
                Logger.Instance.Write("Failed to open " + actualName + " - assuming default MHEG parser format");
                return (true);
            }

            StreamReader streamReader = new StreamReader(fileStream);

            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();

                string[] parts = line.Split(new char[] { ':' });
                if (parts.Length > 0)
                {
                    switch (parts[0])
                    {
                        case "Fields":
                            Logger.Instance.Write("Processing MHEG parser format parameter: " + line);
                            processFields(parts[1]);
                            break;
                        case "Field":
                            Logger.Instance.Write("Processing MHEG parser format parameter: " + line);
                            processField(parts[1]);
                            break;
                        default:
                            break;
                    }
                }
            }

            return (true);
        }

        private static bool processFields(string parts)
        {
            string[] parameters = parts.Split(new char[] { ',' });
            if (parameters.Length != 2)
            {
                Logger.Instance.Write("MHEG Parser parameters file format error: The Fields line is wrong.");
                System.Environment.Exit(3);
            }

            try
            {
                headerFields = Int32.Parse(parameters[0].Trim());
                detailFields = Int32.Parse(parameters[1].Trim()); 
            }
            catch (FormatException)
            {
                Logger.Instance.Write("MHEG Parser parameters file format error: The Fields line is wrong.");
                System.Environment.Exit(3);
            }
            catch (ArithmeticException)
            {
                Logger.Instance.Write("MHEG Parser parameters file format error: The Fields line is wrong.");
                System.Environment.Exit(3);
            }

            return (true);
        }

        private static bool processField(string parts)
        {
            string[] parameters = parts.Split(new char[] { ',' });
            if (parameters.Length != 2)
            {
                Logger.Instance.Write("MHEG Parser parameters file format error: A Field line is wrong.");
                System.Environment.Exit(3);
            }

            try
            {
                ParserParameter parserParameter = new ParserParameter((FieldName)Enum.Parse(typeof(FieldName), parameters[0], true), Int32.Parse(parameters[1].Trim()));
                if (parserParameters == null)
                    parserParameters = new Collection<ParserParameter>();
                parserParameters.Add(parserParameter);
            }
            catch (FormatException)
            {
                Logger.Instance.Write("MHEG Parser parameters file format error: A Field line is wrong.");
                System.Environment.Exit(3);
            }
            catch (ArithmeticException)
            {
                Logger.Instance.Write("MHEG Parser parameters file format error: A Field line is wrong.");
                System.Environment.Exit(3);
            }

            return (true);
        }

        /// <summary>
        /// Get the number of a field.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        /// <returns>The number of the field or -1 if the field name is undefined.</returns>
        public static int GetField(FieldName fieldName)
        {
            if (parserParameters == null)
                return (-1);

            foreach (ParserParameter parserParameter in parserParameters)
            {
                if (parserParameter.FieldName == fieldName)
                    return (parserParameter.FieldNumber);
            }

            return (-1);
        }
    }

    internal class ParserParameter
    {
        internal MHEGParserParameters.FieldName FieldName { get { return (fieldName); } }
        internal int FieldNumber { get { return (fieldNumber); } }
        
        private MHEGParserParameters.FieldName fieldName;
        private int fieldNumber;

        private ParserParameter() { }

        public ParserParameter(MHEGParserParameters.FieldName fieldName, int fieldNumber)
        {
            this.fieldName = fieldName;
            this.fieldNumber = fieldNumber;
        }
    }
}
