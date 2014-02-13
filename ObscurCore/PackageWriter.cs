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
using ObscurCore.DTO;
using System.Collections.Generic;
using ObscurCore.Packaging;
using System.IO;
using System.Text;
using ObscurCore.Cryptography.Ciphers;
using ObscurCore.Cryptography.Ciphers.Stream;
using ObscurCore.Cryptography.Ciphers.Block;
using ObscurCore.Cryptography.Authentication;
using System.Diagnostics;
using ObscurCore.Cryptography.KeyConfirmation;
using ObscurCore.Cryptography.KeyAgreement.Primitives;
using ObscurCore.Cryptography.KeyDerivation;
using System.Linq;
using ObscurCore.Cryptography;

namespace ObscurCore
{
	public sealed class PackageWriter
	{
		private readonly Manifest _manifest;
		private readonly ManifestHeader _manifestHeader;

		private Stream _writingTempStream;

		/// <summary>
		/// Whether package has had Write() called already in its lifetime. 
		/// Multiple invocations are prohibited in order to preserve security properties.
		/// </summary>
		private bool _writingComplete;

		/// <summary>
		/// Configuration of the manifest cipher. Must be serialised into ManifestHeader when writing package.
		/// </summary>
		private IManifestCryptographySchemeConfiguration _manifestCryptoConfig;

		/// <summary>
		/// Key for the manifest cipher prior to key derivation. Only used in writing.
		/// </summary>
		private byte[] _writingPreManifestKey;

		private Dictionary<Guid, byte[]> ItemPreKeys = new Dictionary<Guid, byte[]>();


		// Properties

		/// <summary>
		/// Format version specification of the data transfer objects and logic used in the package.
		/// </summary>
		public int FormatVersion
		{
			get { return _manifestHeader.FormatVersion; }
		}

		/// <summary>
		/// Cryptographic scheme used for the manifest.
		/// </summary>
		public ManifestCryptographyScheme ManifestCryptoScheme 
		{
			get { return _manifestHeader.CryptographySchemeName.ToEnum<ManifestCryptographyScheme>(); }
		}

		/// <summary>
		/// Configuration of symmetric cipher used for encryption of the manifest.
		/// </summary>
		/// <exception cref="InvalidOperationException">Package is being read, not written.</exception>
		internal SymmetricCipherConfiguration ManifestCipher {
			get { return _manifestCryptoConfig.SymmetricCipher; }
			private set {
				switch (ManifestCryptoScheme) {
				case ManifestCryptographyScheme.SymmetricOnly:
					((SymmetricManifestCryptographyConfiguration)_manifestCryptoConfig).SymmetricCipher = value;
					break;
				case ManifestCryptographyScheme.UM1Hybrid:
					((Um1ManifestCryptographyConfiguration)_manifestCryptoConfig).SymmetricCipher = value;
					break;
				}
			}
		}

		/// <summary>
		/// Configuration of function used in verifying the authenticity/integrity of the manifest.
		/// </summary>
		/// <exception cref="InvalidOperationException">Package is being read, not written.</exception>
		internal VerificationFunctionConfiguration ManifestAuthentication {
			get { return _manifestCryptoConfig.Authentication; }
			private set {
				switch (ManifestCryptoScheme) {
				case ManifestCryptographyScheme.SymmetricOnly:
					((SymmetricManifestCryptographyConfiguration)_manifestCryptoConfig).Authentication = value;
					break;
				case ManifestCryptographyScheme.UM1Hybrid:
					((Um1ManifestCryptographyConfiguration)_manifestCryptoConfig).Authentication = value;
					break;
				}
			}
		}

		/// <summary>
		/// Configuration of key derivation used to derive encryption and authentication keys from prior key material. 
		/// These keys are used in those functions of manifest encryption/authentication, respectively.
		/// </summary>
		/// <exception cref="InvalidOperationException">Package is being read, not written.</exception>
		internal KeyDerivationConfiguration ManifestKeyDerivation {
			get { return _manifestCryptoConfig.KeyDerivation; }
			private set {
				switch (ManifestCryptoScheme) {
				case ManifestCryptographyScheme.SymmetricOnly:
					((SymmetricManifestCryptographyConfiguration)_manifestCryptoConfig).KeyDerivation = value;
					break;
				case ManifestCryptographyScheme.UM1Hybrid:
					((Um1ManifestCryptographyConfiguration)_manifestCryptoConfig).KeyDerivation = value;
					break;
				}
			}
		}

