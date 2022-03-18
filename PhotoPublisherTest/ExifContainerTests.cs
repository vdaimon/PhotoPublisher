using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhotoPublisher;
using System;
using System.IO;

namespace PhotoPublisherTest
{
    [TestClass]
    public class ExifContainerTests
    {
        [TestMethod]
        public void PropertyFillingTest()
        {
            SixLabors.ImageSharp.Image img = SixLabors.ImageSharp.Image.Load(@"c:\temp\TestImg.jpg");
            var ex = new ExifContainer(img.Metadata.ExifProfile);

            Assert.AreEqual("Dark", ex.ImageDescription);
        }
    }
}
