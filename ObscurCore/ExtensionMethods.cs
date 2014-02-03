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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ObscurCore.Cryptography;
using ObscurCore.Cryptography.KeyAgreement;
using ObscurCore.Cryptography.Support;
using ObscurCore.Cryptography.Support.Math;
using ObscurCore.Cryptography.Support.Math.EllipticCurve;
using ObscurCore.DTO;

using ObscurCore.Support;

namespace ObscurCore
{
    /// <summary>
    /// Extension methods that should always be available (without namespace includes)
    /// </summary>
    public static class BasicExtensions
    {
		/// <summary>
		/// Determines if value is between the specified low and high limits (range).
		/// </summary>
		/// <returns><c>true</c> if is between the specified value low high; otherwise, <c>false</c>.</returns>
		/// <param name="value">Value to check.</param>
		/// <param name="low">Low/minimum value.</param>
		/// <param name="high">High/maximum value.</param>
		/// <typeparam name="T">Type of value.</typeparam>
        public static bool IsBetween<T>(this T value, T low, T high) where T : IComparable<T>
        {
            return value.CompareTo(low) >= 0 && value.CompareTo(high) <= 0;
        }

		/// <summary>
		/// Determines if specified array is null or zero length.
		/// </summary>
		/// <returns><c>true</c> if is null or zero length; otherwise, <c>false</c>.</returns>
		/// <param name="array">Array to check.</param>
		public static bool IsNullOrZeroLength<T>(this T[] array) {
            return array == null || array.Length == 0;
        }

		/// <summary>
		/// Serialises the dto.
		/// </summary>
		/// <returns>The serialised DTO in a byte array.</returns>
		/// <param name="obj">Object to serialise.</param>
		/// <param name="prefixLength">If set to <c>true</c> prefix length of object to output; otherwise, <c>false</c>.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
        public static byte[] SerialiseDto<T>(this T obj, bool prefixLength = false) where T : IDataTransferObject
        {
            return StratCom.SerialiseDataTransferObject(obj, prefixLength).ToArray();
        }

        public static void SerialiseDto<T>(this T obj, Stream output, bool prefixLength = false) where T : IDataTransferObject {
			StratCom.SerialiseDataTransferObject (obj, output, prefixLength);
		}

		/// <summary>
		/// Parses an enumeration value encoded as a string into its enumeration equivalent.
		/// </summary>
		/// <returns>Enumeration value.</returns>
		/// <param name="stringValue">String to parse.</param>
		/// <param name="ignoreCase">If set to <c>true</c> ignore case; otherwise, <c>false</c>.</param>
		/// <typeparam name="T">Enumeration type.</typeparam>
		public static T ToEnum<T>(this string stringValue, bool ignoreCase = true) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) 
				throw new InvalidOperationException("T must be an enumeration type.");