		/// <summary>
		/// Configuration of key confirmation used for confirming the cryptographic key 
		/// to be used as the basis for key derivation.
		/// </summary>
		/// <exception cref="InvalidOperationException">Package is being read, not written.</exception>
		internal VerificationFunctionConfiguration ManifestKeyConfirmation {
			get { return _manifestCryptoConfig.KeyConfirmation; }
			private set {
				switch (ManifestCryptoScheme) {
				case ManifestCryptographyScheme.SymmetricOnly:
					((SymmetricManifestCryptographyConfiguration)_manifestCryptoConfig).KeyConfirmation = value;
					break;
				case ManifestCryptographyScheme.UM1Hybrid:
					((Um1ManifestCryptographyConfiguration)_manifestCryptoConfig).KeyConfirmation = value;
					break;
				}
			}
		}

		/// <summary>
		/// Layout scheme configuration of the items in the payload.
		/// </summary>
		/// <exception cref="InvalidOperationException">Package is being read, not written.</exception>
		public PayloadLayoutScheme PayloadLayout
		{
			get {
				return _manifest.PayloadConfiguration.SchemeName.ToEnum<PayloadLayoutScheme>();
			}
			set {
				_manifest.PayloadConfiguration = PayloadLayoutConfigurationFactory.CreateDefault(value);
			}
		}



		// Constructors

		/// <summary>
		/// Create a new package using default symmetric-only encryption for security.
		/// </summary>
		/// <param name="key">Cryptographic key known to the receiver to use for the manifest.</param>
		/// <param name="layoutScheme">Scheme to use for the layout of items in the payload.</param>
		public PackageWriter (byte[] key, PayloadLayoutScheme layoutScheme = PayloadLayoutScheme.Frameshift) {
			_manifest = new Manifest();
			_manifestHeader = new ManifestHeader
			{
				FormatVersion = Athena.Packaging.HeaderVersion,
				CryptographySchemeName = ManifestCryptographyScheme.SymmetricOnly.ToString()
			};
			SetManifestCryptoSymmetric(key);
			PayloadLayout = layoutScheme;
		}

		/// <summary>
		/// Create a new package using UM1-hybrid cryptography for security.
		/// </summary>
		/// <param name="key">Cryptographic key known to the receiver to use for the manifest.</param>
		/// <param name="layoutScheme">Scheme to use for the layout of items in the payload.</param>
		public PackageWriter (EcKeypair sender, EcKeypair receiver, PayloadLayoutScheme layoutScheme = PayloadLayoutScheme.Frameshift) {
			_manifest = new Manifest();
			_manifestHeader = new ManifestHeader
			{
				FormatVersion = Athena.Packaging.HeaderVersion,
				CryptographySchemeName = ManifestCryptographyScheme.UM1Hybrid.ToString()
			};
			SetManifestCryptoUM1 (sender.GetPrivateKey(), receiver.ExportPublicKey());
			PayloadLayout = layoutScheme;
		}





		/// <summary>
		/// Add a text payload item (encoded in UTF-8) to the package with a relative path 
		/// of root (/) in the manifest. Default encryption is used.
		/// </summary>
		/// <param name="name">Name of the item. Subject of the text is suggested.</param>
		/// <param name="text">Content of the item.</param>
		/// <exception cref="InvalidOperationException">Package is being read, not written.</exception>
		/// <exception cref="ArgumentException">Supplied null or empty string.</exception>
		public void AddText(string name, string text) {
			if (String.IsNullOrEmpty(name) || String.IsNullOrWhiteSpace(name)) {
				throw new ArgumentException ("Item name is null or empty string.");
			}
			var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
			var newItem = CreateItem (() => stream, PayloadItemType.Utf8, stream.Length, name);

			_manifest.PayloadItems.Add(newItem);
		}

		/// <summary>
		/// Add a file-type payload item to the package with a relative path of root (/) in the manifest. 
		/// Default encryption is used.
		/// </summary>
		/// <param name="filePath">Path of the file to add.</param>
		/// <exception cref="FileNotFoundException">File does not exist.</exception>
		public void AddFile(string filePath) {
			var fileInfo = new FileInfo(filePath);
			if (fileInfo.Exists == false) {
				throw new FileNotFoundException();
			}

			var newItem = CreateItem (fileInfo.OpenRead, PayloadItemType.Binary, fileInfo.Length, fileInfo.Name);
			_manifest.PayloadItems.Add(newItem);
		}

