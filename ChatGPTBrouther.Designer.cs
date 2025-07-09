namespace ChatGPTBrowser
{
	partial class ChatGPTBrouther
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
			this.ChatGPTView = new Microsoft.Web.WebView2.WinForms.WebView2();
			this.sendButton = new System.Windows.Forms.Button();
			this.sendButtonPanel = new System.Windows.Forms.Panel();
			this.textCreateSpace = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.ChatGPTView)).BeginInit();
			this.sendButtonPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// ChatGPTView
			// 
			this.ChatGPTView.AllowExternalDrop = true;
			this.ChatGPTView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ChatGPTView.CreationProperties = null;
			this.ChatGPTView.DefaultBackgroundColor = System.Drawing.Color.White;
			this.ChatGPTView.Location = new System.Drawing.Point(1, 1);
			this.ChatGPTView.Name = "ChatGPTView";
			this.ChatGPTView.Size = new System.Drawing.Size(1261, 484);
			this.ChatGPTView.TabIndex = 0;
			this.ChatGPTView.ZoomFactor = 1D;
			// 
			// sendButton
			// 
			this.sendButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.sendButton.Enabled = false;
			this.sendButton.Location = new System.Drawing.Point(3, 0);
			this.sendButton.Name = "sendButton";
			this.sendButton.Size = new System.Drawing.Size(75, 45);
			this.sendButton.TabIndex = 3;
			this.sendButton.Text = "送信";
			this.sendButton.UseVisualStyleBackColor = true;
			this.sendButton.Click += new System.EventHandler(this.SendButton_Click);
			// 
			// sendButtonPanel
			// 
			this.sendButtonPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.sendButtonPanel.Controls.Add(this.sendButton);
			this.sendButtonPanel.Location = new System.Drawing.Point(1168, 491);
			this.sendButtonPanel.Name = "sendButtonPanel";
			this.sendButtonPanel.Size = new System.Drawing.Size(94, 185);
			this.sendButtonPanel.TabIndex = 2;
			// 
			// textCreateSpace
			// 
			this.textCreateSpace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textCreateSpace.Font = new System.Drawing.Font("MS UI Gothic", 12F);
			this.textCreateSpace.Location = new System.Drawing.Point(263, 491);
			this.textCreateSpace.Multiline = true;
			this.textCreateSpace.Name = "textCreateSpace";
			this.textCreateSpace.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textCreateSpace.Size = new System.Drawing.Size(902, 185);
			this.textCreateSpace.TabIndex = 1;
			this.textCreateSpace.TextChanged += new System.EventHandler(this.TextCreateSpace_TextChanged);
			// 
			// ChatGPTBrouther
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1264, 681);
			this.Controls.Add(this.textCreateSpace);
			this.Controls.Add(this.sendButtonPanel);
			this.Controls.Add(this.ChatGPTView);
			this.MinimumSize = new System.Drawing.Size(800, 600);
			this.Name = "ChatGPTBrouther";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ChatGPTBrouther";
			((System.ComponentModel.ISupportInitialize)(this.ChatGPTView)).EndInit();
			this.sendButtonPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Microsoft.Web.WebView2.WinForms.WebView2 ChatGPTView;
		private System.Windows.Forms.Button sendButton;
		private System.Windows.Forms.Panel sendButtonPanel;
		private System.Windows.Forms.TextBox textCreateSpace;
	}
}

