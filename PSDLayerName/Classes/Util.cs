using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace PSDLayerName.Classes
{
    internal static class Util
    {
        public static bool ReadString(out string result, FileStream fs, int length, Encoding encoding)
        {
            try
            {
                var bs = new byte[length];

                if (fs.Read(bs, 0, bs.Length) == 0)
                {
                    result = "";

                    return false;
                }

                result = encoding.GetString(bs);

                return true;
            }
            catch
            {
                result = "";

                return false;
            }
        }


        public static bool ReadString(out string result, IReadOnlyCollection<byte> bytes, int startIndex, int length, Encoding encoding)
        {
            result = "";

            if (!bytes.Any())
            {
                return false;
            }

            var clampedStartIndex = Math.Max(0, Math.Min(bytes.Count - 1, startIndex));
            var clampedLength = Math.Max(0, Math.Min(bytes.Count - clampedStartIndex, length));

            var arrayList = new ArrayList(bytes.ToArray());
            result = encoding.GetString((byte[])arrayList.GetRange(clampedStartIndex, clampedLength).ToArray(typeof(byte)));

            return true;
        }


        public static bool ReadByte(out byte[] result, FileStream fs, int length, bool reverse)
        {
            try
            {
                result = new byte[length];

                if (fs.Read(result, 0, result.Length) == 0)
                {
                    return false;
                }

                if (reverse)
                {
                    Array.Reverse(result);
                }

                return true;
            }
            catch
            {
                result = null;

                return false;
            }
        }


        public static bool ReadShort(out short result, FileStream fs, int length)
        {
            try
            {
                if (!ReadByte(out var bs, fs, length, BitConverter.IsLittleEndian))
                {
                    result = 0;

                    return false;
                }

                result = short.Parse(bs.GetValue(0).ToString());

                return true;
            }
            catch
            {
                result = 0;

                return false;
            }
        }


        public static bool ReadShort(out short result, IReadOnlyCollection<byte> bytes, int startIndex, int length)
        {
            result = 0;

            if (!bytes.Any())
            {
                return false;
            }

            var clampedStartIndex = Math.Max(0, Math.Min(bytes.Count - 1, startIndex));
            var clampedLength = Math.Max(0, Math.Min(bytes.Count - clampedStartIndex, length));

            var bs = bytes
                .Skip(clampedStartIndex)
                .Take(clampedLength)
                .ToArray();

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bs);
            }

            result = short.Parse(bs.GetValue(0).ToString());

            return true;
        }


        public static bool ReadInt16(out Int16 result, FileStream fs, int length)
        {
            try
            {
                if (!ReadByte(out var bs, fs, length, BitConverter.IsLittleEndian))
                {
                    result = 0;

                    return false;
                }

                result = BitConverter.ToInt16(bs);

                return true;
            }
            catch
            {
                result = 0;

                return false;
            }
        }


        public static bool ReadInt16(out Int16 result, IReadOnlyCollection<byte> bytes, int startIndex, int length)
        {
            result = 0;

            if (!bytes.Any())
            {
                return false;
            }

            var clampedStartIndex = Math.Max(0, Math.Min(bytes.Count - 1, startIndex));
            var clampedLength = Math.Max(0, Math.Min(bytes.Count - clampedStartIndex, length));

            var bs = bytes
                .Skip(clampedStartIndex)
                .Take(clampedLength)
                .ToArray();

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bs);
            }

            result = BitConverter.ToInt16(bs);

            return true;
        }


        public static bool ReadInt32(out Int32 result, FileStream fs, int length)
        {
            try
            {
                if (!ReadByte(out var bs, fs, length, BitConverter.IsLittleEndian))
                {
                    result = 0;

                    return false;
                }

                result = BitConverter.ToInt32(bs);

                return true;
            }
            catch
            {
                result = 0;

                return false;
            }
        }


        public static bool ReadInt32(out Int32 result, IReadOnlyCollection<byte> bytes, int startIndex, int length)
        {
            result = 0;

            if (!bytes.Any())
            {
                return false;
            }

            var clampedStartIndex = Math.Max(0, Math.Min(bytes.Count - 1, startIndex));
            var clampedLength = Math.Max(0, Math.Min(bytes.Count - clampedStartIndex, length));

            var bs = bytes
                .Skip(clampedStartIndex)
                .Take(clampedLength)
                .ToArray();

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bs);
            }

            result = BitConverter.ToInt32(bs);

            return true;
        }

    }
}
