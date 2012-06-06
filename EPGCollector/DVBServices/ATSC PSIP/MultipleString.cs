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
using System.Text;

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that describes an ATSC PSIP multiple string field.
    /// </summary>
    public class MultipleString
    {
        /// <summary>
        /// Get the flag that indicates Unicode encoding of output is necessary.
        /// </summary>
        internal static bool UseUnicodeEncoding { get { return (useUnicodeEncoding); } }

        /// <summary>
        /// Get the collection of strings.
        /// </summary>
        internal Collection<SingleString> Strings { get { return (strings); } }
        
        /// <summary>
        /// Get the index of the next byte in the MPEG2 section following the transport stream.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The transport stream has not been processed.
        /// </exception> 
        public int Index
        {
            get
            {
                if (lastIndex == -1)
                    throw (new InvalidOperationException("MultipleString: Index requested before block processed"));
                return (lastIndex);
            }
        }

        private static bool useUnicodeEncoding;

        private Collection<SingleString> strings;        

        private int lastIndex = -1;

        /// <summary>
        /// Initialize a new instance of the MultipleString class.
        /// </summary>
        public MultipleString() { }

        /// <summary>
        /// Parse the entry.
        /// </summary>
        /// <param name="byteData">The MPEG2 section containing the transport stream.</param>
        /// <param name="index">Index of the first byte of the transport stream in the MPEG2 section.</param>
        internal void Process(byte[] byteData, int index)
        {
            lastIndex = index;

            try
            {
                int numberOfStrings = (int)byteData[lastIndex];
                lastIndex++;

                if (numberOfStrings != 0)
                {
                    strings = new Collection<SingleString>();

                    while (numberOfStrings != 0)
                    {
                        SingleString singleString = new SingleString();
                        singleString.Process(byteData, lastIndex);

                        strings.Add(singleString);
                        lastIndex = singleString.Index;

                        numberOfStrings--;
                    }
                }

                Validate();
            }
            catch (IndexOutOfRangeException)
            {
                throw (new ArgumentOutOfRangeException("The Multiple String message is short"));
            }
        }

        /// <summary>
        /// Validate the fields.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A field is not valid.
        /// </exception>
        public void Validate() { }

        /// <summary>
        /// Log the fields.
        /// </summary>
        public void LogMessage()
        {
            if (Logger.ProtocolLogger == null)
                return;

            Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "MULTIPLE STRING");

            if (strings != null)
            {
                Logger.IncrementProtocolIndent();

                foreach (SingleString singleString in strings)
                    singleString.LogMessage();

                Logger.DecrementProtocolIndent();
            }
        }

        /// <summary>
        /// Get the text held by this instance.
        /// </summary>
        /// <returns>The decoded text.</returns>
        public override string ToString()
        {
            if (strings == null || strings.Count == 0)
                return (string.Empty);

            StringBuilder totalString = new StringBuilder();

            foreach (SingleString singleString in strings)
                totalString.Append(singleString.ToString());

            return (totalString.ToString());
        }

        internal class SingleString
        {
            internal string LanguageCode { get { return (languageCode); } }
            internal Collection<SingleSegment> Strings { get { return (segments); } }            

            internal int Index
            {
                get
                {
                    if (lastIndex == -1)
                        throw (new InvalidOperationException("SingleString: Index requested before block processed"));
                    return (lastIndex);
                }
            }

            private string languageCode;
            private Collection<SingleSegment> segments;
            
            private int lastIndex = -1;

            internal SingleString() { }

            internal void Process(byte[] byteData, int index)
            {
                lastIndex = index;

                try
                {
                    languageCode = Utils.GetString(byteData, lastIndex, 3);
                    lastIndex += 3;

                    int numberOfSegments = (int)byteData[lastIndex];
                    lastIndex++;

                    if (numberOfSegments != 0)
                    {
                        segments = new Collection<SingleSegment>();

                        while (numberOfSegments != 0)
                        {
                            SingleSegment singleSegment = new SingleSegment();
                            singleSegment.Process(byteData, lastIndex);

                            segments.Add(singleSegment);
                            lastIndex = singleSegment.Index;

                            numberOfSegments--;
                        }
                    }

                    Validate();
                }
                catch (IndexOutOfRangeException)
                {
                    throw (new ArgumentOutOfRangeException("The Single String message is short"));
                }
            }

            internal void Validate() { }

            internal void LogMessage()
            {
                if (Logger.ProtocolLogger == null)
                    return;

                Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "SINGLE STRING: Language code: " + languageCode);

                if (segments != null)
                {
                    Logger.IncrementProtocolIndent();

                    foreach (SingleSegment singleSegment in segments)
                        singleSegment.LogMessage();

                    Logger.DecrementProtocolIndent();
                }
            }

            public override string ToString()
            {
                if (segments == null || segments.Count == 0)
                    return (string.Empty);

                StringBuilder totalString = new StringBuilder();

                foreach (SingleSegment singleSegment in segments)
                    totalString.Append(singleSegment.ToString());

                return (totalString.ToString());
            }

            internal class SingleSegment
            {
                internal int CompressionType { get { return (compressionType); } }
                internal int Mode { get { return (mode); } }
                internal byte[] CompressedString { get { return (compressedString); } }

                internal int Index
                {
                    get
                    {
                        if (lastIndex == -1)
                            throw (new InvalidOperationException("SingleSegment: Index requested before block processed"));
                        return (lastIndex);
                    }
                }

                private int compressionType;
                private int mode;
                private byte[] compressedString;

                private int lastIndex = -1;

                internal SingleSegment() { }

                internal void Process(byte[] byteData, int index)
                {
                    lastIndex = index;

                    try
                    {
                        compressionType = (int)byteData[lastIndex];
                        lastIndex++;

                        mode = (int)byteData[lastIndex];
                        lastIndex++;

                        int compressedStringLength = (int)byteData[lastIndex];
                        lastIndex++;

                        if (compressedStringLength != 0)
                        {
                            compressedString = Utils.GetBytes(byteData, lastIndex, compressedStringLength);
                            lastIndex += compressedStringLength;
                        }

                        Validate();
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw (new ArgumentOutOfRangeException("The Single Segment message is short"));
                    }
                }

                internal void Validate() { }

                internal void LogMessage()
                {
                    if (Logger.ProtocolLogger == null)
                        return;

                    string data;
                    if (compressedString == null || compressedString.Length == 0)
                        data = "* Not present *";
                    else
                        data = Utils.ConvertToHex(compressedString);

                    Logger.ProtocolLogger.Write(Logger.ProtocolIndent + "SINGLE SEGMENT: Comp type: " + CompressionType +
                        " mode: " + mode +
                        " string: : " + data);                    
                }

                public override string ToString()
                {
                    if (compressedString == null || compressedString.Length == 0)
                        return (string.Empty);

                    switch (compressionType)
                    {
                        case 0:
                            return(processUncompressedData());                            
                        case 1:
                        case 2:
                            return (processHuffmanData(compressionType));                            
                        default:
                            return (string.Empty);
                    }
                }

                internal string processUncompressedData()
                {
                    if (mode == 0x3e)
                    {
                        SCSUDecompressor scsuDecompressor = new SCSUDecompressor();
                        return (scsuDecompressor.Decompress(compressedString));
                    }

                    if (mode > 0x3f)
                        return ("Text mode 0x" + mode.ToString("X") + " not implemented");

                    int index = 0;

                    if (!useUnicodeEncoding)
                        useUnicodeEncoding = mode > 0x00;

                    if (mode != 0x3f)
                    {
                        byte[] unicodeString = new byte[compressedString.Length * 2];

                        foreach (byte compressedByte in compressedString)
                        {
                            unicodeString[index] = (byte)mode;
                            unicodeString[index + 1] = compressedByte;
                            index += 2;
                        }

                        return (Utils.GetUnicodeString(unicodeString, 0, unicodeString.Length));
                    }
                    else
                        return (Utils.GetUnicodeString(compressedString, 0, compressedString.Length));
                }

                internal string processHuffmanData(int compressionType)
                {
                    return (MultiTreeDictionaryEntry.DecodeData(compressionType, compressedString));
                }
            }
        }
    }
}
