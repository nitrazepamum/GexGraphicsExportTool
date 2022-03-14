using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace GexGraphicsExportTool.GBitmap
{
    class Drawing
    {
        public static Bitmap bitmap;
        static GBitmap gexBitmap;
        static int relX = 0;
        static int relY = 0;

        static public Bitmap CreateBitmap(GBitmap gexBitmap)
        {
            if (gexBitmap.Size == 0) return null; // bad size

            Drawing.gexBitmap = gexBitmap;
            bitmap = new Bitmap(gexBitmap.Width, gexBitmap.Height);

            // Raw pixels one by one
            Stream pixelStream = new MemoryStream();

            //Iteration of proccessing data
            int bitmap_index = 0;

            // Pixel Stream writing  
            using (BinaryWriter pixelWriter = new BinaryWriter(pixelStream))
            {
                // ---- aligment map values processing functions ----
                void writeBitmapPixels()
                {

                    if (bitmap_index >= gexBitmap.bitmap.Length) return;
                    pixelWriter.Write(gexBitmap.bitmap[bitmap_index]);
                    bitmap_index++;

                }

                // end of wrtiting functions, back to normal instructions
                //
                // ---- Pixel stream writing ----
                while (bitmap_index < gexBitmap.Size)
                {
                    writeBitmapPixels();
                }

                pixelStream.Position = 0;
                //________ Painting Bitmap ________
                using (BinaryReader pixelReader = new BinaryReader(pixelStream))
                {
                    // Part by part
                    foreach (Chunk part in gexBitmap.header.parts)
                    {
                        int pixelX = 0;
                        int pixelY = 0;

                        relX = part.getAbsoluteX();
                        relY = part.getAbsoluteY();

                        for (int i = 0; i < part.width * part.height; i++)
                        {
                            if (pixelStream.Position >= pixelStream.Length) break;
                            drawColorIndexedPixel(relX+pixelX, relY+pixelY, pixelReader.ReadByte());
                            autoNextLine(ref pixelX, ref pixelY, part.width);
                        }
                    }
                }
                pixelStream.Close();

                return bitmap;
            }
        }

        static void autoNextLine(ref int pixelX, ref int pixelY, int rowWidth)
        {
            pixelX++;
            if (pixelX >= rowWidth)
            {
                pixelX = 0;
                pixelY++;
            }
        }

        // important functions //
        static void drawColorIndexedPixel(int pixelX, int pixelY, byte colorIndex)
        {
            Color color = gexBitmap.colorPalette.getColor(colorIndex);
            if (colorIndex == 0) color = Color.Transparent;
            bitmap.SetPixel(pixelX, pixelY, color);
        }

    }
}
