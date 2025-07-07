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
			this.TextCreateSpace.Focus();
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
			// 入力内容をJavaScript文字列に変換
			string inputText = JsonSerializer.Serialize(this.TextCreateSpace.Text);

			string script = $@"
				const el = document.getElementById('prompt-textarea');
				if (el) {{
					el.focus();

					// テキスト内容の書き換え
					el.innerText = {inputText};

					// Reactに変更を通知
					el.dispatchEvent(new InputEvent('input', {{
						bubbles: true,
						cancelable: true,
						data: {inputText},
						inputType: 'insertText'
					}}));

					// 念のため compositionend も送ってみる
					el.dispatchEvent(new CompositionEvent('compositionend', {{
						data: {inputText},
						bubbles: true,
						cancelable: true
					}}));
				}}
			";

			await this.ChatGPTView.ExecuteScriptAsync(script);
			await this.ChatGPTView.ExecuteScriptAsync("document.forms['composer-submit-button'].submit");
		}

		/// <summary>
		/// 入力テキスト変更イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextCreateSpace_TextChanged(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(this.TextCreateSpace.Text))
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
