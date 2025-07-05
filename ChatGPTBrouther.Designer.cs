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
			this.ChatCPTView = new Microsoft.Web.WebView2.WinForms.WebView2();
			this.TextCreateSpace = new System.Windows.Forms.RichTextBox();
			this.PreSendButton = new System.Windows.Forms.Button();
			this.PreSendButtonPanel = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.ChatCPTView)).BeginInit();
			this.PreSendButtonPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// ChatCPTView
			// 
			this.ChatCPTView.AllowExternalDrop = true;
			this.ChatCPTView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ChatCPTView.CreationProperties = null;
			this.ChatCPTView.DefaultBackgroundColor = System.Drawing.Color.White;
			this.ChatCPTView.Location = new System.Drawing.Point(1, 1);
			this.ChatCPTView.Name = "ChatCPTView";
			this.ChatCPTView.Size = new System.Drawing.Size(1901, 684);
			this.ChatCPTView.TabIndex = 0;
			this.ChatCPTView.ZoomFactor = 1D;
			// 
			// TextCreateSpace
			// 
			this.TextCreateSpace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TextCreateSpace.Location = new System.Drawing.Point(1, 691);
			this.TextCreateSpace.Name = "TextCreateSpace";
			this.TextCreateSpace.Size = new System.Drawing.Size(1801, 305);
			this.TextCreateSpace.TabIndex = 1;
			this.TextCreateSpace.Text = "";
			// 
			// PreSendButton
			// 
			this.PreSendButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.PreSendButton.Location = new System.Drawing.Point(9, 253);
			this.PreSendButton.Name = "PreSendButton";
			this.PreSendButton.Size = new System.Drawing.Size(75, 45);
			this.PreSendButton.TabIndex = 2;
			this.PreSendButton.Text = "送信";
			this.PreSendButton.UseVisualStyleBackColor = true;
			// 
			// PreSendButtonPanel
			// 
			this.PreSendButtonPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.PreSendButtonPanel.Controls.Add(this.PreSendButton);
			this.PreSendButtonPanel.Location = new System.Drawing.Point(1808, 691);
			this.PreSendButtonPanel.Name = "PreSendButtonPanel";
			this.PreSendButtonPanel.Size = new System.Drawing.Size(94, 305);
			this.PreSendButtonPanel.TabIndex = 3;
			// 
			// ChatGPTBrouther
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1904, 1001);
			this.Controls.Add(this.PreSendButtonPanel);
			this.Controls.Add(this.TextCreateSpace);
			this.Controls.Add(this.ChatCPTView);
			this.Name = "ChatGPTBrouther";
			this.Text = "ChatGPTBrouther";
			((System.ComponentModel.ISupportInitialize)(this.ChatCPTView)).EndInit();
			this.PreSendButtonPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private Microsoft.Web.WebView2.WinForms.WebView2 ChatCPTView;
		private System.Windows.Forms.RichTextBox TextCreateSpace;
		private System.Windows.Forms.Button PreSendButton;
		private System.Windows.Forms.Panel PreSendButtonPanel;
	}
}

