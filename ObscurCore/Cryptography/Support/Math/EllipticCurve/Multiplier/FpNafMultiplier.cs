namespace ObscurCore.Cryptography.Support.Math.EllipticCurve.Multiplier
{
	/**
	* Class implementing the NAF (Non-Adjacent Form) multiplication algorithm.
	*/
	internal class FpNafMultiplier
		: ECMultiplier
	{
		/**
		* D.3.2 pg 101
		* @see ObscurCore.Cryptography.BouncyCastle.math.ec.multiplier.ECMultiplier#multiply(ObscurCore.Cryptography.BouncyCastle.math.ec.ECPoint, java.math.BigInteger)
		*/
		public ECPoint Multiply(ECPoint p, BigInteger k, PreCompInfo preCompInfo)
		{
			// TODO Probably should try to add this
			// BigInteger e = k.Mod(n); // n == order of p
			BigInteger e = k;
			BigInteger h = e.Multiply(BigInteger.Three);

			ECPoint neg = p.Negate();
			ECPoint R = p;

			for (int i = h.BitLength - 2; i > 0; --i)
			{             
				R = R.Twice();

				bool hBit = h.TestBit(i);
				bool eBit = e.TestBit(i);

				if (hBit != eBit)
				{
					R = R.Add(hBit ? p : neg);
				}
			}

			return R;
		}
	}
}
