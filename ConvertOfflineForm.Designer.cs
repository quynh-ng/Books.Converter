namespace net.vieapps.books.Converter
{
	partial class ConvertOfflineForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConvertOfflineForm));
			this.Introduction = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.EpubFile = new System.Windows.Forms.CheckBox();
			this.MobiFile = new System.Windows.Forms.CheckBox();
			this.JsonFile = new System.Windows.Forms.CheckBox();
			this.DataFile = new System.Windows.Forms.CheckBox();
			this.Convert = new System.Windows.Forms.Button();
			this.Logs = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// Introduction
			// 
			this.Introduction.AutoSize = true;
			this.Introduction.Location = new System.Drawing.Point(12, 9);
			this.Introduction.Name = "Introduction";
			this.Introduction.Size = new System.Drawing.Size(521, 65);
			this.Introduction.TabIndex = 0;
			this.Introduction.Text = resources.GetString("Introduction.Text");
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(590, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(55, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Tuỳ chọn:";
			// 
			// EpubFile
			// 
			this.EpubFile.AutoSize = true;
			this.EpubFile.Location = new System.Drawing.Point(593, 32);
			this.EpubFile.Name = "EpubFile";
			this.EpubFile.Size = new System.Drawing.Size(55, 17);
			this.EpubFile.TabIndex = 2;
			this.EpubFile.Text = "EPUB";
			this.EpubFile.UseVisualStyleBackColor = true;
			// 
			// MobiFile
			// 
			this.MobiFile.AutoSize = true;
			this.MobiFile.Location = new System.Drawing.Point(663, 32);
			this.MobiFile.Name = "MobiFile";
			this.MobiFile.Size = new System.Drawing.Size(53, 17);
			this.MobiFile.TabIndex = 3;
			this.MobiFile.Text = "MOBI";
			this.MobiFile.UseVisualStyleBackColor = true;
			// 
			// JsonFile
			// 
			this.JsonFile.AutoSize = true;
			this.JsonFile.Checked = true;
			this.JsonFile.CheckState = System.Windows.Forms.CheckState.Checked;
			this.JsonFile.Location = new System.Drawing.Point(593, 57);
			this.JsonFile.Name = "JsonFile";
			this.JsonFile.Size = new System.Drawing.Size(54, 17);
			this.JsonFile.TabIndex = 4;
			this.JsonFile.Text = "JSON";
			this.JsonFile.UseVisualStyleBackColor = true;
			// 
			// DataFile
			// 
			this.DataFile.AutoSize = true;
			this.DataFile.Location = new System.Drawing.Point(663, 57);
			this.DataFile.Name = "DataFile";
			this.DataFile.Size = new System.Drawing.Size(109, 17);
			this.DataFile.TabIndex = 5;
			this.DataFile.Text = "HTML/OPF/NCX";
			this.DataFile.UseVisualStyleBackColor = true;
			// 
			// Convert
			// 
			this.Convert.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Convert.Location = new System.Drawing.Point(805, 12);
			this.Convert.Name = "Convert";
			this.Convert.Size = new System.Drawing.Size(191, 62);
			this.Convert.TabIndex = 6;
			this.Convert.Text = "Chuyển đổi";
			this.Convert.UseVisualStyleBackColor = true;
			this.Convert.Click += new System.EventHandler(this.Convert_Click);
			// 
			// Logs
			// 
			this.Logs.Location = new System.Drawing.Point(15, 87);
			this.Logs.MaxLength = 0;
			this.Logs.Multiline = true;
			this.Logs.Name = "Logs";
			this.Logs.ReadOnly = true;
			this.Logs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.Logs.Size = new System.Drawing.Size(981, 462);
			this.Logs.TabIndex = 7;
			// 
			// ConvertOfflineForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1008, 561);
			this.Controls.Add(this.Logs);
			this.Controls.Add(this.Convert);
			this.Controls.Add(this.DataFile);
			this.Controls.Add(this.JsonFile);
			this.Controls.Add(this.MobiFile);
			this.Controls.Add(this.EpubFile);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.Introduction);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "ConvertOfflineForm";
			this.Text = "Convert Offline eBooks (JSON/EPUB/Word HTML)";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label Introduction;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox EpubFile;
		private System.Windows.Forms.CheckBox MobiFile;
		private System.Windows.Forms.CheckBox JsonFile;
		private System.Windows.Forms.CheckBox DataFile;
		private System.Windows.Forms.Button Convert;
		private System.Windows.Forms.TextBox Logs;
	}
}