using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        [TestMethod]
        public void TestParse_InvalidFilePath()
        {
            var layerElement = Parser.Parse("");

            Assert.IsNotNull(layerElement);
            Assert.AreEqual(layerElement.Name, "");
            Assert.IsNull(layerElement.Parent);
            Assert.AreEqual(layerElement.GetChildren().Any(), false);
        }
    }
}
