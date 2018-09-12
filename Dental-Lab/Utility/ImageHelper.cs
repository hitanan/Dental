﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Dental_Lab.Utility
{
    public static class ImageHelper
    {
        public const string DEFAULT_IMAGE = "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";
        public static BitmapImage Base64StringToBitmapImage(string base64String)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(Convert.FromBase64String(base64String));
            bitmapImage.EndInit();
            return bitmapImage;
        }

        public static string FileToBase64String(string filename)
        {
            using (var stream = File.Open(filename, FileMode.Open))
            using (var reader = new BinaryReader(stream))
            {
                byte[] allData = reader.ReadBytes((int)reader.BaseStream.Length);
                return Convert.ToBase64String(allData);
            }
        }

    }
}
