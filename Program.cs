using System;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ChatGPTExtend
{
	internal static class Program
	{
		// アプリ名
		private const string APP_NAME = "CHATGPT_EXTEND";

		// ミューテックス
		private static Mutex mutex;

		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main()
		{
			bool created = false;

			// ミューテックスを取得
			mutex = new Mutex(true, APP_NAME, out created);

			if (!created)
			{
				ActivateExistingInstance();
				return;
			}

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new ChatGPTExtend());
		}

		/// <summary>
		/// 本アプリが既に起動中の場合の処理
		/// </summary>
		private static void ActivateExistingInstance()
		{
			// 現在のプロセス情報を取得
			var current = Process.GetCurrentProcess();

			// 同じプロセス名のプロセス一覧を取得
			foreach (var proc in Process.GetProcessesByName(current.ProcessName))
			{
				// 自分自身のプロセスではない場合
				if (proc.Id != current.Id)
				{
					// そのプロセスのメインウィンドウのハンドル取得
					IntPtr hWnd = proc.MainWindowHandle;

					// ウィンドウを持っている場合
					if (hWnd != IntPtr.Zero)
					{
						// 最小化されていた場合はウィンドウの復元
						if (IsIconic(hWnd))
						{
							ShowWindow(hWnd, SW_RESTORE);
						}

						// 既存のウィンドウを最前面に表示
						SetForegroundWindow(hWnd);

						break;
					}
				}
			}
		}

		// 最小化を検知するためのWin32API
		[DllImport("user32.dll")]
		private static extern bool IsIconic(IntPtr hWnd);

		// ウィンドウの復元のためのWin32API
		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		// 最小化・最大化を解除してウィンドウを元の状態に戻す
		private const int SW_RESTORE = 9;

		// ウィンドウを最前面に表示するためのWin32API
		[DllImport("user32.dll")]
		private static extern bool SetForegroundWindow(IntPtr hWnd);
	}
}
