#region License

//  	Copyright 2013-2014 Matthew Ducker
//  	
//  	Licensed under the Apache License, Version 2.0 (the "License");
//  	you may not use this file except in compliance with the License.
//  	
//  	You may obtain a copy of the License at
//  		
//  		http://www.apache.org/licenses/LICENSE-2.0
//  	
//  	Unless required by applicable law or agreed to in writing, software
//  	distributed under the License is distributed on an "AS IS" BASIS,
//  	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  	See the License for the specific language governing permissions and 
//  	limitations under the License.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Obscur.Core.Cryptography.Authentication;
using Obscur.Core.Cryptography.Ciphers;
using Obscur.Core.Cryptography.KeyDerivation;
using Obscur.Core.DTO;

namespace Obscur.Core.Packaging.Multiplexing
{
    /// <summary>
    ///     Multiplexer for stream sources/sinks. Mixes reads/writes among an arbitrary number of streams.
    /// </summary>
    /// <remarks>
    ///     Supports extensions for control of operation size (partial/split item writes), ordering,
    ///     and item headers and trailers. Records I/O history itemwise and total.
    /// </remarks>
    public abstract class PayloadMux
    {
        protected readonly bool Writing;
        protected int Index = -1;
        protected int ItemsCompleted = 0;
        protected readonly bool[] ItemCompletionRegister;
        protected IReadOnlyList<PayloadItem> PayloadItems;
        protected readonly Stream PayloadStream;
        
        protected IReadOnlyDictionary<Guid, byte[]> PayloadItemPreKeys;
        protected readonly ICollection<Guid> ItemSkipRegister;    

        protected PayloadMux(bool writing, Stream payloadStream, IReadOnlyList<PayloadItem> payloadItems,
                             IReadOnlyDictionary<Guid, byte[]> itemPreKeys, ICollection<Guid> skips = null)
        {
            if (payloadStream == null) {
                throw new ArgumentNullException("payloadStream");
            }
            if (payloadItems == null) {
                throw new ArgumentNullException("payloadItems");
            }
            if (itemPreKeys == null) {
                throw new ArgumentNullException("itemPreKeys");
            }

            Writing = writing;
            PayloadStream = payloadStream;
            PayloadItems = payloadItems;
            PayloadItemPreKeys = itemPreKeys;
            ItemSkipRegister = writing ? null : skips;

            ItemCompletionRegister = new bool[PayloadItems.Count];
        }

        /// <summary>
        ///     Create decorator streams implementing the Encrypt-then-MAC scheme (CipherStream bound to a MacStream).
        /// </summary>
        /// <param name="item">Item to create resources for.</param>
        /// <param name="encryptor">Cipher stream (output).</param>
        /// <param name="authenticator">MAC stream (output).</param>
        protected void CreateEtMDecorator(PayloadItem item, out CipherStream encryptor, out MacStream authenticator)
        {
            byte[] encryptionKey, authenticationKey;
            if (item.SymmetricCipherKey.IsNullOrZeroLength() == false && item.AuthenticationKey.IsNullOrZeroLength() == false) {
                encryptionKey = item.SymmetricCipherKey;
                authenticationKey = item.AuthenticationKey;
            } else if (PayloadItemPreKeys.ContainsKey(item.Identifier)) {
                if (item.Authentication.KeySizeBits.HasValue == false) {
                    throw new ConfigurationInvalidException(
                        "Payload item authentication configuration is missing size specification of MAC key.");
                }
                KeyStretchingUtility.DeriveWorkingKeys(PayloadItemPreKeys[item.Identifier],
                    item.SymmetricCipher.KeySizeBits / 8,
                    item.Authentication.KeySizeBits.Value / 8, item.KeyDerivation, out encryptionKey,
                    out authenticationKey);
            } else {
                throw new ItemKeyMissingException(item);
            }

            authenticator = new MacStream(PayloadStream, Writing, item.Authentication,
                authenticationKey, false);
            encryptor = new CipherStream(authenticator, Writing, item.SymmetricCipher,
                encryptionKey, false);
        }

        /// <summary>
        ///     Executes multiplexing operations until source(s) are exhausted.
        /// </summary>
        public void Execute()
        {
            while (ItemsCompleted < PayloadItems.Count) {
                do {
                    NextSource();
                } while (ItemsCompleted < PayloadItems.Count && ItemCompletionRegister[Index]);
                Debug.Print(DebugUtility.CreateReportString("PayloadMux", "Execute", "Selected stream",
                    Index));
                ExecuteOperation();             
            }
        }

        /// <summary>
        ///     Executes a single mux/demux operation.
        /// </summary>
        protected abstract void ExecuteOperation();

        /// <summary>
        ///     Finish processing the current item.
        /// </summary>
        /// <param name="item">Payload item to finish.</param>
        /// <param name="encryptor">Item encryptor/cipher.</param>
        /// <param name="authenticator">Item authenticator/MAC.</param>
        protected abstract void FinishItem(PayloadItem item, CipherStream encryptor, MacStream authenticator);

        /// <summary>
        ///     Determine the index of the next stream to use in an I/O operation
        ///     (whether to completion or otherwise, depending on implementation).
        /// </summary>
        /// <remarks>May be overriden in a derived class to provide for advanced stream selection logic.</remarks>
        /// <returns>The next stream index.</returns>
        protected virtual void NextSource()
        {
            if (++Index == PayloadItems.Count) {
                Index = 0;
            }
            Debug.Print(DebugUtility.CreateReportString("PayloadMux", "NextSource", "Generated index",
                Index));
        }
    }
}
