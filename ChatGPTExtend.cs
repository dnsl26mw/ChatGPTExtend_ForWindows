using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows.Forms;

namespace ChatGPTExtend
{
	public partial class ChatGPTExtend : Form
	{
		#region 設定JSON

		// 設定保持JSONファイル名
		private string settingsJsonName = "./Settings.json";

		// 設定保持JSONファイルのEnter押下による改行有効化の要否保持用のキー
		string enterLineBreakKey = "EnterLineBreak";

		// 設定保持JSONファイルの前回のチャットルームを再開の要否保持用のキー
		string isChatRoomLeftOffKey = "ChatRoomLeftOff";

		// 設定保持JSONファイルの前回のチャットルームURL保持用のキー
		string lastTimeChatRoomUrlKey = "LastTimeChatRoomUrl";

		// 設定保持JSONファイルの表示位置保持用のキー
		string locationKey = "Location";

		// 設定保持JSONファイルの表示位置X座標保持用のキー
		string XKey = "X";

		// 設定保持JSONファイルの表示位置Y座標保持用のキー
		string YKey = "Y";

		// 設定保持JSONファイルの表示サイズ保持用のキー
		string sizeKey = "Size";

		// 設定保持JSONファイルの表示サイズWidth保持用のキー
		string WidthKey = "Width";

		// 設定保持JSONファイルの表示サイズWidth保持用のキー
		string HeightKey = "Height";

		// 設定保持JSONファイルの最大化要否保持用のキー
		string isMaximizedKey = "Maximized";

		#endregion

		#region フィールド変数

		// Enter押下による改行の要否を保持
		private bool isEnterLineBreakKeep = false;

		// 前回のチャットルームの再開の要否を保持
		private bool isChatRoomLeftOffKeep = false;

		// 前回のチャットルームURLを保持
		private string lastTimeChatRoomUrl = string.Empty;

		// 表示位置を保持
		private Point locationKeep = new Point();

		// 表示サイズを保持
		private Size sizeKeep = new Size();

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

			// デフォルト表示位置を画面中央に設定
			this.CenterToScreen();

			// 設定の読み込みおよび適用
			this.ReanSettingsJson();

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
		/// 設定JSONファイルの読み込み
		/// </summary>
		private void ReanSettingsJson()
		{
			try
			{
				// 設定JSONファイルが存在しない場合
				if (!File.Exists(@settingsJsonName))
				{
					// 設定保持用JSONファイル作成
					this.CreateSettingsJson();
					return;
				}

				// 設定JSONファイルの内容を取得
				string json = File.ReadAllText(@settingsJsonName);
				var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

				// Enter押下による改行の要否の読み込み
				bool enterLineBreak = data[this.enterLineBreakKey].GetBoolean();

				// 前回のチャットルームを再開の要否の読み込み
				bool chatRoomLeftOff = data[this.isChatRoomLeftOffKey].GetBoolean();

				// 前回のチャットルームURLの読み込み
				string lastTimeChatRoomUrl = data[this.lastTimeChatRoomUrlKey].GetString();

				// 表示サイズの読み込み
				var sizeDictData = JsonSerializer.Deserialize<Dictionary<string, int>>(data[this.sizeKey].GetRawText());
				Size savedSizeData = new Size(sizeDictData[this.WidthKey], sizeDictData[this.HeightKey]);

				// 表示位置の読み込み
				var locationDictData = JsonSerializer.Deserialize<Dictionary<string, int>>(data[this.locationKey].GetRawText());
				Point savedLocationData = new Point(locationDictData[this.XKey], locationDictData[this.YKey]);

				// 最大化要否の読み込み
				bool maximize = data[this.isMaximizedKey].GetBoolean();

				// Enter押下による改行の要否の保持および設定
				this.SetEnterLineBreak(enterLineBreak);

				// 前回のチャットルームを再開の要否の保持および設定
				this.SetChatRoomLeftOff(chatRoomLeftOff);

				// 前回のチャットルームURLの保持および設定
				this.SetLastTimeChatRoomUrl(lastTimeChatRoomUrl);

				// 表示位置の保持および設定
				this.SetLocationKeep(savedLocationData);

				// 保存済みの表示サイズが画面内に収まっている場合
				this.SetSizeKeep(savedSizeData);

				// 最大化の要否の保持および設定
				this.SetMaximize(maximize);
			}
			catch
			{
				// 設定保持用JSONファイル作成
				this.CreateSettingsJson();
			}
		}

