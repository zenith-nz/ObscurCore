﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObscurCore.Cryptography.Support;
using ObscurCore.Cryptography.Support.Math;
using ObscurCore.DTO;

namespace ObscurCore
{
    public class EcKeypair
    {
        /// <summary>
        /// Name of the curve provider. Used to look up relevant domain parameters to interpret the encoded keys.
        /// </summary>
        public string CurveProviderName { get; set; }
		
        /// <summary>
        /// Name of the elliptic curve in the provider's selection.
        /// </summary>
        public string CurveName { get; set; }
		
        /// <summary>
        /// Byte-array-encoded form of the public key.
        /// </summary>
        public byte[] EncodedPublicKey { get; set; }

        /// <summary>
        /// Byte-array-encoded form of the private key.
        /// </summary>
        public byte[] EncodedPrivateKey { get; set; }

        /// <summary>
        /// Create an EC private key parameter object from the encoded key.
        /// </summary>
        internal ECPrivateKeyParameters PrivateKeyParameter {
            get {
                ECPrivateKeyParameters privateKey;
                try {
                    var domain = Source.GetEcDomainParameters(CurveName);
                    privateKey = new ECPrivateKeyParameters("ECDHC", new BigInteger(EncodedPrivateKey), domain);

                } catch (NotSupportedException) {
                    throw new NotSupportedException(
                        "EC curve specified for UM1 agreement is not in the collection of curves of the provider.");
                } catch (Exception) {
                    throw new InvalidDataException("Unspecified error occured in decoding EC key.");
                }
                return privateKey;
            }
        }

        /// <summary>
        /// Exports the public component of the keypair as an EcKeyConfiguration DTO object.
        /// </summary>
        /// <returns>Public key as EcKeyConfiguration DTO.</returns>
        public EcKeyConfiguration ExportPublicKey() {
            var key = new byte[EncodedPublicKey.Length];
            Buffer.BlockCopy(EncodedPublicKey, 0, key, 0, key.Length);
            
            return new EcKeyConfiguration
                {
                    CurveProviderName = CurveProviderName,
                    CurveName = CurveName,
                    EncodedKey = key
                };
        }

        /// <summary>
        /// Exports the public component of the keypair as a serialised EcKeyConfiguration DTO object.
        /// </summary>
        /// <returns>Public key as bytes of serialised EcKeyConfiguration DTO.</returns>
        public byte[] ExportPublicKeySerialised() {
            return this.ExportPublicKey().SerialiseDto();
        }

        /// <summary>
        /// Exports the public component of the keypair as an EcKeyConfiguration DTO object.
        /// </summary>
        /// <returns>Public key as EcKeyConfiguration DTO.</returns>
        public EcKeyConfiguration GetPrivateKey() {
            var key = new byte[EncodedPublicKey.Length];
            Buffer.BlockCopy(EncodedPublicKey, 0, key, 0, key.Length);
            
            return new EcKeyConfiguration
                {
                    CurveProviderName = CurveProviderName,
                    CurveName = CurveName,
                    EncodedKey = key
                };
        }
    }
}