using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatGPTBrowser
{
	public partial class ChatGPTBrouther : Form
	{
		public ChatGPTBrouther()
		{
			InitializeComponent();
			InitializeAsync();
		}

		#region メソッド
		/// <summary>
		/// WebView2コントロール呼び出し
		/// </summary>
		async void InitializeAsync()
		{
			await ChatCPTView.EnsureCoreWebView2Async();
			ChatCPTView.CoreWebView2.Navigate("https://chatgpt.com/");
		}
		#endregion

		#region イベント
		#endregion
	}
}
