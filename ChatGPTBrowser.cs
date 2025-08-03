using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Text.Json;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace ChatGPTBrowser
{
	public partial class ChatGPTBrowser : Form
	{
		#region フィールド変数
		// 最大化の要否を保持するJSONファイル名
		private string isMaximizedJsonName = "./ismaximized.json";

		// 表示サイズおよび表示位置を保持するJSONファイル名
		private string sizeAndLocationJsonName = "./sizeandlocation.json";

		// JSONでの最大化要否保持用のキー
		string isMaximizedKey = "maximized";

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

		// 表示サイズを保持
		private Size sizeKeep = new Size();

		// 表示位置を保持
		private Point locationKeep = new Point();

		// 送信失敗時の復元用のテキスト退避変数
		private string backupText = string.Empty;
		#endregion

		public ChatGPTBrowser()
		{
			#region 初期化
			InitializeComponent();

			// 表示サイズおよび表示位置を設定
			this.SetSizeAndLocation();

			// 最大化要否を設定
			this.SetMaximized();

			// ChatGPTView初期化
			InitializeAsync();
			#endregion
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

			// 共有チャット以外のチャット内リンクをクリックしたらデフォルトのブラウザを起動
			this.chatGPTView.CoreWebView2.NewWindowRequested += this.ChatGPTView_NewWindowRequested;

			// テキストボックスにフォーカスを移動
			this.textCreateSpace.Focus();
		}

		/// <summary>
		/// 表示サイズおよび表示位置を適用
		/// </summary>
		private void SetSizeAndLocation()
		{
			try
			{
				// 表示サイズおよび表示位置を記録するJSONファイルが存在した場合
				if (File.Exists(@sizeAndLocationJsonName))
				{
					// JSONの内容を取得
					string json = File.ReadAllText(@sizeAndLocationJsonName);
					var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

					// 表示サイズおよび表示位置を読み込み
					var locationDict = JsonSerializer.Deserialize<Dictionary<string, int>>(data[this.locationKey].GetRawText());
					var sizeDict = JsonSerializer.Deserialize<Dictionary<string, int>>(data[this.sizeKey].GetRawText());

					// 保存済みの表示サイズ
					Size savedSize = new Size(sizeDict[this.WidthKey], sizeDict[this.HeightKey]);

					// 保存済みの表示位置
					Point savedLocation = new Point(locationDict[this.XKey], locationDict[this.YKey]);

					// 保存済みの表示サイズのX座標が0未満の場合
					if (savedLocation.X < 0)
					{
						savedLocation.X = 0;
					}

					// 保存済みの表示サイズのX座標が0未満の場合
					if (savedLocation.Y < 0)
					{
						savedLocation.Y = 0;
					}

					// 保存済みの表示位置が画面内に収まっている場合
					if (this.IsLocationSetTrue(savedLocation))
					{
						// 表示位置を適用
						this.Location = savedLocation;

						// 保存済みの表示サイズが表示対象画面のサイズ以下の場合
						if (this.IsSetSizeTrue(savedSize))
						{
							// 保存済みの表示サイズおよび保存済みの表示位置を保持
							this.KeepSizeAndLocation(savedSize, savedLocation);
						}
						else
						{
							// デフォルト表示サイズおよびデフォルト表示位置を記録と保持
							this.RecordSizeAndLocationJson();
						}
					}
					else
					{
						// デフォルト表示サイズおよびデフォルト表示位置を記録と保持
						this.RecordSizeAndLocationJson();
					}
				}
				else
				{
					// 表示サイズおよび表示位置を記録するJSONファイルを作成
					File.Create(@sizeAndLocationJsonName).Dispose();

					// デフォルト表示サイズおよびデフォルト表示位置を記録と保持
					this.RecordSizeAndLocationJson();
				}
			}
			catch
			{
				// デフォルト表示サイズおよびデフォルト表示位置を記録と保持
				this.RecordSizeAndLocationJson();
			}
		}


		/// <summary>
		/// 起動時に読み込んだ保存済みの表示位置が画面内に収まるかどうかを判定
		/// </summary>
		/// <param name="location"></param>
		/// <returns></returns>
		private bool IsLocationSetTrue(Point location)
		{
			// 有効なディスプレイをすべて取得
			foreach (var screen in Screen.AllScreens)
			{
				// 保存済みの表示位置が画面内に収まっている場合
				if (screen.WorkingArea.Contains(location))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 起動時に読み込んだ保存済みの表示サイズが表示対象のディスプレイの画面サイズ以下かどうかを判定
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		private bool IsSetSizeTrue(Size size)
		{
			// 表示されたディスプレイを取得
			var screen = System.Windows.Forms.Screen.FromControl(this);

			// 保存済みの表示サイズが表示対象画面のサイズ以下かどうかを返す
			return size.Width <= screen.Bounds.Width && size.Height <= screen.Bounds.Height;
		}

		/// <summary>
		/// 表示サイズおよび表示位置を記録するJSONファイルへの書き込みを行う
		/// </summary>
		private void RecordSizeAndLocationJson(Size? size = null, Point ? location = null)
		{
			// デフォルト表示サイズ
			Size defaultSize = new Size(1280, 720);

			// 表示サイズおよび表示位置を記録するJSONオブジェクト
			var jsonObject = new Dictionary<string, object>();

			try
			{
				// 引数に表示位置および表示サイズが設定されていた場合
				if (size != null && location != null)
				{
					// 表示サイズおよび表示位置を記録
					jsonObject = new Dictionary<string, object>
					{
						[this.sizeKey] = new Dictionary<string, int>
						{
							[this.WidthKey] = size.Value.Width,
							[this.HeightKey] = size.Value.Height
						},
						[this.locationKey] = new Dictionary<string, int>
						{
							[this.XKey] = location.Value.X,
							[this.YKey] = location.Value.Y
						}
					};
				}
				else
				{
					// 画面中央に表示
					this.CenterToScreen();

					// デフォルト表示サイズおよびデフォルト表示位置を記録
					jsonObject = new Dictionary<string, object>
					{
						[this.sizeKey] = new Dictionary<string, int>
						{
							[this.WidthKey] = defaultSize.Width,
							[this.HeightKey] = defaultSize.Height
						},
						[this.locationKey] = new Dictionary<string, int>
						{
							[this.XKey] = this.Location.X,
							[this.YKey] = this.Location.Y
						}
					};

					// デフォルト表示サイズおよびデフォルト表示位置を保持
					this.KeepSizeAndLocation(defaultSize, this.Location);
				}
			}
			catch
			{
				// JSONファイルが存在しない場合は作成
				if (!File.Exists(@sizeAndLocationJsonName))
				{
					File.Create(@sizeAndLocationJsonName).Dispose();
				}

				// 表示サイズおよび表示位置を記録
				jsonObject = new Dictionary<string, object>
				{
					[this.sizeKey] = new Dictionary<string, int>
					{
						[this.WidthKey] = size.Value.Width,
						[this.HeightKey] = size.Value.Height
					},
					[this.locationKey] = new Dictionary<string, int>
					{
						[this.XKey] = location.Value.X,
						[this.YKey] = location.Value.Y
					}
				};

				// 表示サイズおよび表示位置を保持
				this.KeepSizeAndLocation(size.Value, location.Value);
			}
			finally
			{
				// JSON文字列に変換
				string jsonStr = JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions
				{
					WriteIndented = true
				});

				// JSONファイルに書き込み
				File.WriteAllText(this.sizeAndLocationJsonName, jsonStr);
			}
		}

		/// <summary>
		/// 起動時の表示サイズおよび表示位置の保持と適用
		/// </summary>
		/// <param name="size"></param>
		/// <param name="location"></param>
		private void KeepSizeAndLocation(Size size, Point location)
		{
			// 表示位置の保持
			this.sizeKeep = size;
			this.Size = this.sizeKeep;

			// 表示サイズの保持
			this.locationKeep = location;
			this.Location = this.locationKeep;
		}

		/// <summary>
		/// 最大化要否を設定
		/// </summary>
		private void SetMaximized()
		{
			// 最大化設定フラグ
			bool setFlg = false;

			try
			{
				// 最大化要否を記録するJSONファイルが存在した場合
				if (File.Exists(this.isMaximizedJsonName))
				{
					// JSONの内容を取得
					string json = File.ReadAllText(@isMaximizedJsonName);
					var data = JsonSerializer.Deserialize<Dictionary<string, bool>>(json);

					// 保存済みの最大化要否
					setFlg = data[this.isMaximizedKey];

					// 最大化要否を設定
					if (setFlg)
					{
						this.WindowState = FormWindowState.Maximized;
					}
					else
					{
						this.WindowState = FormWindowState.Normal;
					}
				}
				else
				{
					// JSONファイルを作成
					File.Create(this.isMaximizedJsonName).Dispose();

					// 最大化無しを記録
					this.RecordMaximized(setFlg);

					// 最大化しない
					this.WindowState = FormWindowState.Normal;
				}
			}
			catch
			{
				// 最大化無しを記録
				this.RecordMaximized(setFlg);

				// 最大化しない
				this.WindowState = FormWindowState.Normal;
			}
		}

		/// <summary>
		/// 最大化要否を記録するJSONファイルへの書き込みを行う
		/// </summary>
		private void RecordMaximized(bool flg)
		{
			// 最大化要否を記録するJSONオブジェクト
			var jsonObject = new Dictionary<string, bool>();

			try
			{
				// 最大化要否を記録
				jsonObject = new Dictionary<string, bool>
				{
					[this.isMaximizedKey] = flg
				};
			}
			catch
			{
				// 最大化要否を記録するJSONファイルが存在しない場合
				if (!File.Exists(this.isMaximizedJsonName))
				{
					// JSONファイルを作成
					File.Create(this.isMaximizedJsonName).Dispose();
				}

				// 最大化要否を記録
				jsonObject = new Dictionary<string, bool>
				{
					[this.isMaximizedKey] = flg
				};
			}
			finally
			{
				// JSON文字列に変換
				string jsonStr = JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions
				{
					WriteIndented = true
				});

				// JSONファイルに書き込み
				File.WriteAllText(this.isMaximizedJsonName, jsonStr);
			}
		}

		/// <summary>
		/// DeactiveイベントおよびClosingイベント共通処理
		/// </summary>
		private void DeactiveAndClosing()
		{
			// 表示サイズおよび表示位置を記録
			this.RecordSizeAndLocationJson(this.sizeKeep, this.locationKeep);

			// 最大化要否を記録
			this.RecordMaximized(this.WindowState == FormWindowState.Maximized);
		}

		/// <summary>
		/// フォーム移動イベントおよびサイズ変更イベント共通処理
		/// </summary>
		private void MoveAndSizeChanged()
		{
			// 最小化および最大化でない場合は保持
			if (this.WindowState == FormWindowState.Normal)
			{
				this.sizeKeep = this.Size;
				this.locationKeep = this.Location;
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

			// 入力テキスト送信用文字列変数にセットおよびバックアップ
			this.backupText = string.Empty;
			string sendText = this.textCreateSpace.Text.Trim();
			this.backupText = sendText;

			// テキストをクリップボードにコピー
			Clipboard.Clear();
			Clipboard.SetDataObject(sendText);
			await Task.Delay(waitTime);

			// ChatGPTのテキストボックスにフォーカスを合わせる(クリック動作の再現)
			this.chatGPTView.Focus();
			await Task.Delay(waitTime);

			// 画像が全画面表示されていた場合に備え、エスケープキー押下を再現
			SendKeys.SendWait("{ESC}");
			await Task.Delay(waitTime);

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

			// ChatGPTのテキストボックスへのテキストの貼り付けを可能にする(半角スペース入力→バックスペース押下)
			SendKeys.SendWait(" ");
			await Task.Delay(waitTime);
			SendKeys.SendWait("{BACKSPACE}");
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
			// DeactiveイベントおよびClosingイベント共通処理
			this.DeactiveAndClosing();
		}

		/// <summary>
		/// ChatGPTBrowser_FormClosingイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChatGPTBrowser_FormClosing(object sender, FormClosingEventArgs e)
		{
			// DeactiveイベントおよびClosingイベント共通処理
			this.DeactiveAndClosing();
		}

		/// <summary>
		/// ChatGPTBrowser_フォーム移動イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChatGPTBrowser_Move(object sender, EventArgs e)
		{
			// フォーム移動イベントおよびサイズ変更イベント共通処理
			this.MoveAndSizeChanged();
		}

		/// <summary>
		/// ChatGPTBrowserサイズ変更イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChatGPTBrowser_SizeChanged(object sender, EventArgs e)
		{
			// フォーム移動イベントおよびサイズ変更イベント共通処理
			this.MoveAndSizeChanged();
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

		/// <summary>
		/// ChatGPTView_NewWindowRequestedイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ChatGPTView_NewWindowRequested(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs args)
		{
			// クリックしたリンクのURL
			string uri = args.Uri;

			// ChatGPTの共有リンクは現在の画面で開く
			if (uri.StartsWith("https://chatgpt.com/share/"))
			{
				args.Handled = true;
				this.chatGPTView.CoreWebView2.Navigate(uri);
			}
			else
			{
				// それ以外の外部リンクはデフォルトのブラウザで開く
				args.Handled = true;
				Process.Start(new ProcessStartInfo
				{
					FileName = uri,
					UseShellExecute = true
				});
			}
		}
		#endregion
	}
}
