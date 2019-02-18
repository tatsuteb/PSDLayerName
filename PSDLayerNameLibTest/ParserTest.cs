using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using PSDLayerName;

namespace PSDLayerNameTest
{
    [TestClass]
    public class ParseTest
    {
        [TestMethod]
        public void TestParse()
        {
            var layerElement = Parser.Parse("./TestData.psd");

            Assert.IsNotNull(layerElement);

            string[] rootChildrenNames = {"Group1", "EmptyLayer", "�O���[�v 2", "���{��̃��C���[", "BackgroundImage", "Layer2" };

            var rootChildren = layerElement.GetChildren();

            Assert.AreEqual(rootChildrenNames.Length, rootChildren.Length);

            for (var i = 0; i < rootChildren.Length; i++)
            {
                Assert.AreEqual(rootChildrenNames[i], rootChildren[i].Name);
            }
        }
    }
}
