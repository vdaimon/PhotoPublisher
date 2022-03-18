using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PhotoPublisher
{

    public class EditMetadataFormDatacontext : INPC
    {
        public ImageContainer BaseImg { get; }

        private Dictionary<ExifTag, object> _exif = new Dictionary<ExifTag, object>
        {
            { ExifTag.ImageDescription, null },
            { ExifTag.XPSubject, null },
            { ExifTag.Artist, null },

            { ExifTag.Make, null },
            { ExifTag.Model, null },

            { ExifTag.ApertureValue, null },
            { ExifTag.ExposureTime, null },
            { ExifTag.ISOSpeedRatings, null },
            { ExifTag.FocalLength, null },
            { ExifTag.MaxApertureValue, null },
            { ExifTag.FocalLengthIn35mmFilm, null },


            { ExifTag.GPSLatitude, null },
            { ExifTag.GPSLongitude, null },

        };
        public Dictionary<ExifTag, object> Exif { get => _exif; set => Set(ref _exif, value); }

        private ExifContainer _exifContainer;
        public ExifContainer ExifContainer { get => _exifContainer; set => Set(ref _exifContainer, value); }
        public EditMetadataFormDatacontext(ImageContainer baseImg)
        {
            BaseImg = baseImg;
            var mD = baseImg.SixLaborsImage.Metadata;

            if (mD.ExifProfile == null)
                return;

            var ex = mD.ExifProfile.Values;

            ExifContainer = new ExifContainer(mD.ExifProfile);

            foreach (var el in ex)
                if (_exif.ContainsKey(el.Tag))
                    _exif[el.Tag] = el.GetValue();
        }

    }
    public partial class EditMetadataForm : Window
    {
        private EditMetadataFormDatacontext _dataContext;
        public EditMetadataForm(ImageContainer baseImg)
        {
            _dataContext = new EditMetadataFormDatacontext(baseImg);
            DataContext = _dataContext;

            InitializeComponent();
        }
    }
}
