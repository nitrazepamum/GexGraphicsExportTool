using System;
using System.IO;

namespace GexGraphicsExportTool
{
    class Program
    {
        static void Main(string[] args)
        {
            using (FileStream stream = new FileStream(@"sprite.test", FileMode.Open, FileAccess.Read))
            {
                Sprite.Sprite sprite = new Sprite.Sprite();
                sprite.deserializeStream(stream);
            }
                
        }
    }
}
