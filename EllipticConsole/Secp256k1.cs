using System;
using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;

namespace EllipticConsole
{
    public class Secp256k1
    {
        public Elliptic Curve { get; set; }
        public EPoint G { get; set; }
        public BigInteger n { get; set; }
        private Int16 h { get; set; }

        public Secp256k1()
        {
            Curve = new Elliptic(0, 7, BigInteger.Parse("00FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFC2F", NumberStyles.HexNumber));
            G = new EPoint
            {
                X = BigInteger.Parse("79BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16F81798", NumberStyles.HexNumber),
                Y = BigInteger.Parse("483ADA7726A3C4655DA4FBFC0E1108A8FD17B448A68554199C47D08FFB10D4B8", NumberStyles.HexNumber)
            };
            n = BigInteger.Parse("00FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEBAAEDCE6AF48A03BBFD25E8CD0364141", NumberStyles.HexNumber);
            h = 01;
        }

        public byte[] ToByte(BigInteger key)
        {
            return Utils.BinaryAscii.StringFromNumber(key, this.n.ToString("X").Length / 2);
        }

        public BigInteger GetPrivateKey()
        {
            BigInteger privateKey = Utils.Integer.RandomBetween(1, this.n - 1);

            return privateKey;
        }

        public EPoint GetPublicKey(BigInteger privateKey)
        {
            var publicKey = this.Curve.Multiply(this.G, privateKey);

            return publicKey;
        }

        public (BigInteger r, BigInteger s) Sign(string message, BigInteger privateKey)
        {
            var moder = new Modulo(this.n);
            var hashMessage = Utils.Integer.Sha256(message);
            BigInteger z = BigInteger.Parse("0" + hashMessage, NumberStyles.HexNumber);

            var k = Utils.Integer.RandomBetween(BigInteger.One, this.n - 1);
            var inv_k = moder.Inv(k);

            while(inv_k == 0)
            {
                k = Utils.Integer.RandomBetween(BigInteger.One, this.n - 1);
                inv_k = moder.Inv(k);
            }

            var p = this.Curve.Multiply(this.G, k);

            var r = moder.Mod(p.X);
            var s = moder.Mod(inv_k * (z + privateKey * p.X));

            return (r, s);
        }

        public bool Verify(string message, BigInteger r, BigInteger s, EPoint publicKey)
        {
            var moder = new Modulo(this.n);
            string hashMessage = Utils.Integer.Sha256(message);
            BigInteger z = BigInteger.Parse("0" + hashMessage, NumberStyles.HexNumber);

            BigInteger inv_s = moder.Inv(s);

            EPoint u1 = this.Curve.Multiply(this.G, moder.Mod(inv_s * z));

            EPoint u2 = this.Curve.Multiply(publicKey, moder.Mod(inv_s * r));

            EPoint v = this.Curve.Add(u1, u2);

            return v.X == r;
        }
    }
}