            T value;
            try {
                value = (T)Enum.Parse(typeof(T), stringValue, ignoreCase);
            } catch (ArgumentException) {
                throw new EnumerationParsingException(stringValue, typeof(T));
            }
            return value;
        }

		/// <summary>
		/// Determines if string is/equals one of the specified comparison candidates.
		/// </summary>
		/// <returns><c>true</c> if is one of the specified candidates; otherwise, <c>false</c>.</returns>
		/// <param name="s">String to compare</param>
		/// <param name="comparison">Comparison method</param>
		/// <param name="candidates">Candidate strings</param>
		public static bool IsOneOf(this string s, StringComparison comparison, IEnumerable<string> candidates) {
			return candidates.Any ((candidate) => candidate.Equals(s, comparison));
		}

        /// <summary>
        /// Converts a byte array into a hex-encoded string.
        /// </summary>
        /// <returns>Hex-encoded string, lowercase.</returns>
        public static string ToHexString (this byte[] bytes) {
            var hexBuilder = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
                hexBuilder.AppendFormat("{0:x2}", b);
            return hexBuilder.ToString();
        }

        /// <summary>
		/// Converts a hex-encoded string to a byte array.
        /// </summary>
        /// <param name="hexSrc">Hex-encoded data</param>
        public static byte[] HexToBinary(this string hexSrc) {
			if (hexSrc == null)
				return null;
			return Hex.Decode (hexSrc);
        }

		public static int GetHashCodeExt(this byte[] data)
		{
			if (data == null)
			{
				return 0;
			}

			int i = data.Length;
			int hc = i + 1;

			while (--i >= 0)
			{
				hc *= 257;
				hc ^= data[i];
			}

			return hc;
		}

		public static int GetHashCodeExt(this int[] data)
		{
			if (data == null)
			{
				return 0;
			}

			int i = data.Length;
			int hc = i + 1;

			while (--i >= 0)
			{
				hc *= 257;
				hc ^= data[i];
			}

			return hc;
		}

		public static bool SequenceEqual(
			this byte[]	a,
			byte[]	b)
		{
			if (a == b)
				return true;

			if (a == null || b == null)
				return false;

			return SequenceEqual<byte>(a, b);
		}

		private static bool SequenceEqualByteOptimised(
			byte[]	a,
			byte[]	b)
		{
			int i = a.Length;
			if (i != b.Length)
				return false;
			while (i != 0)
			{
				--i;
				if (a[i] != b[i])
					return false;
			}
			return true;
		}

		public static bool SequenceEqual(this int[] a, int[] b) {
			if (a == b)
				return true;

			if (a == null || b == null)
				return false;

			return SequenceEqual<int>(a, b);
		}

		public static bool SequenceEqual(this uint[] a, uint[] b) {
			if (a == b)
				return true;

			if (a == null || b == null)
				return false;

			return SequenceEqual<uint>(a, b);
		}

		private static bool SequenceEqual<T> (T[] a, T[] b) where T: struct {
			var i = a.Length;
			if (i != b.Length)
				return false;
			while (i != 0) {
				--i;
				if (!a[i].Equals(b[i]))
					return false;
			}
			return true;
		}

		public static byte[] DeepCopy(this byte[] data)
		{
			return data == null ? null : (byte[]) data.Clone();
		}

		public static int[] DeepCopy(this int[] data) {
			return data == null ? null : (int[])data.Clone();
		}

		public static long[] DeepCopy(this long[] data) {
			return data == null ? null : (long[])data.Clone();
		}

		[CLSCompliantAttribute(false)]
		public static ulong[] DeepCopy(
			this ulong[] data)
		{
			return data == null ? null : (ulong[]) data.Clone();
		}
    }
}

namespace ObscurCore.Extensions
{
    namespace EllipticCurve
    {
        public static class ECKeyConfigurationExtensions
        {
            public static ECPublicKeyParameters DecodeToPublicKey(this EcKeyConfiguration config) {
                //if (!config.CurveProviderName.Equals ("Brainpool"))
                //    throw new InvalidDataException ("Curve providers other than \"Brainpool\" are not currently supported.");

                ECPublicKeyParameters publicKey;
                try {
                    var domain = Source.GetEcDomainParameters(config.CurveName);
                    ECPoint point = domain.Curve.DecodePoint(config.EncodedKey);
                    publicKey = new ECPublicKeyParameters("ECDHC", point, domain);
                } catch (NotSupportedException) {
                    throw new NotSupportedException ("EC curve specified for UM1 agreement is not in the collection of curves of the provider.");
                } catch (Exception) {
                    throw new InvalidDataException("Unspecified error occured in decoding EC key.");
                }
                return publicKey;
            }

            public static ECPrivateKeyParameters DecodeToPrivateKey(this EcKeyConfiguration config) {
                //if (!config.CurveProviderName.Equals ("Brainpool"))
                //    throw new InvalidDataException ("Curve providers other than \"Brainpool\" are not currently supported.");

                ECPrivateKeyParameters privateKey;
                try {
                    var domain = Source.GetEcDomainParameters(config.CurveName);
                    privateKey = new ECPrivateKeyParameters("ECDHC", new BigInteger(config.EncodedKey), domain);

                } catch (NotSupportedException) {
                    throw new NotSupportedException ("EC curve specified for UM1 agreement is not in the collection of curves of the provider.");
                } catch (Exception) {
                    throw new InvalidDataException("Unspecified error occured in decoding EC key.");
                }
                return privateKey;
            }

