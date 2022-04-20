using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GexGraphicsExportTool.Sprite
{
    class Sprite
    {
        public Header header = new Header();
        public Stream alignmentMap;
        public BinaryReader alignmentMapReader;
        public byte[] bitmap;
        public Palettes.Palette colorPalette = new Palettes.gexPalette(); //Temporary here

        public int Width
        {
            get
            {
                int max = 0;
                foreach (Chunk part in header.parts)
                {
                    if (part == null) break;
                    int current = part.width + part.rel_positionX; // 0x3C - part.positionX + 
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
                    if(part == null) break;
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
                if (chunk == null) break;
                bitmapPixelsCount += chunk.width * chunk.height;
            }

            int bytesUsed = 0;
            int pixelsDrawn = 0;

            for (int i = 0; i < alignmentMap.Length; i++) {
                var manipulation = nextPixelOperation();
                int val = manipulation.value;
                byte operation = manipulation.operation;
                

                pixelsDrawn += (operation == 0 ? val * 4 : 32 - (0x88 - val) * 4);
                bytesUsed += (operation == 0 ? val * 4 : 4);
                if (pixelsDrawn >= bitmapPixelsCount) break;
            }
            alignmentMap.Position = 0;
            return bytesUsed;

        }

        /// <summary>
        /// checks what bitmap processor should do with aligment map value
        /// </summary>
        /// <param name="value">
        /// Raw aligmentMap value (just alignmentMap[i])
        /// </param>
        /// <returns>operation type</returns>
        private byte mapOperation (byte value){
            return (byte)((value / 128) % 2);
        }

        /// <summary>
        /// gives full value of aligment map with addition of 128 to big values
        /// </summary>
        /// <returns></returns>
        public PixelOperationType nextPixelOperation()
        {
            if (alignmentMap.Length <= alignmentMap.Position)
                return new PixelOperationType(0, 0);

            ushort val = alignmentMapReader.ReadByte();
            PixelOperationType pixelOperation = new PixelOperationType(mapOperation((byte)val), val);

            // join big numbers
            if (val == 0 || val == 0x80)
            {
                pixelOperation.value += 128;
            }

            return pixelOperation;
            //return (short)(alignmentMap[i]); //+ (alignmentMap[i - 1] == 0 ? 128 :  0)); 
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

            header.parts = new Chunk[32];
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

            header.sizeOfAlignmentMap = reader.ReadInt32() - 4;

            /////////// ALIGNMENT MAP ///////////
            
            alignmentMap = new MemoryStream(reader.ReadBytes(header.sizeOfAlignmentMap));
            alignmentMapReader = new BinaryReader(alignmentMap);

            ///////////// BITMAP /////////////
            bitmap = reader.ReadBytes(calcBitmapLen());

            

        }
    }
}
