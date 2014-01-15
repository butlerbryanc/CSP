using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CSP.Models
{
    public class Thumbnail 
    {
        //
        // GET: /Thumbnail/
        private static Dictionary<string, ImageCodecInfo> encoders = null;

       

        public static Dictionary<string, ImageCodecInfo> Encoders
        {
            get
            {
                if (encoders == null)
                {
                    encoders = new Dictionary<string, ImageCodecInfo>();
                }

                if (encoders.Count == 0)
                {
                    foreach(ImageCodecInfo codec in ImageCodecInfo.GetImageEncoders())
                    {
                        encoders.Add(codec.MimeType.ToLower(), codec);
                    }
                }

                return encoders;
            }
        }

        public static string GetThumbnail(string fileName)
        {
            string path = "C:\\Users\\Bryan\\Documents\\Visual Studio 2013\\Projects\\CSP\\CSP\\Images\\Products\\";

            string str = System.IO.Path.Combine(path, fileName);

            Bitmap myBitmap = new Bitmap(str);

            Bitmap thumbNail = ResizeImage(myBitmap, 180, 180);

            string newFile = SaveImage(fileName, thumbNail, 100, "-Thumbnail");

            return newFile;
        }

        public static string GetProductProfileImage(string fileName)
        {
            string path = "C:\\Users\\Bryan\\Documents\\Visual Studio 2013\\Projects\\CSP\\CSP\\Images\\Products\\";

            string str = System.IO.Path.Combine(path, fileName);

            Bitmap myBitmap = new Bitmap(str);

            Bitmap myProduct = ResizeImage(myBitmap, 400, 400);

            string newFile = SaveImage(fileName, myProduct, 100, "-ProductProfile");

            return newFile;

        }

        public static System.Drawing.Bitmap ResizeImage(System.Drawing.Bitmap image, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);

            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (Graphics graphics = Graphics.FromImage(result))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                graphics.DrawImage(image, 0, 0, result.Width, result.Height);


            }

            return result;
        }

        public static string SaveImage(string fileName, Image image, int quality, string imageTag)
        {
            bool newName = true;
            var str = "";
            var str2 = "";
            var newFile = System.IO.Path.GetFileNameWithoutExtension(fileName) + imageTag +".jpg";

            EncoderParameter qualityParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            ImageCodecInfo imageCodec = GetEncoderInfo("image/jpeg");

            EncoderParameters encoderParameters = new EncoderParameters(1);

            encoderParameters.Param[0] = qualityParameter;

            string path = "C:\\Users\\Bryan\\Documents\\Visual Studio 2013\\Projects\\CSP\\CSP\\Images\\Products\\";
            var allFilenames = Directory.EnumerateFiles(path).Select(p => Path.GetFileName(p));

            foreach(var name in allFilenames)
            {
                str = System.IO.Path.GetFileNameWithoutExtension(name);
                str2 = System.IO.Path.GetFileNameWithoutExtension(fileName) + imageTag;

                if(str == str2)
                {
                    newName = false;
                }

            }

            if (newName == true)
            {
                newFile = str2 +".jpg";
                try 
                {
                    using (Bitmap tempImage = new Bitmap(image))
                    {
                        tempImage.Save(path+newFile, imageCodec, encoderParameters);
                    }
                }

                catch (Exception e)
                {
                    
                }

            }

            return newFile;

        }

        public static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            string lookupKey = mimeType.ToLower();

            ImageCodecInfo foundCodec = null;

            if (Encoders.ContainsKey(lookupKey))
            {
                foundCodec = Encoders[lookupKey];
            }

            return foundCodec;
        }

    }
}
