using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace PSDLayerNameLibTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var layerElement = PSDLayerName.PSDLayerNameLib.GetLayerName("./TestData.psd");

            Assert.IsNotNull(layerElement);

            string[] rootChildrenNames = {"Group1", "EmptyLayer", "グループ 2", "日本語のレイヤー", "BackgroundImage", "Layer2" };

            var rootChildren = layerElement.GetChildren();

            Assert.AreEqual(rootChildren.Length, rootChildrenNames.Length);

            for (var i = 0; i < rootChildren.Length; i++)
            {
                Assert.AreEqual(rootChildrenNames[i], rootChildren[i].Name);
            }
        }
    }
}
