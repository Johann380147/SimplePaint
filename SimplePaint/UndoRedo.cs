using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace SimplePaint
{
    class UndoRedo
    {
        List<byte[]> undoBitmapData = new List<byte[]>(50);
        List<byte[]> redoBitmapData = new List<byte[]>(50);

        public UndoRedo()
        {
            //undoBitmapData.OnAdd += listOnAdd;
        }

        public void Undo(Bitmap bmp)
        {
            if (bmp == null || undoBitmapData.Count < 2) return;

            Rectangle r = new Rectangle(Point.Empty, new Size(bmp.Width, bmp.Height));
            BitmapData bmpData = bmp.LockBits(r, ImageLockMode.ReadWrite, bmp.PixelFormat);

            int byteSize = Math.Abs(bmpData.Stride * bmp.Height);

            IntPtr ptr = bmpData.Scan0;
            byte[] rgbValues = undoBitmapData[undoBitmapData.Count - 2];


            try
            {
                if (byteSize == rgbValues.Length)
                {
                    Marshal.Copy(rgbValues, 0, ptr, rgbValues.Length);


                    if (redoBitmapData.Count >= 50) redoBitmapData.RemoveAt(0);
                    redoBitmapData.Add(undoBitmapData[undoBitmapData.Count - 1]);
                    undoBitmapData.RemoveAt(undoBitmapData.Count - 1);
                }
            }
            catch (AccessViolationException)
            {
                undoBitmapData.Add(redoBitmapData[redoBitmapData.Count - 1]);
                redoBitmapData.RemoveAt(redoBitmapData.Count - 1);
            }
            finally
            {
                bmp.UnlockBits(bmpData);
            }
            
        }

        public void Redo(Bitmap bmp)
        {
           
            if (bmp == null || redoBitmapData.Count < 1) return;

            Rectangle r = new Rectangle(Point.Empty, new Size(bmp.Width, bmp.Height));
            BitmapData bmpData = bmp.LockBits(r, ImageLockMode.ReadWrite, bmp.PixelFormat);

            IntPtr ptr = bmpData.Scan0;
            byte[] rgbValues = redoBitmapData[redoBitmapData.Count - 1];


            try
            {
                Marshal.Copy(rgbValues, 0, ptr, rgbValues.Length);

                if (undoBitmapData.Count >= 50) undoBitmapData.RemoveAt(0);
                undoBitmapData.Add(rgbValues);
                redoBitmapData.RemoveAt(redoBitmapData.Count - 1);
            }
            catch (AccessViolationException)
            {
                redoBitmapData.Add(undoBitmapData[undoBitmapData.Count - 1]);
                undoBitmapData.RemoveAt(undoBitmapData.Count - 1);
            }
            finally
            {
                bmp.UnlockBits(bmpData);
            }
            
        }

        public void InsertUndoBitmapData(byte[] byteArray)
        {
            if (undoBitmapData.Count >= 50) undoBitmapData.RemoveAt(0);
            undoBitmapData.Add(byteArray);

        }

        public void DeleteUndoRedoBitmapData()
        {
            undoBitmapData.Clear();
            redoBitmapData.Clear();
        }

    }



    
}



        