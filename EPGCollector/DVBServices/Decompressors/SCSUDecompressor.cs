///////////////////////////////////////////////////////////////////////////////// 
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
using System.Text;

namespace DVBServices
{
    internal class SCSUDecompressor
    {
        private const byte sq0 = 0x01;
        private const byte sq1 = 0x02;
        private const byte sq2 = 0x03;
        private const byte sq3 = 0x04;
        private const byte sq4 = 0x05;
        private const byte sq5 = 0x06;
        private const byte sq6 = 0x07;
        private const byte sq7 = 0x08;

        private const byte sdx = 0x0b;
        private const byte srs = 0x0c;
        private const byte squ = 0x0e;
        private const byte scu = 0x0f;

        private const byte sc0 = 0x10;
        private const byte sc1 = 0x11;
        private const byte sc2 = 0x12;
        private const byte sc3 = 0x13;
        private const byte sc4 = 0x14;
        private const byte sc5 = 0x15;
        private const byte sc6 = 0x16;
        private const byte sc7 = 0x17;

        private const byte sd0 = 0x18;
        private const byte sd1 = 0x19;
        private const byte sd2 = 0x1a;
        private const byte sd3 = 0x1b;
        private const byte sd4 = 0x1c;
        private const byte sd5 = 0x1d;
        private const byte sd6 = 0x1e;
        private const byte sd7 = 0x1f;

        private const byte uc0 = 0xe0;
        private const byte uc1 = 0xe1;
        private const byte uc2 = 0xe2;
        private const byte uc3 = 0xe3;
        private const byte uc4 = 0xe4;
        private const byte uc5 = 0xe5;
        private const byte uc6 = 0xe6;
        private const byte uc7 = 0xe7;

        private const byte ud0 = 0xe8;
        private const byte ud1 = 0xe9;
        private const byte ud2 = 0xeA;
        private const byte ud3 = 0xeB;
        private const byte ud4 = 0xeC;
        private const byte ud5 = 0xeD;
        private const byte ud6 = 0xeE;
        private const byte ud7 = 0xeF;

        private const byte uqu = 0xf0;
        private const byte udx = 0xf1;
        private const byte urs = 0xf2;

        private const uint gapThreshold = 0x68;
        private const uint gapOffset = 0xac00;
        private const uint reservedStart = 0xa8;
        private const uint fixedThreshold = 0xf9;

        private uint[] staticOffset =
        {
            0x0000, 0x0080, 0x0100, 0x0300,
            0x2000, 0x2080, 0x2100, 0x3000
        };

        private uint[] initialDynamicOffset =
        {
            0x0080, 0x00C0, 0x0400, 0x0600,
            0x0900, 0x3040, 0x30A0, 0xFF00
        };

        private uint[] fixedOffset =
        {
            0x00c0, 0x0250, 0x0370, 0x0530,
            0x3040, 0x30a0, 0xff60
        };

        private uint[] dynamicOffset = new uint[8];
        private uint window = 0;

        private int _character;
        private int _byte;

        internal SCSUDecompressor()
        {
            Array.Copy(initialDynamicOffset, dynamicOffset, initialDynamicOffset.Length);
        }

