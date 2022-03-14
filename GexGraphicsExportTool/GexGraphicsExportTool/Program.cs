using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace GexGraphicsExportTool
{
    class Program
    {
        /// <summary>
        /// Array of Bytes scan from stream function
        /// </summary>
        /// <param name="arr">
        /// searched array of bytes in "XX XX XX ??" format.
        /// ?? - any value
        /// <returns>Index Of nearest occurance of searched aob or -1 if not found</returns>
        /// TODO FIX THIS SHIT _______________________-----------------_____________________----------------------
        static public long AOBscan(string arr, Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);


            //convert string to byte array
            string[] splitArray = arr.Split(' ');
            byte[] byteArray = new byte[splitArray.Length];
            int byteIndex = 0;
            foreach (string hex in splitArray)
            {
                byteIndex++;
                if (hex == "??") continue;
                byteArray[byteIndex-1] = Convert.ToByte(hex, 16);
            }

            //search
            int valid = 0;
            long i = stream.Position;

            while (true)
            {
                
                long streamPos = stream.Position;
                try
                {
                    for (int j = 0; reader.ReadByte() == byteArray[j] || splitArray[j] == "??"; j++)
                    {
                        valid++;
                        // found pattern
                        if (valid >= byteArray.Length)
                        {
                            stream.Position -= byteArray.Length;
                            return i;
                        }
                    }
                } catch (EndOfStreamException e ) { return -1; }

                stream.Position = streamPos + 1;
                valid = 0;
            }

        }

        static void saveExportedGraphic(Sprite.Sprite sprite, Bitmap bitmap, long i, string dir = "exported")
        {
            if (sprite == null || bitmap == null) return;
            bitmap.Save(dir + "/" + i.ToString("X") + "-"
                    + sprite.header.unknown_1.ToString("X").Substring(0, 2) + "-"
                    + sprite.header.unknown_2.ToString("X").Substring(0, 2)
                    + ".png", ImageFormat.Png);
        }
        static void saveExportedGraphic(GBitmap.GBitmap gexBitmap, Bitmap bitmap, long i, string dir = "exported")
        {
            if (gexBitmap == null || bitmap == null) return;
            bitmap.Save(dir + "/" + i.ToString("X") + "-"
                    + gexBitmap.header.unknown_1.ToString("X").Substring(0, 2) + "-"
                    + gexBitmap.header.unknown_2.ToString("X").Substring(0, 2)
                    + ".png", ImageFormat.Png);
        }

        static void Main(string[] args)
        {
            
            Console.WriteLine("OFFSET \t\tUNKNOWN 1 \tUNKNOWN 2");
            Directory.CreateDirectory("exported");
            long index;

            // initialization of graphics type objects
            Sprite.Sprite sprite;
            GBitmap.GBitmap gexBitmap;


            using (FileStream stream = new FileStream(@"GEX002.LEV", FileMode.Open, FileAccess.Read))
            {
                // -------- Sprite ---------
                // find sprite file by signature
                while ((index = AOBscan(Sprite.Header.signature, stream)) > -1){
                    sprite = new Sprite.Sprite();

                    // back 16 bytes from the found signature
                    stream.Position -= 16;
                    sprite.deserializeStream(stream);
                    // print info of sprite in console
                    Console.WriteLine(index.ToString("X")+"\t\t0x"
                        + sprite.header.unknown_1.ToString("X").Substring(0, 2)+"\t\t0x"
                        + sprite.header.unknown_2.ToString("X").Substring(0, 2) + "");
                    //save PNG file
                    Bitmap bitmap = Sprite.Drawing.CreateBitmap(sprite);
                    saveExportedGraphic(sprite, bitmap, index);

                }

                stream.Position = 0;
                // ------ GEX BITMAP --------
                // find sprite file by signature
                while ((index = AOBscan(GBitmap.Header.signature, stream)) > -1)
                {
                    gexBitmap = new GBitmap.GBitmap();

                    // back 16 bytes from the found signature
                    stream.Position -= 16;
                    gexBitmap.deserializeStream(stream);

                    //save PNG file
                    Bitmap bitmap = GBitmap.Drawing.CreateBitmap(gexBitmap);
                    saveExportedGraphic(gexBitmap, bitmap, index);
                }
            }
            
        }
    }
}
