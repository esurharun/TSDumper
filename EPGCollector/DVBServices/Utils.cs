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
using System.Reflection;

using DomainObjects;

namespace DVBServices
{
    /// <summary>
    /// General utility methods.
    /// </summary>
    public sealed class Utils
    {
        /// <summary>
        /// Get the full assembly version number.
        /// </summary>
        public static string AssemblyVersion
        {
            get
            {
                System.Version version = Assembly.GetExecutingAssembly().GetName().Version;
                return (version.Major + "." + version.Minor + "." + version.Build + "." + version.Revision);
            }
        }

        /// <summary>
        /// Convert 2 bytes to an integer with the most significant byte first.
        /// </summary>
        /// <param name="byteData">The byte array containing the byes to convert.</param>
        /// <param name="index">The index of the first byte in the array.</param>
        /// <returns>The converted value.</returns>
        public static int Convert2BytesToInt(byte[] byteData, int index)
        {
            return ((byteData[index] * 256) + (int)byteData[index + 1]); 
        }

        /// <summary>
        /// Convert 4 bytes to an integer with the most significant byte first.
        /// </summary>
        /// <param name="byteData">The byte array containing the byes to convert.</param>
        /// <param name="index">The index of the first byte in the array.</param>
        /// <returns>The converted value.</returns>
        public static int Convert4BytesToInt(byte[] byteData, int index)
        {
            int temp = (int)byteData[index];
            temp = (temp * 256) + (int)byteData[index + 1];
            temp = (temp * 256) + (int)byteData[index + 2];
            temp = (temp * 256) + (int)byteData[index + 3];

            return (temp);            
        }

        /// <summary>
        /// Convert 8 bytes to a long with the most significant byte first.
        /// </summary>
        /// <param name="byteData">The byte array containing the byes to convert.</param>
        /// <param name="index">The index of the first byte in the array.</param>
        /// <returns>The converted value.</returns>
        public static long Convert8BytesToLong(byte[] byteData, int index)
        {
            long temp = (long)byteData[index];
            temp = (temp * 256) + (long)byteData[index + 1];
            temp = (temp * 256) + (long)byteData[index + 2];
            temp = (temp * 256) + (long)byteData[index + 3];
            temp = (temp * 256) + (long)byteData[index + 4];
            temp = (temp * 256) + (long)byteData[index + 5];
            temp = (temp * 256) + (long)byteData[index + 6];
            temp = (temp * 256) + (long)byteData[index + 7];

            return (temp);
        }

        /// <summary>
        /// Convert a BCD encoded string of bytes to an integer.
        /// </summary>
        /// <param name="byteData">The bytes to be converted.</param>
        /// <param name="index">Offset to the first byte.</param>
        /// <param name="count">The number of BCD nibbles to convert.</param>
        /// <returns>The converted value.</returns>
        public static int ConvertBCDToInt(byte[] byteData, int index, int count)
        {
            int result = 0;
            int shift = 4;

            for (int nibbleIndex = 0; nibbleIndex < count; nibbleIndex++)
            {
                result = (result * 10) + ((byteData[index] >> shift) & 0x0f);

                if (shift == 4)
                    shift = 0;
                else
                {
                    shift = 4;
                    index++;
                }
            }

            return (result);
        }

        /// <summary>
        /// Convert an integer value to a hex string.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>The converted string.</returns>
        public static string ConvertToHex(int value)
        {
            if (value == 0)
                return ("0x00");

            uint tempValue = (uint)value;

            char[] outputChars = new char[8];

            int outputIndex = 7;

            for (int index = 3; index > -1; index--)
            {
                uint hexByte = (tempValue << 24) >> 24;
                int hexByteLeft = (int)(hexByte >> 4);
                int hexByteRight =(int)(hexByte & 0x0f);

                outputChars[outputIndex] = getHex(hexByteRight);
                outputChars[outputIndex - 1] = getHex(hexByteLeft);

                outputIndex -= 2;
                tempValue = tempValue >> 8;
            }

            string replyString = new string(outputChars).TrimStart(new char[] { '0' });

            if (replyString.Length % 2 == 0)
                return("0x" + replyString);
            else
                return("0x0" + replyString);
        }

