using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Nelian
{
    public class ModernFlowLayoutPanel : FlowLayoutPanel
    {
        [DllImport("user32.dll")]
        private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        private const int SB_BOTH = 3;

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            ShowScrollBar(Handle, SB_BOTH, false);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            ShowScrollBar(Handle, SB_BOTH, false);
        }
    }
}
