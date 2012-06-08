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

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// The class that describes a dictionary entry for a single tree Huffman scenario.
    /// </summary>
    public class SingleTreeDictionaryEntry
    {
        /// <summary>
        /// Get or set the flag for not starting at the first bit in the compressed string.
        /// </summary>
        public static bool OffsetStart
        {
            get { return (offsetStart); }
            set { offsetStart = value; }
        }

        /// <summary>
        /// Get the decode string.
        /// </summary>
        public string Decode { get { return (decode); } }

        private string decode;
        private string pattern;

        private static HuffmanEntry[] roots = new HuffmanEntry[2];
        private static bool offsetStart = true;

        /// <summary>
        /// Initialize a new instance of the SingleTreeDictionaryEntry class.
        /// </summary>
        /// <param name="pattern">The Huffman bit pattern.</param>
        /// <param name="decode">The decode for the bit pattern.</param>
        public SingleTreeDictionaryEntry(string pattern, string decode)
        {
            this.pattern = pattern;
            this.decode = decode;
        }

        /// <summary>
        /// Load the dictionary entries ino the first root.
        /// </summary>
        /// <returns>True if the file has been loaded; false otherwise.</returns>
        public static bool Load(string fileName)
        {
            return (Load(fileName, 1));
        }

        /// <summary>
        /// Load the dictionary entries.
        /// </summary>
        /// <returns>True if the file has been loaded; false otherwise.</returns>
        public static bool Load(string fileName, int rootNumber)
        {
            FileStream fileStream = null;

            Logger.Instance.Write("Loading Huffman Dictionary from " + fileName);

            try { fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read); }
            catch (IOException e)
            {
                Logger.Instance.Write("Huffman Dictionary file " + fileName + " not available");
                Logger.Instance.Write(e.Message);
                return (false);
            }

            StreamReader streamReader = new StreamReader(fileStream);
            roots[rootNumber - 1] = null;

            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();

                if (line != string.Empty && !line.StartsWith("####"))
                {
                    string[] parts = line.Split(new char[] { '=' });
                    if (parts.Length == 2)
                        addEntry(rootNumber, parts[1], parts[0]);
                    else
                    {
                        if (parts.Length == 3 && parts[0] == string.Empty && parts[1] == string.Empty)
                            addEntry(rootNumber, parts[2], "=");
                        else
                            Logger.Instance.Write("Dictionary line '" + line + "' format wrong - line ignored ");
                    }
                }
            }

            streamReader.Close();
            fileStream.Close();

            Logger.Instance.Write("Dictionary loaded");

            return (true);
        }

        private static void addEntry(int rootNumber, string pattern, string decode)
        {
            if (roots[rootNumber - 1] == null)
                roots[rootNumber - 1] = new HuffmanEntry();

            HuffmanEntry currentEntry = roots[rootNumber - 1];

            for (int index = 0; index < pattern.Length; index++)
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
                                currentEntry.Value = decode;
                        }
                        else
                        {
                            currentEntry = currentEntry.P0;
                            if (currentEntry.Value != null && index == pattern.Length - 1)
                                Logger.Instance.Write("Dictionary entry already set");
                        }
                        break;
                    case '1':
                        if (currentEntry.P1 == null)
                        {
                            currentEntry.P1 = new HuffmanEntry();
                            currentEntry = currentEntry.P1;
                            if (index == pattern.Length - 1)
                                currentEntry.Value = decode;
                        }
                        else
                        {
                            currentEntry = currentEntry.P1;
                            if (currentEntry.Value != null && index == pattern.Length - 1)
                                Logger.Instance.Write("Dictionary entry already set");
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Decompress a compressed Huffman string using the first root table.
        /// </summary>
        /// <param name="byteData">The compressed byte data.</param>
        /// <returns>The decompressed string.</returns>
        public static string DecodeData(byte[] byteData)
        {
            return (DecodeData(1, byteData));
        }

        /// <summary>
        /// Decompress a compressed Huffman string.
        /// </summary>
        /// <param name="rootNumber">The root table to use.</param>
        /// <param name="byteData">The compressed byte data.</param>
        /// <returns>The decompressed string.</returns>
        public static string DecodeData(int rootNumber, byte[] byteData)
        {
            StringBuilder outputString = new StringBuilder();

            HuffmanEntry currentEntry = roots[rootNumber - 1];
            
            byte mask;
            if (offsetStart)
                mask = 0x20;
            else
                mask = 0x80;

            StringBuilder bitString = new StringBuilder();

            for (int index = 0; index < byteData.Length; index++)
            {
                byte dataByte = byteData[index];                

                while (mask > 0)
                {
                    if (currentEntry.Value != null)
                    {
                        if (currentEntry.Value != "??")
                            outputString.Append(currentEntry.Value);
                        currentEntry = roots[rootNumber - 1];
                        bitString = new StringBuilder();
                    }

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

                mask = 0x80;
            }

            if (currentEntry.Value != null && currentEntry.Value != "??")
                outputString.Append(currentEntry.Value);

            return(outputString.ToString());
        }	  
    }
}