		/// <summary>
		/// Add a directory of files as payload items to the package with a relative path 
		/// of root (/) in the manifest. Default encryption is used.
		/// </summary>
		/// <param name="path">Path of the directory to search for and add files from.</param>
		/// <param name="search">Search for files in subdirectories (default) or not.</param>
		/// <exception cref="ArgumentException">Path supplied is not a directory.</exception>
		public void AddDirectory(string path, SearchOption search = SearchOption.AllDirectories) {
			var dir = new DirectoryInfo(path);

			if (Path.HasExtension(path)) {
				throw new ArgumentException ("Path is not a directory.");
			} else if (!dir.Exists) {
				throw new DirectoryNotFoundException();
			}

			var rootPathLength = dir.FullName.Length;
			var files = dir.EnumerateFiles("*", search);
			foreach (var file in files) {
				var itemRelPath = search == SearchOption.TopDirectoryOnly
				                  ? file.Name : file.FullName.Remove(0, rootPathLength + 1);
				if (Path.DirectorySeparatorChar != Athena.Packaging.PathDirectorySeperator) {
					itemRelPath = itemRelPath.Replace(Path.DirectorySeparatorChar, Athena.Packaging.PathDirectorySeperator);
				}
				var newItem = CreateItem (file.OpenRead, PayloadItemType.Binary, file.Length, itemRelPath);

				_manifest.PayloadItems.Add(newItem);
			}
		}


		/// <summary>
		/// Creates a new PayloadItem DTO object, but does not add it to the manifest, returning it instead.
		/// </summary>
		/// <returns>A payload item.</returns>
		/// <remarks>
		/// Default encryption is AES-256/CTR with random IV and key.
		/// Default authentication is 
		/// </remarks>
		/// <param name="itemData">Function supplying a stream of the item data.</param>
		/// <param name="itemType">Type of the item, e.g., Utf8 (text) or Binary (data/file).</param>
		/// <param name="externalLength">External length (outside the payload) of the item.</param>
		/// <param name="relativePath">Relative path of the item.</param>
		/// <param name="skipCrypto">
		/// If set to <c>true</c>, leaves Encryption property set to null - 
		/// for post-method-modification.
		/// </param>
		private static PayloadItem CreateItem(Func<Stream> itemData, PayloadItemType itemType, long externalLength, 
			string relativePath, bool skipCrypto = false)
		{
			var newItem = new PayloadItem {
				ExternalLength = externalLength,
				Type = itemType,
				RelativePath = relativePath,
				Encryption = !skipCrypto ? CreateDefaultPayloadItemCipherConfiguration() : null,
				Authentication = !skipCrypto ? CreateDefaultPayloadItemAuthenticationConfiguration() : null
			};

			if (skipCrypto == false) {
				newItem.EncryptionKey = new byte[newItem.Encryption.KeySizeBits / 8];
				StratCom.EntropySource.NextBytes (newItem.EncryptionKey);
				newItem.AuthenticationKey = new byte[newItem.Authentication.KeySizeBits / 8];
				StratCom.EntropySource.NextBytes (newItem.AuthenticationKey);
			}

			newItem.SetStreamBinding (itemData);
			return newItem;
		}

		private static SymmetricCipherConfiguration CreateDefaultPayloadItemCipherConfiguration() {
			return SymmetricCipherConfigurationFactory.CreateStreamCipherConfiguration (SymmetricStreamCipher.ChaCha);
		}

		private static VerificationFunctionConfiguration CreateDefaultPayloadItemAuthenticationConfiguration() {
			return AuthenticationConfigurationFactory.CreateAuthenticationConfigurationPoly1305(SymmetricBlockCipher.Aes);
		}

		/// <summary>
		/// Advanced method. Manually set a symmetric-only manifest cryptography configuration. 
		/// Misuse will likely result in unreadable package.
		/// </summary>
		/// <param name="configuration">Configuration to apply.</param>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="ArgumentException">Object not a recognised type.</exception>
		/// <exception cref="InvalidOperationException">Package is being read, not written.</exception>
		public void SetManifestCryptography(IManifestCryptographySchemeConfiguration configuration) {
			if (configuration is IDataTransferObject && (configuration is SymmetricManifestCryptographyConfiguration ||
				configuration is Um1ManifestCryptographyConfiguration))
			{
				_manifestCryptoConfig = configuration;
			} else {
				throw new ArgumentException("Object is not a valid configuration within the ObscurCore package format specification.", "configuration");
			}
		}

