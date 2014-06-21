//
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

using System;
using ProtoBuf;

namespace ObscurCore.DTO
{
    /// <summary>
    ///     Configuration of how payload items are physically laid out
    ///     in sequences of bytes relative to each other.
    /// </summary>
    [ProtoContract]
    public sealed class PayloadConfiguration : IPayloadConfiguration,
        IDataTransferObject, IEquatable<PayloadConfiguration>
    {
        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        ///     true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(PayloadConfiguration other)
        {
            if (ReferenceEquals(null, other)) {
                return false;
            }
            if (ReferenceEquals(this, other)) {
                return true;
            }
            return String.Equals(SchemeName, other.SchemeName, StringComparison.OrdinalIgnoreCase) &&
                   (SchemeConfiguration == null
                       ? other.SchemeConfiguration == null
                       : SchemeConfiguration.SequenceEqualShortCircuiting(other.SchemeConfiguration)) &&
                   String.Equals(PrngName, other.PrngName, StringComparison.OrdinalIgnoreCase) &&
                   (PrngConfiguration == null
                       ? other.PrngConfiguration == null
                       : PrngConfiguration.SequenceEqualShortCircuiting(other.PrngConfiguration));
        }

        /// <summary>
        ///     Name of the payload layout scheme, e.g. Frameshift.
        /// </summary>
        [ProtoMember(1, IsRequired = true)]
        public string SchemeName { get; set; }

        /// <summary>
        ///     Configuration for the scheme.
        /// </summary>
        /// <remarks>Format of the configuration is that of the consuming type.</remarks>
        [ProtoMember(2, IsRequired = false)]
        public byte[] SchemeConfiguration { get; set; }

        /// <summary>
        ///     Name of the PRNG used for selecting the active stream,
        ///     and other scheme-specific states.
        /// </summary>
        [ProtoMember(3, IsRequired = false)]
        public string PrngName { get; set; }

        /// <summary>
        ///     Configuration for the primary PRNG.
        /// </summary>
        /// <remarks>Format of the configuration is that of the consuming type.</remarks>
        [ProtoMember(4, IsRequired = false)]
        public byte[] PrngConfiguration { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            return obj is PayloadConfiguration && Equals((PayloadConfiguration) obj);
        }

        /// <summary>
        ///     Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        ///     A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked {
                int hashCode = SchemeName.ToLowerInvariant().GetHashCode();
                hashCode = (hashCode * 397) ^ (SchemeConfiguration != null ? SchemeConfiguration.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PrngName != null ? PrngName.ToLowerInvariant().GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PrngConfiguration != null ? PrngConfiguration.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
