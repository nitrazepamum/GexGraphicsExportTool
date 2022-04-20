using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace GexGraphicsExportTool.Sprite
{
    struct PixelOperationType
    {
        public byte operation; // 0 - bitmap pixels // 1 - fillment
        public ushort value; // count
        public PixelOperationType(byte operation, ushort value)
        {
            this.operation = operation;
            this.value = value;
        }
    }
    struct Drawing
    {
        static Bitmap bitmap;
        static Sprite sprite;
        static int relX = 0;
        static int relY = 0;

        static public Bitmap CreateBitmap(Sprite sprite)
        {
            if (sprite.Size == 0) return null;
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
                void writeBitmapPixels(int mapValue)
                {
                    int pixelsPerBlock = mapValue * 4;

                   

                    for (int j = 0; j < pixelsPerBlock; j++)
                    {
                        if (bitmap_index >= sprite.bitmap.Length) return;
                        pixelWriter.Write(sprite.bitmap[bitmap_index]);
                        bitmap_index++;
                    }

                 }

                void writeFillmentData(int mapValue)
                {
                    short val = (short)mapValue;
                    int shift = 32 - (0x88 - val) * 4;
                    for (int j = 0; j < shift; j++)
                    {
                        //repeat 4 bytes
                        if (bitmap_index + j % 4 >= sprite.bitmap.Length) return;
                        pixelWriter.Write(sprite.bitmap[bitmap_index + j % 4]);
                    }
                    bitmap_index += 4;
                }
                // end of wrtiting functions, back to normal instructions
                //
                // ---- Pixel stream writing ----
                sprite.alignmentMap.Position = 0;
                while (map_index < sprite.header.sizeOfAlignmentMap)
                {

                    var manipulation = sprite.nextPixelOperation();
                    int mapValue = manipulation.value;
                    byte operation = manipulation.operation;

                    if (operation == 0)
                    {
                        writeBitmapPixels(mapValue);
                    }
                    else
                    {
                        writeFillmentData(mapValue);
                    }
                    map_index++;
                } 

                pixelStream.Position = 0;
                //________ Painting Bitmap ________
                using (BinaryReader pixelReader = new BinaryReader(pixelStream))
                {
                    // Part by part
                    foreach (Chunk part in sprite.header.parts)
                    {
                        if (part == null) break;
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
            Color color = sprite.colorPalette.getColor(colorIndex);
            if (colorIndex == 0) color = Color.Transparent;
            bitmap.SetPixel(pixelX, pixelY, color);
        }

    }
}
