using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PSDLayerName.Classes;

namespace PSDLayerName
{
    public static class Parser
    {
        public static LayerElement Parse(string filePath)
        {
            var layerElements = new List<LayerElement>();

            try
            {
                using (var fs = new FileStream($@"{filePath}", FileMode.Open, FileAccess.Read))
                {
                    /*
                     * File Header Section
                     */

                    // Signature
                    Util.ReadString(out var fileSignature, fs, 4, Encoding.UTF8);
                    if (fileSignature != "8BPS")
                        return new LayerElement();

                    // Version
                    Util.ReadInt16(out var version, fs, 2);
                    if (version != 1)
                        return new LayerElement();

                    // Skip the rest
                    fs.Seek(20, SeekOrigin.Current);


                    /*
                     * Color Mode Data Section
                     */

                    // Length
                    Util.ReadInt32(out var colorModeLength, fs, 4);
                    if (colorModeLength > 0)
                    {
                        // Skip the rest
                        fs.Seek(colorModeLength, SeekOrigin.Current);
                    }


                    /*
                     * Image Resources Section
                     */

                    Util.ReadInt32(out var imgResLength, fs, 4);
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
                    Util.ReadInt16(out var layerCount, fs, 2);
                    layerCount = Math.Abs(layerCount);

                    // Layer records
                    for (var i = 0; i < layerCount; i++)
                    {
                        // Layer record -> Rectangle (Skip)
                        fs.Seek(16, SeekOrigin.Current);

                        // Layer record -> Number of channels
                        Util.ReadInt16(out var channelCount, fs, 2);

                        // Skip channel info, blend mode signature, blend mode key, opacity, clipping, flags, filler
                        fs.Seek(6 * channelCount + 12, SeekOrigin.Current);

                        // Layer record -> Length of the extra data field
                        Util.ReadInt32(out var extraDataLength, fs, 4);

                        // Layer record -> Layer mask data
                        // Layer record -> Layer mask data -> Length
                        Util.ReadInt32(out var maskDataLength, fs, 4);
                        if (maskDataLength > 0)
                        {
                            // Skip the rest
                            fs.Seek(maskDataLength, SeekOrigin.Current);

                            extraDataLength -= maskDataLength;
                        }

                        extraDataLength -= 4;

                        // Layer record -> Layer blending ranges data
                        // Layer record -> Layer blending ranges data -> Length
                        Util.ReadInt32(out var blendingRangesDataLength, fs, 4);
                        if (blendingRangesDataLength > 0)
                        {
                            // Skip the rest
                            fs.Seek(blendingRangesDataLength, SeekOrigin.Current);

                            extraDataLength -= blendingRangesDataLength;
                        }

                        extraDataLength -= 4;

                        // Layer record -> Layer name
                        // NOTE: Pascal string, padded to a multiple of 4 bytes.
                        Util.ReadShort(out var layerNameCount, fs, 1);

                        //Skip UTF8 layer name
                        fs.Seek(layerNameCount, SeekOrigin.Current);

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
                        Util.ReadByte(out var layerInfoBytes, fs, extraDataLength, false);

                        var layerInfo = AdditionalLayerInformation.BuildFromByte(layerInfoBytes);

                        layerElements.Add(new LayerElement()
                        {
                            Name = layerInfo.UnicodeName,
                            IsGroup = layerInfo.Type == AdditionalLayerInformation.LayerSectionType.OpenFolder ||
                                      layerInfo.Type == AdditionalLayerInformation.LayerSectionType.ClosedFolder,
                            IsSectionDivider = layerInfo.Type == AdditionalLayerInformation.LayerSectionType.BoundingSectionDivider
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new LayerElement();
            }

            layerElements.Reverse();

            // Build layer tree and return it
            return BuildLayerTree(layerElements.ToArray());
        }


        private static LayerElement BuildLayerTree(LayerElement[] layerElements)
        {
            var rootElement = new LayerElement()
            {
                IsGroup = true
            };

            var parentElement = rootElement;
            foreach (var element in layerElements)
            {
                if (element.IsSectionDivider)
                {
                    parentElement = parentElement.Parent;

                    continue;
                }

                element.Parent = parentElement;

                parentElement.AddChild(element);

                if (element.IsGroup)
                {
                    parentElement = element;
                }
            }

            return rootElement;
        }
    }
}
