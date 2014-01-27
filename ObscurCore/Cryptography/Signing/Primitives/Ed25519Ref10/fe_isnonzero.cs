﻿using System;

// From Chaos.NaCl, authored by CodesInChaos (https://github.com/CodesInChaos/Chaos.NaCl)

namespace ObscurCore.Cryptography.Signing.Primitives.Ed25519Ref10
{
    internal static partial class FieldOperations
    {
        /*
        return 1 if f == 0
        return 0 if f != 0

        Preconditions:
           |f| bounded by 1.1*2^26,1.1*2^25,1.1*2^26,1.1*2^25,etc.
        */
        internal static int fe_isnonzero(ref FieldElement f)
        {
            FieldElement fr;
            fe_reduce(out fr, ref f);
            int differentBits = 0;
            differentBits |= fr.x0;
            differentBits |= fr.x1;
            differentBits |= fr.x2;
            differentBits |= fr.x3;
            differentBits |= fr.x4;
            differentBits |= fr.x5;
            differentBits |= fr.x6;
            differentBits |= fr.x7;
            differentBits |= fr.x8;
            differentBits |= fr.x9;
            return (int)((((uint)differentBits - 1) >> 31) ^ 1);
        }
    }
}