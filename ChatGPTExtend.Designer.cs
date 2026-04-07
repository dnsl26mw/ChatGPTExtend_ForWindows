using Microsoft.Web.WebView2.Core;
using System.Windows.Forms;

namespace ChatGPTExtend
{
	partial class ChatGPTExtend
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChatGPTExtend));
			this.chatGPTView = new Microsoft.Web.WebView2.WinForms.WebView2();
			this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.dispToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.line1 = new System.Windows.Forms.ToolStripSeparator();
			this.enterLineBreakToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.chatRoomLeftOffStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.line2 = new System.Windows.Forms.ToolStripSeparator();
			this.reloadStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.line3 = new System.Windows.Forms.ToolStripSeparator();
			this.closeStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.chatGPTView)).BeginInit();
			this.contextMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// chatGPTView
			// 
			this.chatGPTView.AllowExternalDrop = true;
			this.chatGPTView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.chatGPTView.CreationProperties = null;
			this.chatGPTView.DefaultBackgroundColor = System.Drawing.Color.White;
			this.chatGPTView.Location = new System.Drawing.Point(0, 0);
			this.chatGPTView.Margin = new System.Windows.Forms.Padding(0);
			this.chatGPTView.Name = "chatGPTView";
			this.chatGPTView.Size = new System.Drawing.Size(1264, 681);
			this.chatGPTView.TabIndex = 0;
			this.chatGPTView.ZoomFactor = 0.88D;
			// 
			// notifyIcon
			// 
			this.notifyIcon.ContextMenuStrip = this.contextMenu;
			this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
			this.notifyIcon.Text = "ChatGPTExtend";
			this.notifyIcon.Visible = true;
			this.notifyIcon.MouseUp += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_MouseUp);
			// 
			// contextMenu
			// 
			this.contextMenu.DropShadowEnabled = false;
			this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dispToolStripMenuItem,
            this.line1,
            this.enterLineBreakToolStripMenuItem,
            this.chatRoomLeftOffStripMenuItem,
            this.line2,
			this.reloadStripMenuItem,
			this.line3,
			this.closeStripMenuItem});
			this.contextMenu.Name = "contextMenu";
			this.contextMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.contextMenu.Size = new System.Drawing.Size(217, 126);
			// 
			// dispToolStripMenuItem
			// 
			this.dispToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.dispToolStripMenuItem.Name = "dispToolStripMenuItem";
			this.dispToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
			this.dispToolStripMenuItem.Text = "表示";
			this.dispToolStripMenuItem.Click += new System.EventHandler(this.ContextDispMenu_Click);
			// 
			// line1
			// 
			this.line1.Name = "line1";
			this.line1.Size = new System.Drawing.Size(213, 6);
			// 
			// enterLineBreakToolStripMenuItem
			// 
			this.enterLineBreakToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.enterLineBreakToolStripMenuItem.Name = "enterLineBreakToolStripMenuItem";
			this.enterLineBreakToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
			this.enterLineBreakToolStripMenuItem.Height = 28;
			this.enterLineBreakToolStripMenuItem.Text = "Enter押下で改行";
			this.enterLineBreakToolStripMenuItem.Click += new System.EventHandler(this.ContextEnterLineBreakMenu_Click);
			// 
			// chatRoomLeftOffStripMenuItem
			// 
			this.chatRoomLeftOffStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.chatRoomLeftOffStripMenuItem.Name = "chatRoomLeftOffStripMenuItem";
			this.chatRoomLeftOffStripMenuItem.Size = new System.Drawing.Size(216, 22);
			this.chatRoomLeftOffStripMenuItem.Height = 28;
			this.chatRoomLeftOffStripMenuItem.Text = "起動時に前回開いたチャットを開く";
			this.chatRoomLeftOffStripMenuItem.Click += new System.EventHandler(this.ContextChatRoomLeftOffMenu_Click);
			// 
			// line2
			// 
			this.line2.Name = "line2";
			this.line2.Size = new System.Drawing.Size(213, 6);
			// 
			// ReloadStripMenuItem
			// 
			this.reloadStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.reloadStripMenuItem.Name = "chatRoomLeftOffStripMenuItem";
			this.reloadStripMenuItem.Size = new System.Drawing.Size(216, 22);
			this.reloadStripMenuItem.Height = 28;
			this.reloadStripMenuItem.Text = "再読み込み";
			this.reloadStripMenuItem.Click += new System.EventHandler(this.ContextReloadMenu_Click);
			// 
			// line3
			// 
			this.line3.Name = "line3";
			this.line3.Size = new System.Drawing.Size(213, 6);
			// 
			// closeStripMenuItem
			// 
			this.closeStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.closeStripMenuItem.Name = "closeStripMenuItem";
			this.closeStripMenuItem.Size = new System.Drawing.Size(216, 22);
			this.closeStripMenuItem.Height = 28;
			this.closeStripMenuItem.Text = "終了";
			this.closeStripMenuItem.Click += new System.EventHandler(this.ContextCloseMenu_Click);
			// 
			// ChatGPTExtend
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1264, 681);
			this.Controls.Add(this.chatGPTView);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MinimumSize = new System.Drawing.Size(500, 700);
			this.Name = "ChatGPTExtend";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "ChatGPTExtend";
			this.Deactivate += new System.EventHandler(this.ChatGPTExtend_Deactive);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChatGPTExtend_FormClosing);
			this.SizeChanged += new System.EventHandler(this.ChatGPTExtend_SizeChanged);
			this.Move += new System.EventHandler(this.ChatGPTExtend_Move);
			((System.ComponentModel.ISupportInitialize)(this.chatGPTView)).EndInit();
			this.contextMenu.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private Microsoft.Web.WebView2.WinForms.WebView2 chatGPTView;
		private NotifyIcon notifyIcon;
		private ContextMenuStrip contextMenu;
		private ToolStripMenuItem dispToolStripMenuItem;
		private ToolStripSeparator line1;
		private ToolStripMenuItem enterLineBreakToolStripMenuItem;
		private ToolStripMenuItem chatRoomLeftOffStripMenuItem;
		private ToolStripSeparator line2;
		private ToolStripMenuItem reloadStripMenuItem;
		private ToolStripSeparator line3;
		private ToolStripMenuItem closeStripMenuItem;
	}
}

