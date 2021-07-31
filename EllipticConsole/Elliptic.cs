using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;

namespace EllipticConsole
{
    public class EPoint
    {
        public BigInteger X { get; set; }
        public BigInteger Y { get; set; }

        private byte[] CombineByteArrays(List<byte[]> byteArrayList)
        {
            int totalLength = 0;
            foreach (byte[] bytes in byteArrayList)
            {
                totalLength += bytes.Length;
            }

            byte[] combined = new byte[totalLength];
            int consumedLength = 0;

            foreach (byte[] bytes in byteArrayList)
            {
                Array.Copy(bytes, 0, combined, consumedLength, bytes.Length);
                consumedLength += bytes.Length;
            }

            return combined;
        }

        public byte[] Combine(bool encoded = false)
        {
            byte[] xString = Utils.BinaryAscii.StringFromNumber(this.X, 32);
            byte[] yString = Utils.BinaryAscii.StringFromNumber(this.Y, 32);

            if (encoded)
            {
                return CombineByteArrays(new List<byte[]>
                {
                    Utils.BinaryAscii.BinaryFromHex("00"),
                    Utils.BinaryAscii.BinaryFromHex("04"),
                    xString,
                    yString
                });
            }
            
            return CombineByteArrays(new List<byte[]>
            {
                xString,
                yString
            });
        }
    }

    public class Elliptic
    {
        public BigInteger A { get; set; }
        public BigInteger B { get; set; }
        public BigInteger P { get; set; }
        private Modulo moder;

        public Elliptic(BigInteger a, BigInteger b, BigInteger p)
        {
            this.A = a;
            this.B = b;
            this.P = p;
            this.moder = new Modulo(p);
        }

        public EPoint Add(EPoint p, EPoint q)
        {
            if (p.X == q.X && p.Y == q.Y)
            {
                return this.Double(p);
            }

            var my = this.moder.Subtract(p.Y, q.Y);
            var mx = this.moder.Subtract(p.X, q.X);
            var m = this.moder.Divide(my, mx);

            var x3 = this.moder.Subtract(this.moder.Subtract(this.moder.Multiply(m, m), p.X), q.X);
            var y3 = this.moder.Subtract(this.moder.Multiply(m, this.moder.Subtract(p.X, x3)), p.Y);
            var p3 = new EPoint { X = this.moder.Mod(x3), Y = this.moder.Mod(y3) };

            return p3;
        }

        public EPoint Double(EPoint p)
        {
            var mn = (3 * (p.X * p.X)) + this.A;
            var md = 2 * p.Y;
            var m = this.moder.Divide(mn, md);

            var x3 = this.moder.Mod((m * m) - p.X * 2);
            var y3 = this.moder.Mod(m * (p.X - x3) - p.Y);
            var p3 = new EPoint { X = x3, Y = y3 };

            return p3;
        }

        //public EPoint Multiply(EPoint p, BigInteger s)
        //{
        //    var q = new EPoint { X = p.X, Y = p.Y };

        //    if (s == 1)
        //    {
        //        return q;
        //    }

        //    var bArr = s.ToBinaryArray();

        //    for (var i = 1; i < bArr.Length; i++)
        //    {
        //        q = this.Double(q.X, q.Y);
        //        if (bArr[i] == 1)
        //        {
        //            q = this.Add(p.X, p.Y, q.X, q.Y);
        //        }
        //    }

        //    return q;
        //}

        public EPoint Multiply(EPoint p, BigInteger s)
        {
            if (s <= 0)
            {
                return null;
            }

            if (s == 1)
            {
                return p;
            }

            if (s % 2 == 1)
            {
                return this.Add(p, Multiply(p, s - 1));
            }
            else
            {
                return this.Multiply(this.Double(p), s / 2);
            }
        }
    }

    

    public static class BigIntegerHelper
    {
        public static byte[] ToBinaryArray(this BigInteger bigint)
        {
            var bytes = bigint.ToByteArray();
            var index = bytes.Length - 1;
            var binarystr = Convert.ToString(bytes[index], 2);

            var byteArr = new Byte[binarystr.Length];
            for (var i = 0; i < binarystr.Length; i++)
            {
                byteArr[i] = byte.Parse(binarystr[i].ToString());
            }

            //return base2.ToString();
            return byteArr;
        }
    }
}


