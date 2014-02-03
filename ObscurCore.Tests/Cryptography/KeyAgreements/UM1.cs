using System;
using System.Collections.Generic;
using NUnit.Framework;
using ObscurCore.Cryptography;
using ObscurCore.Cryptography.KeyAgreement;
using ObscurCore.Cryptography.KeyAgreement.Primitives;
using ObscurCore.Cryptography.Support;
using ObscurCore.DTO;

namespace ObscurCore.Tests.Cryptography.KeyAgreements
{
	[TestFixture]
	public class UM1Agreements
	{
		private readonly Dictionary<string, ECTestKPStore> _ecKeypairs = 
			new Dictionary<string, ECTestKPStore>();
		
		private class ECTestKPStore
		{
			public EcKeypair Initiator { get; set; }
			public EcKeypair Responder { get; set; }
		}
		
		[TestFixtureSetUp]
		public void Init () {		
			var curves = Enum.GetNames (typeof(BrainpoolEllipticCurve));
			for (var i = 1; i < curves.Length; i++) {
			    var domain = Source.GetEcDomainParameters(curves[i]);
				var kpInitiator = ECAgreementUtility.GenerateKeyPair (domain);
				var kpResponder = ECAgreementUtility.GenerateKeyPair (domain);
				
				var kpStore = new ECTestKPStore {
					Initiator = new EcKeypair {
						CurveProviderName = "Brainpool",
						CurveName = curves[i],
						EncodedPublicKey = ((ECPublicKeyParameters)kpInitiator.Public).Q.GetEncoded(),
						EncodedPrivateKey = ((ECPrivateKeyParameters)kpInitiator.Private).D.ToByteArray()
					},
					Responder = new EcKeypair {
						CurveProviderName = "Brainpool",
						CurveName = curves[i],
						EncodedPublicKey = ((ECPublicKeyParameters)kpResponder.Public).Q.GetEncoded(),
						EncodedPrivateKey = ((ECPrivateKeyParameters)kpResponder.Private).D.ToByteArray()
					}
				};
				
				_ecKeypairs.Add (curves [i], kpStore);
			}

            curves = Enum.GetNames (typeof(Sec2EllipticCurve));
            for (var i = 1; i < curves.Length; i++) {
			    var domain = Source.GetEcDomainParameters(curves[i]);
				var kpInitiator = ECAgreementUtility.GenerateKeyPair (domain);
				var kpResponder = ECAgreementUtility.GenerateKeyPair (domain);
				
				var kpStore = new ECTestKPStore {
					Initiator = new EcKeypair {
						CurveProviderName = "SEC",
						CurveName = curves[i],
						EncodedPublicKey = ((ECPublicKeyParameters)kpInitiator.Public).Q.GetEncoded(),
						EncodedPrivateKey = ((ECPrivateKeyParameters)kpInitiator.Private).D.ToByteArray()
					},
					Responder = new EcKeypair {
						CurveProviderName = "SEC",
						CurveName = curves[i],
						EncodedPublicKey = ((ECPublicKeyParameters)kpResponder.Public).Q.GetEncoded(),
						EncodedPrivateKey = ((ECPrivateKeyParameters)kpResponder.Private).D.ToByteArray()
					}
				};
				
				_ecKeypairs.Add (curves [i], kpStore);
			}

			var privEntropy = new byte[32];
			StratCom.EntropySource.NextBytes(privEntropy);
			var privateKeySender = Curve25519.CreatePrivateKey(privEntropy);
			var publicKeySender = Curve25519.CreatePublicKey(privateKeySender);

			StratCom.EntropySource.NextBytes(privEntropy);
			var privateKeyRecipient = Curve25519.CreatePrivateKey(privEntropy);
			var publicKeyRecipient = Curve25519.CreatePublicKey(privateKeyRecipient);

			_ecKeypairs.Add (DjbCurve.Curve25519.ToString (), new ECTestKPStore {
				Initiator = new EcKeypair {
					CurveProviderName = "DJB",
					CurveName = DjbCurve.Curve25519.ToString (),
					EncodedPublicKey = publicKeySender,
					EncodedPrivateKey = privateKeySender
				},
				Responder = new EcKeypair {
					CurveProviderName = "DJB",
					CurveName = DjbCurve.Curve25519.ToString (),
					EncodedPublicKey = publicKeyRecipient,
					EncodedPrivateKey = privateKeyRecipient
				}
			});
		}

