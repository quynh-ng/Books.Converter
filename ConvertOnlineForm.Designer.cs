namespace net.vieapps.books.Converter
{
	partial class ConvertOnlineForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConvertOnlineForm));
			this.label1 = new System.Windows.Forms.Label();
			this.SourceUrl = new System.Windows.Forms.TextBox();
			this.AddSource = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.SourceUrls = new System.Windows.Forms.ListBox();
			this.RemoveSource = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.DataFile = new System.Windows.Forms.CheckBox();
			this.JsonFile = new System.Windows.Forms.CheckBox();
			this.MobiFile = new System.Windows.Forms.CheckBox();
			this.EpubFile = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.Convert = new System.Windows.Forms.Button();
			this.Logs = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(4, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(83, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Url sách/truyện:";
			// 
			// SourceUrl
			// 
			this.SourceUrl.Location = new System.Drawing.Point(96, 11);
			this.SourceUrl.Name = "SourceUrl";
			this.SourceUrl.Size = new System.Drawing.Size(819, 20);
			this.SourceUrl.TabIndex = 1;
			// 
			// AddSource
			// 
			this.AddSource.Location = new System.Drawing.Point(921, 9);
			this.AddSource.Name = "AddSource";
			this.AddSource.Size = new System.Drawing.Size(75, 23);
			this.AddSource.TabIndex = 2;
			this.AddSource.Text = "Thêm";
			this.AddSource.UseVisualStyleBackColor = true;
			this.AddSource.Click += new System.EventHandler(this.AddSource_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(93, 34);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(642, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Url của sách/truyện online (chỉ hỗ trợ VNThuQuan và iSach.info). VD: http://isach" +
    ".info/story.php?story=nhoc_nicolas__rene_goscinny";
			// 
			// SourceUrls
			// 
			this.SourceUrls.FormattingEnabled = true;
			this.SourceUrls.Location = new System.Drawing.Point(96, 64);
			this.SourceUrls.Name = "SourceUrls";
			this.SourceUrls.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.SourceUrls.Size = new System.Drawing.Size(819, 95);
			this.SourceUrls.TabIndex = 3;
			// 
			// RemoveSource
			// 
			this.RemoveSource.Location = new System.Drawing.Point(921, 64);
			this.RemoveSource.Name = "RemoveSource";
			this.RemoveSource.Size = new System.Drawing.Size(75, 23);
			this.RemoveSource.TabIndex = 4;
			this.RemoveSource.Text = "Bớt";
			this.RemoveSource.UseVisualStyleBackColor = true;
			this.RemoveSource.Click += new System.EventHandler(this.RemoveSource_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 64);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(86, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Các url đã nhập:";
			// 
			// DataFile
			// 
			this.DataFile.AutoSize = true;
			this.DataFile.Location = new System.Drawing.Point(326, 182);
			this.DataFile.Name = "DataFile";
			this.DataFile.Size = new System.Drawing.Size(109, 17);
			this.DataFile.TabIndex = 8;
			this.DataFile.Text = "HTML/OPF/NCX";
			this.DataFile.UseVisualStyleBackColor = true;
			// 
			// JsonFile
			// 
			this.JsonFile.AutoSize = true;
			this.JsonFile.Location = new System.Drawing.Point(250, 181);
			this.JsonFile.Name = "JsonFile";
			this.JsonFile.Size = new System.Drawing.Size(54, 17);
			this.JsonFile.TabIndex = 7;
			this.JsonFile.Text = "JSON";
			this.JsonFile.UseVisualStyleBackColor = true;
			// 
			// MobiFile
			// 
			this.MobiFile.AutoSize = true;
			this.MobiFile.Location = new System.Drawing.Point(175, 182);
			this.MobiFile.Name = "MobiFile";
			this.MobiFile.Size = new System.Drawing.Size(53, 17);
			this.MobiFile.TabIndex = 6;
			this.MobiFile.Text = "MOBI";
			this.MobiFile.UseVisualStyleBackColor = true;
			// 
			// EpubFile
			// 
			this.EpubFile.AutoSize = true;
			this.EpubFile.Checked = true;
			this.EpubFile.CheckState = System.Windows.Forms.CheckState.Checked;
			this.EpubFile.Location = new System.Drawing.Point(96, 182);
			this.EpubFile.Name = "EpubFile";
			this.EpubFile.Size = new System.Drawing.Size(55, 17);
			this.EpubFile.TabIndex = 5;
			this.EpubFile.Text = "EPUB";
			this.EpubFile.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 182);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(55, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = "Tuỳ chọn:";
			// 
			// Convert
			// 
			this.Convert.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Convert.Location = new System.Drawing.Point(724, 171);
			this.Convert.Name = "Convert";
			this.Convert.Size = new System.Drawing.Size(191, 37);
			this.Convert.TabIndex = 9;
			this.Convert.Text = "Chuyển đổi";
			this.Convert.UseVisualStyleBackColor = true;
			this.Convert.Click += new System.EventHandler(this.Convert_Click);
			// 
			// Logs
			// 
			this.Logs.Location = new System.Drawing.Point(12, 222);
			this.Logs.MaxLength = 0;
			this.Logs.Multiline = true;
			this.Logs.Name = "Logs";
			this.Logs.ReadOnly = true;
			this.Logs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.Logs.Size = new System.Drawing.Size(984, 407);
			this.Logs.TabIndex = 15;
			// 
			// ConvertOnlineForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1008, 641);
			this.Controls.Add(this.Logs);
			this.Controls.Add(this.Convert);
			this.Controls.Add(this.DataFile);
			this.Controls.Add(this.JsonFile);
			this.Controls.Add(this.MobiFile);
			this.Controls.Add(this.EpubFile);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.SourceUrls);
			this.Controls.Add(this.RemoveSource);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.AddSource);
			this.Controls.Add(this.SourceUrl);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "ConvertOnlineForm";
			this.Text = "Convert Online eBooks";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox SourceUrl;
		private System.Windows.Forms.Button AddSource;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox SourceUrls;
		private System.Windows.Forms.Button RemoveSource;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox DataFile;
		private System.Windows.Forms.CheckBox JsonFile;
		private System.Windows.Forms.CheckBox MobiFile;
		private System.Windows.Forms.CheckBox EpubFile;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button Convert;
		private System.Windows.Forms.TextBox Logs;
	}
}