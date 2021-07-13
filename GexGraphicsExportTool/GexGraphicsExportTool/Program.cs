using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace GexGraphicsExportTool
{
    class Program
    {
        static long streamIndexOf(byte[] arr, Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            
                int valid = 0;
                for (long i = stream.Position; i < stream.Length; i++)
                {
                    for (int j = 0; reader.ReadByte() == arr[j]; j++)
                    {
                        valid++;
                    if (valid >= arr.Length)
                    {
                        stream.Position -= arr.Length;
                        return i;
                    }
                    }
                    valid = 0;
                }
                return -1;
            
        }

        static void Main(string[] args)
        {
            long index;
            Sprite.Sprite sprite;
            Random rand = new Random();
            using (FileStream stream = new FileStream(@"GEX002.LEV", FileMode.Open, FileAccess.Read))
            {
                sprite = new Sprite.Sprite();
                while ((index = streamIndexOf(new byte[4]{0x85, 0x99, 0xff, 0xff}, stream)) > -1){
                    Console.WriteLine(index);
                    stream.Position -= 8;
                    sprite.deserializeStream(stream);
                    Bitmap bitmap = Sprite.Drawing.CreateBitmap(sprite);
                    bitmap.Save("exported/"+index+".png", ImageFormat.Png);
                }
            }
            
        }
    }
}