		[Test()]
		public void UM1Exchange_Curve25519 () {
			DoUM1Exchange(_ecKeypairs[DjbCurve.Curve25519.ToString()]);
		}
		
		[Test()]
		public void UM1Exchange_BrainpoolP160r1 () {
			DoUM1Exchange(_ecKeypairs[BrainpoolEllipticCurve.BrainpoolP160r1.ToString()]);
		}

        [Test()]
		public void UM1Exchange_BrainpoolP160t1 () {
			DoUM1Exchange(_ecKeypairs[BrainpoolEllipticCurve.BrainpoolP160t1.ToString()]);
		}
		
		[Test()]
		public void UM1Exchange_BrainpoolP192r1 () {
			DoUM1Exchange(_ecKeypairs[BrainpoolEllipticCurve.BrainpoolP192r1.ToString()]);
		}

        [Test()]
		public void UM1Exchange_BrainpoolP192t1 () {
			DoUM1Exchange(_ecKeypairs[BrainpoolEllipticCurve.BrainpoolP192t1.ToString()]);
		}
		
        [Test()]
        public void UM1Exchange_BrainpoolP224r1 () {
			DoUM1Exchange(_ecKeypairs[BrainpoolEllipticCurve.BrainpoolP224r1.ToString()]);
        }

        [Test()]
		public void UM1Exchange_BrainpoolP224t1 () {
			DoUM1Exchange(_ecKeypairs[BrainpoolEllipticCurve.BrainpoolP224t1.ToString()]);
		}
		
		[Test()]
		public void UM1Exchange_BrainpoolP256r1 () {
			DoUM1Exchange(_ecKeypairs[BrainpoolEllipticCurve.BrainpoolP256r1.ToString()]);
		}

        [Test()]
		public void UM1Exchange_BrainpoolP256t1 () {
			DoUM1Exchange(_ecKeypairs[BrainpoolEllipticCurve.BrainpoolP256t1.ToString()]);
		}
		
		[Test()]
		public void UM1Exchange_BrainpoolP320r1 () {
			DoUM1Exchange(_ecKeypairs[BrainpoolEllipticCurve.BrainpoolP320r1.ToString()]);
		}

		[Test()]
		public void UM1Exchange_BrainpoolP320t1 () {
			DoUM1Exchange(_ecKeypairs[BrainpoolEllipticCurve.BrainpoolP320t1.ToString()]);
		}
		
        [Test()]
        public void UM1Exchange_BrainpoolP384r1 () {
			DoUM1Exchange(_ecKeypairs[BrainpoolEllipticCurve.BrainpoolP384r1.ToString()]);
        }

		[Test()]
		public void UM1Exchange_BrainpoolP384t1 () {
			DoUM1Exchange(_ecKeypairs[BrainpoolEllipticCurve.BrainpoolP384t1.ToString()]);
		}
		
		[Test()]
		public void UM1Exchange_BrainpoolP512r1 () {
			DoUM1Exchange(_ecKeypairs[BrainpoolEllipticCurve.BrainpoolP512r1.ToString()]);
		}

		[Test()]
		public void UM1Exchange_BrainpoolP512t1 () {
			DoUM1Exchange(_ecKeypairs[BrainpoolEllipticCurve.BrainpoolP512t1.ToString()]);
		}
		
        [Test()]
		public void UM1Exchange_Secp192k1 () {
			DoUM1Exchange(_ecKeypairs[Sec2EllipticCurve.Secp192k1.ToString()]);
		}

        [Test()]
		public void UM1Exchange_Secp192r1 () {
			DoUM1Exchange(_ecKeypairs[Sec2EllipticCurve.Secp192r1.ToString()]);
		}

        [Test()]
		public void UM1Exchange_Secp224k1 () {
			DoUM1Exchange(_ecKeypairs[Sec2EllipticCurve.Secp224k1.ToString()]);
		}

        [Test()]
		public void UM1Exchange_Secp224r1 () {
			DoUM1Exchange(_ecKeypairs[Sec2EllipticCurve.Secp224r1.ToString()]);
		}

