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
			this.chatGPTViewContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			((System.ComponentModel.ISupportInitialize)(this.chatGPTView)).BeginInit();
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
			this.chatGPTView.ZoomFactor = 1D;
			// 
			// chatGPTViewContextMenu
			// 
			this.chatGPTViewContextMenu.Name = "chatGPTViewContextMenu";
			this.chatGPTViewContextMenu.Size = new System.Drawing.Size(61, 4);
			this.chatGPTViewContextMenu.Text = "ChatGPTExtend";
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
			this.ResumeLayout(false);

		}

		#endregion

		private Microsoft.Web.WebView2.WinForms.WebView2 chatGPTView;
		private System.Windows.Forms.ContextMenuStrip chatGPTViewContextMenu;
	}
}

