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
using System.Windows.Forms.VisualStyles;
using Microsoft.Web.WebView2.Core;

namespace ChatGPTBrowser
{
	public partial class ChatGPTBrouther : Form
	{
		// 送信失敗時の復元用のテキスト退避変数
		private string backupText = string.Empty;

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
			this.chatGPTView.CoreWebView2.Navigate("https://chatgpt.com/");

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

			thread = new Thread((MethodInvoker) =>
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
			// DOM変更待ち時間
			int waitTime = 100;

			// クリップボードの内容を退避
			IDataObject backupData = this.BackUpClipBoard();
			await Task.Delay(waitTime);

			// ChatGPTViewにフォーカスを移動
			this.chatGPTView.Focus();

			// ChatGPTのテキストボックスにフォーカスを移動
			await this.chatGPTView.ExecuteScriptAsync(@"
				(function() {
					const promptTextArea = document.querySelector('textarea[data-testid=""prompt-textarea""]');
					if (promptTextArea) {
						const mousedown = new MouseEvent('mousedown', { bubbles: true });
						const mouseup = new MouseEvent('mouseup', { bubbles: true });
						const click = new MouseEvent('click', { bubbles: true });

						promptTextArea.dispatchEvent(mousedown);
						promptTextArea.dispatchEvent(mouseup);
						promptTextArea.dispatchEvent(click);

						promptTextArea.focus();
						promptTextArea.select();
					}
				})();
			");
			await Task.Delay(waitTime);

			// 送信失敗時に復元できるよう入力したテキストを退避
			this.backupText = string.Empty;
			string sendText = this.textCreateSpace.Text.Trim();
			sendText = sendText.TrimStart();
			sendText = sendText.Trim().TrimStart('　');
			this.backupText = sendText;

			// テキストをChatGPTにクリップボード経由で入力
			Clipboard.Clear();
			Clipboard.SetText(sendText);
			SendKeys.SendWait(" ^{v}");
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
			// Control押下
			if (e.Control)
			{
				this.textCreateSpace.Select(this.textCreateSpace.Text.Length, 0);
			}

			// 送信ボタンクリックイベントの呼び出し
			if (e.Control && e.KeyCode == Keys.Enter)
			{
				SendButton_Click(sender, e);
			}

			// 前回送信のテキストを復元
			if (e.Control && e.Alt && e.KeyCode == Keys.B)
			{
				if (!string.IsNullOrEmpty(this.backupText))
				{
					this.textCreateSpace.Text = this.backupText;
					this.textCreateSpace.Select(this.textCreateSpace.Text.Length, 0);
				}
			}
		}
		#endregion
	}
}
