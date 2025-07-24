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
using System.IO;
using System.Text.Json.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ChatGPTBrowser
{
	public partial class ChatGPTBrowser : Form
	{
		// 表示位置と表示サイズを保持するJSONファイル名
		private string locationAndSizeJsonName = "./locationandsize.json";

		// 表示位置
		private Point location = new Point();

		// 表示サイズ
		private Size size = new Size();

		// JSONでの表示位置保持用のキー
		string locationKey = "location";

		// JSONでの表示サイズ保持用のキー
		string sizeKey = "size";

		// 送信失敗時の復元用のテキスト退避変数
		private string backupText = string.Empty;

		public ChatGPTBrowser()
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
			// 表示位置と表示サイズを設定
			this.SetLocationAndSize();

			// ChatGPTを読み込み
			await this.chatGPTView.EnsureCoreWebView2Async();
			this.chatGPTView.CoreWebView2.Navigate("https://chatgpt.com/");

			// テキストボックスにフォーカスを移動
			this.textCreateSpace.Focus();
		}

		/// <summary>
		/// 表示位置と表示サイズを設定
		/// </summary>
		private void SetLocationAndSize()
		{
			try
			{

				// 前回の表示位置と表示サイズを復元
				if (File.Exists(@locationAndSizeJsonName))
				{
					// JSON読み込み
					string jsonStr = File.ReadAllText(locationAndSizeJsonName);
					var json = JsonSerializer.Deserialize<string>(jsonStr);

					// 前回の表示位置
					Point lastTimeLocation = new Point();

					// 前回の表示サイズ
					Size lastTimeSize = new Size();

					// 表示サイズの設定
					if (lastTimeSize.Width > this.Width)
					{
						this.Width = lastTimeSize.Width;
					}
					if (lastTimeSize.Height > this.Height)
					{
						this.Height = lastTimeSize.Height;
					}

					// 表示位置を設定
					this.Location = lastTimeLocation;
				}
				else
				{
					// 表示位置と表示サイズ保持用のJSONファイル作成
					File.Create(@locationAndSizeJsonName);
				}
			}
			catch
			{
				// 表示位置と表示サイズ保持用のJSONファイル作成
			}
		}

		/// <summary>
		/// JSONファイルの作成と記録を行う
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		private void RecordForJson(string key, string value)
		{
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

			// 送信失敗時に復元できるよう入力したテキストを退避
			this.textCreateSpace.Focus();
			await Task.Delay(waitTime);
			this.backupText = string.Empty;
			string sendText = this.textCreateSpace.Text.Trim();
			this.backupText = sendText;

			// テキストをChatGPTにクリップボード経由で入力
			Clipboard.Clear();
			Clipboard.SetDataObject(sendText);
			await Task.Delay(waitTime);

			// ChatGPTViewにフォーカスを移動
			this.chatGPTView.Focus();
			await Task.Delay(waitTime);

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
				this.textCreateSpace.Select(this.textCreateSpace.Text.Length, 0);
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

		/// <summary>
		/// ChatGPTBrowser位置変更イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChatGPTBrowser_LocationChanged(object sender, EventArgs e)
		{
			// 現在の表示位置を記録
			this.location = this.Location;
		}

		/// <summary>
		/// ChatGPTBrowserサイズ変更イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChatGPTBrowser_SizeChanged(object sender, EventArgs e)
		{
			// 現在の表示サイズを記録
			this.size = this.Size;
		}

		/// <summary>
		/// ChatGPTBrowser_Deactiveイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChatGPTBrowser_Deactive(object sender, EventArgs e)
		{
			//this.RecordForJson();
		}

		/// <summary>
		/// ChatGPTBrowser_FormClosingイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChatGPTBrowser_FormClosing(object sender, FormClosingEventArgs e)
		{
			//this.RecordForJson();
		}

		/// <summary>
		/// テキストボックスDragEnterイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextCreateSpace_DragEnter(object sender, DragEventArgs e)
		{
			// テキストボックスにドラッグ操作を許可
			e.Effect = DragDropEffects.Copy;
		}

		/// <summary>
		/// テキストボックスドラッグ&ドロップイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextCreateSpace_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.Text))
			{
				// 文字列がドラッグされた場合
				this.textCreateSpace.Text += (string)e.Data.GetData(DataFormats.Text);
				this.textCreateSpace.Select(this.textCreateSpace.Text.Length, 0);
			}
			else
			{
				// 文字列以外のファイルがドラッグされた場合
				MessageBox.Show("ファイルの添付はChatGPTの画面で直接行ってください。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}
		#endregion
	}
}
