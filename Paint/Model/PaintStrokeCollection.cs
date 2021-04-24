using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;

namespace Paint
{
    class PaintStrokeCollection
    {
        private StrokeCollection _strokes;
        public StrokeCollection strokes
        {
            get => _strokes;
            set => _strokes = value;
        }

        private string _tag;
        public string tag
        {
            get => _tag;
            set => _tag = value;
        }
    }
}
