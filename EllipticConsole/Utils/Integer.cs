using System;
using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace EllipticConsole.Utils
{
    public class Integer
    {
        public static string Sha256(string message)
        {
            byte[] bytes;

            using (SHA256 sha256Hash = SHA256.Create())
            {
                bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(message));
            }

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }

            return builder.ToString();
        }

        public static BigInteger RandomBetween(BigInteger minimum, BigInteger maximum)
        {
            if (maximum < minimum)
            {
                throw new ArgumentException("maximum must be greater than minimum");
            }

            BigInteger range = maximum - minimum;

            Tuple<int, BigInteger> response = CalculateParameters(range);
            int bytesNeeded = response.Item1;
            BigInteger mask = response.Item2;

            byte[] randomBytes = new byte[bytesNeeded];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(randomBytes);
            }

            BigInteger randomValue = new BigInteger(randomBytes);

            /* We apply the mask to reduce the amount of attempts we might need
                * to make to get a number that is in range. This is somewhat like
                * the commonly used 'modulo trick', but without the bias:
                *
                *   "Let's say you invoke secure_rand(0, 60). When the other code
                *    generates a random integer, you might get 243. If you take
                *    (243 & 63)-- noting that the mask is 63-- you get 51. Since
                *    51 is less than 60, we can return this without bias. If we
                *    got 255, then 255 & 63 is 63. 63 > 60, so we try again.
                *
                *    The purpose of the mask is to reduce the number of random
                *    numbers discarded for the sake of ensuring an unbiased
                *    distribution. In the example above, 243 would discard, but
                *    (243 & 63) is in the range of 0 and 60."
                *
                *   (Source: Scott Arciszewski)
                */

            randomValue &= mask;

            if (randomValue <= range)
            {
                /* We've been working with 0 as a starting point, so we need to
                    * add the `minimum` here. */
                return minimum + randomValue;
            }

            /* Outside of the acceptable range, throw it away and try again.
                * We don't try any modulo tricks, as this would introduce bias. */
            return RandomBetween(minimum, maximum);
        }

        private static Tuple<int, BigInteger> CalculateParameters(BigInteger range)
        {
            int bitsNeeded = 0;
            int bytesNeeded = 0;
            BigInteger mask = new BigInteger(1);

            while (range > 0)
            {
                if (bitsNeeded % 8 == 0)
                {
                    bytesNeeded += 1;
                }

                bitsNeeded++;

                mask = (mask << 1) | 1;

                range >>= 1;
            }

            return Tuple.Create(bytesNeeded, mask);
        }
    }
}
