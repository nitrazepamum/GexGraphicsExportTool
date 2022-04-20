using System;
using System.Collections.Generic;
using System.Text;

namespace GexGraphicsExportTool.Sprite
{
    class Chunk
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
            //Int32 expected = rel_positionY / 8;
            //Int32 sub = (expected - positionY) * 8;
            return (short)(rel_positionY);//+sub);
        }
        public int size
        {
            get { return width * height; }
        }
    }
}
