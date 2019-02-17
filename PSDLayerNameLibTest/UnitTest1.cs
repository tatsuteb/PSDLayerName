using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PSDLayerNameLibTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var layerName = PSDLayerName.PSDLayerNameLib.GetLayerName("./TestData.psd");
            //Assert.IsNotNull(layerName, "Failed to get layer name.");
            Assert.AreEqual("8BPS", layerName);
        }
    }
}
