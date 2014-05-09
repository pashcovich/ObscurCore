//  Copyright 2013  Matthew Ducker
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

using ObscurCore.Cryptography.Ciphers.Stream.Primitives;
using ObscurCore.DTO;

namespace ObscurCore.Cryptography.Entropy.Primitives
{
    /// <summary>
    /// Generates deterministic cryptographically secure pseudorandom number sequence 
    /// using internal Salsa20 stream cipher.
    /// </summary>
    public sealed class Salsa20Generator : StreamCsprng
    {
		public Salsa20Generator(StreamCipherCsprngConfiguration config) : base(new Salsa20Engine(), config) {
			Cipher.Init(true, Config.Key, Config.Nonce);
        }

        public Salsa20Generator(byte[] config) : base(new Salsa20Engine(), config) {
			Cipher.Init(true, Config.Key, Config.Nonce);
        }
    }
}
