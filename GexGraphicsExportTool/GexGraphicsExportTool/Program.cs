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
            long bytesToSteamEnd = stream.Length - stream.Position;
            for (long i = stream.Position; i < bytesToSteamEnd; i++)
            {
        
                for (int j = 0; reader.ReadByte() == arr[j]; j++)
                {
                    if (i + j >= bytesToSteamEnd) break;
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
            Console.WriteLine("OFFSET \t\tUNKNOWN 1 \tUNKNOWN 2");
            long index;
            Sprite.Sprite sprite;
            Random rand = new Random();
            using (FileStream stream = new FileStream(@"GEX002.LEV", FileMode.Open, FileAccess.Read))
            {
                sprite = new Sprite.Sprite();
                while ((index = streamIndexOf(new byte[4]{0x85, 0x99, 0xff, 0xff}, stream)) > -1){
                    
                    stream.Position -= 16;
                    sprite.deserializeStream(stream);
                    Console.WriteLine(index.ToString("X")+"\t\t0x"
                        + sprite.header.unknown_1.ToString("X").Substring(0, 2)+"\t\t0x"
                        + sprite.header.unknown_2.ToString("X").Substring(0, 2) + "");
                    
                    Bitmap bitmap = Sprite.Drawing.CreateBitmap(sprite);
                    bitmap.Save("exported/"+index.ToString("X") +"-"
                        + sprite.header.unknown_1.ToString("X").Substring(0, 2)+"-"
                        + sprite.header.unknown_2.ToString("X").Substring(0, 2)
                        + ".png", ImageFormat.Png);
                }
            }
            
        }
    }
}
