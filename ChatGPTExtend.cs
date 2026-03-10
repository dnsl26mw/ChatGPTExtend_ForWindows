using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ChatGPTExtend
{
	public partial class ChatGPTExtend : Form
	{
		#region フィールド変数

		// 表示サイズおよび表示位置を保持するJSONファイル名
		private string sizeAndLocationJsonName = "./sizeandlocation.json";

		// 最大化の要否を保持するJSONファイル名
		private string isMaximizedJsonName = "./ismaximized.json";

		// Enter押下による改行有効化の要否を保持するJSONファイル名
		private string isEnterLineBreakJsonName = "./isenterlinebreak.json";

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

		// JSONでのEnter押下による改行有効化の要否保持用のキー
		string isEnterLineBreakKey = "enterLineBreak";

		// 表示サイズを保持
		private Size sizeKeep = new Size();

		// 表示位置を保持
		private Point locationKeep = new Point();

		// Enter押下による改行有効化の要否を保持
		private bool isEnterLineBreakKeep = false;

		// ユーザデータ
		private CoreWebView2Environment chatGPTViewEnvironment;

		[DllImport("dwmapi.dll")]
		// タイトルバーの色をダークモード/ライトモードに合わせて変更するためのWin32API
		private static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

		// ダークモード/ライトモードに合わせたタイトルバーの色変更のための定数
		private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ChatGPTExtend()
		{
			InitializeComponent();

			// Enter押下による改行有効化の要否を設定
			this.SetEnterLineBreak();

			// 表示サイズおよび表示位置を設定
			this.SetSizeAndLocation();

			// 最大化要否を設定
			this.SetMaximized();

			// ChatGPTView初期化
			this.ChatGPTViewInitialize();

			// 起動時にダークモード/ライトモードに合わせて色を変更
			this.SetColorMode();

			// 起動中にダークモード/ライトモードの変更を検知して色を変更
			Microsoft.Win32.SystemEvents.UserPreferenceChanged += (s, e) =>
			{
				this.SetColorMode();
			};
		}

		#region メソッド

		/// <summary>
		/// ChatGPTView初期化
		/// </summary>
		private async void ChatGPTViewInitialize()
		{
			// ユーザデータ保持
			var options = new CoreWebView2EnvironmentOptions();
			options.AdditionalBrowserArguments =
				"--disable-features=msSmartScreenProtection " +
				"--enable-gpu-rasterization " +
				"--disable-gpu-shader-disk-cache " +
				"--disable-background-timer-throttling " +
				"--process-per-site" +
				"--enable - features = OverlayScrollbar" +
				"--disable - features = CalculateNativeWinOcclusion" +
				"--enable-zero-copy " +
				"--enable-gpu-memory-buffer-video-frames";
			this.chatGPTViewEnvironment = await CoreWebView2Environment.CreateAsync(null, "UserData", options);

			// chatGPTViewの初期化
			await this.chatGPTView.EnsureCoreWebView2Async(chatGPTViewEnvironment);

			// 初期化および再生成時の共通処理
			await this.InitAndRegenerateCommonProc(this.chatGPTView);

			// ChatGPTに移動
			this.chatGPTView.CoreWebView2.Navigate("https://chatgpt.com/");
		}

		/// <summary>
		/// ChatGPTView再生成
		/// </summary>
		/// <returns></returns>
		private async Task ReGenerateChatGPTView()
		{
			// ユーザデータがNULLの場合
			if (this.chatGPTViewEnvironment == null)
			{
				return;
			}

			// chatGPTViewがNULLの場合
			if (this.chatGPTView == null)
			{
				return;
			}

			// chatGPTView.CoreWebView2がNULLの場合
			if (this.chatGPTView.CoreWebView2 == null)
			{
				return;
			}

			// 現在のchatGPTViewを取得
			var oldView = this.chatGPTView;

			// 現在のURLを取得
			string url = oldView.CoreWebView2.Source;

			// 新しいchatGPTViewを生成
			var newView = new WebView2();
			newView.Dock = DockStyle.Fill;
			newView.Visible = false;
			this.Controls.Add(newView);

			// 新しいchatGPTViewのchatGPTView.CoreWebView2の初期化
			await newView.EnsureCoreWebView2Async(this.chatGPTViewEnvironment);

			// 初期化および再生成時の共通処理
			await this.InitAndRegenerateCommonProc(newView);

			// 新しいchatGPTViewで現在のURLを読み込み
			newView.CoreWebView2.Navigate(url);

			// ホワイトアウト防止のため、読み込み完了まで待機
			var tcs = new TaskCompletionSource<bool>();
			newView.CoreWebView2.NavigationCompleted += (s, e) =>
			{
				tcs.TrySetResult(true);
			};
			await tcs.Task;

			// 新しいchatGPTViewに差し替え
			newView.Visible = true;
			this.Controls.Remove(oldView);
			oldView.Dispose();
			this.chatGPTView = newView;
		}

		/// <summary>
		/// chatGPTViewの初期化および再生成時の共通処理
		/// </summary>
		/// <param name="webView2"></param>
		/// <param name="url"></param>
		private async Task InitAndRegenerateCommonProc(WebView2 webView2)
		{
			// WebMessageReceivedイベントの追加
			webView2.CoreWebView2.WebMessageReceived -= ChatGPTView_WebMessageReceived;
			webView2.CoreWebView2.WebMessageReceived += ChatGPTView_WebMessageReceived;

			// Enter押下による改行を行う場合
			if (this.isEnterLineBreakKeep)
			{
				// Enter押下による改行有効化のためのJavaScriptコードをchatGPTViewに追加
				await webView2.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(@"

					// キー押下イベントのリスナーを追加
					window.addEventListener('keydown', function(e) {
					
						// Enter押下の場合
						if (e.key === 'Enter' && !e.altKey && !e.shiftKey && !e.ctrlKey) {

							// デフォルトのEnter押下動作を無効化
							e.preventDefault();

							// キー押下メッセージを送信
							e.stopPropagation();

							// C#側にEnter押下を通知
							chrome.webview.postMessage('enter');
						}
					}, true);
				");
			}

			// DOM削除のためのJavaScriptコードをchatGPTViewに追加
			await webView2.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(@"

				// DOMContentLoadedイベントのリスナーを追加
				window.addEventListener('DOMContentLoaded', () => {

					// DOMの変化を監視するMutationObserverを作成
					const observer = new MutationObserver(() => {

						// DOM内容を取得
						const messages = document.querySelectorAll('[data-message-author-role]');

						// メッセージが120件を超えている場合は古いメッセージから削除
						if (messages.length > 120) {
							for (let i = 0; i < messages.length - 120; i++) {
								messages[i].remove();
							}
						}
					});

					// DOMの変化を監視
					observer.observe(document.body, {

						// 子ノードの追加と削除を監視
						childList: true,

						// サブツリー全体の変化を監視
						subtree: true	
					});	
					
				});
			");

			// 開発者ツール無効化
			webView2.CoreWebView2.Settings.AreDevToolsEnabled = false;

			// ステータスバー表示無効化
			webView2.CoreWebView2.Settings.IsStatusBarEnabled = false;

			// デフォルトの右クリックメニュー無効化
			webView2.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;

			// ズームコントロール無効化
			webView2.CoreWebView2.Settings.IsZoomControlEnabled = false;

			// 組み込みエラーページ無効化
			webView2.CoreWebView2.Settings.IsBuiltInErrorPageEnabled = false;

			// 共有チャット以外のチャット内リンクをクリックしたらデフォルトのブラウザを起動
			webView2.CoreWebView2.NewWindowRequested -= this.ChatGPTView_NewWindowRequested;
			webView2.CoreWebView2.NewWindowRequested += this.ChatGPTView_NewWindowRequested;

			// チャットルーム移動時にChatGPTView再生成
			webView2.CoreWebView2.NavigationStarting -= this.ChatGPTView_NavigationStarting;
			webView2.CoreWebView2.NavigationStarting += this.ChatGPTView_NavigationStarting;
		}

		/// <summary>
		/// 表示サイズおよび表示位置を設定
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
		/// <param name="size"></param>
		/// <param name="location"></param>
		/// </summary>
		private void RecordSizeAndLocationJson(Size? size = null, Point? location = null)
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
				// 表示サイズおよび表示位置を記録するJSONファイルが存在しない場合は作成
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

				// 表示サイズおよび表示位置を記録するJSONファイルへの書き込み
				File.WriteAllText(@sizeAndLocationJsonName, jsonStr);
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
				if (File.Exists(@isMaximizedJsonName))
				{
					// 最大化要否を記録するJSONファイルの内容を取得
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
					// 最大化要否を記録するJSONファイルを作成
					File.Create(@isMaximizedJsonName).Dispose();

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
				if (!File.Exists(@isMaximizedJsonName))
				{
					// 最大化要否を記録するJSONファイルJSONファイルを作成
					File.Create(@isMaximizedJsonName).Dispose();
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

				// 最大化要否を記録するJSONファイルJSONファイルへの書き込み
				File.WriteAllText(@isMaximizedJsonName, jsonStr);
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
		/// Enter押下による改行有効化の要否の設定
		/// </summary>
		private void SetEnterLineBreak()
		{
			// Enter押下による改行有効化設定フラグ
			bool setFlg = false;

			try
			{
				// Enter押下による改行有効化設定を記録するJSONファイルが存在しない場合
				if (!File.Exists(@isEnterLineBreakJsonName))
				{
					// Enter押下による改行有効化設定を記録するJSONファイルを作成
					File.Create(@isEnterLineBreakJsonName).Dispose();

					// Enter押下による改行有効化設定を記録するJSONファイルへの書き込みを行う
					this.RecordEnterLineBreak(setFlg);
				}

				// Enter押下による改行有効化設定を記録するJSONの内容を取得
				string json = File.ReadAllText(@isEnterLineBreakJsonName);
				var data = JsonSerializer.Deserialize<Dictionary<string, bool>>(json);

				// 保存済みのEnter押下による改行有効化設定
				setFlg = data[this.isEnterLineBreakKey];
			}
			catch
			{
				// Enter押下による改行有効化設定を記録するJSONファイルを作成
				File.Create(@isEnterLineBreakJsonName).Dispose();

				// Enter押下による改行有効化設定を記録するJSONファイルへの書き込みを行う
				this.RecordEnterLineBreak(setFlg);
			}
			finally
			{
				// Enter押下による改行要否を設定
				this.isEnterLineBreakKeep = setFlg;
			}
		}

		/// <summary>
		/// Enter押下による改行要否を記録するJSONファイルへの書き込みを行う
		/// </summary>
		private void RecordEnterLineBreak(bool flg)
		{
			// Enter押下による改行要否を記録するJSONオブジェクト
			var jsonObject = new Dictionary<string, bool>();

			try
			{
				// Enter押下による改行要否を記録
				jsonObject = new Dictionary<string, bool>
				{
					[this.isEnterLineBreakKey] = flg
				};
			}
			catch
			{
				// Enter押下による改行要否を記録するJSONファイルが存在しない場合
				if (!File.Exists(@isEnterLineBreakJsonName))
				{
					// JSONファイルを作成
					File.Create(@isEnterLineBreakJsonName).Dispose();
				}

				// Enter押下による改行要否を記録
				jsonObject = new Dictionary<string, bool>
				{
					[this.isEnterLineBreakKey] = flg
				};
			}
			finally
			{
				// JSON文字列に変換
				string jsonStr = JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions
				{
					WriteIndented = true
				});

				// Enter押下による改行要否を記録するJSONファイルへの書き込み
				File.WriteAllText(@isEnterLineBreakJsonName, jsonStr);
			}
		}

		/// <summary>
		/// ダークモード/ライトモードに合わせて色を設定
		/// </summary>
		private void SetColorMode()
		{
			// ダークモード/ライトモードを取得
			int useDarkMode = IsDarkMode() ? 1 : 0;

			// ダークモードの場合
			if (useDarkMode == 1)
			{
				// フォームの背景色をダークモードに合わせる
				this.BackColor = Color.FromArgb(32, 32, 32);

				// chatGPTViewの背景色をダークモードに合わせる
				this.chatGPTView.DefaultBackgroundColor = Color.FromArgb(32, 32, 32);
			}
			// ダークモードではない場合
			else
			{
				// フォームの背景色をライトモードに合わせる
				this.BackColor = Color.FromArgb(255, 255, 255);

				// chatGPTViewの背景色をライトモードに合わせる
				this.chatGPTView.DefaultBackgroundColor = Color.FromArgb(255, 255, 255);
			}

			// タイトルバーの色をダークモード/ライトモードに合わせる
			DwmSetWindowAttribute(this.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, sizeof(int));
		}

		/// <summary>
		///  ダークモード/ライトモードを判定
		/// </summary>
		/// <returns></returns>
		private bool IsDarkMode()
		{
			// レジストリからダークモード/ライトモードの設定を取得
			using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
				@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
			{
				// レジストリキーが存在しない場合はライトモードとする
				if (key == null)
				{
					return false;
				}

				// AppsUseLightThemeの値を取得
				object appUseLightTheme = key?.GetValue("AppsUseLightTheme");

				// SystemUseLightThemeの値を取得
				object systemUseLightTheme = key?.GetValue("SystemUseLightTheme");

				// AppsUseLightThemeの値が0の場合はダークモード、1の場合はライトモード
				if (appUseLightTheme is int appUseLightThemeValue)
				{
					return appUseLightThemeValue == 0;
				}

				// SystemUseLightThemeの値が0の場合はダークモード、1の場合はライトモード
				if (appUseLightTheme is int systemUseLightThemeValue)
				{
					return systemUseLightThemeValue == 0;
				}

				// どちらの値も取得できない場合はライトモードとする
				return false;
			}
		}

		#endregion

		#region イベント

		/// <summary>
		/// ChatGPTView_WebMessageReceivedイベント
		/// </summary>
		/// <returns></returns>
		private void ChatGPTView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
		{
			// キー押下メッセージを取得
			string msg = string.Empty;
			msg = e.TryGetWebMessageAsString();

			// Enter押下の場合
			if (string.Equals(msg, "enter"))
			{
				// Shift + Enterを送信
				SendKeys.SendWait("+{ENTER}");
			}
		}

		/// <summary>
		/// ChatGPTExtend_Deactiveイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChatGPTExtend_Deactive(object sender, EventArgs e)
		{
			// DeactiveイベントおよびClosingイベント共通処理
			this.DeactiveAndClosing();
		}

		/// <summary>
		/// ChatGPTExtend_FormClosingイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChatGPTExtend_FormClosing(object sender, FormClosingEventArgs e)
		{
			// DeactiveイベントおよびClosingイベント共通処理
			this.DeactiveAndClosing();
		}

		/// <summary>
		/// ChatGPTExtend_フォーム移動イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChatGPTExtend_Move(object sender, EventArgs e)
		{
			// フォーム移動イベントおよびサイズ変更イベント共通処理
			this.MoveAndSizeChanged();
		}

		/// <summary>
		/// ChatGPTExtendサイズ変更イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChatGPTExtend_SizeChanged(object sender, EventArgs e)
		{
			// フォーム移動イベントおよびサイズ変更イベント共通処理
			this.MoveAndSizeChanged();
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

		/// <summary>
		/// ChatGPTView_NavigationStartingイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void ChatGPTView_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
		{
			// チャットルーム移動時
			if (e.Uri.Contains("/c/"))
			{
				// ChatGPTView再生成
				await this.ReGenerateChatGPTView();
			}
		}

		#endregion
	}
}