		/// <summary>
		/// 設定JSONファイル作成
		/// </summary>
		private void CreateSettingsJson()
		{
			// 設定JSONファイルが存在していた場合は削除
			if (File.Exists(@settingsJsonName))
			{
				File.Delete(@settingsJsonName);
			}

			// 設定JSONファイルを作成
			File.Create(@settingsJsonName).Dispose();

			// 設定JSONファイルに記録
			this.RecordSettingsJson();
		}

		/// <summary>
		/// 設定JSONファイルへ記録
		/// </summary>
		private void RecordSettingsJson()
		{
			try
			{
				// 設定JSONオブジェクト
				var jsonObject = new Dictionary<string, object>
				{
					// Enter押下による改行
					[this.enterLineBreakKey] = this.isEnterLineBreakKeep,

					// 前回のチャットルームを再開
					[this.isChatRoomLeftOffKey] = this.isChatRoomLeftOffKeep,

					// 前回のチャットルームURL
					[this.lastTimeChatRoomUrlKey] = this.lastTimeChatRoomUrl,

					// 表示位置
					[this.locationKey] = new Dictionary<string, int>
					{
						[this.XKey] = this.locationKeep.X,
						[this.YKey] = this.locationKeep.Y
					},

					// 表示サイズ
					[this.sizeKey] = new Dictionary<string, int>
					{
						[this.WidthKey] = this.sizeKeep.Width,
						[this.HeightKey] = this.sizeKeep.Height
					},

					// 最大化
					[this.isMaximizedKey] = this.WindowState == FormWindowState.Maximized
				};

				// JSON文字列に変換
				string jsonStr = JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions
				{
					WriteIndented = true
				});

				// 設定JSONファイルに記録
				File.WriteAllText(@settingsJsonName, jsonStr);
			}
			catch
			{
				// 設定保持用JSONファイル作成
				this.CreateSettingsJson();
			}
		}

		/// <summary>
		/// Enter押下による改行の要否の保持および設定
		/// </summary>
		/// <param name="flg"></param>
		private void SetEnterLineBreak(bool flg)
		{
			this.isEnterLineBreakKeep = flg;
		}

		/// <summary>
		/// 前回のチャットルームを再開の要否の保持および設定
		/// </summary>
		/// <param name="flg"></param>
		private void SetChatRoomLeftOff(bool flg)
		{
			this.isChatRoomLeftOffKeep = flg;
		}

		/// <summary>
		/// 前回のチャットルームURLの保持および設定
		/// </summary>
		/// <param name="url"></param>
		private void SetLastTimeChatRoomUrl(string url)
		{
			// 前回のチャットルームを再開する場合
			if (this.isChatRoomLeftOffKeep)
			{
				this.lastTimeChatRoomUrl = url;
			}
		}

		/// <summary>
		/// 表示位置の保持および設定
		/// </summary>
		/// <param name="location"></param>
		private void SetLocationKeep(Point location)
		{
			// 表示位置が画面内に収まっている場合
			if (this.IsLocationSetTrue(location))
			{
				this.Location = location;
			}
		}

		/// <summary>
		/// 表示サイズの保持および設定
		/// </summary>
		/// <param name="size"></param>
		private void SetSizeKeep(Size size)
		{
			// 表示サイズが画面内に収まっている場合
			if (this.IsSizeSetTrue(size))
			{
				// 保存済みの表示サイズを設定
				this.Size = size;
			}
			// 表示サイズが画面内に収まっていない場合
			else
			{
				// 最小サイズに設定
				this.Size = this.MinimumSize;
			}
		}

		/// <summary>
		/// 最大化の要否を設定
		/// </summary>
		/// <param name="flg"></param>
		private void SetMaximize(bool flg)
		{
			// 最大化する場合
			if (flg)
			{
				this.WindowState = FormWindowState.Maximized;
			}
			// 最大化しない場合
			else
			{
				this.WindowState = FormWindowState.Normal;
			}
		}

