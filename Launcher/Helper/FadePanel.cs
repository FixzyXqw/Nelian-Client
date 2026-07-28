using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nelian
{
    public class FadePanel : Panel
    {
        public int Alpha { get; set; }

        public FadePanel()
        {
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using var brush = new SolidBrush(
                Color.FromArgb(Alpha, Color.Black));

            e.Graphics.FillRectangle(
                brush,
                ClientRectangle);
        }
    }
}
