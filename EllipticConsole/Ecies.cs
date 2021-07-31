using System;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;

namespace EllipticConsole
{
    public class EciesResult
    {
        /// <summary>
        /// Encrypter public key
        /// </summary>
        public EPoint R { get; set; }

        /// <summary>
        /// Noi dung da ma hoa
        /// </summary>
        public byte[] CipherText { get; set; }

        /// <summary>
        /// Dung de verify
        /// </summary>
        public byte[] MAC { get; set; }

        /// <summary>
        /// AES IV
        /// </summary>
        public byte[] IV { get; set; }

        public EciesResult(EPoint r, byte[] cipherText, byte[] mac, byte[] iv)
        {
            this.R = r;
            this.CipherText = cipherText;
            this.MAC = mac;
            this.IV = iv;
        }
    }

    public class Ecies
    {
        public EciesResult Encrypt(EPoint receiverPublicKey, string message)
        {
            var secp256k1 = new Secp256k1();

            var privateKey = secp256k1.GetPrivateKey();
            // Send publickey with message
            var publicKey = secp256k1.GetPublicKey(privateKey);

            var s = secp256k1.Curve.Multiply(receiverPublicKey, privateKey).Combine();
            //1. Compress s
            //2. Hash SHA-512 s
            var shaM = new SHA512Managed();
            var hash = shaM.ComputeHash(s);

            var encryptionKey = new byte[32];
            Buffer.BlockCopy(hash, 0, encryptionKey, 0, 32);

            var macKey = new byte[32];
            Buffer.BlockCopy(hash, 0, macKey, 0, 32);

            byte[] encrypted;
            byte[] iv;
            byte[] mac;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = encryptionKey;
                //aesAlg.IV = IV;
                aesAlg.GenerateIV();
                iv = aesAlg.IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(message);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Initialize the keyed hash object.
            using (HMACSHA256 hmac = new HMACSHA256(macKey))
            {
                // Compute the hash of CipherText
                mac = hmac.ComputeHash(encrypted);
            }

            return new EciesResult(publicKey, encrypted, mac, iv);
        }

        public string Decrypt(BigInteger privateKey, EPoint ephemPublicKey, byte[] cipherText, byte[] iv)
        {
            var secp256k1 = new Secp256k1();
            var s = secp256k1.Curve.Multiply(ephemPublicKey, privateKey).Combine();
            //1. Compress s
            //2. Hash SHA-512 s
            var shaM = new SHA512Managed();
            var hash = shaM.ComputeHash(s);

            var encryptionKey = new byte[32];
            Buffer.BlockCopy(hash, 0, encryptionKey, 0, 32);

            var macKey = new byte[32];
            Buffer.BlockCopy(hash, 0, macKey, 0, 32);

            string decrypt;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = encryptionKey;
                //aesAlg.IV = IV;
                aesAlg.IV = iv;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            decrypt = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return decrypt;
        }
    }
}