        /// <summary>
        /// Convert a string of bytes to a hex string.
        /// </summary>
        /// <param name="inputChars">The string to be converted.</param>
        /// <returns>The string of hex characters.</returns>
        public static string ConvertToHex(byte[] inputChars)
        {
            return (ConvertToHex(inputChars, inputChars.Length));
        }

        /// <summary>
        /// Convert a string of bytes to a hex string.
        /// </summary>
        /// <param name="inputChars">The array holding the bytes to be converted.</param>
        /// <param name="length">The number of byte to be converted.</param>
        /// <returns>The string of hex characters.</returns>
        public static string ConvertToHex(byte[] inputChars, int length)
        {
            return (ConvertToHex(inputChars, 0, length));
        }

        /// <summary>
        /// Convert a string of bytes to a hex string.
        /// </summary>
        /// <param name="inputChars">The array holding the bytes to be converted.</param>
        /// <param name="offset">The the offset to the first byte to be converted.</param> 
        /// <param name="length">The number of byte to be converted.</param>
        /// <returns>The string of hex characters.</returns>
        public static string ConvertToHex(byte[] inputChars, int offset, int length)
        {
            char[] outputChars = new char[length * 2];
            int outputIndex = 0;

            for (int inputIndex = 0; inputIndex < length; inputIndex++)
            {
                int hexByteLeft = inputChars[offset] >> 4;
                int hexByteRight = inputChars[offset] & 0x0f;

                outputChars[outputIndex] = getHex(hexByteLeft);
                outputChars[outputIndex + 1] = getHex(hexByteRight);

                outputIndex += 2;
                offset++;
            }

            return ("0x" + new string(outputChars));
        }

        /// <summary>
        /// Convert a byte array to a string of 1 and 0;
        /// </summary>
        /// <param name="inputChars">The byte array.</param>
        /// <returns>The byte array with 1 bit represented by each character.</returns>
        public static string ConvertToBits(byte[] inputChars)
        {
            StringBuilder bitString = new StringBuilder();            

            foreach (byte byteData in inputChars)
            {
                byte mask = 0x80;

                for (int shift = 0; shift < 8; shift++)
                {
                    if ((byteData & mask) == 0)
                        bitString.Append("0");
                    else
                        bitString.Append("1");

                    mask = (byte)(mask >> 1);
                }

            }

            return (bitString.ToString());
        }        

        private static char getHex(int value)
        {
            if (value < 10)
                return ((char)('0' + value));

            return ((char)('a' + (value - 10)));
        }

        /// <summary>
        /// Convert an array of bytes to an integer value.
        /// </summary>
        /// <param name="inputBytes">The array of input bytes.</param>
        /// <returns>The converted value.</returns>
        public static int ConvertCharByteToInt(byte[] inputBytes)
        {
            int result = 0;

            foreach (byte inputByte in inputBytes)
                result = (result * 10) + (inputByte - 0x30);

            return (result);
        }

        /// <summary>
        /// Convert an array of bytes to an integer up to a terminating byte.
        /// </summary>
        /// <param name="inputBytes">The array of input bytes.</param>
        /// <param name="terminator">The terminating byte.</param>
        /// <returns>The converted value.</returns>
        public static int ConvertCharByteToInt(byte[] inputBytes, byte terminator)
        {
            int result = 0;

            try
            {
                foreach (byte inputByte in inputBytes)
                {
                    if (inputByte == terminator)
                        return (result);
                    else
                        result = (result * 10) + (inputByte - 0x30);
                }
                return(result);
            }
            catch (ArithmeticException)
            {
                throw (new ArgumentOutOfRangeException("ConvertCharByteToInt result too big"));
            }
        }

        /// <summary>
        /// Convert 2 bytes to an integer value with the least significant byte first. 
        /// </summary>
        /// <param name="byteData">The array of bytes containg the byte to be converted.</param>
        /// <param name="index">The index of the first byte.</param>
        /// <returns>The converted value.</returns>
        public static int Swap2BytesToInt(byte[] byteData, int index)
        {
            return ((byteData[index + 1] * 256) + (int)byteData[index]);            
        }

