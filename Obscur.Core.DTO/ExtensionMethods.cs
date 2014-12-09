﻿#region License

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
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Obscur.Core.DTO
{
    internal static class ExtensionMethods
    {
        #region Equality checking for arrays

        public static bool SequenceEqualConstantTime<T>(this T[] a, T[] b, bool lengthMustMatch = false) where T : struct
        {
            if (a == null && b == null) {
                return true;
            }

            if (a == null) {
                throw new ArgumentNullException("a");
            } else if (b == null) {
                throw new ArgumentNullException("b");
            }

            int i = a.Length;
            if (lengthMustMatch && i != b.Length) {
                return false;
            }

            bool equal = true;
            while (i != 0) {
                --i;
                if (!a[i].Equals(b[i])) {
                    equal = false;
                }
            }
            return equal;
        }

        /// <summary>
        ///     A constant time equals comparison - DOES NOT terminate early if
        ///     test will fail.
        ///     Checks as far as <paramref name="a"/> is in length by default (<paramref name="lengthMustMatch"/> is false).
        /// </summary>
        /// <param name="a">Array to compare against.</param>
        /// <param name="b">Array to test for equality.</param>
        /// <returns>If <c>true</c>, array section tested is equal.</returns>
        public static bool SequenceEqualConstantTime(this byte[] a, byte[] b, bool lengthMustMatch = false)
        {
            if (a == null && b == null) {
                return true;
            }
            if (a == null) {
                throw new ArgumentNullException("a");
            } else if (b == null) {
                throw new ArgumentNullException("b");
            }
            int aLen = a.Length;
            if (lengthMustMatch && aLen != b.Length) {
                return false;
            }

            return a.SequenceEqualConstantTime(0, b, 0, a.Length);
        }

        /// <summary>
        ///     A constant time equals comparison - DOES NOT terminate early if
        ///     test will fail.
        /// </summary>
        /// <param name="a">Array to compare against.</param>
        /// <param name="aOffset">Index in <paramref name="a"/> to start comparison at.</param>
        /// <param name="b">Array to test for equality.</param>
        /// <param name="bOffset">Index in <paramref name="b"/> to start comparison at.</param>
        /// <param name="length">Number of bytes to compare.</param>
        /// <returns>If <c>true</c>, array section tested is equal.</returns>
        public static bool SequenceEqualConstantTime(this byte[] a, int aOffset, byte[] b, int bOffset, int length)
        {
            if (a == null && b == null) {
                return true;
            }
            if (a == null) {
                throw new ArgumentNullException("a");
            }
            if (aOffset < 0) {
                throw new ArgumentOutOfRangeException("aOffset", "aOffset < 0");
            }
            if (b == null) {
                throw new ArgumentNullException("b");
            }
            if (bOffset < 0) {
                throw new ArgumentOutOfRangeException("bOffset", "bOffset < 0");
            }
            if (length < 0) {
                throw new ArgumentOutOfRangeException("length", "length < 0");
            }
            if ((uint)aOffset + (uint)length > (uint)a.Length) {
                throw new ArgumentOutOfRangeException("length", "aOffset + length > a.Length");
            }
            if ((uint)bOffset + (uint)length > (uint)b.Length) {
                throw new ArgumentOutOfRangeException("length", "bOffset + length > b.Length");
            }

            return SequenceEqualConstantTime_NoChecks(a, aOffset, b, bOffset, length);
        }

        public static bool SequenceEqualConstantTime_NoChecks(this byte[] a, int aOffset, byte[] b, int bOffset, int length)
        {
            if (a == null && b == null) {
                return true;
            }

#if INCLUDE_UNSAFE
            if (length >= StratCom.UnmanagedThreshold) {
                unsafe {
                    fixed (byte* srcPtr = &a[aOffset]) {
                        fixed (byte* dstPtr = &b[bOffset]) {
                            return ByteArraysEqual_ConstantTime_Internal(srcPtr, dstPtr, length);
                        }
                    }
                }
            } else {
#endif
                int differentbits = 0;
                for (int i = 0; i < length; i++) {
                    differentbits |= a[aOffset + i] ^ b[bOffset + i];
                }
                return (1 & (((uint)differentbits - 1) >> 8)) != 0;
#if INCLUDE_UNSAFE
            }
#endif
        }

        public static bool SequenceEqualVariableTime<T>(this T[] a, T[] b, bool lengthMustMatch = false) where T : struct
        {
            if (a == null && b == null) {
                return true;
            }

            if (a == null) {
                throw new ArgumentNullException("a");
            } else if (b == null) {
                throw new ArgumentNullException("b");
            }

            int i = a.Length;
            if (lengthMustMatch && i != b.Length) {
                return false;
            }

            while (i != 0) {
                --i;
                if (!a[i].Equals(b[i])) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///     A variable time equals comparison - DOES terminate early if
        ///     test will fail.
        ///     Checks as far as <paramref name="a"/> is in length by default (<paramref name="lengthMustMatch"/> is false).
        /// </summary>
        /// <param name="a">Array to compare against.</param>
        /// <param name="b">Array to test for equality.</param>
        /// <returns>If <c>true</c>, array section tested is equal.</returns>
        public static bool SequenceEqualVariableTime(this byte[] a, byte[] b, bool lengthMustMatch = false)
        {
            if (a == null && b == null) {
                return true;
            }
            if (a == null) {
                throw new ArgumentNullException("a");
            } else if (b == null) {
                throw new ArgumentNullException("b");
            }
            int aLen = a.Length;
            if (lengthMustMatch && aLen != b.Length) {
                return false;
            }

            return a.SequenceEqualVariableTime(0, b, 0, a.Length);
        }

        /// <summary>
        ///     A variable time equals comparison - DOES terminate early if test will fail.
        /// </summary>
        /// <param name="a">Array to compare against.</param>
        /// <param name="aOffset">Index in <paramref name="a"/> to start comparison at.</param>
        /// <param name="b">Array to test for equality.</param>
        /// <param name="bOffset">Index in <paramref name="b"/> to start comparison at.</param>
        /// <param name="length">Number of bytes to compare.</param>
        /// <returns>If <c>true</c>, array section tested is equal.</returns>
        public static bool SequenceEqualVariableTime(this byte[] a, int aOffset, byte[] b, int bOffset, int length)
        {
            if (a == null && b == null) {
                return true;
            }
            if (a == null) {
                throw new ArgumentNullException("a");
            }
            if (aOffset < 0) {
                throw new ArgumentOutOfRangeException("aOffset", "aOffset < 0");
            }
            if (b == null) {
                throw new ArgumentNullException("b");
            }
            if (bOffset < 0) {
                throw new ArgumentOutOfRangeException("bOffset", "bOffset < 0");
            }
            if (length < 0) {
                throw new ArgumentOutOfRangeException("length", "length < 0");
            }
            if ((uint)aOffset + (uint)length > (uint)a.Length) {
                throw new ArgumentOutOfRangeException("length", "aOffset + length > a.Length");
            }
            if ((uint)bOffset + (uint)length > (uint)b.Length) {
                throw new ArgumentOutOfRangeException("length", "bOffset + length > b.Length");
            }

            return SequenceEqualVariableTime_NoChecks(a, aOffset, b, bOffset, length);
        }

        public static bool SequenceEqualVariableTime_NoChecks(this byte[] a, int aOffset, byte[] b, int bOffset, int length)
        {
            if (a == null && b == null) {
                return true;
            }

#if INCLUDE_UNSAFE
            if (length >= StratCom.UnmanagedThreshold) {
                unsafe {
                    fixed (byte* srcPtr = &a[aOffset]) {
                        fixed (byte* dstPtr = &b[bOffset]) {
                            return ByteArraysEqual_ConstantTime_Internal(srcPtr, dstPtr, length);
                        }
                    }
                }
            } else {
#endif

                int i = a.Length;
                while (i != 0) {
                    --i;
                    if (!a[i].Equals(b[i])) {
                        return false;
                    }
                }
                return true;
#if INCLUDE_UNSAFE
            }
#endif
        }

#if INCLUDE_UNSAFE

        internal static unsafe bool ByteArraysEqual_ConstantTime_Internal(byte* aPtr, byte* bPtr, int length)
        {
            const int u32Size = sizeof(UInt32);
            const int u64Size = sizeof(UInt64);

            byte* aEndPtr = aPtr + length;
            UInt32 differentBits8 = 0u;
            UInt32 differentBits32 = 0u;
            UInt64 differentBits64 = 0ul;

            if (StratCom.PlatformWordSize == u32Size) {
                while (aPtr + u64Size <= aEndPtr) {
                    differentBits32 |= *(UInt32*)aPtr ^ *(UInt32*)bPtr;
                    aPtr += u32Size;
                    bPtr += u32Size;
                    differentBits32 |= *(UInt32*)aPtr ^ *(UInt32*)bPtr;
                    aPtr += u32Size;
                    bPtr += u32Size;
                }
            } else if (StratCom.PlatformWordSize == u64Size) {
                const int u128Size = u64Size * 2;
                while (aPtr + u128Size <= aEndPtr) {
                    differentBits64 |= *(UInt64*)aPtr ^ *(UInt64*)bPtr;
                    aPtr += u64Size;
                    bPtr += u64Size;
                    differentBits64 |= *(UInt64*)aPtr ^ *(UInt64*)bPtr;
                    aPtr += u64Size;
                    bPtr += u64Size;
                }
                if (aPtr + u64Size <= aEndPtr) {
                    differentBits64 |= *(UInt64*)aPtr ^ *(UInt64*)bPtr;
                    aPtr += u64Size;
                    bPtr += u64Size;
                }
            }
            if (StratCom.PlatformWordSize == u32Size && aPtr + u32Size <= aEndPtr) {
                differentBits32 |= *(UInt32*)aPtr ^ *(UInt32*)bPtr;
                aPtr += u32Size;
                bPtr += u32Size;
            }
            // Process remainder (shorter than native word size) as individual bytes
            while (aPtr + 1 <= aEndPtr) {
                differentBits8 |= (UInt32)(*aPtr++ ^ *bPtr++);
            }

            // Assess for differences
            bool diff8 = (1 & ((differentBits8 - 1) >> 8)) != 0;
            if (StratCom.PlatformWordSize == u32Size) {
                bool diff32 = (1 & ((differentBits32 - 1) >> 32)) != 0;
                return diff8 | diff32;
            } else if (StratCom.PlatformWordSize == u64Size) {
                bool diff64 = (1 & ((differentBits64 - 1) >> 64)) != 0;
                return diff8 | diff64;
            }
            throw new NotSupportedException("ISA from the future or the past being used - this code doesn't support native word sizes other than 32 or 64 bits!");
        }

        internal static unsafe bool ByteArraysEqual_VariableTime_Internal(byte* aPtr, byte* bPtr, int length)
        {
            const int u32Size = sizeof(UInt32);
            const int u64Size = sizeof(UInt64);

            byte* aEndPtr = aPtr + length;
            UInt32 differentBits32 = 0u;
            UInt64 differentBits64 = 0ul;

            if (StratCom.PlatformWordSize == u32Size) {
                while (aPtr + u64Size <= aEndPtr) {
                    differentBits32 |= *(UInt32*)aPtr ^ *(UInt32*)bPtr;
                    aPtr += u32Size;
                    bPtr += u32Size;
                    differentBits32 |= *(UInt32*)aPtr ^ *(UInt32*)bPtr;
                    aPtr += u32Size;
                    bPtr += u32Size;
                    if (differentBits32 != 0u)
                        return false;
                }
            } else if (Shared.PlatformWordSize == u64Size) {
                const int u128Size = u64Size * 2;
                while (aPtr + u128Size <= aEndPtr) {
                    differentBits64 |= *(UInt64*)aPtr ^ *(UInt64*)bPtr;
                    aPtr += u64Size;
                    bPtr += u64Size;
                    differentBits64 |= *(UInt64*)aPtr ^ *(UInt64*)bPtr;
                    aPtr += u64Size;
                    bPtr += u64Size;
                }
                if (differentBits64 != 0ul)
                    return false;
                if (aPtr + u64Size <= aEndPtr) {
                    if (*(UInt64*)aPtr != *(UInt64*)bPtr) {
                        return false;
                    }
                    aPtr += u64Size;
                    bPtr += u64Size;
                }
            }
            if (Shared.PlatformWordSize == u32Size && aPtr + u32Size <= aEndPtr) {
                if (*(UInt32*)aPtr != *(UInt32*)bPtr) {
                    return false;
                }
                aPtr += u32Size;
                bPtr += u32Size;
            }
            if (aPtr + sizeof(UInt16) <= aEndPtr) {
                if (*(UInt16*)aPtr != *(UInt16*)bPtr) {
                    return false;
                }
                aPtr += sizeof(UInt16);
                bPtr += sizeof(UInt16);
            }
            if (aPtr + 1 <= aEndPtr) {
                return *aPtr == *bPtr;
            }
            return true;
        }

#endif

        #endregion

        #region Secure erase memory

        /// <summary>
        ///     Securely erase <paramref name="data"/> by clearing the memory used to store it.
        /// </summary>
        /// <param name="data">Data to erase.</param>
        public static void SecureWipe<T>(this T[] data) where T : struct
        {
            if (data == null) {
                throw new ArgumentNullException("data");
            }

            InternalWipe(data, 0, data.Length);
        }

        /// <summary>
        ///     Securely erase <paramref name="data"/> by clearing the memory used to store it.
        /// </summary>
        /// <param name="data">Data to erase.</param>
        /// <param name="offset">Offset in <paramref name="data"/> to erase from.</param>
        /// <param name="count">Number of elements to erase.</param>
        public static void SecureWipe<T>(this T[] data, int offset, int count) where T : struct
        {
            Contract.Requires<ArgumentNullException>(data != null);
            Contract.Requires<ArgumentOutOfRangeException>(offset >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(count > 0);

            Contract.Ensures(offset + count <= data.Length);

            InternalWipe(data, offset, count);
        }

        /// <summary>
        ///     Securely erase <paramref name="data"/> by clearing the memory used to store it.
        /// </summary>
        /// <param name="data">Data to erase.</param>
        public static void SecureWipe(this byte[] data)
        {
            Contract.Requires<ArgumentNullException>(data != null);

            InternalWipe(data, 0, data.Length);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void InternalWipe<T>(T[] data, int offset, int count) where T : struct
        {
            Array.Clear(data, offset, count);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void InternalWipe(byte[] data, int offset, int count)
        {
#if INCLUDE_UNSAFE
            unsafe {
                fixed (byte* ptr = data) {
                    WipeMemory(ptr + offset, count);
                }
            }
#else
            Array.Clear(data, offset, count);
#endif
        }

#if INCLUDE_UNSAFE
        internal static unsafe void WipeMemory(ushort* src, int length)
        {
            WipeMemory((byte*)src, sizeof(ushort) * length);
        }

        internal static unsafe void WipeMemory(uint* src, int length)
        {
            WipeMemory((byte*)src, sizeof(uint) * length);
        }

        internal static unsafe void WipeMemory(ulong* src, int length)
        {
            WipeMemory((byte*)src, sizeof(ulong) * length);
        }

        internal static unsafe void WipeMemory(byte* targetPtr, int length)
        {
            const int u32Size = sizeof(UInt32);
            const int u64Size = sizeof(UInt64);

            byte* targetEndPtr = targetPtr + length;

            if (Shared.PlatformWordSize == u32Size) {
                while (targetPtr + u64Size <= targetEndPtr) {
                    *(UInt32*)targetPtr = 0u;
                    targetPtr += u32Size;
                    *(UInt32*)targetPtr = 0u;
                    targetPtr += u32Size;
                }
            } else if (Shared.PlatformWordSize == u64Size) {
                const int u128Size = u64Size * 2;
                while (targetPtr + u128Size <= targetEndPtr) {
                    *(UInt64*)targetPtr = 0ul;
                    targetPtr += u64Size;
                    *(UInt64*)targetPtr = 0ul;
                    targetPtr += u64Size;
                }
                if (targetPtr + u64Size <= targetEndPtr) {
                    *(UInt64*)targetPtr = 0ul;
                    targetPtr += u64Size;
                }
            }
            if (targetPtr + u32Size <= targetEndPtr) {
                *(UInt32*)targetPtr = 0u;
                targetPtr += u32Size;
            }
            if (targetPtr + sizeof(UInt16) <= targetEndPtr) {
                *(UInt16*)targetPtr = 0;
                targetPtr += sizeof(UInt16);
            }
            if (targetPtr <= targetEndPtr) {
                *targetPtr = (byte)0;
            }
        }

        internal static unsafe void SetMemory(byte* targetPtr, byte val, int length)
        {
            const int u32Size = sizeof(UInt32);
            const int u64Size = sizeof(UInt64);

            byte* targetEndPtr = targetPtr + length;
            byte* val64 = stackalloc byte[u64Size];
            for (int i = 0; i < u64Size; i++) {
                val64[i] = val;
            }

            if (Shared.PlatformWordSize == u32Size) {
                while (targetPtr + u64Size <= targetEndPtr) {
                    *(UInt32*)targetPtr = *(UInt32*)val64;
                    targetPtr += u32Size;
                    *(UInt32*)targetPtr = *(UInt32*)val64;
                    targetPtr += u32Size;
                }
            } else if (Shared.PlatformWordSize == u64Size) {
                const int u128Size = u64Size * 2;
                while (targetPtr + u128Size <= targetEndPtr) {
                    *(UInt64*)targetPtr = *val64;
                    targetPtr += u64Size;
                    *(UInt64*)targetPtr = *val64;
                    targetPtr += u64Size;
                }
                if (targetPtr + u64Size <= targetEndPtr) {
                    *(UInt64*)targetPtr = *val64;
                    targetPtr += u64Size;
                }
            }

            if (targetPtr + u32Size <= targetEndPtr) {
                *(UInt32*)targetPtr = *(UInt32*)val64;
                targetPtr += u32Size;
            }

            if (targetPtr + sizeof(UInt16) <= targetEndPtr) {
                *(UInt16*)targetPtr = *(UInt16*)val64;
                targetPtr += sizeof(UInt16);
            }

            if (targetPtr <= targetEndPtr) {
                *targetPtr = val;
            }
        }
#endif

        #endregion

        public static int GetHashCodeExt(this byte[] data)
        {
            return data.GetHashCodeExt(0, data.Length);
        }

        public static int GetHashCodeExt(this byte[] data, int off, int count)
        {
            if (data == null) {
                return 0;
            }

            int i = off + count;
            int hc = count + 1;

            while (--i >= 0) {
                hc *= 257;
                hc ^= data[i];
            }

            return hc;
        }


    }
}