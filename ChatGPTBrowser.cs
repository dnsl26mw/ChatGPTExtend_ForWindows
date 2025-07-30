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
		#region フィールド変数
		// 表示位置と表示サイズを保持するJSONファイル名
		private string locationAndSizeJsonName = "./locationandsize.json";

		// JSONでの表示位置保持用のキー
		string locationKey = "Location";

		// JSONでの表示サイズ保持用のキー
		string sizeKey = "Size";

		// JSONでの表示位置X座標保持用のキー
		string XKey = "X";

		// JSONでの表示位置Y座標保持用のキー
		string YKey = "Y";

		// JSONでの表示サイズWidth保持用のキー
		string WidthKey = "Width";

		// JSONでの表示サイズWidth保持用のキー
		string HeightKey = "Height";

		// 送信失敗時の復元用のテキスト退避変数
		private string backupText = string.Empty;
		#endregion

		public ChatGPTBrowser()
		{
			InitializeComponent();

			// 表示位置と表示サイズを設定
			this.SetLocationAndSize();

			// ChatGPTView初期化
			InitializeAsync();
		}

		#region メソッド
		/// <summary>
		/// ChatGPTView初期化
		/// </summary>
		private async void InitializeAsync()
		{
			// ChatGPTを読み込み
			await this.chatGPTView.EnsureCoreWebView2Async();
			this.chatGPTView.CoreWebView2.Navigate("https://chatgpt.com/");

			// 開発者ツール無効化
			this.chatGPTView.CoreWebView2.Settings.AreDevToolsEnabled = false;

			// ステータスバー表示無効化
			this.chatGPTView.CoreWebView2.Settings.IsStatusBarEnabled = false;

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
					// JSONの内容を取得
					string json = File.ReadAllText(@locationAndSizeJsonName);
					var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

					// 表示位置および表示サイズを読み込み
					var locationDict = JsonSerializer.Deserialize<Dictionary<string, int>>(data[this.locationKey].GetRawText());
					var sizeDict = JsonSerializer.Deserialize<Dictionary<string, int>>(data[this.sizeKey].GetRawText());

					// 前回の表示位置
					Point lastTimeLocation = new Point(locationDict[this.XKey], locationDict[this.YKey]);

					// 前回の表示サイズ
					Size lastTimeSize = new Size(sizeDict[this.WidthKey], sizeDict[this.HeightKey]);

					// 前回の表示位置および表示サイズを設定
					this.SetLocationAndSize(lastTimeLocation, lastTimeSize);
				}
				else
				{
					// デフォルト表示位置およびデフォルト表示サイズを設定
					this.SetLocationAndSize(null, null);
				}
			}
			catch
			{
				// デフォルト表示位置およびデフォルト表示サイズを設定
				this.SetLocationAndSize(null, null);
			}
		}

		/// <summary>
		/// 表示位置と表示サイズを設定
		/// </summary>
		/// <param name="location"></param>
		/// <param name="size"></param>
		private void SetLocationAndSize(Point? location = null, Size? size = null)
		{
			// 表示位置設定
			if (location == null)
			{
				this.CenterToScreen();
			}
			else
			{
				this.Location = (Point)location;

				// 最大化されていた場合
				if (this.Location.Y < 0)
				{
					this.WindowState = FormWindowState.Maximized;
					return;
				}
			}

			// 表示サイズ設定
			if (size == null || size.Value.Width < 800 || size.Value.Height < 600)
			{
				this.Size = new Size(1200, 720);
			}
			else
			{
				this.Size = (Size)size;
			}
		}

		/// <summary>
		/// 表示サイズと表示位置を記録する
		/// </summary>
		private void RecordFLocationAndSize()
		{
			// 表示位置と表示サイズ記録用のJSONが見つからなかった場合は作成
			if (!File.Exists(locationAndSizeJsonName))
			{
				File.Create(@locationAndSizeJsonName).Dispose();
			}

			try
			{
				// 表示位置および表示サイズを記録
				var jsonObject = new Dictionary<string, object>
				{
					[this.locationKey] = new Dictionary<string, int>
					{
						[this.XKey] = this.Location.X,
						[this.YKey] = this.Location.Y
					},
					[this.sizeKey] = new Dictionary<string, int>
					{
						[this.WidthKey] = this.Size.Width,
						[this.HeightKey] = this.Size.Height
					}
				};

				// JSON文字列に変換
				string jsonStr = JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions
				{
					WriteIndented = true
				});

				// JSONファイルに書き込み
				File.WriteAllText(locationAndSizeJsonName, jsonStr);
			}
			catch
			{
				return;
			}
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

					// 取得データを保持
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

					// 退避データを復元
					Clipboard.Clear();
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

			// 送信失敗時の復元用に入力したテキストを退避
			this.textCreateSpace.Focus();
			await Task.Delay(waitTime);
			this.backupText = string.Empty;
			string sendText = this.textCreateSpace.Text.Trim();
			this.backupText = sendText;

			// テキストをクリップボードにコピー
			Clipboard.Clear();
			Clipboard.SetDataObject(sendText);
			await Task.Delay(waitTime);

			// ChatGPTのテキストボックスにフォーカスを移動
			this.chatGPTView.Focus();
			await Task.Delay(waitTime);
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

			// ChatGPTのテキストボックスにテキストを貼り付け
			SendKeys.SendWait("^{v}");
			await Task.Delay(waitTime);

			// 送信
			SendKeys.SendWait("{ENTER}");
			await Task.Delay(waitTime);

			// クリップボードの内容を復元
			this.RestoreClipboard(backupData);

			// テキストボックスにフォーカスを移動
			this.textCreateSpace.Focus();

			// テキストをクリア
			this.textCreateSpace.Clear();
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
			if (e.Alt && e.KeyCode == Keys.B)
			{
				if (!string.IsNullOrEmpty(this.backupText))
				{
					this.textCreateSpace.Text = this.backupText;
					this.textCreateSpace.Select(this.textCreateSpace.Text.Length, 0);
				}
			}
		}

		/// <summary>
		/// ChatGPTBrowser_Deactiveイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChatGPTBrowser_Deactive(object sender, EventArgs e)
		{
			// 表示位置および表示サイズを記録
			if (this.WindowState != FormWindowState.Minimized)
			{
				this.RecordFLocationAndSize();
			}
		}

		/// <summary>
		/// ChatGPTBrowser_FormClosingイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChatGPTBrowser_FormClosing(object sender, FormClosingEventArgs e)
		{
			// 表示位置および表示サイズを記録
			if (this.WindowState != FormWindowState.Minimized)
			{
				this.RecordFLocationAndSize();
			}
		}

		/// <summary>
		/// ChatGPTBrowserサイズ変更イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChatGPTBrowser_SizeChanged(object sender, EventArgs e)
		{
			// 最大化解除でタイトルバーが隠れないようにするための処理
			if (this.WindowState != FormWindowState.Maximized)
			{
				this.CenterToScreen();
			}
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
