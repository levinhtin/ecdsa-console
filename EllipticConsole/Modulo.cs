using System;
using System.Numerics;

namespace EllipticConsole
{
    public class Modulo
    {
        private BigInteger P { get; set; }
        public Modulo(BigInteger p)
        {
            this.P = p;
        }

        public BigInteger Mod(BigInteger n)
        {
            return (n % this.P + this.P) % this.P;
        }

        //public BigInteger Inv2(BigInteger n)
        //{
        //    n %= this.P;
        //    for (var i = 1; i < this.P; i++)
        //    {
        //        if ((n * i) % this.P == 1)
        //        {
        //            return i;
        //        }
        //    }

        //    return 0; // not exist
        //}

        public BigInteger Inv(BigInteger x)
        {
            //Extended Euclidean Algorithm.It's the 'division' in elliptic curves

            //:param x: Divisor
            //:param n: Mod for division
            //:return: Value representing the division

            if (x.IsZero)
            {
                return 0;
            }

            BigInteger lm = BigInteger.One;
            BigInteger hm = BigInteger.Zero;
            BigInteger low = this.Mod(x);
            BigInteger high = this.P;
            BigInteger r, nm, newLow;

            while (low > 1)
            {
                r = high / low;

                nm = hm - (lm * r);
                newLow = high - (low * r);

                high = low;
                hm = lm;
                low = newLow;
                lm = nm;
            }

            return this.Mod(lm);
        }

        public BigInteger Add(BigInteger a, BigInteger b)
        {
            return this.Mod(a + b);
        }

        public BigInteger Subtract(BigInteger a, BigInteger b)
        {
            return this.Mod(a - b);
        }

        public BigInteger Multiply(BigInteger a, BigInteger b)
        {
            return this.Mod(a * b);
        }

        public BigInteger Power(BigInteger a, BigInteger b)
        {
            var x = new BigInteger(1);
            while (b > 0)
            {
                if (a == 0)
                {
                    return 0;
                }
                if (b % 2 == 1)
                {
                    x = this.Multiply(x, a);
                }
                b = b / 2;
                a = this.Multiply(a, a);
            }

            return x;
        }

        public BigInteger Divide(BigInteger c, BigInteger a)
        {
            var ap = this.Power(a, this.P - 2);
            return this.Mod(this.Multiply(c, ap));
        }

        //public BigInteger SquareRoots(BigInteger k)
        //{
        //    BigInteger p1 = 0;
        //    p1 = p1 || (this.P - 1) / 2;
        //    if (this.power(k, this.p1) !== 1n) {
        //                throw 'no integer square root'
        //    }
        //            this.p2 = this.p2 || (this.p + 1n) / 4n
        //    const root = this.power(k, this.p2)
        //    const negativeRoot = this.p - root

        //    return [root, negativeRoot]
        //}
    }
}
