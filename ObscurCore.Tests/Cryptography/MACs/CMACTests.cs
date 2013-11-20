﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ObscurCore.Cryptography;

namespace ObscurCore.Tests.Cryptography.MACs
{
    class CMACTests : MACTestBase
    {
        const MACFunctions function = MACFunctions.CMAC;

        public CMACTests() {
            SetRandomFixtureParameters(128);
        }

        [Test]
        public void CMAC_AES () {
            RunMACTest(function, Encoding.UTF8.GetBytes(SymmetricBlockCiphers.AES.ToString()));
        }

        [Test]
        public void CMAC_Blowfish () {
            RunMACTest(function, Encoding.UTF8.GetBytes(SymmetricBlockCiphers.Blowfish.ToString()));
        }

        [Test]
        public void CMAC_Camellia () {
            RunMACTest(function, Encoding.UTF8.GetBytes(SymmetricBlockCiphers.Camellia.ToString()), null, CreateRandomBytes(256));
        }

        [Test]
        public void CMAC_CAST5 () {
            RunMACTest(function, Encoding.UTF8.GetBytes(SymmetricBlockCiphers.CAST5.ToString()));
        }

        [Test]
        public void CMAC_CAST6 () {
             RunMACTest(function, Encoding.UTF8.GetBytes(SymmetricBlockCiphers.CAST6.ToString()), null, CreateRandomBytes(256));
        }
#if(INCLUDE_GOST28147)
        [Test]
        public void CMAC_GOST28147 () {
            RunMACTest(function, Encoding.UTF8.GetBytes(SymmetricBlockCiphers.GOST28147.ToString()), null, CreateRandomBytes(256));
        }
#endif
        [Test]
        public void CMAC_IDEA () {
            RunMACTest(function, Encoding.UTF8.GetBytes(SymmetricBlockCiphers.CAST5.ToString()));
        }

        [Test]
        public void CMAC_NOEKEON () {
            RunMACTest(function, Encoding.UTF8.GetBytes(SymmetricBlockCiphers.CAST5.ToString()));
        }

        [Test]
        public void CMAC_RC6 () {
            RunMACTest(function, Encoding.UTF8.GetBytes(SymmetricBlockCiphers.CAST5.ToString()), null, CreateRandomBytes(256));
        }
#if(INCLUDE_RIJNDAEL)
        [Test]
        public void CMAC_Rijndael () {
            RunMACTest(function, Encoding.UTF8.GetBytes(SymmetricBlockCiphers.CAST5.ToString()));
        }
#endif
        [Test]
        public void CMAC_Serpent () {
            RunMACTest(function, Encoding.UTF8.GetBytes(SymmetricBlockCiphers.CAST5.ToString()), null, CreateRandomBytes(256));
        }

        [Test]
        public void CMAC_TripleDES () {
            RunMACTest(function, Encoding.UTF8.GetBytes(SymmetricBlockCiphers.CAST5.ToString()), null, CreateRandomBytes(192));
        }

        [Test]
        public void CMAC_Twofish () {
            RunMACTest(function, Encoding.UTF8.GetBytes(SymmetricBlockCiphers.CAST5.ToString()), null, CreateRandomBytes(256));
        }
    }
}