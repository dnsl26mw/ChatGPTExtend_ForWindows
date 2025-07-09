using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

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
		private async void InitializeAsync()
		{
			await ChatGPTView.EnsureCoreWebView2Async();
			ChatGPTView.CoreWebView2.Navigate("https://chatgpt.com/");
			this.textCreateSpace.Focus();
		}
		#endregion

		#region イベント
		/// <summary>
		/// 送信ボタンクリックイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void SendButton_Click(object sender, EventArgs e)
		{
			// ChatGPTViewにフォーカスを移動
			this.ChatGPTView.Focus();

			// ChatGPTのテキストボックスにフォーカスを移動
			await this.ChatGPTView.ExecuteScriptAsync(@"
				let el = document.getElementById('prompt-textarea');
				if (el) {
					el.focus();
					let range = document.createRange();
					range.selectNodeContents(el);
					let sel = window.getSelection();
					sel.removeAllRanges();
					sel.addRange(range);
				}
			");

			// DOM反映を待つ
			await Task.Delay(100);

			// テキストをクリップボード経由で入力
			Clipboard.SetText(this.textCreateSpace.Text);
			SendKeys.SendWait("^{v}");

			// テキストをクリア
			this.textCreateSpace.Clear();

			// DOM反映を待つ
			await Task.Delay(100);

			// フォーカスをテキスト作成エリアに戻す
			this.textCreateSpace.Focus();
		}

		/// <summary>
		/// 入力テキスト変更イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextCreateSpace_TextChanged(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(this.textCreateSpace.Text.Trim()))
			{
				this.sendButton.Enabled = true;
			}
			else
			{
				this.sendButton.Enabled= false;
			}
		}
		#endregion
	}
}
