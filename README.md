ObscurCore
==========

A general purpose, easy to use, highly customisable and extensible encryption and data packaging library.

Thanks
------

To the Legion of the [Bouncy Castle](http://www.bouncycastle.org/).

![BouncyCastle C# logo](http://www.bouncycastle.org/images/csharp_logo.gif)

This work is built heavily on top of theirs, and likely would not have been possible without it. So - thank you very much for your hard work!

Also a big thanks to **Marc Gravell** for his excellent [protobuf-net](https://code.google.com/p/protobuf-net/) and its associated serialisation assembly precompiler. It is used extensively throughout (especially for the packaging features) and provides great flexibility and performance.


Why use this?
-------------

ObscurCore is designed to make cryptography accessible *and* customisable, supporting advanced schemes.
The packaging system included is a pertinent example of this, fusing together many primitives to create a strong cryptosystem without the developer requiring deep knowledge of the design of such systems. It hopefully also eliminates or at least greatly cuts down on duplication of effort, since many developers end up building similar systems ad-hoc.

If the features offered are insufficient for some particular circumstance, it offers a strong foundation for further extension; most components are abstracted in a way to support this.


How do I use it?
----------------

First up, a disclaimer:

**This library as it is currently should not be considered production ready and is a work in progress. It is currently in Beta.**

*****

Now that that's out of the way, here's how you can use it for doing some stuff!


ObscurCore Packaging System
---------------------------

**Please note that this part of the API is new and is subject to change on its way to a stable release. I'll try to minimise this, but the fact remains.**

This feature is an automated system that acts somewhat like an very paranoid archive format.
It allows you to bundle together collections of messages, data, and key agreements/actions.

The index of the archive is encrypted too, as with the actual contents. Each item has its own unique encryption configuration, so you can have plausible deniability by using different keys on different items, and/or including decoy/fake items if needed.
If deniability is not needed, one could use this feature by distributing a package to multiple recipients, with the different items encrypted with recipient-specific keys, so each sees only that which they are able to decrypt.

The index of the archive (where all the information about the items in the archive is kept), termed a *manifest* in ObscurCore nonclemature, is encrypted with a choice of schemes: 

+	Symmetric cipher (non-public key; participants must share key securely somehow)
+	UM1-hybrid (public key)
+	Curve25519-UM1-hybrid (public key)

The hybrid schemes derive a key from the public key cryptosystems, derive it further with a KDF, and uses this as the key for a symmetric cipher.
The symmetric-only scheme derives the supplied shared key further with a KDF, and uses this as the key for a symmetric cipher.

The collection of items in the package is termed the "payload". There is a choice of schemes:

+	Simple
+	Frameshift
+	Fabric

Simple just concatenates them together in *random* order.
Frameshift does the same but inserts *random* lengths of bytes before and after each item.
Fabric multiplexes *random*-length stripes of each item, mixing them all up, much like the Rubberhose file system.

... Obviously, if it were *really* random, you'd never get your data back. No - rather, "random" is the output of a CSPRNG.


All items and their configurations, along with the manifest and its configuration, are authenticated with a MAC in the **Encrypt-then-MAC (EtM)** scheme, which provides strong verification and minimum information leakage. Upon detection of any alteration, all operations are aborted.


Here's an example using the packager:

    using (var output = new MemoryStream()) {
    	var package = new Package(key);
    	package.AddFile(filePath);
    	package.Write(ms);
    }

The above...
*	Creates the package with frameshifting payload layout (default) and a SOSEMANUK-based CSPRNG
*	Sets up key confirmation for the manifest with BLAKE2B-256 (default)
*	Adds a file to a package with AES-256/CTR encryption (random key, default) and Poly1305-AES EtM authentication (random key and nonce, default)
*	Derives a package/manifest key (from the supplied one) with scrypt KDF
*	Writes it out to the output stream

Pretty easy huh?

All schemes offer key confirmation capability with a choice of algorithms (e.g. MAC, KDF, etc.)
If a package is recieived from a sender for which you hold multiple keys on file for (whatever kind they may be), all them will be verified with the key confirmation data (if present, which it is by default - generated automatically) to determine the correct one to proceed with.

Packages include the capability to communicate new keys (of any kind), or request invalidation of keys for subsequent communications. For example, you could send a symmetric key for manifests, so as to reduce overhead incurred by public key schemes.

More to come... it is designed to be a highly comprehensive system.


Functionality exposed through streams
-------------------------------------

**Please note the parameter closeOnDispose. If you test these methods using a MemoryStream, and the parameter set to true (by default or explicitly), the stream will be closed, and your data will be missing.**

### Encryption/decryption ###

	var config = SymmetricCipherConfigurationFactory.CreateBlockCipherConfiguration(SymmetricBlockCipher.Aes,
		BlockCipherMode.Ctr, BlockCipherPadding.None);
	using (var cs = new SymmetricCryptoStream(destStream, encrypting:true, config, key, closeOnDispose:false) ) {
		sourceStream.CopyTo(cs);
	}

These block ciphers are supported:

+	AES
+	Blowfish
+	Camellia
+	CAST-5
+	CAST-6
+	IDEA
+	NOEKEON
+	RC-6
+	Serpent
+	Twofish
+	Threefish

... in CTR, CBC mode (variety of paddings provided), CFB, and OFB.
Paddings available for CBC mode: ISO 10126-2, ISO/IEC 7816-4, PKCS7, TBC, and ANSI X.923.

And these stream ciphers:

+	HC-128
+	HC-256
+	Rabbit
+	RC-4 *[disabled; optionally included]*
+	Salsa20
+	ChaCha
+	XSalsa20
+	SOSEMANUK


### Hashing and MAC ###

	byte[] hash = null;
	using (var hs = new HashStream(destStream, writing:true, HashFunction.Blake2B256, hash, closeOnDispose:true) ) {
		sourceStream.CopyTo(cs);
	}

	byte[] mac = null;
	using (var ms = new MacStream(destStream, writing:true, MacFunction.BlakeB256, mac, key, salt, config:null, closeOnDispose:true) ) {
		sourceStream.CopyTo(cs);
	}

Here's all the hash/digest functions supported (*HashFunction* enumeration) :

+	BLAKE-2B-256 / 384 / 512
+	Keccak-224 / 256 / 384 / 512 (SHA-3-224 / 256 / 384 / 512)
+	RIPEMD-160
+	SHA-1
+	SHA-2-256 / 512
+	Tiger

And here's all the MAC functions (*MacFunction* enumeration) :

+	BLAKE-2B-256 / 384 / 512
+	Keccak-224 / 256 / 384 / 512 (SHA-3-224 / 256 / 384 / 512)
+	Skein
+	Poly1305
+	*CMAC*
+	*HMAC*

CMAC can use any symmetric block cipher (see above in Encryption section) with a block size of 64 or 128 bits. 
Poly1305 can use any symmetric block cipher (see above in Encryption section) with a block size of 128 bits.
HMAC can use any hash/digest function.


Primitives
----------

### Key derivation ###

	var config = new ScryptConfiguration {
		IterationPower = 16,
		Blocks = 8,
		Parallelism = 2
	};
	var configBytes = config.SerialiseDto();
	var derivedKey = Source.DeriveKeyWithKDF(KeyDerivationFunction.Scrypt, key, salt, outputSizeBits, configBytes);

These are in serious need of a convenience method. It's on the list.


### Key agreements ###

Please note that currently no 3-pass algorithms are implemented. Sorry. It's definitely a desired feature, for that sweet, sweet Perfect Forward Secrecy...
There are, however, implemented UM1-type agreements, which provide unilateral forward secrecy - which is much better than nothing.

There are no convenience methods for key agreements currently; they must be used manually by manipulating the primitives. Fortunately, they're pretty approachable for primitives - an example with *Curve25519-UM1*:

Creating keys:

	var entropy = new byte[32];
    StratCom.EntropySource.NextBytes(entropy);
    privateKeySender = Curve25519.CreatePrivateKey(entropy);
    publicKeySender = Curve25519.CreatePublicKey(privateKeySender);

    StratCom.EntropySource.NextBytes(entropy);
    privateKeyRecipient = Curve25519.CreatePrivateKey(entropy);
    publicKeyRecipient = Curve25519.CreatePublicKey(privateKeyRecipient);

And calculating shared secret:

    byte[] ephemeral;
    initiatorSS = Curve25519UM1Exchange.Initiate(_publicKeyRecipient, _privateKeySender, out eph);
	responderSS = Curve25519UM1Exchange.Respond(_publicKeySender, _privateKeyRecipient, eph);


### Signatures ###

No concrete implementation is yet in place. ECDSA is being added. 
The preferred example of this is Ed25519.
DSA proper (using RSA) will most likely not be added due to concerns with security and efficiency.
Watch this space.


Some words on Streams
---------------------

SymmetricCryptoStream **encrypts only when being written to**, and **decrypts only when being read from**. The other core stream types (for example, the HashStream) do not enforce directionality like this.

ObscurCore uses stream decorators for I/O, so anything that is derived from the abstract Stream class of the .NET BCL can be plugged into the constructor of an ObscurCore *SymmetricCryptoStream/HashStream/MacStream*. This means in practice pretty much anything, since pretty much anything can be serialised to a byte array.


Please note that ObscurCore's main stream classes **close on dispose by default**. This means the stream they were bound to on construction will be closed with it, when the wrapping ObscurCore stream is closed/disposed.
Don't make the mistake of thinking stuff isn't working when binding on a MemoryStream, and wondering why the data is missing afterward.

The main stream classes have a parameter, *closeOnDispose*, that controls this. Set it to **false** if you *don't* want them to close. They're set to *true* by default to try and ensure that if they're bound to a FileStream, the OS hooks get disposed of properly.

It's good practice to use the **using** block, because it calls Stream.Dispose() once you're done writing/reading. It's most important when writing, as block ciphers have different behaviour when writing the last block of data, so disposing the stream lets it know when to do this. If you don't, **you'll probably be missing the final block** (which might be *all* of your data if you only wrote a little, and are using a block cipher!) in the output. **This is a common mistake.**
If *using* isn't what you favour, then just call .Dispose() or .Close() when you're all done with the stream.


Recommendations
---------------

The author's recommendations, among block ciphers:
 
+	AES/CTR
+	Twofish/CTR
+	Serpent/CTR

and in stream ciphers:

+	HC-128
+	SOSEMANUK
+	XSalsa20

In hash and MAC functions:

+	BLAKE2B
+	Poly1305-AES
+	Keccak/SHA-3

In KDFs:

+	Scrypt

Advanced
--------

Want to play around with the cryptographic primitives instead? This is not recommended, but you can.
Have a look around the **Source** object. It provides easy instantiation and configuration of the most useful classes.

Some features of ObscurCore are only accessible this way currently, but that's soon to change. If you don't want to deal with low-level features, you shouldn't have to. That's the point of automation.

What's next?
------------

Mostly, the work to come is tidying up the API. Style of use will ideally become more consistent, and less contact with intermediate objects will be required.

The next major planned feature is automatic nonce management. 
The idea is that you simply pass in a collection that conforms to a particular interface (whether this is just a reference to a simple in-memory database that's repopulated from disk on startup, or a *real* database), ObscurCore checks this DB whenever you reference a key, and makes sure it doesn't use the same nonce again when it's using a cryptographic scheme for which this is essential to maintain its security assurances.

Any suggestions or code checkins are appreciated!

Where can I get more information?
---------------------------------

Check out the wiki. Hopefully I'll have filled it out some more. Or send me a message.