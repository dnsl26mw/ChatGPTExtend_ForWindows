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
			await chatGPTView.EnsureCoreWebView2Async();
			chatGPTView.CoreWebView2.Navigate("https://chatgpt.com/");
			this.textCreateSpace.Focus();
		}

		/// <summary>
		/// 非同期処理でAwaitを行う
		/// </summary>
		private async void Await(int value)
		{
			 await Task.Delay(value);
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
			this.chatGPTView.Focus();

			// ChatGPTのテキストボックスにフォーカスを移動
			await this.chatGPTView.ExecuteScriptAsync(@"
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
			this.Await(100);

			// テキストをChatGPTにクリップボード経由で入力
			Clipboard.SetText(this.textCreateSpace.Text);
			SendKeys.SendWait("^{v}");
			this.Await(100);

			// 送信
			SendKeys.SendWait("{ENTER}");
			this.Await(100);

			// テキストをクリア
			this.textCreateSpace.Clear();

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
				this.sendButton.Enabled = false;
			}
		}

		/// <summary>
		/// テキスト作成エリアKeyDownイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextCreateSpace_KeyDown(object sender, KeyEventArgs e)
		{
			// 送信ボタンクリックイベントの呼び出し
			if (e.Control && e.KeyCode == Keys.Enter)
			{
				SendButton_Click(sender, e);
			}
		}
		#endregion
	}
}
