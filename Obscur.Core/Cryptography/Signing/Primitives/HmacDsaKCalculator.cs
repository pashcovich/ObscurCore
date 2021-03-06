﻿#region License

// 	Copyright 2014-2014 Matthew Ducker
// 	
// 	Licensed under the Apache License, Version 2.0 (the "License");
// 	you may not use this file except in compliance with the License.
// 	
// 	You may obtain a copy of the License at
// 		
// 		http://www.apache.org/licenses/LICENSE-2.0
// 	
// 	Unless required by applicable law or agreed to in writing, software
// 	distributed under the License is distributed on an "AS IS" BASIS,
// 	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// 	See the License for the specific language governing permissions and 
// 	limitations under the License.

#endregion

using System;
using Obscur.Core.Cryptography.Authentication;
using Obscur.Core.Cryptography.Authentication.Primitives;
using Obscur.Core.Cryptography.Entropy;
using Obscur.Core.Cryptography.Support.Math;

namespace Obscur.Core.Cryptography.Signing.Primitives
{
    /// <summary>
    ///     A deterministic K-value calculator for use with DSA-type schemes. 
    ///     Recommended to use instead of <see cref="RandomDsaKCalculator"/>, wherever the use-case allows for it.
    /// </summary>
    /// <remarks>
    ///     Reduces reliance on a secure, reliable, and high quality entropy source - 
    ///     only true entropy is required for key generation. 
    ///     Signature generation may simply use a derived entropy from this 
    ///     deterministic generation scheme. Security properties are very likely 
    ///     improved for doing so, due to uncertain trust in entropy sources, 
    ///     and even if not, high-quality entropy being needlessly consumed is avoided. 
    ///     Happily, compatibility is is no way affected by using this modification of the [EC]DSA specification - 
    ///     to a 3rd party consuming the actual produced values, there is no practical or perceivable difference.
    ///     <para>Based on the algorithm in section 3.2 of RFC 6979.</para>
    /// </remarks>
    /// <seealso cref="ECDsaSigner"/>
    public class HmacDsaKCalculator
        : IDsaKCalculator
    {
        private readonly Hmac _hmac;
        private readonly byte[] _k;
        private readonly byte[] _v;

        private BigInteger _n;

        /**
         * Base constructor.
         *
         * @param digest digest to build the HMAC on.
         */

        public HmacDsaKCalculator(IHash digest)
        {
            this._hmac = new Hmac(digest);
            this._v = new byte[_hmac.OutputSize];
            this._k = new byte[_hmac.OutputSize];
        }

        public virtual bool IsDeterministic
        {
            get { return true; }
        }

        public virtual void Init(BigInteger n, CsRng random)
        {
            throw new InvalidOperationException("Operation not supported/appropriate.");
        }


        public void Init(BigInteger n, BigInteger d, byte[] message)
        {
            this._n = n;

            _v.Fill_NoChecks(0x01, 0, _v.Length);
            _k.Fill_NoChecks(0x00, 0, _k.Length);

            var x = new byte[(n.BitLength + 7) / 8];
            byte[] dVal = d.ToByteArrayUnsigned();

            Array.Copy(dVal, 0, x, x.Length - dVal.Length, dVal.Length);

            var m = new byte[(n.BitLength + 7) / 8];

            BigInteger mInt = BitsToInt(message);

            if (mInt.CompareTo(n) >= 0) {
                mInt = mInt.Subtract(n);
            }

            byte[] mVal = mInt.ToByteArrayUnsigned();

            Array.Copy(mVal, 0, m, m.Length - mVal.Length, mVal.Length);

            _hmac.Init(_k);

            _hmac.BlockUpdate(_v, 0, _v.Length);
            _hmac.Update(0x00);
            _hmac.BlockUpdate(x, 0, x.Length);
            _hmac.BlockUpdate(m, 0, m.Length);

            _hmac.DoFinal(_k, 0);

            _hmac.Init(_k);

            _hmac.BlockUpdate(_v, 0, _v.Length);

            _hmac.DoFinal(_v, 0);

            _hmac.BlockUpdate(_v, 0, _v.Length);
            _hmac.Update(0x01);
            _hmac.BlockUpdate(x, 0, x.Length);
            _hmac.BlockUpdate(m, 0, m.Length);

            _hmac.DoFinal(_k, 0);

            _hmac.Init(_k);

            _hmac.BlockUpdate(_v, 0, _v.Length);

            _hmac.DoFinal(_v, 0);
        }

        public virtual BigInteger NextK()
        {
            var t = new byte[((_n.BitLength + 7) / 8)];

            for (;;) {
                int tOff = 0;

                while (tOff < t.Length) {
                    _hmac.BlockUpdate(_v, 0, _v.Length);

                    _hmac.DoFinal(_v, 0);

                    int len = Math.Min(t.Length - tOff, _v.Length);
                    Array.Copy(_v, 0, t, tOff, len);
                    tOff += len;
                }

                BigInteger k = BitsToInt(t);

                if (k.SignValue > 0 && k.CompareTo(_n) < 0) {
                    return k;
                }

                _hmac.BlockUpdate(_v, 0, _v.Length);
                _hmac.Update(0x00);

                _hmac.DoFinal(_k, 0);

                _hmac.Init(_k);

                _hmac.BlockUpdate(_v, 0, _v.Length);

                _hmac.DoFinal(_v, 0);
            }
        }

        private BigInteger BitsToInt(byte[] t)
        {
            var v = new BigInteger(1, t);

            if (t.Length * 8 > _n.BitLength) {
                v = v.ShiftRight(t.Length * 8 - _n.BitLength);
            }

            return v;
        }
    }
}
