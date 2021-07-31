using System;
using System.Numerics;

namespace EllipticConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //var g = new EPoint { X = 16, Y = 5 };
            //var elliptic = new Elliptic(9, 17, 100000000057);

            //var q = elliptic.Multiply(g, 100000000019);
            //var qr = elliptic.MultiplyRecursive(g, 100000000019);

            //var q1 = elliptic.Double(g.X, g.Y);
            //var q2 = elliptic.Add(q1.X, q1.Y, 19, 20);

            //Console.WriteLine($"Multiply: {q.X} {q.Y}");
            //Console.WriteLine($"Multiply Recursive: {qr.X} {qr.Y}");
            //Console.WriteLine($"Double G: {q1.X} {q1.Y}");
            //Console.WriteLine($"G + P: {q2.X} {q2.Y}");

            var secp256k1 = new Secp256k1();

            //var doub = secp256k1.Curve.Double(secp256k1.G.X, secp256k1.G.Y);
            //Console.WriteLine($"Double G: {doub.X} {doub.Y}");

            var privateKey = secp256k1.GetPrivateKey();
            var publicKey = secp256k1.GetPublicKey(privateKey);

            Console.WriteLine($"PrivateKey: {privateKey}");
            Console.WriteLine($"PrivateKey Hex: {Utils.BinaryAscii.HexFromNumber(privateKey, secp256k1.n.ToString("X").Length / 2)}");
            Console.WriteLine($"PublicKey: {publicKey.X}, {publicKey.Y}");

            var message = "hello world";
            var sign = secp256k1.Sign(message, privateKey);

            Console.WriteLine($"Sign: {sign.r}, {sign.s}");
            Console.WriteLine($"Verify: {secp256k1.Verify(message, sign.r, sign.s, publicKey)}");

            var ecies = new Ecies();
            var encrypt = ecies.Encrypt(publicKey, message);

            var decrypt = ecies.Decrypt(privateKey, encrypt.R, encrypt.CipherText, encrypt.IV);

            Console.WriteLine(decrypt);
        }
    }
}
