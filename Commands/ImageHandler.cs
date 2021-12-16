using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Commands
{
    public class ImageHandler
    {
        public enum ColorFilterTypes { Red, Green, Blue };

        public static Image PictureResize(Bitmap bmp, Size size)
        {
            if (size.Width >= 0 && size.Height >= 0)
            {
                Bitmap temp = (Bitmap)bmp;
                Bitmap bmap = new Bitmap(size.Width, size.Height, temp.PixelFormat);

                double nWidthFactor = (double)temp.Width / (double)size.Width;
                double nHeightFactor = (double)temp.Height / (double)size.Height;

                double fx, fy, nx, ny;
                int cx, cy, fr_x, fr_y;
                Color color1 = new Color();
                Color color2 = new Color();
                Color color3 = new Color();
                Color color4 = new Color();
                byte alpha, nRed, nGreen, nBlue;

                byte bp1, bp2;

                for (int x = 0; x < bmap.Width; ++x)
                {
                    for (int y = 0; y < bmap.Height; ++y)
                    {

                        fr_x = (int)Math.Floor(x * nWidthFactor);
                        fr_y = (int)Math.Floor(y * nHeightFactor);
                        cx = fr_x + 1;
                        if (cx >= temp.Width) cx = fr_x;
                        cy = fr_y + 1;
                        if (cy >= temp.Height) cy = fr_y;
                        fx = x * nWidthFactor - fr_x;
                        fy = y * nHeightFactor - fr_y;
                        nx = 1.0 - fx;
                        ny = 1.0 - fy;

                        color1 = temp.GetPixel(fr_x, fr_y);
                        color2 = temp.GetPixel(cx, fr_y);
                        color3 = temp.GetPixel(fr_x, cy);
                        color4 = temp.GetPixel(cx, cy);

                        alpha = temp.GetPixel(x, y).A;

                        // Blue
                        bp1 = (byte)(nx * color1.B + fx * color2.B);

                        bp2 = (byte)(nx * color3.B + fx * color4.B);

                        nBlue = (byte)(ny * (double)(bp1) + fy * (double)(bp2));

                        // Green
                        bp1 = (byte)(nx * color1.G + fx * color2.G);

                        bp2 = (byte)(nx * color3.G + fx * color4.G);

                        nGreen = (byte)(ny * (double)(bp1) + fy * (double)(bp2));

                        // Red
                        bp1 = (byte)(nx * color1.R + fx * color2.R);

                        bp2 = (byte)(nx * color3.R + fx * color4.R);

                        nRed = (byte)(ny * (double)(bp1) + fy * (double)(bp2));


                        bmap.SetPixel(x, y, Color.FromArgb(alpha, nRed, nGreen, nBlue));
                    }
                }
                return bmp = (Bitmap)bmap.Clone();
            }
            else return null;
        }

        public static Bitmap PictureBoxZoom(Image img, float zoomFactor)
        {
            if (img != null)
            {
                Bitmap temp;
                using (Graphics g = Graphics.FromImage(img))
                {
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    temp = new Bitmap((int)(img.Width * zoomFactor), (int)(img.Height * zoomFactor), g);
                    using (Graphics h = Graphics.FromImage(temp))
                    {
                        h.DrawImage(img, 0, 0, (int)(img.Width * zoomFactor), (int)(img.Height * zoomFactor));
                    }
                }
                return (Bitmap)(img = new Bitmap(temp));
            }
            return null;
        }

        public static Tuple<float, string> ZoomHandler(int zoomBarValue)
        {
            if (zoomBarValue == 0)
                return Tuple.Create(0.125f, "Zoom: 12.5%");
            if (zoomBarValue == 1)
                return Tuple.Create(0.25f, "Zoom: 25%");
            if (zoomBarValue == 2)
                return Tuple.Create(0.5f, "Zoom: 50%");
            if (zoomBarValue == 3)
                return Tuple.Create(1.0f, "Zoom: 100%");
            if (zoomBarValue == 4)
                return Tuple.Create(1.5f, "Zoom: 150%");
            if (zoomBarValue == 5)
                return Tuple.Create(2.0f, "Zoom: 200%");
            if (zoomBarValue == 6)
                return Tuple.Create(3.0f, "Zoom: 300%");
            if (zoomBarValue == 7)
                return Tuple.Create(4.0f, "Zoom: 400%");
            if (zoomBarValue == 8)
                return Tuple.Create(5.0f, "Zoom: 500%");
            else return Tuple.Create(-1.0f, "");
        }

        public static void RotateFlip(Image bmp, Image other, RotateFlipType rotateFlipType, bool isZoomed)
        {
            bmp.RotateFlip(rotateFlipType);
            
            if(isZoomed)
            {
                other.RotateFlip(rotateFlipType);
            }
        }

        public static void SetInvert(Bitmap bmp)
        {
            Rectangle rectangle = new Rectangle(Point.Empty, new Size(bmp.Width, bmp.Height));
            BitmapData bmpData = bmp.LockBits(rectangle, ImageLockMode.ReadWrite, bmp.PixelFormat);

            byte bitsPerPixel = GetBitsPerPixel(bmp.PixelFormat);

            IntPtr ptr = bmpData.Scan0;
            int numOfBytes = Math.Abs(bmpData.Stride * bmp.Height);
            byte[] argbValues = new byte[numOfBytes];


            if (BitConverter.IsLittleEndian)
            {
                try
                {
                    Marshal.Copy(ptr, argbValues, 0, numOfBytes);

                    for (int r = 0; r < numOfBytes; r += bitsPerPixel / 8)
                    {
                        argbValues[r] = (byte)(255 - argbValues[r]);
                    }
                    for (int g = 1; g < numOfBytes; g += bitsPerPixel / 8)
                    {
                        argbValues[g] = (byte)(255 - argbValues[g]);
                    }
                    for (int b = 2; b < numOfBytes; b += bitsPerPixel / 8)
                    {
                        argbValues[b] = (byte)(255 - argbValues[b]);
                    }

                    Marshal.Copy(argbValues, 0, ptr, numOfBytes);
                }
                finally
                {
                    bmp.UnlockBits(bmpData);
                }
            }
            else
            {
                try
                {
                    Marshal.Copy(ptr, argbValues, 0, numOfBytes);

                    for (int r = 1; r < numOfBytes; r += bitsPerPixel / 8)
                    {
                        argbValues[r] = (byte)(255 - argbValues[r]);
                    }
                    for (int g = 2; g < numOfBytes; g += bitsPerPixel / 8)
                    {
                        argbValues[g] = (byte)(255 - argbValues[g]);
                    }
                    for (int b = 3; b < numOfBytes; b += bitsPerPixel / 8)
                    {
                        argbValues[b] = (byte)(255 - argbValues[b]);
                    }

                    Marshal.Copy(argbValues, 0, ptr, numOfBytes);
                }
                finally
                {
                    bmp.UnlockBits(bmpData);
                }
            }

        }

        public static void SetGrayscale(Bitmap bmp)
        {
            Rectangle rectangle = new Rectangle(Point.Empty, new Size(bmp.Width, bmp.Height));
            BitmapData bmpData = bmp.LockBits(rectangle, ImageLockMode.ReadWrite, bmp.PixelFormat);

            byte bitsPerPixel = GetBitsPerPixel(bmp.PixelFormat);

            IntPtr ptr = bmpData.Scan0;
            int numOfBytes = Math.Abs(bmpData.Stride * bmp.Height);
            byte[] argbValues = new byte[numOfBytes];


            if (BitConverter.IsLittleEndian)
            {
                try
                {
                    Marshal.Copy(ptr, argbValues, 0, numOfBytes);

                    int g = 1;
                    int r = 2;

                    for (int b = 0; b < numOfBytes; b += bitsPerPixel / 8)
                    {
                        byte gray = (byte)(.114 * argbValues[b] + .587 * argbValues[g] + .299 * argbValues[r]);

                        argbValues[r] = argbValues[g] = argbValues[b] = gray;

                        g += bitsPerPixel / 8;
                        r += bitsPerPixel / 8;
                        if (g > numOfBytes || r > numOfBytes) break;
                    }

                    Marshal.Copy(argbValues, 0, ptr, numOfBytes);
                }
                finally
                {
                    bmp.UnlockBits(bmpData);
                }
            }
            else
            {
                try
                {
                    Marshal.Copy(ptr, argbValues, 0, numOfBytes);

                    int g = 2;
                    int b = 3;

                    for (int r = 1; r < numOfBytes; r += bitsPerPixel / 8)
                    {
                        byte gray = (byte)((.299 * argbValues[r]) + (.587 * argbValues[g]) + (.114 * argbValues[b]));

                        argbValues[r] = argbValues[g] = argbValues[b] = gray;

                        g += bitsPerPixel / 8;
                        b += bitsPerPixel / 8;
                        if (g > numOfBytes || b > numOfBytes) break;
                    }
                    Marshal.Copy(argbValues, 0, ptr, numOfBytes);
                }
                finally
                {
                    bmp.UnlockBits(bmpData);
                }
            }
        }

        public static void SetColorFilter(Bitmap bmp, ColorFilterTypes colorFilterType)
        {
            Rectangle rectangle = new Rectangle(Point.Empty, new Size(bmp.Width, bmp.Height));
            BitmapData bmpData = bmp.LockBits(rectangle, ImageLockMode.ReadWrite, bmp.PixelFormat);

            byte bitsPerPixel = GetBitsPerPixel(bmp.PixelFormat);

            IntPtr ptr = bmpData.Scan0;
            int numOfBytes = Math.Abs(bmpData.Stride * bmp.Height);
            byte[] argbValues = new byte[numOfBytes];


            if (BitConverter.IsLittleEndian)
            {
                try
                {
                    Marshal.Copy(ptr, argbValues, 0, numOfBytes);

                    if (colorFilterType == ColorFilterTypes.Red)
                    {
                        for (int b = 0; b < numOfBytes; b += bitsPerPixel / 8)
                        {
                            argbValues[b] = 0;
                        }
                        for (int g = 1; g < numOfBytes; g += bitsPerPixel / 8)
                        {
                            argbValues[g] = 0;
                        }
                    }
                    else if (colorFilterType == ColorFilterTypes.Green)
                    {
                        for (int b = 0; b < numOfBytes; b += bitsPerPixel / 8)
                        {
                            argbValues[b] = 0;
                        }
                        for (int r = 2; r < numOfBytes; r += bitsPerPixel / 8)
                        {
                            argbValues[r] = 0;
                        }
                    }
                    else
                    {
                        for (int g = 1; g < numOfBytes; g += bitsPerPixel / 8)
                        {
                            argbValues[g] = 0;
                        }
                        for (int r = 2; r < numOfBytes; r += bitsPerPixel / 8)
                        {
                            argbValues[r] = 0;
                        }
                    }

                    Marshal.Copy(argbValues, 0, ptr, numOfBytes);
                }
                finally
                {
                    bmp.UnlockBits(bmpData);
                }
            }
            else
            {
                try
                {
                    Marshal.Copy(ptr, argbValues, 0, numOfBytes);

                    if (colorFilterType == ColorFilterTypes.Red)
                    {
                        for (int g = 2; g < numOfBytes; g += bitsPerPixel / 8)
                        {
                            argbValues[g] = 0;
                        }
                        for (int b = 3; b < numOfBytes; b += bitsPerPixel / 8)
                        {
                            argbValues[b] = 0;
                        }
                    }
                    else if (colorFilterType == ColorFilterTypes.Green)
                    {
                        for (int r = 1; r < numOfBytes; r += bitsPerPixel / 8)
                        {
                            argbValues[r] = 0;
                        }
                        for (int b = 3; b < numOfBytes; b += bitsPerPixel / 8)
                        {
                            argbValues[b] = 0;
                        }
                    }
                    else
                    {
                        for (int r = 1; r < numOfBytes; r += bitsPerPixel / 8)
                        {
                            argbValues[r] = 0;
                        }
                        for (int g = 2; g < numOfBytes; g += bitsPerPixel / 8)
                        {
                            argbValues[g] = 0;
                        }
                    }
                    Marshal.Copy(argbValues, 0, ptr, numOfBytes);
                }
                finally
                {
                    bmp.UnlockBits(bmpData);
                }
            }


        }

        public static void SetGamma(Bitmap bmp, double red, double green, double blue)
        {
            byte[] redGamma = CreateGammaArray(red);
            byte[] greenGamma = CreateGammaArray(green);
            byte[] blueGamma = CreateGammaArray(blue);

            Rectangle rectangle = new Rectangle(Point.Empty, new Size(bmp.Width, bmp.Height));
            BitmapData bmpData = bmp.LockBits(rectangle, ImageLockMode.ReadWrite, bmp.PixelFormat);

            byte bitsPerPixel = GetBitsPerPixel(bmp.PixelFormat);

            IntPtr ptr = bmpData.Scan0;
            int numOfBytes = Math.Abs(bmpData.Stride * bmp.Height);
            byte[] argbValues = new byte[numOfBytes];


            if (BitConverter.IsLittleEndian)
            {
                try
                {
                    Marshal.Copy(ptr, argbValues, 0, numOfBytes);

                    for (int b = 0; b < numOfBytes; b += bitsPerPixel / 8)
                    {
                        argbValues[b] = (byte)blueGamma[argbValues[b]];
                    }
                    for (int g = 1; g < numOfBytes; g += bitsPerPixel / 8)
                    {
                        argbValues[g] = (byte)greenGamma[argbValues[g]];
                    }
                    for (int r = 2; r < numOfBytes; r += bitsPerPixel / 8)
                    {
                        argbValues[r] = (byte)redGamma[argbValues[r]];
                    }

                    Marshal.Copy(argbValues, 0, ptr, numOfBytes);
                }
                finally
                {
                    bmp.UnlockBits(bmpData);
                }
            }
            else
            {
                try
                {
                    Marshal.Copy(ptr, argbValues, 0, numOfBytes);

                    for (int r = 1; r < numOfBytes; r += bitsPerPixel / 8)
                    {
                        argbValues[r] = (byte)redGamma[argbValues[r]];
                    }
                    for (int g = 2; g < numOfBytes; g += bitsPerPixel / 8)
                    {
                        argbValues[g] = (byte)greenGamma[argbValues[g]];
                    }
                    for (int b = 3; b < numOfBytes; b += bitsPerPixel / 8)
                    {
                        argbValues[b] = (byte)blueGamma[argbValues[b]];
                    }

                    Marshal.Copy(argbValues, 0, ptr, numOfBytes);
                }
                finally
                {
                    bmp.UnlockBits(bmpData);
                }
            }
        }

        public static byte[] CreateGammaArray(double color)
        {
            byte[] gammaArray = new byte[256];
            for (int i = 0; i < 256; ++i)
            {
                gammaArray[i] = (byte)Math.Min(255, (int)((255.0 * Math.Pow(i / 255.0, 1.0 / color)) + 0.5));
            }
            return gammaArray;
        }

        public static void SetBrightness(Bitmap bmp, int brightness)
        {
            Rectangle rectangle = new Rectangle(Point.Empty, new Size(bmp.Width, bmp.Height));
            BitmapData bmpData = bmp.LockBits(rectangle, ImageLockMode.ReadWrite, bmp.PixelFormat);

            byte bitsPerPixel = GetBitsPerPixel(bmp.PixelFormat);

            IntPtr ptr = bmpData.Scan0;
            int numOfBytes = Math.Abs(bmpData.Stride * bmp.Height);
            byte[] argbValues = new byte[numOfBytes];


            if (BitConverter.IsLittleEndian)
            {
                try
                {
                    Marshal.Copy(ptr, argbValues, 0, numOfBytes);

                    for (int r = 0; r < numOfBytes; r += bitsPerPixel / 8)
                    {
                        if (argbValues[r] + brightness > 255)
                        {
                            argbValues[r] = 255;
                        }
                        else if (argbValues[r] + brightness < 0)
                        {
                            argbValues[r] = 1;
                        }
                        else
                        {
                            argbValues[r] = (byte)(argbValues[r] + brightness);
                        }

                    }
                    for (int g = 1; g < numOfBytes; g += bitsPerPixel / 8)
                    {
                        if (argbValues[g] + brightness > 255)
                        {
                            argbValues[g] = 255;
                        }
                        else if (argbValues[g] + brightness < 0)
                        {
                            argbValues[g] = 1;
                        }
                        else
                        {
                            argbValues[g] = (byte)(argbValues[g] + brightness);
                        }
                    }
                    for (int b = 2; b < numOfBytes; b += bitsPerPixel / 8)
                    {
                        if (argbValues[b] + brightness > 255)
                        {
                            argbValues[b] = 255;
                        }
                        else if (argbValues[b] + brightness < 0)
                        {
                            argbValues[b] = 1;
                        }
                        else
                        {
                            argbValues[b] = (byte)(argbValues[b] + brightness);
                        }
                    }

                    Marshal.Copy(argbValues, 0, ptr, numOfBytes);
                }
                finally
                {
                    bmp.UnlockBits(bmpData);
                }
            }
            else
            {
                try
                {
                    Marshal.Copy(ptr, argbValues, 0, numOfBytes);

                    for (int r = 1; r < numOfBytes; r += bitsPerPixel / 8)
                    {
                        if (argbValues[r] + brightness > 255)
                        {
                            argbValues[r] = 255;
                        }
                        else if (argbValues[r] + brightness < 0)
                        {
                            argbValues[r] = 1;
                        }
                        else
                        {
                            argbValues[r] = (byte)(argbValues[r] + brightness);
                        }
                    }
                    for (int g = 2; g < numOfBytes; g += bitsPerPixel / 8)
                    {
                        if (argbValues[g] + brightness > 255)
                        {
                            argbValues[g] = 255;
                        }
                        else if (argbValues[g] + brightness < 0)
                        {
                            argbValues[g] = 1;
                        }
                        else
                        {
                            argbValues[g] = (byte)(argbValues[g] + brightness);
                        }
                    }
                    for (int b = 3; b < numOfBytes; b += bitsPerPixel / 8)
                    {
                        if (argbValues[b] + brightness > 255)
                        {
                            argbValues[b] = 255;
                        }
                        else if (argbValues[b] + brightness < 0)
                        {
                            argbValues[b] = 1;
                        }
                        else
                        {
                            argbValues[b] = (byte)(argbValues[b] + brightness);
                        }
                    }

                    Marshal.Copy(argbValues, 0, ptr, numOfBytes);
                }
                finally
                {
                    bmp.UnlockBits(bmpData);
                }
            }
        }

        public static void SetContrast(Bitmap bmp, double contrast)
        {
            Rectangle rectangle = new Rectangle(Point.Empty, new Size(bmp.Width, bmp.Height));
            BitmapData bmpData = bmp.LockBits(rectangle, ImageLockMode.ReadWrite, bmp.PixelFormat);

            byte bitsPerPixel = GetBitsPerPixel(bmp.PixelFormat);

            IntPtr ptr = bmpData.Scan0;
            int numOfBytes = Math.Abs(bmpData.Stride * bmp.Height);
            byte[] argbValues = new byte[numOfBytes];

            contrast = (100.0 + contrast) / 100.0;
            contrast *= contrast;

            if (BitConverter.IsLittleEndian)
            {
                try
                {
                    Marshal.Copy(ptr, argbValues, 0, numOfBytes);

                    for (int r = 0; r < numOfBytes; r += bitsPerPixel / 8)
                    {
                        double pR = argbValues[r] / 255.0;
                        pR -= 0.5;
                        pR *= contrast;
                        pR += 0.5;
                        pR *= 255;
                        if (pR < 0) pR = 0;
                        if (pR > 255) pR = 255;

                        argbValues[r] = (byte)pR;
                    }
                    for (int g = 1; g < numOfBytes; g += bitsPerPixel / 8)
                    {
                        double pG = argbValues[g] / 255.0;
                        pG -= 0.5;
                        pG *= contrast;
                        pG += 0.5;
                        pG *= 255;
                        if (pG < 0) pG = 0;
                        if (pG > 255) pG = 255;

                        argbValues[g] = (byte)pG;
                    }
                    for (int b = 2; b < numOfBytes; b += bitsPerPixel / 8)
                    {
                        double pB = argbValues[b] / 255.0;
                        pB -= 0.5;
                        pB *= contrast;
                        pB += 0.5;
                        pB *= 255;
                        if (pB < 0) pB = 0;
                        if (pB > 255) pB = 255;

                        argbValues[b] = (byte)pB;
                    }

                    Marshal.Copy(argbValues, 0, ptr, numOfBytes);
                }
                finally
                {
                    bmp.UnlockBits(bmpData);
                }
            }
            else
            {
                try
                {
                    Marshal.Copy(ptr, argbValues, 0, numOfBytes);

                    for (int r = 1; r < numOfBytes; r += bitsPerPixel / 8)
                    {
                        double pR = argbValues[r] / 255.0;
                        pR -= 0.5;
                        pR *= contrast;
                        pR += 0.5;
                        pR *= 255;
                        if (pR < 0) pR = 0;
                        if (pR > 255) pR = 255;

                        argbValues[r] = (byte)pR;
                    }
                    for (int g = 2; g < numOfBytes; g += bitsPerPixel / 8)
                    {
                        double pG = argbValues[g] / 255.0;
                        pG -= 0.5;
                        pG *= contrast;
                        pG += 0.5;
                        pG *= 255;
                        if (pG < 0) pG = 0;
                        if (pG > 255) pG = 255;

                        argbValues[g] = (byte)pG;
                    }
                    for (int b = 3; b < numOfBytes; b += bitsPerPixel / 8)
                    {
                        double pB = argbValues[b] / 255.0;
                        pB -= 0.5;
                        pB *= contrast;
                        pB += 0.5;
                        pB *= 255;
                        if (pB < 0) pB = 0;
                        if (pB > 255) pB = 255;

                        argbValues[b] = (byte)pB;
                    }

                    Marshal.Copy(argbValues, 0, ptr, numOfBytes);
                }
                finally
                {
                    bmp.UnlockBits(bmpData);
                }
            }
        }

        public static byte[] StoreCurrentBitmap(Bitmap currentBmp)
        {
            Rectangle r = new Rectangle(Point.Empty, new Size(currentBmp.Width, currentBmp.Height));
            BitmapData cbmpData = currentBmp.LockBits(r, ImageLockMode.ReadWrite, currentBmp.PixelFormat);


            IntPtr cptr = cbmpData.Scan0;
            int cbytes = Math.Abs(cbmpData.Stride * currentBmp.Height);
            byte[] cRgbValues = new byte[cbytes];

            try
            {
                Marshal.Copy(cptr, cRgbValues, 0, cbytes);
            }
            catch (AccessViolationException) { }
            finally
            {
                currentBmp.UnlockBits(cbmpData);
            }


            return cRgbValues;
        }

        private static byte GetBitsPerPixel(PixelFormat pixFormat)
        {
            switch (pixFormat)
            {
                case PixelFormat.Format1bppIndexed: return 1;
                case PixelFormat.Format4bppIndexed: return 4;
                case PixelFormat.Format8bppIndexed: return 8;

                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format16bppGrayScale:
                case PixelFormat.Format16bppRgb555: return 16;

                case PixelFormat.Format24bppRgb: return 24;

                case PixelFormat.Canonical:
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format32bppRgb: return 32;

                case PixelFormat.Format48bppRgb: return 48;

                case PixelFormat.Format64bppArgb:
                case PixelFormat.Format64bppPArgb: return 64;

                default: return 32;
            }
        }

        public static void ComparePixelBytes(Bitmap oldBmp, Bitmap newBmp) //incomplete
        {
            Rectangle oldR = new Rectangle(Point.Empty, new Size(oldBmp.Width, oldBmp.Height));
            Rectangle newR = new Rectangle(Point.Empty, new Size(newBmp.Width, newBmp.Height));
            BitmapData oldBmpData = oldBmp.LockBits(oldR, ImageLockMode.ReadOnly, oldBmp.PixelFormat);
            BitmapData newBmpData = newBmp.LockBits(newR, ImageLockMode.ReadOnly, newBmp.PixelFormat);

            IntPtr oldPtr = oldBmpData.Scan0;
            IntPtr newPtr = newBmpData.Scan0;

            int oldLength = Math.Abs(oldBmpData.Stride * oldBmp.Height);
            int newLength = Math.Abs(newBmpData.Stride * newBmp.Height);

            byte[] oldBytes = new byte[oldLength];
            byte[] newBytes = new byte[newLength];

            Marshal.Copy(oldPtr, oldBytes, 0, oldLength);
            Marshal.Copy(newPtr, newBytes, 0, newLength);

            if (oldLength == newLength)
                for (int x = 0; x < oldLength; x++)
                {
                    if (newBytes[x] == oldBytes[x])
                    {

                    }
                    else if (newBytes[x] < oldBytes[x])
                    {

                    }
                    else if (newBytes[x] > oldBytes[x])
                    {

                    }
                }
            else throw new Exception("The images are of unequal Sizes");
        }

        public static Tuple<Point, Size> RectAlgorithm(Point startPoint, Point endPoint)
        {
            if (startPoint.X < endPoint.X && startPoint.Y < endPoint.Y)
            {
                return Tuple.Create(new Point(startPoint.X, startPoint.Y),
                    new Size(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y));
            }
            else if (startPoint.X < endPoint.X && startPoint.Y > endPoint.Y)
            {
                return Tuple.Create(new Point(startPoint.X, endPoint.Y),
                    new Size(endPoint.X - startPoint.X, startPoint.Y - endPoint.Y));
            }
            else if (endPoint.X < startPoint.X && endPoint.Y < startPoint.Y)
            {
                return Tuple.Create(new Point(endPoint.X, endPoint.Y),
                    new Size(startPoint.X - endPoint.X, startPoint.Y - endPoint.Y));
            }
            else
            {
                return Tuple.Create(new Point(endPoint.X, startPoint.Y),
                    new Size(startPoint.X - endPoint.X, endPoint.Y - startPoint.Y));
            }
        }

    }
}
