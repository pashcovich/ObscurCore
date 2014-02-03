//
//  Copyright 2014  Matthew Ducker
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using System;
using NUnit.Framework;

using ObscurCore.Cryptography.KeyAgreement;
using ObscurCore.Cryptography.KeyAgreement.Primitives;
using ObscurCore.Cryptography.Authentication;

namespace ObscurCore.Tests.Cryptography.KeyAgreements
{
	[TestFixture]
	public class JPake
	{
		[Test]
		public void Secp521r1_Blake2B512() {
			TestECJPake (Sec2EllipticCurve.Secp521r1.ToString (), HashFunction.Blake2B512);
		}

		[Test]
		public void Brainpool256t1_Blake2B256() {
			TestECJPake (BrainpoolEllipticCurve.BrainpoolP256t1.ToString (), HashFunction.Blake2B256);
		}

		[Test]
		public void Brainpool512t1_Blake2B512() {
			TestECJPake (BrainpoolEllipticCurve.BrainpoolP512t1.ToString (), HashFunction.Blake2B512);
		}

		private void TestECJPake(string curveName, HashFunction hashFunction) {
			const string password = "green eggs and ham";
			var ecParams = Source.GetEcDomainParameters (curveName);
			var digest = Source.CreateHashPrimitive (hashFunction);

			var alice = new EcJpakeParticipant ("ObscurCore_P0", password, ecParams, digest, StratCom.EntropySource);
			var bob = new EcJpakeParticipant ("ObscurCore_P1", password, ecParams, digest, StratCom.EntropySource);

			var sw = System.Diagnostics.Stopwatch.StartNew ();

			var aliceR1 = alice.CreateRound1ToSend ();
			var bobR1 = bob.CreateRound1ToSend ();

			alice.ValidateRound1Received (bobR1);
			bob.ValidateRound1Received (aliceR1);

			var aliceR2 = alice.CreateRound2ToSend ();
			var bobR2 = bob.CreateRound2ToSend ();

			alice.ValidateRound2Received (bobR2);
			bob.ValidateRound2Received (aliceR2);

			var aliceKey = alice.CalculateKeyingMaterial ();
			var bobKey = bob.CalculateKeyingMaterial ();

			sw.Stop ();

			Assert.IsTrue (aliceKey.SequenceEqual (bobKey), "Keys produced ARE NOT equal! Protocol implementation is broken.");

			Assert.Pass ("J-PAKE protocol completed successfuly in {0} milliseconds.\nKey = {1}", sw.ElapsedMilliseconds, aliceKey.ToHexString ());
		}



	}
}

