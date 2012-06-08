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

using System.IO;
using System.Text;
using System.Globalization;

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that describes a dictionary entry for a multi-tree Hufmman scenario.
    /// </summary>
    public class MultiTreeDictionaryEntry
    {
        /// <summary>
        /// Return true if the translation tables have been loaded; false otherwise.
        /// </summary>
        public static bool Loaded { get { return (loaded); } }

        private const int stop = 0x02;
        private const int start = 0x00;
        private const int escape = 0x01;

        private static HuffmanEntry[] table1Roots = new HuffmanEntry[256];
        private static HuffmanEntry[] table2Roots = new HuffmanEntry[256];        

        /// <summary>
        /// Get the decode string.
        /// </summary>
        public string Decode { get { return (decode); } }

        private string decode;
        private string pattern;
        private static bool loaded;

        /// <summary>
        /// Initialize a new instance of the MultiTreeDictionaryEntry class.
        /// </summary>
        /// <param name="pattern">The Huffman bit pattern.</param>
        /// <param name="decode">The decode for the bit pattern.</param>
        public MultiTreeDictionaryEntry(string pattern, string decode)
        {
            this.pattern = pattern;
            this.decode = decode;
        }

        /// <summary>
        /// Load the reference tables.
        /// </summary>
        /// <param name="fileName1">The full name of the T1 file.</param>
        /// <param name="fileName2">The full name of the T2 file.</param>
        /// <returns>True if the file is loaded successfully;false otherwise.</returns>
        public static bool Load(string fileName1, string fileName2)
        {
            if (loaded)
                return (true);

            Logger.Instance.Write("Loading Huffman Dictionary 1 from " + fileName1);
            try
            {
                loadFile(table1Roots, fileName1);
            }
            catch (IOException e)
            {
                Logger.Instance.Write("Huffman Dictionary file " + fileName1 + " not available");
                Logger.Instance.Write(e.Message);
                return (false);
            }

            Logger.Instance.Write("Loading Huffman Dictionary 2 from " + fileName2);
            try
            {
                loadFile(table2Roots, fileName2);
            }
            catch (IOException e)
            {
                Logger.Instance.Write("Huffman Dictionary file " + fileName2 + " not available");
                Logger.Instance.Write(e.Message);
                return (false);
            }

            Logger.Instance.Write("Dictionaries loaded");

            loaded = true;
            return (true);
        }

        private static void loadFile(HuffmanEntry[] roots, string filename)
        {
            FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            StreamReader streamReader = new StreamReader(fileStream);

            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();

                if (line != string.Empty && !line.StartsWith("####"))
                {
                    string[] parts = line.Split(new char[] { ':' });
                    if (parts.Length == 4)
                    {
                        int rootOffSet = (int)(resolveChar(parts[0]));

                        if (roots[rootOffSet] == null)
                            roots[rootOffSet] = new HuffmanEntry();

                        HuffmanEntry currentEntry = roots[rootOffSet];
                        string pattern = parts[1];    

                        for (int index = 0; index < parts[1].Length; index++)
                        {
                            char patternChar = pattern[index];
                            
                            switch (patternChar)
                            {
                                case '0':
                                    if (currentEntry.P0 == null)
                                    {
                                        currentEntry.P0 = new HuffmanEntry();
                                        currentEntry = currentEntry.P0;
                                        if (index == pattern.Length - 1)
                                            currentEntry.Value = resolveChar(parts[2]).ToString();
                                    }
                                    else
                                    {
                                        currentEntry = currentEntry.P0;
                                        if (currentEntry.HoldsValue && index == pattern.Length - 1)
                                            Logger.Instance.Write("Dictionary entry already set");
                                    }
                                    break;
                                case '1':
                                    if (currentEntry.P1 == null)
                                    {
                                        currentEntry.P1 = new HuffmanEntry();
                                        currentEntry = currentEntry.P1;
                                        if (index == pattern.Length - 1)
                                            currentEntry.Value = resolveChar(parts[2]).ToString();
                                    }
                                    else
                                    {
                                        currentEntry = currentEntry.P1;
                                        if (currentEntry.HoldsValue && index == pattern.Length - 1)
                                            Logger.Instance.Write("Dictionary entry already set: " + line);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }                        
                    }
                }
            }

            streamReader.Close();
            fileStream.Close();
        }

        /// <summary>
        /// Decode a Multi-tree text string which includes the text prefix (ie 0x1f?? where ?? indicates the table number 1 or 2).
        /// </summary>
        /// <param name="byteData">The encoded string.</param>
        /// <returns>The decoded string.</returns>
        public static string DecodeData(byte[] byteData)
        {
            if (byteData[1] == 1)
                return(decodeData(byteData, table1Roots, 2));
            else
                return(decodeData(byteData, table2Roots, 2));
        }

        /// <summary>
        /// Decode a Multi-tree text string with no prefix.
        /// </summary>
        /// <param name="table">The decode table to use (1 or 2).</param>
        /// <param name="byteData">The encoded string.</param>
        /// <returns>The decoded string.</returns>
        public static string DecodeData(int table, byte[] byteData)
        {
            if (table == 1)
                return (decodeData(byteData, table1Roots, 0));
            else
                return (decodeData(byteData, table2Roots, 0));
        }

        private static string decodeData(byte[] byteData, HuffmanEntry[] roots, int startIndex)
        {
            StringBuilder outputString = new StringBuilder();

            HuffmanEntry currentEntry = roots[0];
            byte mask = 0x80;
            bool finished = false;

            StringBuilder bitString = new StringBuilder();

            for (int index = startIndex; index < byteData.Length && !finished; index++)
            {
                byte dataByte = byteData[index];

                while (mask > 0 && !finished)
                {
                    if (currentEntry.HoldsValue)
                    {
                        switch ((int)currentEntry.Value[0])
                        {
                            case stop:
                                finished = true;
                                break;
                            case escape:                                
                                byte encodedValue;
                                do
                                {
                                    encodedValue = 0x00;

                                    for (int bitCount = 0; bitCount < 8; bitCount++)
                                    {
                                        encodedValue = (byte)(encodedValue << 1);

                                        if ((dataByte & mask) != 0)
                                            encodedValue |= 0x01;

                                        mask = (byte)(mask >> 1);
                                        if (mask == 0)
                                        {
                                            index++;
                                            dataByte = byteData[index];
                                            mask = 0x80;
                                        }
                                    }
                                }
                                while ((encodedValue & 0x80) != 0);
                                
                                finished = ((int)encodedValue < 0x20);

                                if (!finished)
                                {
                                    outputString.Append((char)encodedValue);
                                    currentEntry = roots[encodedValue];
                                    bitString = new StringBuilder();
                                }

                                break;
                            default:
                                outputString.Append(currentEntry.Value);
                                currentEntry = roots[(int)currentEntry.Value[0]];
                                bitString = new StringBuilder();
                                break;
                        }                        
                    }

                    if (!finished)
                    {
                        if ((dataByte & mask) == 0)
                        {
                            bitString.Append("0");

                            if (currentEntry.P0 != null)
                                currentEntry = currentEntry.P0;
                            else
                            {
                                Logger.Instance.Write(" ** DECOMPRESSION FAILED **");
                                Logger.Instance.Write("Original data: " + Utils.ConvertToHex(byteData));
                                Logger.Instance.Write("Decoded data: " + outputString.ToString());
                                Logger.Instance.Write("Bit string: " + bitString.ToString());
                                return (outputString.ToString() + " ** DECOMPRESSION FAILED **");
                            }
                        }
                        else
                        {
                            bitString.Append("1");

                            if (currentEntry.P1 != null)
                                currentEntry = currentEntry.P1;
                            else
                            {
                                Logger.Instance.Write(" ** DECOMPRESSION FAILED **");
                                Logger.Instance.Write("Original data: " + Utils.ConvertToHex(byteData));
                                Logger.Instance.Write("Decoded data: " + outputString.ToString());
                                Logger.Instance.Write("Bit string: " + bitString.ToString());
                                return (outputString.ToString() + " ** DECOMPRESSION FAILED **");
                            }
                        }

                        mask = (byte)(mask >> 1);
                    }
                }

                mask = 0x80;
            }

            return (outputString.ToString());
        }

        private static char resolveChar(string input)
        {
            int val = new int();
            char myChar = input[0]; //default value

            switch (input.ToUpper())
            {
                case "START":
                    myChar = (char)0x00;
                    break;
                case "STOP":
                    myChar = (char)0x02;
                    break;
                case "ESCAPE":
                    myChar = (char)0x01;
                    break;
                default:
                    try
                    {
                        if (input.Substring(0, 2) == "0x")
                        {
                            val = int.Parse(input.Substring(2, input.Length - 2), NumberStyles.AllowHexSpecifier); //ASCII for the input character
                        }
                        myChar = (char)val;
                    }
                    catch
                    {

                    }
                    break;
            }

            return (myChar);
        }
    }
}