            public static void EncodePublicKey(this EcKeyConfiguration config, string curveProvider, string curveName, ECPoint key) {
                //if (!curveProvider.Equals ("Brainpool"))
                //    throw new ArgumentException ("Curve providers other than \"Brainpool\" are not currently supported.");
                config.CurveProviderName = curveProvider;
                config.CurveName = curveName;
                //config.EncodedKey = ECKeyUtility.Write (key);
                config.EncodedKey = key.GetEncoded();
            }

            public static void EncodePrivateKey(this EcKeyConfiguration config, string curveProvider, string curveName, BigInteger key) {
                //if (!curveProvider.Equals ("Brainpool"))
                //    throw new ArgumentException ("Curve providers other than \"Brainpool\" are not currently supported.");
                config.CurveProviderName = curveProvider;
                config.CurveName = curveName;
                config.EncodedKey = key.ToByteArray ();
            }
        }
    }

    namespace Streams
    {
        public static class StreamExtensions
        {
            public static void WritePrimitive(this Stream stream, bool value) {
                stream.WriteByte(value ? (byte) 1 : (byte) 0);
            }

            public static void ReadPrimitive(this Stream stream, out bool value) {
                var b = stream.ReadByte();
                value = b != 0;
            }

            public static void WritePrimitive(this Stream stream, byte value) {
                stream.WriteByte(value);
            }

            public static void ReadPrimitive(this Stream stream, out byte value) {
                value = (byte) stream.ReadByte();
            }

            public static void WritePrimitive(this Stream stream, sbyte value) {
                stream.WriteByte((byte) value);
            }

            public static void ReadPrimitive(this Stream stream, out sbyte value) {
                value = (sbyte) stream.ReadByte();
            }

            public static void WritePrimitive(this Stream stream, char value) {
                WriteVarint32(stream, value);
            }

            public static void ReadPrimitive(this Stream stream, out char value) {
                value = (char) ReadVarint32(stream);
            }

            public static void WritePrimitive(this Stream stream, ushort value) {
                WriteVarint32(stream, value);
            }

            public static void ReadPrimitive(this Stream stream, out ushort value) {
                value = (ushort) ReadVarint32(stream);
            }

            public static void WritePrimitive(this Stream stream, short value) {
                WriteVarint32(stream, EncodeZigZag32(value));
            }

            public static void ReadPrimitive(this Stream stream, out short value) {
                value = (short) DecodeZigZag32(ReadVarint32(stream));
            }

            public static void WritePrimitive(this Stream stream, uint value) {
                WriteVarint32(stream, value);
            }

            public static void ReadPrimitive(this Stream stream, out uint value) {
                value = ReadVarint32(stream);
            }

            public static void WritePrimitive(this Stream stream, int value) {
                WriteVarint32(stream, EncodeZigZag32(value));
            }

            public static void ReadPrimitive(this Stream stream, out int value) {
                value = DecodeZigZag32(ReadVarint32(stream));
            }

            public static void WritePrimitive(this Stream stream, ulong value) {
                WriteVarint64(stream, value);
            }

            public static void ReadPrimitive(this Stream stream, out ulong value) {
                value = ReadVarint64(stream);
            }

            public static void WritePrimitive(this Stream stream, long value) {
                WriteVarint64(stream, EncodeZigZag64(value));
            }

            public static void ReadPrimitive(this Stream stream, out long value) {
                value = DecodeZigZag64(ReadVarint64(stream));
            }

#if INCLUDE_UNSAFE
        public static unsafe void WritePrimitive(this Stream stream, float value)
		{
			uint v = *(uint*)(&value);
			WriteVarint32(stream, v);
		}
		