        internal string Decompress(byte[] byteArray)
        {
            StringBuilder builder = new StringBuilder(byteArray.Length);
            _character = 0;
            int current;

            for (current = 0; current < byteArray.Length; current++)
            {
                uint staticWindow = 0;
                uint dynamicWindow = window;

                switch (byteArray[current])
                {
                    case sq0:                        
                    case sq1:                        
                    case sq2:                        
                    case sq3:                        
                    case sq4:                        
                    case sq5:                        
                    case sq6:                        
                    case sq7:
                        if (current < (byteArray.Length - 1))
                        {
                            dynamicWindow = staticWindow = (uint)(byteArray[current] - sq0);
                            current++;

                            if (byteArray[current] < 128)
                            {
                                uint temp = byteArray[current] + staticOffset[staticWindow];
                                builder.Append((char)temp);
                                _character++;
                            }
                            else
                            {
                                uint temp = (uint)(byteArray[current]);
                                temp -= 0x80;
                                temp += dynamicOffset[dynamicWindow];

                                if (temp < (1 << 16))
                                {
                                    builder.Append((char)temp);
                                    _character++;
                                }
                                else
                                {
                                    temp -= 0x10000;
                                    builder.Append((char)(0xD800 + (temp >> 10)));
                                    _character++;
                                    builder.Append((char)(0xDC00 + (temp & (~0xfc00))));
                                    _character++;
                                }
                            }
                        }
                        break;
                    case sdx:
                        current += 2;
                        if (current < byteArray.Length)
                            defineExtendedWindow((uint)charFromTwoBytes(byteArray[current - 1], byteArray[current]));
                        break;
                    case sd0:                        
                    case sd1:                        
                    case sd2:                        
                    case sd3:                        
                    case sd4:                        
                    case sd5:                        
                    case sd6:                        
                    case sd7:
                        current++;
                        if (current < byteArray.Length)
                            defineWindow((uint)(byteArray[current - 1] - sd0), byteArray[current]);
                        break;
                    case sc0:                        
                    case sc1:                        
                    case sc2:                        
                    case sc3:                        
                    case sc4:                       
                    case sc5:                        
                    case sc6:                        
                    case sc7:
                        window = (uint)(byteArray[current] - sc0);
                        break;
                    case scu:
                        current = expandUnicode(byteArray, current + 1, builder);
                        break;
                    case squ:
                        current += 2;
                        if (current < byteArray.Length)
                        {                            
                            char temp = charFromTwoBytes(byteArray[current - 1], byteArray[current]);
                            builder.Append((char)temp);
                            _character++;
                        }
                        break;
                    case srs:
                        throw (new ArgumentException("SCSU Decompressor failed"));
                }
            }

            if (current >= byteArray.Length)
            {
                builder.Length = _character;
                _byte = current;

                return (builder.ToString());
            }

            throw (new InvalidOperationException("SCSU Decompressor failed"));
        }

        private void defineExtendedWindow(uint character)
        {
            window = character >> 13;
            dynamicOffset[window] = ((character & 0x1fff) << 7) + (1 << 16);            
        }

        private char charFromTwoBytes(byte high, byte low)
        {
            char temp = (char)(low);
            return (char)(temp + (char)((high) << 8));
        }

        private void defineWindow(uint window, byte offset)
        {
            uint tempOffset = (uint)(offset);

            if (tempOffset == 0)
                throw (new InvalidOperationException("SCSU Decompressor failed"));
            else
            {
                if (tempOffset < gapThreshold)
                    dynamicOffset[window] = tempOffset << 7;
                else
                {
                    if (tempOffset < reservedStart)
                        dynamicOffset[window] = (tempOffset << 7) + gapOffset;
                    else
                    {
                        if (tempOffset < fixedThreshold)
                            throw (new InvalidOperationException("SCSU Decompressor failed"));
                        else
                            dynamicOffset[window] = fixedOffset[tempOffset - fixedThreshold];
                    }
                }
            }

            this.window = window;
        }

        private int expandUnicode(byte[] input, int current, StringBuilder builder)
        {
            for (; current < (input.Length - 1); current += 2)
            {
                byte temp = input[current];

                if (temp >= uc0 && temp <= uc7)
                {
                    window = (uint)(temp - uc0);
                    return current;
                }
                else
                {
                    if (temp >= ud0 && temp <= ud7)
                    {
                        defineWindow((uint)(temp - ud0), input[current + 1]);
                        return current + 1;
                    }
                    else
                    {
                        if (temp == udx)
                        {
                            if (current >= (input.Length - 2))
                                break;

                            defineExtendedWindow(charFromTwoBytes(input[current + 1], input[current + 2]));
                            return current + 2;
                        }
                        else
                        {
                            if (temp == uqu)
                            {
                                if (current >= (input.Length - 2))
                                    break;

                                current++;
                            }
                        }
                    }
                }

                char character = charFromTwoBytes(input[current], input[current + 1]);
                builder.Append(character);
                _character++;
            }

            if (current == input.Length)
                return current;

            throw (new InvalidOperationException("SCSU Decompressor failed"));
        }
    }
}