        /// <summary>
        /// Convert 4 bytes to an integer value with the least significant byte first. 
        /// </summary>
        /// <param name="byteData">The array of bytes containg the byte to be converted.</param>
        /// <param name="index">The index of the first byte.</param>
        /// <returns>The converted value.</returns>
        public static int Swap4BytesToInt(byte[] byteData, int index)
        {
            int temp = (int)byteData[index + 3];
            temp = (temp * 256) + (int)byteData[index + 2];
            temp = (temp * 256) + (int)byteData[index + 1];
            temp = (temp * 256) + (int)byteData[index];

            return (temp);            
        }

        /// <summary>
        /// Convert 8 bytes to a long value with the least significant byte first. 
        /// </summary>
        /// <param name="byteData">The array of bytes containg the byte to be converted.</param>
        /// <param name="index">The index of the first byte.</param>
        /// <returns>The converted value.</returns>
        public static long Swap8BytesToLong(byte[] byteData, int index)
        {
            long temp = (long)byteData[index + 7];
            temp = (temp * 256) + (long)byteData[index + 6];
            temp = (temp * 256) + (long)byteData[index + 5];
            temp = (temp * 256) + (long)byteData[index + 4];
            temp = (temp * 256) + (long)byteData[index + 3];
            temp = (temp * 256) + (long)byteData[index + 2];
            temp = (temp * 256) + (long)byteData[index + 1];
            temp = (temp * 256) + (long)byteData[index];

            return (temp);            
        }

        /// <summary>
        /// Convert an array of Unicode bytes to a string.
        /// </summary>
        /// <param name="byteData">The array of bytes.</param>
        /// <param name="offset">The index of the first byte to be converted.</param>
        /// <param name="length">The number of bytes to be converted.</param>
        /// <returns>The converted string.</returns>
        public static string GetUnicodeString(byte[] byteData, int offset, int length)
        {
            UnicodeEncoding encoding = new UnicodeEncoding(true, false);
            return (encoding.GetString(byteData, offset, length));
        }

        /// <summary>
        /// Convert an array of bytes to a string.
        /// </summary>
        /// <param name="byteData">The array of bytes.</param>
        /// <returns>The converted string.</returns>
        public static string GetAsciiString(byte[] byteData)
        {
            return (GetAsciiString(byteData, false));
        }

        /// <summary>
        /// Convert an array of bytes to a string conditionally replacing non-Ascii characters.
        /// </summary>
        /// <param name="byteData">The array of bytes.</param>
        /// <param name="replace">True to replace non-Ascii bytes with a space; false to ignore them.</param>
        /// <returns>The converted string.</returns>
        public static string GetAsciiString(byte[] byteData, bool replace)
        {
            StringBuilder stringData = new StringBuilder();

            for (int index = 0; index < byteData.Length; index++)
            {
                if (byteData[index] > 0x1f && byteData[index] < 0x7f)
                    stringData.Append((char)byteData[index]);
                else
                {
                    if (replace)
                        stringData.Append(' ');
                }
            }

            return (stringData.ToString());
        }

        /// <summary>
        /// Convert a subset of an array of bytes to a string.
        /// </summary>
        /// <param name="byteData">The array of bytes.</param>
        /// <param name="offset">The index of the first byte to be converted.</param>
        /// <param name="length">The number of bytes to be converted.</param>
        /// <returns>The converted string.</returns>
        public static string GetString(byte[] byteData, int offset, int length)
        {
            return (GetString(byteData, offset, length, false));
        }