		public static unsafe void ReadPrimitive(this Stream stream, out float value)
		{
			uint v = ReadVarint32(stream);
			value = *(float*)(&v);
		}
		
		public static unsafe void WritePrimitive(this Stream stream, double value)
		{
			ulong v = *(ulong*)(&value);
			WriteVarint64(stream, v);
		}
		
		public static unsafe void ReadPrimitive(this Stream stream, out double value)
		{
			ulong v = ReadVarint64(stream);
			value = *(double*)(&v);
		}
#endif

            public static void WritePrimitive(this Stream stream, DateTime value) {
                var v = value.ToBinary();
                WritePrimitive(stream, v);
            }

            public static void ReadPrimitive(this Stream stream, out DateTime value) {
                long v;
                ReadPrimitive(stream, out v);
                value = DateTime.FromBinary(v);
            }

            public static void WritePrimitive(this Stream stream, string value) {
                if (value == null) {
                    WritePrimitive(stream, (uint) 0);
                    return;
                }

                var encoding = new UTF8Encoding(false, true);

                var len = encoding.GetByteCount(value);

                WritePrimitive(stream, (uint) len + 1);

                var buf = new byte[len];

                encoding.GetBytes(value, 0, value.Length, buf, 0);

                stream.Write(buf, 0, len);
            }

            public static void ReadPrimitive(this Stream stream, out string value) {
                uint len;
                ReadPrimitive(stream, out len);

                switch (len) {
                    case 0:
                        value = null;
                        return;
                    case 1:
                        value = string.Empty;
                        return;
                }

                len -= 1;

                var encoding = new UTF8Encoding(false, true);

                var buf = new byte[len];

                var l = 0;

                while (l < len) {
                    int r = stream.Read(buf, l, (int) len - l);
                    if (r == 0)
                        throw new EndOfStreamException();
                    l += r;
                }

                value = encoding.GetString(buf);
            }



            public static void WritePrimitive(this Stream stream, byte[] value) {
                if (value == null) {
                    WritePrimitive(stream, (uint) 0);
                    return;
                }

                WritePrimitive(stream, (uint) value.Length + 1);
                stream.Write(value, 0, value.Length);
            }

            public static void WritePrimitive(this Stream stream, byte[] value, int offset, int count) {
                if (value == null) {
                    WritePrimitive(stream, (uint) 0);
                    return;
                }

                WritePrimitive(stream, (uint) count + 1);
                stream.Write(value, offset, count);
            }

            public static void WritePrimitiveMeta(this Stream stream, byte[] value, bool negative) {
                stream.WritePrimitiveMeta(value, 0, value.Length, negative);
            }

            /// <summary>
			/// Writes a length-encoded byte array with additional 
			/// boolean property stored in sign of length prefix.
            /// </summary>
            /// <param name="stream">Stream to write to.</param>
            /// <param name="value">Source byte array.</param>
            /// <param name="offset">Offset at which to start writing bytes from the source array.</param>
            /// <param name="count">Number of bytes to be written.</param>
            /// <param name="negative">If set to <c>true</c> length-specifying integer will be stored with negative sign.</param>
            public static void WritePrimitiveMeta(this Stream stream, byte[] value, int offset, int count,
                                                  bool negative) {
                if (value == null) {
                    WritePrimitive(stream, 0);
                    return;
                }

                WritePrimitive(stream, negative ? -(count + 1) : count + 1);
                stream.Write(value, offset, count);
            }

            private static readonly byte[] EmptyByteArray = new byte[0];

            public static void ReadPrimitive(this Stream stream, out byte[] value) {
                uint len;
                ReadPrimitive(stream, out len);

                switch (len) {
                    case 0:
                        value = null;
                        return;
                    case 1:
                        value = EmptyByteArray;
                        return;
                }

                len -= 1;

                value = new byte[len];
                var l = 0;

                while (l < len) {
                    var r = stream.Read(value, l, (int) len - l);
                    if (r == 0) throw new EndOfStreamException();
                    l += r;
                }
            }

