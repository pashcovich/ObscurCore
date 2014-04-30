﻿//
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

namespace ObscurCore.DTO
{
    /// <summary>
    /// Defines data that must be available in manifest cryptography scheme configurations.
    /// </summary>
	public interface IManifestCryptographySchemeConfiguration
    {
		/// <summary>
		/// Configuration for the key confirmation scheme used to validate the existence and 
		/// validity of keying material at respondent's side without disclosing the key itself.
		/// </summary>
		VerificationFunctionConfiguration KeyConfirmation { get; }

		/// <summary>
		/// Output of the key confirmation scheme given correct input data.
		/// </summary>
		byte[] KeyConfirmationVerifiedOutput { get; }

		/// <summary>
		/// Configuration for the scheme used to derive a key from the shared secret.
		/// </summary>
		KeyDerivationConfiguration KeyDerivation { get; }

		/// <summary>
		/// Configuration of the cipher used in encryption of the manifest.
		/// </summary>
        CipherConfiguration SymmetricCipher { get; }

		/// <summary>
		/// Configuration for the authentication of the manifest and cipher configuration.
		/// </summary>
		VerificationFunctionConfiguration Authentication { get; }

		/// <summary>
		/// Output of the authentication scheme given correct input data.
		/// </summary>
		byte[] AuthenticationVerifiedOutput { get; }
    }
}
