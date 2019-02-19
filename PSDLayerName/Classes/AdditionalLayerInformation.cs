using System.Collections.Generic;
using System.Text;

namespace PSDLayerName.Classes
{
    internal class AdditionalLayerInformation
    {
        public enum LayerSectionType
        {
            AnyOtherTypeOfLayer,
            OpenFolder,
            ClosedFolder,
            BoundingSectionDivider
        }
        public string UnicodeName { get; private set; }
        public LayerSectionType Type { get; private set; }

        private AdditionalLayerInformation()
        {

        }

        public static AdditionalLayerInformation BuildFromByte(IReadOnlyCollection<byte> bytes)
        {
            var layerInfo = new AdditionalLayerInformation();
            var sp = 0;

            while (true)
            {
                // Signature
                if (!Util.ReadString(out var sig, bytes, sp, 4, Encoding.UTF8))
                    return layerInfo;

                sp += 4;

                if (sig != "8BIM" && sig != "8B64")
                {
                    return layerInfo;
                }

                // Key
                if (!Util.ReadString(out var key, bytes, sp, 4, Encoding.UTF8))
                    return layerInfo;

                sp += 4;

                // Data length
                if (!Util.ReadInt32(out var dataLength, bytes, sp, 4))
                    return layerInfo;

                sp += 4;

                switch (key)
                {
                    case "luni":
                        {
                            /*
                             * Get unicode layer name
                             */

                            // Length of layer name
                            // NOTE: The string of Unicode values, two bytes per character.
                            if (!Util.ReadInt32(out var count, bytes, sp, 4))
                                return layerInfo;

                            sp += 4;

                            count *= 2;

                            // Unicode string
                            if (!Util.ReadString(out var unicodeLayerName, bytes, sp, count, Encoding.BigEndianUnicode))
                                return layerInfo;

                            sp += count;

                            // Add unicode layer name
                            layerInfo.UnicodeName = unicodeLayerName;

                            sp += dataLength - 4 - count;
                            break;
                        }
                    case "lsct":
                        {
                            if (!Util.ReadInt32(out var type, bytes, sp, 4))
                                return layerInfo;

                            sp += 4;

                            switch (type)
                            {
                                case 1:
                                    layerInfo.Type = LayerSectionType.OpenFolder;
                                    break;
                                case 2:
                                    layerInfo.Type = LayerSectionType.ClosedFolder;
                                    break;
                                case 3:
                                    layerInfo.Type = LayerSectionType.BoundingSectionDivider;
                                    break;
                                default:
                                    layerInfo.Type = LayerSectionType.AnyOtherTypeOfLayer;
                                    break;
                            }

                            sp += dataLength - 4;
                            break;
                        }
                    default:
                        sp += dataLength;
                        break;
                }

                // Break while loop after reading all
                if (sp >= bytes.Count)
                {
                    break;
                }
            }

            return layerInfo;
        }
    }

}
