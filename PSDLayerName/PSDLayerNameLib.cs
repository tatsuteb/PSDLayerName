using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PSDLayerName.Classes;

namespace PSDLayerName
{
    public static class PSDLayerNameLib
    {
        public static string GetLayerName(string filePath)
        {
            var rootElement = new LayerElement();
            var layerNameJson = "";
            var layerNameArray = new List<string>();

            using (var fs = new FileStream($@"{filePath}", FileMode.Open, FileAccess.Read))
            {
                /*
                 * File Header Section
                 */

                // Signature
                ReadString(out var fileSignature, fs, 4, Encoding.UTF8);
                if (fileSignature != "8BPS")
                    return layerNameJson;

                // Version
                ReadInt16(out var version, fs, 2);
                if (version != 1)
                    return layerNameJson;

                // Reserved
                fs.Seek(6, SeekOrigin.Current);

                // Skip the rest
                fs.Seek(14, SeekOrigin.Current);

                /*
                 * Color Mode Data Section
                 */

                // Length
                ReadInt32(out var length, fs, 4);
                if (length > 0)
                {
                    // Skip the rest
                    fs.Seek(length, SeekOrigin.Current);
                }

                /*
                 * Image Resources Section
                 */

                ReadInt32(out var imgResLength, fs, 4);
                if (imgResLength > 0)
                {
                    // Skip the rest
                    fs.Seek(imgResLength, SeekOrigin.Current);
                }

                /*
                 * Layer and Mask Information Section
                 */

                // Length (Skip)
                fs.Seek(4, SeekOrigin.Current);

                // Layer info
                // Layer info -> Length
                fs.Seek(4, SeekOrigin.Current);
                //ReadInt32(out var layerInfoLength, fs, 4);

                // Layer info -> Layer count
                ReadInt16(out var layerCount, fs, 2);
                layerCount = Math.Abs(layerCount);

                // Layer records
                var parentElement = rootElement;
                for (var i = 0; i < layerCount; i++)
                {
                    // Layer record -> Rectangle (Skip)
                    fs.Seek(16, SeekOrigin.Current);

                    // Layer record -> Number of channels
                    ReadInt16(out var channelCount, fs, 2);

                    // Skip channel info, blend mode signature, blend mode key, opacity, clipping, flags, filler
                    fs.Seek(6 * channelCount + 12, SeekOrigin.Current);

                    // Layer record -> Length of the extra data field
                    ReadInt32(out var extraDataLength, fs, 4);

                    // Layer record -> Layer mask data
                    // Layer record -> Layer mask data -> Length
                    ReadInt32(out var maskDataLength, fs, 4);
                    if (maskDataLength > 0)
                    {
                        // Skip the rest
                        fs.Seek(maskDataLength, SeekOrigin.Current);

                        extraDataLength -= maskDataLength;
                    }

                    extraDataLength -= 4;

                    // Layer record -> Layer blending ranges data
                    // Layer record -> Layer blending ranges data -> Length
                    ReadInt32(out var blendingRangesDataLength, fs, 4);
                    if (blendingRangesDataLength > 0)
                    {
                        // Skip the rest
                        fs.Seek(blendingRangesDataLength, SeekOrigin.Current);

                        extraDataLength -= blendingRangesDataLength;
                    }

                    extraDataLength -= 4;

                    // Layer record -> Layer name
                    // Pascal string, padded to a multiple of 4 bytes.
                    ReadShort(out var layerNameCount, fs, 1);

                    //Skip UTF8 layer name
                    fs.Seek(layerNameCount, SeekOrigin.Current);
                    //Get layer name
                    //ReadString(out var layerName, fs, layerNameCount, Encoding.UTF8);
                    //layerNameJson += Encoding.UTF8.GetString(layerNameByteArray);

                    extraDataLength -= layerNameCount + 1;

                    var remainder = (layerNameCount + 1) % 4;
                    if (remainder > 0)
                    {
                        var paddedCount = 4 - remainder;

                        // Skip padded data
                        fs.Seek(paddedCount, SeekOrigin.Current);

                        extraDataLength -= paddedCount;
                    }

                    // Additional Layer Information
                    var layerElement = new LayerElement();
                    parentElement.AddChild(layerElement);
                    while (true)
                    {
                        // Signature
                        if (!ReadString(out var sig, fs, 4, Encoding.UTF8))
                            break;

                        extraDataLength -= 4;

                        if (sig != "8BIM" && sig != "8B64")
                        {
                            break;
                        }

                        // Key
                        if (!ReadString(out var key, fs, 4, Encoding.UTF8))
                            break;

                        extraDataLength -= 4;

                        // Data length
                        if (!ReadInt32(out var dataLength, fs, 4))
                            break;

                        extraDataLength -= 4;

                        //layerNameArray.Add(key);

                        if (key == "luni")
                        {
                            /*
                             * Get unicode layer name
                             */

                            // Length of layer name
                            if (!ReadInt32(out var count, fs, 4))
                                break;
                            // NOTE: The string of Unicode values, two bytes per character.
                            count *= 2;

                            // Unicode string
                            if (!ReadString(out var unicodeLayerName, fs, count, Encoding.BigEndianUnicode))
                                break;

                            // Add unicode layer name
                            layerElement.Name = unicodeLayerName;
                            layerElement.SetParent(parentElement);
                            layerNameArray.Add(unicodeLayerName);

                            fs.Seek(dataLength - 4 - count, SeekOrigin.Current);
                        }
                        else if (key == "lsct")
                        {
                            if (!ReadInt32(out var type, fs, 4))
                                break;

                            if (type == 1 || type == 2)
                            {
                                layerElement.IsGroup = true;
                                parentElement = layerElement;
                            }
                            else if (type == 3)
                            {
                                layerElement.IsSectionDivider = true;
                                if (layerElement.GetParent() != null)
                                {
                                    parentElement = layerElement.GetParent();
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Unknown type.TYPE:{type}");
                            }

                            fs.Seek(dataLength - 4, SeekOrigin.Current);
                        }
                        else
                        {
                            fs.Seek(dataLength, SeekOrigin.Current);
                        }
                        
                        extraDataLength -= dataLength;

                        if (extraDataLength <= 0)
                            break;
                    }

                    // Skip the rest
                    if (extraDataLength > 0)
                    {
                        fs.Seek(extraDataLength, SeekOrigin.Current);
                    }
                }
            }
            
            layerNameArray.Reverse();
            layerNameJson = String.Join(",", layerNameArray);

            return layerNameJson;
        }


        private static bool ReadString(out string result, FileStream fs, int length, Encoding encoding)
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


        private static bool ReadByte(out byte[] result, FileStream fs, int length, bool reverse)
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


        private static bool ReadShort(out short result, FileStream fs, int length)
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


        private static bool ReadInt16(out Int16 result, FileStream fs, int length)
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


        private static bool ReadInt32(out Int32 result, FileStream fs, int length)
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
    }
}
