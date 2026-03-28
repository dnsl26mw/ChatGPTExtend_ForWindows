using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace ChatGPTextend
{
	class ContextMenuRenderer : ToolStripProfessionalRenderer
	{
		public ContextMenuRenderer() : base(new ColorTable()) { }

		/// <summary>
		/// 右クリックメニューの枠線
		/// </summary>
		protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
		{
			using (var pen = new Pen(Color.FromArgb(230, 230, 230), 1.5f))
			{
				var rect = new Rectangle(0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1);
				e.Graphics.DrawRectangle(pen, rect);
			}
		}
	}

	class ColorTable : ProfessionalColorTable
	{
		// 右クリックメニュー背景色
		public override Color ToolStripDropDownBackground => Color.FromArgb(250, 250, 250);

		// アイコン表示領域の背景色(グラデーション開始)
		public override Color ImageMarginGradientBegin => Color.FromArgb(250, 250, 250);

		// アイコン表示領域中央の背景色
		public override Color ImageMarginGradientMiddle => Color.FromArgb(250, 250, 250);

		// アイコン表示領域の背景色(グラデーション終了)
		public override Color ImageMarginGradientEnd => Color.FromArgb(250, 250, 250);

		// 右クリックメニューマウスホバー色
		public override Color MenuItemSelected => Color.FromArgb(243, 243, 243);

		// マウスホバー時の右クリックメニュー枠線色
		public override Color MenuItemBorder => Color.Transparent;
	}

}
