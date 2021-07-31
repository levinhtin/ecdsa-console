﻿using System;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace EllipticConsole.Utils
{
    public static class BinaryAscii
    {
        public static string HexFromBinary(byte[] bytes)
        {
            StringBuilder hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }

        public static byte[] BinaryFromHex(string hex)
        {
            int numberChars = hex.Length;
            if ((numberChars % 2) == 1)
            {
                hex = "0" + hex;
                numberChars++;
            }
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }

        public static BigInteger NumberFromHex(string hex)
        {
            if (((hex.Length % 2) == 1) || hex[0] != '0')
            {
                hex = "0" + hex; // if the hex string doesnt start with 0, the parse will assume its negative
            }

            return BigInteger.Parse(hex, NumberStyles.HexNumber);
        }

        public static string HexFromNumber(BigInteger number, int length)
        {
            string hex = number.ToString("X");

            if (hex.Length <= 2 * length)
            {
                hex = (new string('0', 2 * length - hex.Length)) + hex;
            }
            else if (hex[0] == '0')
            {
                hex = hex.Substring(1);
            }
            else
            {
                throw new ArgumentException("number hex length is bigger than 2*length: " + number + ", length=" + length);
            }

            return hex;
        }

        public static byte[] StringFromNumber(BigInteger number, int length)
        {
            string hex = HexFromNumber(number, length);

            return BinaryFromHex(hex);
        }

        public static BigInteger NumberFromString(byte[] bytes)
        {
            string hex = HexFromBinary(bytes);

            return NumberFromHex(hex);
        }
    }
}