		/// <summary>
		/// 保存済みの表示位置が画面内に収まるかどうかを判定
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
		/// 保存済みの表示サイズが画面内に収まるかどうかを判定
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		private bool IsSizeSetTrue(Size size)
		{
			// 表示されたディスプレイを取得
			var currentScreen = System.Windows.Forms.Screen.FromControl(this);

			// 保存済みの表示サイズが画面内に収まっている場合
			if (size.Width <= currentScreen.WorkingArea.Width && size.Height <= currentScreen.WorkingArea.Height)
			{
				return true;
			}

			return false;
		}

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
				"--enable-features = OverlayScrollbar" +
				"--disable-features = CalculateNativeWinOcclusion" +
				"--enable-zero-copy " +
				"--enable-gpu-memory-buffer-video-frames";
			this.chatGPTViewEnvironment = await CoreWebView2Environment.CreateAsync(null, "UserData", options);

			// chatGPTView.CoreWebview2の初期化
			await this.chatGPTView.EnsureCoreWebView2Async(chatGPTViewEnvironment);

			// WebMessageReceivedイベントの追加
			this.chatGPTView.CoreWebView2.WebMessageReceived -= ChatGPTView_WebMessageReceived;
			this.chatGPTView.CoreWebView2.WebMessageReceived += ChatGPTView_WebMessageReceived;

			// Enter押下による改行を行う場合
			if (this.isEnterLineBreakKeep)
			{
				// Enter押下による改行有効化のためのJavaScriptコードをchatGPTViewに追加
				await this.chatGPTView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(@"

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
			await this.chatGPTView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(@"

				// DOMContentLoadedイベントのリスナーを追加
				window.addEventListener('DOMContentLoaded', () => {

					// DOMの変化を監視するMutationObserverを作成
					const observer = new MutationObserver(() => {

						// DOM内容を取得
						const messages = document.querySelectorAll('[data-message-author-role]');

						// メッセージが120件を超えている場合は古いメッセージから非表示
						if (messages.length > 120) {
							for (let i = 0; i < messages.length - 120; i++) {

								// メッセージの親要素のarticleタグを取得
								const container = messages[i].closest('article');

								// articleタグが存在する場合は非表示
								if(container) {
									container.style.display = 'none';
								}
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
			this.chatGPTView.CoreWebView2.Settings.AreDevToolsEnabled = false;

			// ステータスバー表示無効化
			this.chatGPTView.CoreWebView2.Settings.IsStatusBarEnabled = false;

			// デフォルトの右クリックメニュー無効化
			this.chatGPTView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;

			// ズームコントロール無効化
			this.chatGPTView.CoreWebView2.Settings.IsZoomControlEnabled = false;

			// 共有チャット以外のチャット内リンクをクリックした場合の処理
			this.chatGPTView.CoreWebView2.NewWindowRequested -= this.ChatGPTView_NewWindowRequested;
			this.chatGPTView.CoreWebView2.NewWindowRequested += this.ChatGPTView_NewWindowRequested;

			// チャットルーム移動時の処理
			this.chatGPTView.CoreWebView2.HistoryChanged -= this.ChatGPTView_HistoryChanged;
			this.chatGPTView.CoreWebView2.HistoryChanged += this.ChatGPTView_HistoryChanged;

			// 前回のチャットルームを再開する場合
			if (this.isChatRoomLeftOffKeep)
			{
				// 前回のチャットルームURLが保存されている場合
				if (this.lastTimeChatRoomUrl.Contains("chatgpt.com/c/"))
				{
					// 前回のチャットルームURLを読み込み
					this.chatGPTView.CoreWebView2.Navigate(this.lastTimeChatRoomUrl);
				}
				else
				{
					// ChatGPTのトップページの読み込み
					this.chatGPTView.CoreWebView2.Navigate("https://chatgpt.com/");
				}
			}
			else
			{
				// ChatGPTのトップページの読み込み
				this.chatGPTView.CoreWebView2.Navigate("https://chatgpt.com/");
			}
		}

		/// <summary>
		/// DeactiveイベントおよびClosingイベント共通処理
		/// </summary>
		private void DeactiveAndClosing()
		{
			// 設定JSONファイルへの書き込み
			this.RecordSettingsJson();
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
		/// ChatGPTView_HistoryChangedイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChatGPTView_HistoryChanged(object sender, object e)
		{
			// 現在のURLを取得
			string currentUrl = this.chatGPTView.CoreWebView2.Source;

			// チャットルーム移動時
			if (currentUrl.Contains("chatgpt.com/c/"))
			{
				// チャットルームURLを保持
				this.SetLastTimeChatRoomUrl(currentUrl);
			}
		}

		#endregion
	}
}
