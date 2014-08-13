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

using System;
using System.Linq;
using ProtoBuf;

namespace ObscurCore.DTO
{
    /// <summary>
    ///     Configuration of a function or scheme used for checking the authenticity 
    ///     (e.g. MAC functions) and/or integrity (e.g. hash functions) of data.
    /// </summary>
    [ProtoContract]
    public class AuthenticationFunctionConfiguration : IDataTransferObject, 
        ICloneableSafely<AuthenticationFunctionConfiguration>, IEquatable<AuthenticationFunctionConfiguration>, IAuthenticationFunctionConfiguration
    {
        /// <summary>
        ///     Category/type of the function primitive, e.g. Digest, MAC, or KDF.
        /// </summary>
        [ProtoMember(1, IsRequired = true)]
        public string FunctionType { get; set; }

        /// <summary>
        ///     Name of the function used to verify some data (e.g. a key, a payload item, etc.).
        ///     This may be a key derivation function, MAC function, hash function, etc.
        /// </summary>
        [ProtoMember(2, IsRequired = true)]
        public string FunctionName { get; set; }

        /// <summary>
        ///     Configuration for the verification function, where applicable.
        /// </summary>
        /// <remarks>Format of the configuration is that of the consuming type.</remarks>
        [ProtoMember(3, IsRequired = false)]
        public byte[] FunctionConfiguration { get; set; }

        /// <summary>
        ///     Size of the key in bits for the verification function, where applicable.
        /// </summary>
        [ProtoMember(4, IsRequired = false)]
        public int? KeySizeBits { get; set; }

        /// <summary>
        ///     Salt for the verification function, where applicable.
        /// </summary>
        [ProtoMember(5, IsRequired = false)]
        public byte[] Nonce { get; set; }

        /// <summary>
        ///     Salt for the verification function, where applicable.
        /// </summary>
        [ProtoMember(6, IsRequired = false)]
        public byte[] Salt { get; set; }

        /// <summary>
        ///     Additional data for the verification function, where applicable.
        /// </summary>
        [ProtoMember(7, IsRequired = false)]
        public byte[] AdditionalData { get; set; }

        /// <inheritdoc />
        public bool Equals(AuthenticationFunctionConfiguration other)
        {
            if (ReferenceEquals(null, other)) {
                return false;
            }
            if (ReferenceEquals(this, other)) {
                return true;
            }
            return String.Equals(FunctionName, other.FunctionName, StringComparison.OrdinalIgnoreCase) && 
                FunctionConfiguration != null ? 
                FunctionConfiguration.SequenceEqualShortCircuiting(other.FunctionConfiguration) : Salt != null ? 
                Salt.SequenceEqual(other.Salt) : AdditionalData == null || AdditionalData.SequenceEqualShortCircuiting(other.AdditionalData);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            if (obj.GetType() != GetType()) {
                return false;
            }
            return Equals((AuthenticationFunctionConfiguration) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked {
                int hashCode = FunctionType.GetHashCode();
                hashCode = (hashCode * 397) ^ FunctionName.ToUpperInvariant().GetHashCode();
                hashCode = (hashCode * 397) ^ (FunctionConfiguration != null ? FunctionConfiguration.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (KeySizeBits.HasValue ? KeySizeBits.Value.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Nonce != null ? Nonce.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Salt != null ? Salt.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AdditionalData != null ? AdditionalData.GetHashCode() : 0);
                return hashCode;
            }
        }

        /// <inheritdoc />
        public AuthenticationFunctionConfiguration CloneSafely()
        {
            return new AuthenticationFunctionConfiguration {
                FunctionType = this.FunctionType,
                FunctionName = String.Copy(this.FunctionName),
                FunctionConfiguration = this.FunctionConfiguration.DeepCopy(),
                KeySizeBits = this.KeySizeBits,
                Nonce = null,
                Salt = null,
                AdditionalData = null
            };
        }
    }
}