        /// <summary>
        /// Convert a subset of an array of text bytes to a string.
        /// </summary>
        /// <param name="byteData">The array of bytes.</param>
        /// <param name="offset">The index of the first byte to be converted.</param>
        /// <param name="length">The number of bytes to be converted.</param>
        /// <param name="replace">True to replace non-Ascii bytes with a space; false to ignore them.</param>
        /// <returns>The converted string.</returns>
        public static string GetString(byte[] byteData, int offset, int length, bool replace)
        {
            if (length == 0)
                return (string.Empty);

            string isoTable = null;
            int startByte = 0;

            if (RunParameters.Instance.CharacterSet != null)
            {
                isoTable = RunParameters.Instance.CharacterSet;
                if (byteData[offset] < 0x20)
                    startByte = 1;
            }
            else
            {
                if (byteData[offset] >= 0x20)
                    isoTable = "iso-8859-1";
                else
                {
                    switch (byteData[offset])
                    {
                        case 0x01:
                        case 0x02:
                        case 0x03:
                        case 0x04:
                        case 0x05:
                        case 0x06:
                        case 0x07:
                        case 0x08:
                        case 0x09:
                        case 0x0a:
                        case 0x0b:
                            isoTable = "iso-8859-" + (byteData[offset] + 4).ToString();
                            startByte = 1;
                            break;
                        case 0x10:
                            if (byteData[offset + 1] == 0x00)
                            {
                                if (byteData[offset + 2] != 0x00 && byteData[offset + 2] != 0x0c)
                                {
                                    isoTable = "iso-8859-" + ((int)byteData[offset + 2]).ToString();
                                    startByte = 3;
                                    break;
                                }
                                else
                                    return ("Invalid DVB text string: byte 3 is not a valid value");
                            }
                            else
                                return ("Invalid DVB text string: byte 2 is not a valid value");
                        case 0x11:
                        case 0x15:
                            isoTable = "utf-8";
                            startByte = 1;
                            break;
                        case 0x1f:
                            if (byteData[offset + 1] == 0x01 || byteData[offset + 1] == 0x02)
                            {
                                if (MultiTreeDictionaryEntry.Loaded)
                                    return (MultiTreeDictionaryEntry.DecodeData(Utils.GetBytes(byteData, offset, length + 1)));
                                else
                                    return ("Huffman text: " + Utils.ConvertToHex(byteData, offset, length));
                            }
                            else
                                return ("Invalid DVB text string: Custom text specifier is not recognized");
                        default:
                            return ("Invalid DVB text string: byte 1 is not a valid value");
                    }
                }
            }

            byte[] editedBytes = new byte[length];
            int editedLength = 0;

            for (int index = startByte; index < length; index++)
            {
                if (byteData[offset + index] > 0x1f)
                {
                    if (byteData[offset + index] < 0x80 || byteData[offset + index] > 0x9f)
                    {
                        editedBytes[editedLength] = byteData[offset + index];
                        editedLength++;
                    }
                }
                else
                {
                    if (replace)
                    {
                        editedBytes[editedLength] = 0x20;
                        editedLength++;
                    }
                }
            }            

            if (editedLength == 0)
                return (string.Empty);

            try
            {
                Encoding sourceEncoding = Encoding.GetEncoding(isoTable);
                if (sourceEncoding == null)
                    sourceEncoding = Encoding.GetEncoding("iso-8859-1");

                return (sourceEncoding.GetString(editedBytes, 0, editedLength));
            }
            catch (ArgumentException e)
            {
                Logger.Instance.Write("<E> A text string could not be decoded");
                Logger.Instance.Write("<E> String: " + Utils.ConvertToHex(byteData, offset, length));
                Logger.Instance.Write("<E> Error: " + e.Message);
                return ("** ERROR DECODING STRING - SEE COLLECTION LOG **");
            }
        }

        /// <summary>
        /// Convert an array of bytes up to a terminator to a string.
        /// </summary>
        /// <param name="byteData">The array of bytes.</param>
        /// <param name="offset">The index of the first byte to be converted.</param>
        /// <param name="terminator">The value of the terminator.</param>
        /// <returns>The converted string.</returns>
        public static string GetString(byte[] byteData, int offset, byte terminator)
        {
            StringBuilder stringData = new StringBuilder();

            while (byteData[offset] != terminator)
            {
                if (byteData[offset] > 0x1f && byteData[offset] < 0x7f)
                    stringData.Append((char)byteData[offset]);
                else
                    stringData.Append(' ');
                offset++;
            }

            return (stringData.ToString());
        }