		/// <summary>
		/// Set the manifest to use symmetric-only security.
		/// </summary>
		/// <param name="key">Key known to the receiver of the package.</param>
		/// <exception cref="InvalidOperationException">Package is being read, not written.</exception>
		/// <exception cref="ArgumentException">Array is null or zero-length.</exception>
		public void SetManifestCryptoSymmetric(byte[] key) {
			if (key.IsNullOrZeroLength()) {
				throw new ArgumentException("Key is null or zero-length.", "key");
			}

			if (_writingPreManifestKey != null) {
				Array.Clear(_writingPreManifestKey, 0, _writingPreManifestKey.Length);
			}

			_writingPreManifestKey = new byte[key.Length];
			Array.Copy(key, _writingPreManifestKey, key.Length);
			Debug.Print(DebugUtility.CreateReportString("Package", "SetManifestCryptoSymmetric", "Manifest pre-key",
				_writingPreManifestKey.ToHexString()));

			SymmetricCipherConfiguration cipherConfig = _manifestCryptoConfig == null
			                                            ? CreateDefaultManifestCipherConfiguration()
			                                            : _manifestCryptoConfig.SymmetricCipher ?? CreateDefaultManifestCipherConfiguration();

			VerificationFunctionConfiguration authenticationConfig = _manifestCryptoConfig == null
			                                                         ? CreateDefaultManifestAuthenticationConfiguration () : 
			                                                         _manifestCryptoConfig.Authentication ?? CreateDefaultManifestAuthenticationConfiguration();

			KeyDerivationConfiguration derivationConfig =  _manifestCryptoConfig == null
			                                              ? CreateDefaultManifestKeyDerivation(cipherConfig.KeySizeBits / 8, lowEntropyPreKey:true)
			                                              : _manifestCryptoConfig.KeyDerivation ?? CreateDefaultManifestKeyDerivation(cipherConfig.KeySizeBits / 8);

			byte[] keyConfirmationOutput;
			var keyConfirmationConfig = ConfirmationUtility.CreateDefaultManifestKeyConfirmation (
				_writingPreManifestKey, out keyConfirmationOutput);

			_manifestCryptoConfig = new SymmetricManifestCryptographyConfiguration {
				SymmetricCipher = cipherConfig,
				Authentication = authenticationConfig,
				KeyConfirmation = keyConfirmationConfig,
				KeyConfirmationVerifiedOutput = keyConfirmationOutput,
				KeyDerivation = derivationConfig 
			};
			_manifestHeader.CryptographySchemeName = ManifestCryptographyScheme.SymmetricOnly.ToString();
		}

		/// <summary>
		/// Set a specific block cipher configuration to be used for the cipher used for manifest encryption.
		/// </summary>
		/// <exception cref="InvalidOperationException">Package is being written, not read.</exception>
		/// <exception cref="ArgumentException">Enum was set to None.</exception>
		public void ConfigureManifestSymmetricCrypto(SymmetricBlockCipher cipher, BlockCipherMode mode, 
			BlockCipherPadding padding)
		{
			if (cipher == SymmetricBlockCipher.None) {
				throw new ArgumentException("Cipher cannot be set to none.", "cipher");
			} else if (mode == BlockCipherMode.None) {
				throw new ArgumentException("Mode cannot be set to none.", "mode");
			} else if (cipher == SymmetricBlockCipher.None) {
				throw new ArgumentException();
			}

			ManifestCipher = SymmetricCipherConfigurationFactory.CreateBlockCipherConfiguration(cipher, mode, padding);
		}

		/// <summary>
		/// Set a specific stream cipher to be used for the cipher used for manifest encryption.
		/// </summary>
		/// <exception cref="InvalidOperationException">Package is being written, not read.</exception>
		/// <exception cref="ArgumentException">Cipher was set to None.</exception>
		public void ConfigureManifestCryptoSymmetric (SymmetricStreamCipher cipher) {
			if (cipher == SymmetricStreamCipher.None) {
				throw new ArgumentException();
			}

			ManifestCipher = SymmetricCipherConfigurationFactory.CreateStreamCipherConfiguration(cipher);
		}

