using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace PSDLayerName.Classes
{
    [DataContract]
    public class LayerElement
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public bool IsGroup { get; set; }
        public bool IsSectionDivider { get; set; }

        private LayerElement _parent;

        [DataMember(Name = "Children")]
        private readonly List<LayerElement> _children;

        public LayerElement()
        {
            Name = "";
            IsGroup = false;
            IsSectionDivider = false;

            _parent = null;
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

        public LayerElement GetParent()
        {
            return _parent;
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

        public void RemoveParent()
        {
            _parent = null;
        }

        public void SetParent(LayerElement element)
        {
            _parent = element;
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
