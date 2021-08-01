using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace GexGraphicsExportTool.Sprite
{
    class Drawing
    {
        static Bitmap bitmap;
        static Sprite sprite;
        static int relX = 0;
        static int relY = 0;

        static public Bitmap CreateBitmap(Sprite sprite)
        {
            Drawing.sprite = sprite;
            bitmap = new Bitmap(sprite.Width, sprite.Height);

            // Raw pixels one by one
            Stream pixelStream = new MemoryStream();

            //Iteration of proccessing data
            int map_index = 0;
            int bitmap_index = 0;

            // Pixel Stream writing  
            using (BinaryWriter pixelWriter = new BinaryWriter(pixelStream))
            {
                // ---- aligment map values processing functions ----
                void writeBitmapPixels()
                {
                    int pixelsPerBlock = sprite.aligmentMap[map_index] * 4;

                    for (int j = 0; j < pixelsPerBlock; j++)
                    {
                        if (bitmap_index >= sprite.bitmap.Length) return;
                        pixelWriter.Write(sprite.bitmap[bitmap_index]);
                        bitmap_index++;
                    }

                 }

                void writeFillmentData()
                {
                    int shift = 32 - (0x88 - sprite.aligmentMap[map_index]) * 4;
                    for (int j = 0; j < shift; j++)
                    {
                        //repeat 4 bytes
                        if (bitmap_index + j % 4 >= sprite.bitmap.Length) return;
                       pixelWriter.Write(sprite.bitmap[bitmap_index + j % 4]);
                    }
                    bitmap_index += 4;
                }
                // end of wrtiting functions, back to normal instructions
                // ---- Pixel stream writing ----
                for (int i = 0; i < sprite.size; i++)
                {

                    if (map_index >= sprite.aligmentMap.Length) break;

                    if (sprite.aligmentMap[map_index] < 0x70)
                    {
                        writeBitmapPixels();
                    }
                    else
                    {
                        writeFillmentData();
                    }
                    map_index++;
                } // (end) using BinaryWriter pixelWriter

                pixelStream.Position = 0;
                //________ Painting Bitmap ________
                using (BinaryReader pixelReader = new BinaryReader(pixelStream))
                {
                    // Part by part
                    foreach (Chunk part in sprite.header.parts)
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

        /*static int revertedY(int y)
        {
            return sprite.Height - y - 1;
        } */
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
        //!!! reverts y here
        static void drawColorIndexedPixel(int pixelX, int pixelY, byte colorIndex)
        {
            Color color = sprite.colorPalette.getColor(colorIndex);
            if (colorIndex == 0) color = Color.Transparent;
            bitmap.SetPixel(pixelX, pixelY, color);
        }

    }
}
