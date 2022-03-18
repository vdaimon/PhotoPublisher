using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp;
using System.ComponentModel;

namespace PhotoPublisher
{
    public class ExifContainer : INotifyPropertyChanged 
    {
        private ExifProfile _exif;
        public event PropertyChangedEventHandler PropertyChanged;
        public string ImageDescription { get => GetExifProperty(ExifTag.ImageDescription); set => SetExifProperty(ExifTag.ImageDescription, value);}
        public byte[] XPSubject { get => GetExifProperty(ExifTag.XPSubject); set=> SetExifProperty(ExifTag.XPSubject, value);}
        public Rational ApertureValue { get => GetExifProperty(ExifTag.ApertureValue); set => SetExifProperty(ExifTag.ApertureValue, value); }





        public ExifContainer (ExifProfile exif)
        {
            _exif = exif;
        }
        private void SetExifProperty<T>(ExifTag<T> tag, T value)
        {
            var currentValue = GetExifProperty<T>(tag);

            if (currentValue != null)
            {
                if (!currentValue.Equals(value))
                {
                    _exif.SetValue(tag, value);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageDescription)));
                }

            }
            else if (value != null)
            {
                _exif.SetValue(tag, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageDescription)));
            }
        }
        private T GetExifProperty<T>(ExifTag<T> tag)
        {
            var et = _exif.GetValue(tag);
            if (et != null)
                return et.Value;

            return default(T);
        }

    }
}