        [Test()]
		public void UM1Exchange_Secp256k1 () {
			DoUM1Exchange(_ecKeypairs[Sec2EllipticCurve.Secp256k1.ToString()]);
		}

        [Test()]
		public void UM1Exchange_Secp256r1 () {
			DoUM1Exchange(_ecKeypairs[Sec2EllipticCurve.Secp256r1.ToString()]);
		}

        [Test()]
		public void UM1Exchange_Secp384r1 () {
			DoUM1Exchange(_ecKeypairs[Sec2EllipticCurve.Secp384r1.ToString()]);
		}

        [Test()]
		public void UM1Exchange_Secp521r1 () {
			DoUM1Exchange(_ecKeypairs[Sec2EllipticCurve.Secp521r1.ToString()]);
		}

		[Test()]
		public void UM1Exchange_Sect163r1 () {
			DoUM1Exchange(_ecKeypairs[Sec2EllipticCurve.Sect163r1.ToString()]);
		}

        [Test()]
		public void UM1Exchange_Sect163r2 () {
			DoUM1Exchange(_ecKeypairs[Sec2EllipticCurve.Sect163r2.ToString()]);
		}

		[Test()]
		public void UM1Exchange_Sect193r1 () {
			DoUM1Exchange(_ecKeypairs[Sec2EllipticCurve.Sect193r1.ToString()]);
		}

		[Test()]
		public void UM1Exchange_Sect192r2 () {
			DoUM1Exchange(_ecKeypairs[Sec2EllipticCurve.Sect193r2.ToString()]);
		}

        [Test()]
        public void UM1Exchange_Sect233k1 () {
			DoUM1Exchange(_ecKeypairs[Sec2EllipticCurve.Sect233k1.ToString()]);
        }

        [Test()]
		public void UM1Exchange_Sect233r1 () {
			DoUM1Exchange(_ecKeypairs[Sec2EllipticCurve.Sect233r1.ToString()]);
		}

        [Test()]
		public void UM1Exchange_Sect239k1 () {
			DoUM1Exchange(_ecKeypairs[Sec2EllipticCurve.Sect239k1.ToString()]);
        }

        [Test()]
		public void UM1Exchange_Sect283k1 () {
			DoUM1Exchange(_ecKeypairs[Sec2EllipticCurve.Sect283k1.ToString()]);
		}

		[Test()]
		public void UM1Exchange_Sect283r1 () {
			DoUM1Exchange(_ecKeypairs[Sec2EllipticCurve.Sect283r1.ToString()]);
		}

        [Test()]
        public void UM1Exchange_Sect409k1 () {
			DoUM1Exchange(_ecKeypairs[Sec2EllipticCurve.Sect409k1.ToString()]);
        }

        [Test()]
		public void UM1Exchange_Sect409r1 () {
			DoUM1Exchange(_ecKeypairs[Sec2EllipticCurve.Sect409r1.ToString()]);
		}

        [Test()]
        public void UM1Exchange_Sect571k1 () {
			DoUM1Exchange(_ecKeypairs[Sec2EllipticCurve.Sect571k1.ToString()]);
        }

        [Test()]
		public void UM1Exchange_Sect571r1 () {
			DoUM1Exchange(_ecKeypairs[Sec2EllipticCurve.Sect571r1.ToString()]);
		}

		private static void DoUM1Exchange(ECTestKPStore keypair) {
			var sw = System.Diagnostics.Stopwatch.StartNew ();

			EcKeyConfiguration ephemeral;
			var initiatorSS = UM1Exchange.Initiate(keypair.Responder.ExportPublicKey(),
				keypair.Initiator.GetPrivateKey(), out ephemeral);
			var responderSS = UM1Exchange.Respond(keypair.Initiator.ExportPublicKey(), 
				keypair.Responder.GetPrivateKey(), ephemeral);

			sw.Stop ();

			Assert.IsTrue(initiatorSS.SequenceEqual(responderSS));
			Assert.Pass ("Key agreement completed succesfully in {0} milliseconds.\nKey = {1}", 
				sw.ElapsedMilliseconds, initiatorSS.ToHexString ());
		}
	}
}