		/// <summary>
		/// Set manifest to use UM1-Hybrid cryptography.
		/// </summary>
		/// <param name="senderKey">Key of the sender (private key).</param>
		/// <param name="receiverKey">Key of the receiver (public key).</param>
		public void SetManifestCryptoUM1 (EcKeyConfiguration senderKey, EcKeyConfiguration receiverKey) {
			if (senderKey == null) {
				throw new ArgumentNullException("senderKey");
			} else if(receiverKey == null) {
				throw new ArgumentNullException("receiverKey");
			}

			if (senderKey.CurveName.Equals(receiverKey.CurveName) == false) {
				throw new InvalidOperationException ("EC math requires public and private keys be in the same curve domain.");
			}

			EcKeyConfiguration ephemeral;
			_writingPreManifestKey = UM1Exchange.Initiate(receiverKey, senderKey, out ephemeral);
			Debug.Print(DebugUtility.CreateReportString("Package", "SetManifestCryptoUM1", "Manifest pre-key",
				_writingPreManifestKey.ToHexString()));

			SymmetricCipherConfiguration cipherConfig = _manifestCryptoConfig == null
			                                            ? CreateDefaultManifestCipherConfiguration()
			                                            : _manifestCryptoConfig.SymmetricCipher ?? CreateDefaultManifestCipherConfiguration();

			VerificationFunctionConfiguration authenticationConfig = _manifestCryptoConfig == null
			                                                         ? CreateDefaultManifestAuthenticationConfiguration () : 
			                                                         _manifestCryptoConfig.Authentication ?? CreateDefaultManifestAuthenticationConfiguration();

			KeyDerivationConfiguration derivationConfig =  _manifestCryptoConfig == null
			                                              ? CreateDefaultManifestKeyDerivation(cipherConfig.KeySizeBits / 8, lowEntropyPreKey:false)
			                                              : _manifestCryptoConfig.KeyDerivation ?? CreateDefaultManifestKeyDerivation(cipherConfig.KeySizeBits / 8);

			byte[] keyConfirmationOutput;
			var keyConfirmationConfig = ConfirmationUtility.CreateDefaultManifestKeyConfirmation (
				_writingPreManifestKey, out keyConfirmationOutput);

			_manifestCryptoConfig = new Um1ManifestCryptographyConfiguration {
				SymmetricCipher = cipherConfig,
				Authentication = authenticationConfig,
				KeyConfirmation = keyConfirmationConfig,
				KeyConfirmationVerifiedOutput = keyConfirmationOutput,
				KeyDerivation = derivationConfig,
				EphemeralKey = ephemeral
			};
			_manifestHeader.CryptographySchemeName = ManifestCryptographyScheme.UM1Hybrid.ToString();
		}

		private static SymmetricCipherConfiguration CreateDefaultManifestCipherConfiguration () {
			return SymmetricCipherConfigurationFactory.CreateStreamCipherConfiguration (SymmetricStreamCipher.XSalsa20);
		}

		private static VerificationFunctionConfiguration CreateDefaultManifestAuthenticationConfiguration () {
			int outputSize;
			return AuthenticationConfigurationFactory.CreateAuthenticationConfiguration (MacFunction.Blake2B256, out outputSize);
		}

		/// <summary>
		/// Creates a default manifest key derivation configuration.
		/// </summary>
		/// <remarks>Default KDF configuration is scrypt</remarks>
		/// <returns>Key derivation configuration.</returns>
		/// <param name="keyLengthBytes">Length of key to produce.</param>
		private static KeyDerivationConfiguration CreateDefaultManifestKeyDerivation (int keyLengthBytes, bool lowEntropyPreKey = true) {
			var schemeConfig = new ScryptConfiguration {
				Iterations = lowEntropyPreKey ? 16384 : 4096,
				Blocks = 8,
				Parallelism = 2
			};
			var config = new KeyDerivationConfiguration {
				SchemeName = KeyDerivationFunction.Scrypt.ToString(),
				SchemeConfiguration = schemeConfig.SerialiseDto(),
				Salt = new byte[keyLengthBytes]
			};
			StratCom.EntropySource.NextBytes(config.Salt);
			return config;
		}

