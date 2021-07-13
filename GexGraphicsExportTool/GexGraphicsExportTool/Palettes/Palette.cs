using System.Drawing;

namespace GexGraphicsExportTool.Palettes
{
    abstract class Palette
    {
        protected string[] colors;
        public Color getColor(byte index)
        {
            return ColorTranslator.FromHtml("#" + colors[index]);

        }
    }
}
