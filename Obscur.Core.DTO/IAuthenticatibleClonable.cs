#region License

// 	Copyright 2013-2014 Matthew Ducker
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

namespace Obscur.Core.DTO
{
    /// <summary>
    ///     Interface for objects required to produce clones lacking any data 
    ///     dependent on post-authentication state/data.
    /// </summary>
    /// <typeparam name="T">Object to clone.</typeparam>
	public interface IAuthenticatibleClonable<T>
	{
        /// <summary>
        ///     Clones this <typeparamref name="T"/>, excluding any data  
        ///     dependent on post-authentication state/data.
        /// </summary>
        /// <returns>Authenticable object clone.</returns>
		T CreateAuthenticatibleClone();
	}
}
