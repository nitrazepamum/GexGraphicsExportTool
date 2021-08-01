using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GexGraphicsExportTool.Sprite
{
    class Sprite
    {
        public Header header;
        public byte[] aligmentMap;
        public byte[] bitmap;
        public Palettes.Palette colorPalette = new Palettes.gexPalette(); //Temporary here

        public int Width
        {
            get
            {
                int max = 0;
                foreach (Chunk part in header.parts)
                {
                    int current =part.width + part.rel_positionX; // 0x3C - part.positionX + 
                    if (current > max) max = current;
                }
                return max;
            }
        }
        public int Height
        {
            get
            {
                int max = 0;
                foreach (Chunk part in header.parts)
                {
                    int current = part.rel_positionY + part.height; //part.positionY  * 4 + part.height +
                    if (current > max) max = current;
                }
                return max;
            }
        }
        public int Size
        {
            get
            {
                return this.Height * this.Width;
            }
        }

        private int calcBitmapLen()
        {
            int bitmapPixelsCount = 0;
            foreach (Chunk chunk in header.parts)
            {
                bitmapPixelsCount += chunk.width * chunk.height;
            }

            int bytesUsed = 0;
            int pixelsDrawn = 0;

            foreach (byte val in aligmentMap)
            {
                pixelsDrawn += (val < 0x70 ? val * 4 : 32 - (0x88 - val) * 4);
                bytesUsed += (val < 0x70 ? val * 4 : 4);
                if (pixelsDrawn >= bitmapPixelsCount) break;
            }

            return bytesUsed;

        }
        public void deserializeStream(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);

            //////////// HEADER ////////////
            this.header.unknown_1 = reader.ReadInt32();
            this.header.unknown_2 = reader.ReadInt32();
            this.header.bitmap_shiftX = reader.ReadInt32();
            this.header.bitmap_shiftY = reader.ReadInt32();
            reader.ReadBytes(4); // easy recognize symbols 85 99 ff ff

            header.parts = new Chunk[8];
            //Chunks info (not calculated)
            int i = 0;
            do
            {
                header.parts[i] = new Chunk();
                header.parts[i].positionX = reader.ReadByte();
                header.parts[i].positionY = reader.ReadByte();
                header.parts[i].width = reader.ReadByte();
                header.parts[i].height = reader.ReadByte();
                header.parts[i].rel_positionX = reader.ReadInt16();
                header.parts[i].rel_positionY = reader.ReadInt16();
                i++;
            } while (header.parts[i - 1].width > 0);

            header.sizeOfAligmentMap = reader.ReadInt32();

            /////////// ALIGMENT MAP ///////////
            aligmentMap = reader.ReadBytes(header.sizeOfAligmentMap - 4);

            ///////////// BITMAP /////////////
            bitmap = reader.ReadBytes(calcBitmapLen());

            

        }
    }
}