		/// <summary>
		/// Advanced method. Manually set a payload configuration for the package.
		/// </summary>
		/// <param name="payloadConfiguration">Payload configuration to set.</param>
		/// <exception cref="InvalidOperationException">Package is being read, not written.</exception>
		public void SetPayloadConfiguration (PayloadConfiguration payloadConfiguration) {
			if (payloadConfiguration == null) {
				throw new ArgumentNullException("payloadConfiguration");
			}
			_manifest.PayloadConfiguration = payloadConfiguration;
		}


		/// <summary>
		/// Write package out to bound stream.
		/// </summary>
		/// <param name="outputStream">Stream which the package is to be written to.</param>
		/// <param name="closeOnComplete">Whether to close the destination stream upon completion of writing.</param>
		/// <exception cref="AggregateException">
		/// <para>Collection of however many items have no stream bindings as <see cref="ItemStreamBindingAbsentException"/></para>
		/// <para>or</para>
		/// <para>Collection of however many items have no cipher keys as <see cref="ItemStreamBindingAbsentException"/></para>
		/// </exception>
		public void Write (Stream outputStream, bool closeOnComplete = true) {
			// Sanity checks
			if (_writingComplete) {
				throw new NotSupportedException("Multiple writes from one package are not supported; it may compromise security properties.");
			}
			if (_manifestCryptoConfig == null) {
				throw new InvalidOperationException("Manifest cryptography scheme and its configuration is not set up.");
			}

			if (_manifest.PayloadItems.Count == 0) {
				throw new InvalidOperationException("No payload items have been added.");
			}
			if (_manifest.PayloadItems.Any(item => !item.StreamHasBinding)) {
				throw new AggregateException(
					_manifest.PayloadItems.Where(payloadItem => !payloadItem.StreamHasBinding)
					.Select(payloadItem => new ItemStreamBindingAbsentException(payloadItem)));
			}
			if (_manifest.PayloadItems.Any(item => !ItemPreKeys.ContainsKey(item.Identifier) && 
				(item.EncryptionKey.IsNullOrZeroLength() || item.AuthenticationKey.IsNullOrZeroLength())))
			{
				var exceptions = from payloadItem in _manifest.PayloadItems
					                 where !ItemPreKeys.ContainsKey (payloadItem.Identifier) &&
				                 (payloadItem.EncryptionKey.IsNullOrZeroLength () ||
					                 payloadItem.AuthenticationKey.IsNullOrZeroLength ())
				                 select new ItemKeyMissingException (payloadItem);

				throw new AggregateException (exceptions);
			}

			if (!outputStream.CanWrite) throw new IOException("Cannot write to output stream.");
			if (_writingTempStream == null) {
				// Default to writing to memory
				_writingTempStream = new MemoryStream();
			}

			/*			 Now we write the package */

			// Write the header tag
			Debug.Print(DebugUtility.CreateReportString("Package", "Write", "[*PACKAGE START*] Offset",
				outputStream.Position));
			var headerTag = Athena.Packaging.GetHeaderTag();
			outputStream.Write(headerTag, 0, headerTag.Length);

			// Derive working manifest encryption & authentication keys from the manifest pre-key
			byte[] workingManifestCipherKey, workingManifestMacKey;
			KeyStretchingUtility.DeriveWorkingKeys (_writingPreManifestKey, _manifestCryptoConfig.SymmetricCipher.KeySizeBits / 8,
				_manifestCryptoConfig.Authentication.KeySizeBits / 8, _manifestCryptoConfig.KeyDerivation, out workingManifestCipherKey, out workingManifestMacKey);

			Debug.Print(DebugUtility.CreateReportString("Package", "Write", "Manifest working key",
				workingManifestCipherKey.ToHexString()));

			/*			 Write the payload to temporary storage (payloadTemp) */
			PayloadLayoutScheme payloadScheme;
			try {
				payloadScheme = _manifest.PayloadConfiguration.SchemeName.ToEnum<PayloadLayoutScheme>();
			} catch (Exception) {
				throw new ConfigurationValueInvalidException(
					"Package payload schema specified is unsupported/unknown or missing.");
			}
			// Bind the multiplexer to the temp stream
			var mux = Source.CreatePayloadMultiplexer(payloadScheme, true, _writingTempStream,
				_manifest.PayloadItems, ItemPreKeys, _manifest.PayloadConfiguration);

			try {
				mux.Execute();
			} catch (Exception e) {
				throw;
			}

			/* Write the manifest in encrypted + authenticated form to memory at first, then to actual output */
			using (var manifestTemp = new MemoryStream()) {
				byte[] manifestMac = null;
				using (var authenticator = new MacStream (manifestTemp, true, _manifestCryptoConfig.Authentication, 
					out manifestMac, workingManifestMacKey, false)) 
				{
					using (var cs = new SymmetricCipherStream (authenticator, true, _manifestCryptoConfig.SymmetricCipher, 
						workingManifestCipherKey, false)) 
					{
						_manifest.SerialiseDto(cs);
					}

					authenticator.Update (((uint)authenticator.BytesOut).ToLittleEndian(), 0, 4);
					byte[] encryptionConfiguration = _manifestCryptoConfig.SymmetricCipher.SerialiseDto ();
					byte[] authenticationConfiguration = _manifestCryptoConfig.Authentication.SerialiseDto ();
					byte[] keyDerivationConfiguration = _manifestCryptoConfig.KeyDerivation.SerialiseDto ();
					authenticator.Update (encryptionConfiguration, 0, encryptionConfiguration.Length);
					authenticator.Update (authenticationConfiguration, 0, authenticationConfiguration.Length);
					authenticator.Update (keyDerivationConfiguration, 0, keyDerivationConfiguration.Length);
				}

				// After committing the authentication output, serialise the now finished manifest crypto configuration
				switch (ManifestCryptoScheme) {
				case ManifestCryptographyScheme.SymmetricOnly:
					((SymmetricManifestCryptographyConfiguration)_manifestCryptoConfig).AuthenticationVerifiedOutput = manifestMac;
					_manifestHeader.CryptographySchemeConfiguration =
						((SymmetricManifestCryptographyConfiguration) _manifestCryptoConfig).SerialiseDto();
					break;
				case ManifestCryptographyScheme.UM1Hybrid:
					((Um1ManifestCryptographyConfiguration)_manifestCryptoConfig).AuthenticationVerifiedOutput = manifestMac;
					_manifestHeader.CryptographySchemeConfiguration =
						((Um1ManifestCryptographyConfiguration) _manifestCryptoConfig).SerialiseDto();
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}

				// Serialise and write ManifestHeader (this part is written as plaintext, otherwise INCEPTION!)
				Debug.Print(DebugUtility.CreateReportString("Package", "Write", "Manifest header offset",
					outputStream.Position));
				_manifestHeader.SerialiseDto(outputStream, prefixLength: true);

				// Write length prefix
				Debug.Print(DebugUtility.CreateReportString("Package", "Write", "Manifest length prefix offset (absolute)",
					outputStream.Position));

				byte[] manifestLengthLEBytes = ((uint)manifestTemp.Length).ToLittleEndian ();
				outputStream.Write (manifestLengthLEBytes, 0, 4);

				Debug.Print(DebugUtility.CreateReportString("Package", "Write", "Manifest offset (absolute)",
					outputStream.Position));

				manifestTemp.WriteTo(outputStream);
			}

			// Clear manifest keys from memory
			Array.Clear(workingManifestCipherKey, 0, workingManifestCipherKey.Length);
			Array.Clear(workingManifestMacKey, 0, workingManifestMacKey.Length);

			/* Write out payloadTemp to output stream */
			Debug.Print(DebugUtility.CreateReportString("Package", "Write", "Payload offset (absolute)",
				outputStream.Position));
			_writingTempStream.Seek(0, SeekOrigin.Begin);
			_writingTempStream.CopyTo(outputStream);

			// Write the trailer tag
			Debug.Print(DebugUtility.CreateReportString("Package", "Write", "Trailer offset (absolute)",
				outputStream.Position));
			var trailerTag = Athena.Packaging.GetTrailerTag();
			outputStream.Write(trailerTag, 0, trailerTag.Length);

			Debug.Print(DebugUtility.CreateReportString("Package", "Write", "[* PACKAGE END *] Offset (absolute)",
				outputStream.Position));

			// All done! HAPPY DAYS.
			_writingTempStream.Close();
			_writingTempStream = null;
			if (closeOnComplete) outputStream.Close();
			_writingComplete = true;
		}

		/// <summary>
		/// Set the location of temporary data written during the write process. 
		/// </summary>
		/// <param name="stream">Stream to use for temporary storage.</param>
		public void SetTemporaryStorageStream (Stream stream) {
			if (stream == null || stream == Stream.Null) {
				throw new ArgumentException("Stream is null or points toward oblivion.");
			}
			_writingTempStream = stream;
		}
	}
}
