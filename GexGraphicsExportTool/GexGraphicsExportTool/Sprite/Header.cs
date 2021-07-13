using System;
using System.Collections.Generic;
using System.Text;

namespace GexGraphicsExportTool.Sprite
{
    struct Header
    {
        //TODO data indentifiers
        public Int32 bitmap_shiftX; // fixed-point 16.16
        public Int32 bitmap_shiftY; // fixed-point 16.16

        // 85 99 FF FF
        /*
        public byte graphicsType;
        public byte extensionSymbol1; //0x99
        public short extensionSymbol2;//0xFFFF
        */

        public Chunk[] parts;

        public Int32 sizeOfAligmentMap; // including this DWORD
    }
}
