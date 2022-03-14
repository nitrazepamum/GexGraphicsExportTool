using System;
using System.Collections.Generic;
using System.Text;

namespace GexGraphicsExportTool.GBitmap
{
    struct Chunk
    {
        public byte positionX;
        public byte positionY;
        public byte width;
        public byte height;
        public Int16 rel_positionX;
        public Int16 rel_positionY;

        //TODO check positionX, positionY mechanism
        public short getAbsoluteX()
        {

            return (short)(rel_positionX);
        }
        public short getAbsoluteY()
        {
            return (short)(rel_positionY);//((short)positionY * 4 + rel_positionY);
        }
        public int size
        {
            get { return width * height; }
        }
    }
}
