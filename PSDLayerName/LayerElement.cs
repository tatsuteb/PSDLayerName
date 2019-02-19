using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace PSDLayerName
{
    [DataContract]
    public class LayerElement
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public bool IsGroup { get; set; }
        public bool IsSectionDivider { get; set; }
        public LayerElement Parent { get; set; }

        [DataMember(Name = "Children")]
        private readonly List<LayerElement> _children;


        public LayerElement()
        {
            Name = "";
            IsGroup = false;
            IsSectionDivider = false;
            Parent = null;

            _children = new List<LayerElement>();
        }

        public void AddChild(LayerElement element)
        {
            _children.Add(element);
        }

        public LayerElement GetChild(int index)
        {
            return index >= _children.Count ? null : _children[index];
        }

        public LayerElement[] GetChildren()
        {
            return _children.ToArray();
        }

        public void RemoveChild(int index)
        {
            if (index >= _children.Count)
            {
                return;
            }

            _children.RemoveAt(index);
        }

        public void RemoveChild(LayerElement element)
        {
            _children.Remove(element);
        }

        public string Serialize()
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(GetType());
                serializer.WriteObject(stream, this);

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }
}
