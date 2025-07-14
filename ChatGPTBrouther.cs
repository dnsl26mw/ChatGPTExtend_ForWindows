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
using System.Threading;
using System.Runtime.InteropServices;

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
		/// WebView2コントロール初期化
		/// </summary>
		private async void InitializeAsync()
		{
			// ChatGPTを読み込み
			await this.chatGPTView.EnsureCoreWebView2Async();
			chatGPTView.CoreWebView2.Navigate("https://chatgpt.com/");

			// テキストボックスにフォーカスを移動
			this.textCreateSpace.Focus();
		}

		/// <summary>
		/// クリップボードのデータを退避
		/// </summary>
		private IDataObject BackUpClipBoard()
		{
			IDataObject backupData = null;
			Thread thread;

			thread = new Thread(() =>
			{
				try
				{
					// クリップボードのデータを取得
					var original = Clipboard.GetDataObject();

					if (original == null)
					{
						return;
					}

					var copy = new DataObject();

					foreach (var format in original.GetFormats())
					{
						copy.SetData(format, original.GetData(format));
					}

					backupData = copy;
				}
				catch
				{
					return;
				}
			});

			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			thread.Join();

			return backupData;
		}

		/// <summary>
		/// クリップボードのデータを復元
		/// </summary>
		/// <param name="backupData">退避データ</param>
		private void RestoreClipboard(IDataObject backupData)
		{
			Thread thread = new Thread(() =>
			{
				// 退避データが存在しない場合
				if (backupData == null)
				{
					return;
				}

				try
				{
					// 退避データのフォーマットを取得
					var formats = backupData.GetFormats();

					// 有効なフォーマットが存在しない場合
					if (formats == null || formats.Length == 0)
					{
						return;
					}

					// クリップボードの内容を復元
					Clipboard.SetDataObject(backupData, true);
				}
				catch
				{
					return;
				}
			});

			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			thread.Join();
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
			int waitTime = 150;

			// クリップボードの内容を退避
			IDataObject backupData = this.BackUpClipBoard();
			await Task.Delay(300);

			// ChatGPTViewにフォーカスを移動
			this.chatGPTView.Focus();

			// ChatGPTのテキストボックスにフォーカスを移動
			await this.chatGPTView.ExecuteScriptAsync(@"
				(async () => {
					function waitForPromptTextAreaEnabled() {
						return new Promise((resolve) => {
						const interval = setInterval(() => {
							const promptTextArea = document.querySelector('textarea[data-testid=""prompt-textarea""]');
							if (promptTextArea && !promptTextArea.disabled) {
								clearInterval(interval);
								resolve(promptTextArea);
							}
						}, 100);
					});
				}

				const promptTextArea = await waitForPromptTextAreaEnabled();
				promptTextArea.focus();
				promptTextArea.select();
				promptTextArea.setSelectionRange(promptTextArea.value.length, promptTextArea.value.length);
			})();
			");
			await Task.Delay(waitTime);

			// テキストをChatGPTにクリップボード経由で入力
			Clipboard.SetText(this.textCreateSpace.Text);
			SendKeys.SendWait("^{v}");
			await Task.Delay(waitTime);

			// 送信
			SendKeys.SendWait("{ENTER}");
			await Task.Delay(waitTime);

			// クリップボードの内容を復元
			this.RestoreClipboard(backupData);

			// テキストをクリア
			this.textCreateSpace.Clear();

			// テキストボックスにフォーカスを移動
			this.textCreateSpace.Focus();
		}

		/// <summary>
		/// テキストボックスのテキスト変更イベント
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
		/// テキストボックスKeyDownイベント
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