        /// <summary>
        /// Convert an array of bytes to a string given an encoding.
        /// </summary>
        /// <param name="byteData">The array of bytes.</param>
        /// <param name="encoding">The type of encoding.</param>
        /// <returns>The converted string.</returns>
        public static string GetString(byte[] byteData, string encoding)
        {
            if (byteData.Length == 0)
                return (string.Empty);

            try
            {
                Encoding sourceEncoding = Encoding.GetEncoding(encoding);
                if (sourceEncoding == null)
                    sourceEncoding = Encoding.GetEncoding("iso-8859-1");

                return (sourceEncoding.GetString(byteData, 0, byteData.Length));
            }
            catch (ArgumentException e)
            {
                Logger.Instance.Write("<E> A text string could not be decoded");
                Logger.Instance.Write("<E> String: " + Utils.ConvertToHex(byteData, 0, byteData.Length));
                Logger.Instance.Write("<E> Error: " + e.Message);
                return ("** ERROR DECODING STRING - SEE COLLECTION LOG **");
            }
        }

        /// <summary>
        /// Get subset of bytes from an array.
        /// </summary>
        /// <param name="byteData">The array of bytes.</param>
        /// <param name="offset">The index of the first byte of the subset.</param>
        /// <param name="length">The length of the subset.</param>
        /// <returns>The subset of bytes.</returns>
        public static byte[] GetBytes(byte[] byteData, int offset, int length)
        {
            if (length < 1)
                throw (new ArgumentOutOfRangeException("GetBytes length wrong"));

            try
            {
                byte[] outputBytes = new byte[length];

                for (int index = 0; index < length; index++)
                    outputBytes[index] = byteData[offset + index];

                return (outputBytes);
            }
            catch (OutOfMemoryException)
            {
                throw (new ArgumentOutOfRangeException("GetBytes length wrong"));
            }
        }

        /// <summary>
        /// Get a subset of bytes from an array up to a terminator.
        /// </summary>
        /// <param name="byteData">The array of bytes.</param>
        /// <param name="offset">The index of the first byte of the subset.</param>
        /// <param name="terminator">The terminating value.</param>
        /// <returns>The subset of bytes.</returns>
        public static byte[] GetBytes(byte[] byteData, int offset, byte terminator)
        {
            int length = 0;

            while (offset + length < byteData.Length && byteData[offset + length] != terminator)
                length++;

            if (length == 0)
                return (new byte[0]);
            else
                return (GetBytes(byteData, offset, length));
        }

        /// <summary>
        /// Compare the bytes of 2 arrays for equality including length.
        /// </summary>
        /// <param name="array1">The first array.</param>
        /// <param name="array2">The second array.</param>
        /// <returns>Treu if the arrays are equal; false otherwise.</returns>
        public static bool CompareBytes(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
                return (false);

            for (int index = 0; index < array1.Length; index++)
            {
                if (array1[index] != array2[index])
                    return(false);
            }

            return(true);
        }

        /// <summary>
        /// Compare a subset of the bytes of 2 arrays for equality.
        /// </summary>
        /// <param name="array1">The first array.</param>
        /// <param name="array2">The second array.</param>
        /// <param name="length">The number of bytes to compare.</param>
        /// <returns>True if the arrays are equal; false otherwise.</returns>
        public static bool CompareBytes(byte[] array1, byte[] array2, int length)
        {
            if (array1.Length < length || array2.Length < length)
                return (false);

            for (int index = 0; index < length; index++)
            {
                if (array1[index] != array2[index])
                    return (false);
            }

            return (true);
        }

        /// <summary>
        /// Remove redundant spaces in a string.
        /// </summary>
        /// <param name="inputString">The string to be scanned.</param>
        /// <returns>The compaced string.</returns>
        public static string Compact(string inputString)
        {
            StringBuilder outputString = new StringBuilder();
            bool ignoreSpace = false;

            foreach (char inputChar in inputString)
            {
                if (inputChar == ' ')
                {
                    if (!ignoreSpace)
                    {
                        outputString.Append(inputChar);
                        ignoreSpace = true;
                    }
                }
                else
                {
                    outputString.Append(inputChar);
                    ignoreSpace = false;
                }
            }

            return (outputString.ToString().Trim());
        }

