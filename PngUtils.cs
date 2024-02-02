using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gltfMaker
{
    class PngUtils
    {
        public static void TransparentPng(string fileName, float alpha, string outFileName)
        {
            Bitmap bitmap = (Bitmap)Bitmap.FromFile(fileName, true);
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color c = bitmap.GetPixel(x, y);
                    Color c2 = Color.FromArgb((int)(alpha * 255), c.R, c.G, c.B);
                    bitmap.SetPixel(x, y, c2);
                }
            }
            bitmap.Save(outFileName);
        }
    }
}
