using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
namespace PhotoPublisher
{
    internal class SixLaborsTest
    {
        public Image Image { get;}

        public SixLaborsTest(string path)
        {
            using (Image img = Image.Load(path))
            {
            }
        }


    }
}