            /// <summary>
            /// Reads a length-encoded byte array with additional boolean property stored as integer sign.
            /// </summary>
            /// <param name="stream">Stream to be read from.</param>
            /// <param name="value">Output byte array.</param>
            /// <param name="negative">Stored boolean state. Will be <c>true</c> if stored integer has negative sign.</param>
            public static void ReadPrimitiveMeta(this Stream stream, out byte[] value, out bool negative) {
                int len;
                ReadPrimitive(stream, out len);

                negative = Math.Sign(len) < 0;
                len = Math.Abs(len);

                switch (len) {
                    case 0:
                        value = null;
                        return;
                    case 1:
                        value = EmptyByteArray;
                        return;
                }

                len -= 1;

                value = new byte[len];
				int l = 0;

                while (l < len) {
                    var r = stream.Read(value, l, len - l);
                    if (r == 0) throw new EndOfStreamException();
                    l += r;
                }
            }

            /// <summary>
            /// Reads an enumeration value from a stream that was encoded as a string.
            /// </summary>
            /// <typeparam name='T'>
            /// Must be an enumeration type.
            /// </typeparam>
            public static void ReadPrimitive<T>(this Stream stream, out T value) where T : struct, IConvertible {

                if (!typeof (T).IsEnum) throw new InvalidOperationException("T must be an enumerated type.");
                try {
                    string stringValue;
                    ReadPrimitive(stream, out stringValue);
                    value = (T) Enum.Parse(typeof (T), stringValue);
                } catch (ArgumentException) {
                    throw new ArgumentException("Enumeration member is unknown or otherwise invalid.");
                }
            }

            /// <summary>
            /// Writes an enumeration value into a stream, encoded as a string .
            /// </summary>
            /// <typeparam name='T'>
            /// Must be an enumeration type.
            /// </typeparam>
            public static void WritePrimitive<T>(this Stream stream, T value) where T : struct, IConvertible {
                if (!typeof (T).IsEnum) throw new InvalidOperationException("T must be an enumerated type.");

                WritePrimitive(stream, Enum.GetName(typeof (T), value));
            }

            private static uint EncodeZigZag32(int n) {
                return (uint) ((n << 1) ^ (n >> 31));
            }

            private static ulong EncodeZigZag64(long n) {
                return (ulong) ((n << 1) ^ (n >> 63));
            }

            private static int DecodeZigZag32(uint n) {
                return (int) (n >> 1) ^ -(int) (n & 1);
            }

            private static long DecodeZigZag64(ulong n) {
                return (long) (n >> 1) ^ -(long) (n & 1);
            }

            private static uint ReadVarint32(Stream stream) {
                var result = 0;
                var offset = 0;

                for (; offset < 32; offset += 7) {
                    int b = stream.ReadByte();
                    if (b == -1)
                        throw new EndOfStreamException();

                    result |= (b & 0x7f) << offset;

                    if ((b & 0x80) == 0)
                        return (uint) result;
                }

                throw new InvalidDataException();
            }

            private static void WriteVarint32(Stream stream, uint value) {
                for (; value >= 0x80u; value >>= 7)
                    stream.WriteByte((byte) (value | 0x80u));

                stream.WriteByte((byte) value);
            }

            private static ulong ReadVarint64(Stream stream) {
                long result = 0;
                var offset = 0;

                for (; offset < 64; offset += 7) {
                    int b = stream.ReadByte();
                    if (b == -1)
                        throw new EndOfStreamException();

                    result |= ((long) (b & 0x7f)) << offset;

                    if ((b & 0x80) == 0)
                        return (ulong) result;
                }

                throw new InvalidDataException();
            }

            private static void WriteVarint64(Stream stream, ulong value) {
                for (; value >= 0x80u; value >>= 7)
                    stream.WriteByte((byte) (value | 0x80u));

                stream.WriteByte((byte) value);
            }
        }
    }
}