        /// <summary>
        /// Split an array of bytes into an array of strings based on a terminating character.
        /// </summary>
        /// <param name="inputBytes">The array of input bytes.</param>
        /// <param name="splitter">The terminating byte for each substring.</param>
        /// <returns>The array of strings.</returns>
        public static string[] SplitBytesToStrings(byte[] inputBytes, byte splitter)
        {
            int entries = 0;

            foreach (byte inputByte in inputBytes)
            {
                if (inputByte == splitter)
                    entries++;
            }

            if (entries == 0)
                entries = 1;

            string[] outputStrings = new string[entries];
            int outputCount = 0;

            StringBuilder currentString = new StringBuilder();

            foreach (byte inputByte in inputBytes)
            {
                if (inputByte != splitter)
                    currentString.Append(inputByte);
                else
                {
                    outputStrings[outputCount] = currentString.ToString();
                    outputCount++;
                    currentString.Length = 0;
                }

            }

            if (currentString.Length != 0)
                outputStrings[outputCount] = currentString.ToString();

            return (outputStrings);
        }

        /// <summary>
        /// Split an array of bytes into a collection of byte arrays based on a terminator.
        /// </summary>
        /// <param name="inputBytes">The array of bytes.</param>
        /// <param name="splitter">The terminating byte for each collection.</param>
        /// <returns>A collection of byte arrays.</returns>
        public static Collection<byte[]> SplitBytes(byte[] inputBytes, byte splitter)
        {
            Collection<byte[]> outputStrings = new Collection<byte[]>();

            int startIndex = 0;
            int currentIndex = 0;

            for (; currentIndex < inputBytes.Length; currentIndex++)
            {
                if (inputBytes[currentIndex] == splitter)
                {
                    outputStrings.Add(getByteEntry(inputBytes, startIndex, currentIndex));
                    startIndex = currentIndex + 1;
                }
            }

            if (currentIndex != startIndex)
                outputStrings.Add(getByteEntry(inputBytes, startIndex, currentIndex));

            return (outputStrings);
        }

        private static byte[] getByteEntry(byte[] inputBytes, int startIndex, int currentIndex)
        {
            byte[] bytes = new byte[currentIndex - startIndex];

            int outputIndex = 0;

            for (; startIndex < currentIndex; startIndex++)
            {
                bytes[outputIndex] = inputBytes[startIndex];
                outputIndex++;
            }

            return (bytes);
        }

        /// <summary>
        /// Optionally round a date and time to the nearest 5 minutes.
        /// </summary>
        /// <param name="oldDateTime">The date and time to be rounded.</param>
        /// <returns>The adjusted date and time.</returns>
        public static DateTime RoundTime(DateTime oldDateTime)
        {
            if (!RunParameters.Instance.Options.Contains("ROUNDTIME"))
                return (oldDateTime);

            int partSeconds = (int)(oldDateTime.TimeOfDay.TotalSeconds % 300);
            
            if (partSeconds != 0)
            {
                if (partSeconds < 180)
                    return(oldDateTime.AddSeconds(partSeconds * -1));
                else
                    return(oldDateTime.AddSeconds(300 - partSeconds));                    
            }
            else
                return(oldDateTime);            
        }

        /// <summary>
        /// Optionally round a time to the nearest 5 minutes.
        /// </summary>
        /// <param name="oldTime">The time to be rounded.</param>
        /// <returns>The adjusted time.</returns>
        public static TimeSpan RoundTime(TimeSpan oldTime)
        {
            if (!RunParameters.Instance.Options.Contains("ROUNDTIME"))
                return (oldTime);

            int partSeconds = (int)(oldTime.TotalSeconds % 300);

            if (partSeconds != 0)
            {
                if (partSeconds < 180)
                {
                    if (partSeconds != oldTime.TotalSeconds)
                        return (oldTime - getTimeSpan(partSeconds * -1));
                    else
                        return(new TimeSpan(0, 5, 0));
                }
                else
                    return (oldTime + getTimeSpan(300 - partSeconds));
            }
            else
                return (oldTime); 
        }

        private static TimeSpan getTimeSpan(int seconds)
        {
            return (new TimeSpan(0, seconds / 60, seconds % 60));
        }

        private Utils() { }
    }
}
