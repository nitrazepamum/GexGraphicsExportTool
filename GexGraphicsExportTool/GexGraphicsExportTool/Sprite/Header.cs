using System;
using System.Collections.Generic;
using System.Text;

namespace GexGraphicsExportTool.Sprite
{
    class Header
    {
        

        //data indentifiers
        public Int32 unknown_1;
        public Int32 unknown_2;

        public Int32 bitmap_shiftX; // fixed-point 16.16
        public Int32 bitmap_shiftY; // fixed-point 16.16

        public static string signature = "85 ?? FF FF";

        public Chunk[] parts;

        public Int32 sizeOfAlignmentMap; // including this DWORD
    }
}
