using System;
using System.Collections.Generic;

namespace PSDLayerName.Classes
{
    public class LayerElement
    {
        public string Name { get; set; }
        public bool IsGroup { get; set; }
        public bool IsSectionDivider { get; set; }

        private LayerElement parent;
        private readonly List<LayerElement> children;

        public LayerElement()
        {
            IsGroup = false;
            IsSectionDivider = false;

            parent = null;
            children = new List<LayerElement>();
        }

        public void AddChild(LayerElement element)
        {
            children.Add(element);
        }

        public LayerElement GetChild(int index)
        {
            if (index >= children.Count)
            {
                return null;
            }

            return children[index];
        }

        public LayerElement[] GetChildren()
        {
            return children.ToArray();
        }

        public LayerElement GetParent()
        {
            return parent;
        }

        public void RemoveChild(int index)
        {
            if (index >= children.Count)
            {
                return;
            }

            children.RemoveAt(index);
        }

        public void RemoveParent()
        {
            parent = null;
        }

        public void SetParent(LayerElement element)
        {
            parent = element;
        }
    }
}
