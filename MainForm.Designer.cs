namespace net.vieapps.books.Converter
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.ConvertOnline = new System.Windows.Forms.Button();
			this.ConvertOffline = new System.Windows.Forms.Button();
			this.Introduction = new System.Windows.Forms.Button();
			this.Logs = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// ConvertOnline
			// 
			this.ConvertOnline.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ConvertOnline.Location = new System.Drawing.Point(12, 12);
			this.ConvertOnline.Name = "ConvertOnline";
			this.ConvertOnline.Size = new System.Drawing.Size(361, 60);
			this.ConvertOnline.TabIndex = 1;
			this.ConvertOnline.Text = "Chuyển đổi sách online (web)";
			this.ConvertOnline.UseVisualStyleBackColor = true;
			this.ConvertOnline.Click += new System.EventHandler(this.OpenOnlineForm);
			// 
			// ConvertOffline
			// 
			this.ConvertOffline.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ConvertOffline.Location = new System.Drawing.Point(391, 14);
			this.ConvertOffline.Name = "ConvertOffline";
			this.ConvertOffline.Size = new System.Drawing.Size(437, 60);
			this.ConvertOffline.TabIndex = 2;
			this.ConvertOffline.Text = "Chuyển đổi file sách (JSON/EPUB/Word HTML)";
			this.ConvertOffline.UseVisualStyleBackColor = true;
			this.ConvertOffline.Click += new System.EventHandler(this.OpenOfflineForm);
			// 
			// Introduction
			// 
			this.Introduction.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Introduction.Location = new System.Drawing.Point(846, 14);
			this.Introduction.Name = "Introduction";
			this.Introduction.Size = new System.Drawing.Size(150, 60);
			this.Introduction.TabIndex = 3;
			this.Introduction.Text = "Giới thiệu";
			this.Introduction.UseVisualStyleBackColor = true;
			this.Introduction.Click += new System.EventHandler(this.Introduction_Click);
			// 
			// Logs
			// 
			this.Logs.Location = new System.Drawing.Point(12, 89);
			this.Logs.MaxLength = 0;
			this.Logs.Multiline = true;
			this.Logs.Name = "Logs";
			this.Logs.ReadOnly = true;
			this.Logs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.Logs.Size = new System.Drawing.Size(984, 540);
			this.Logs.TabIndex = 15;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1008, 641);
			this.Controls.Add(this.Logs);
			this.Controls.Add(this.Introduction);
			this.Controls.Add(this.ConvertOffline);
			this.Controls.Add(this.ConvertOnline);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "vieBooks.net Converter";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button ConvertOnline;
		private System.Windows.Forms.Button ConvertOffline;
		private System.Windows.Forms.Button Introduction;
		private System.Windows.Forms.TextBox Logs;
	}
}

