#region Related components
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

using net.vieapps.Components.Utility;
using net.vieapps.books.Components;
#endregion

namespace net.vieapps.books.Converter
{
	public partial class ConvertOnlineForm : Form
	{
		List<string> _urls = null;
		CancellationTokenSource _cts = null;
		bool _isConverting = false;

		#region Event handlers
		public ConvertOnlineForm()
		{
			this.InitializeComponent();
			this.InitializeEnvironent();
		}

		public delegate void UpdateLogsDelegator(string logs, bool updateAppLogs);

		void UpdateLogs(string logs, bool updateAppLogs)
		{
			if (base.InvokeRequired)
			{
				UpdateLogsDelegator method = new UpdateLogsDelegator(this.UpdateLogs);
				base.Invoke(method, new object[] { logs, updateAppLogs });
			}
			else
			{
				if (updateAppLogs)
					Program.MainForm.UpdateLogs(logs);
				this.Logs.AppendText(logs + "\r\n");
				this.Logs.SelectionStart = this.Logs.TextLength;
				this.Logs.ScrollToCaret();
			}
		}

		public delegate void UpdateStatesDelegator(bool state);

		void UpdateStates(bool state)
		{
			if (base.InvokeRequired)
			{
				UpdateStatesDelegator method = new UpdateStatesDelegator(this.UpdateStates);
				base.Invoke(method, new object[] { state });
			}
			else
				this.SourceUrl.Enabled = this.AddSource.Enabled = this.SourceUrls.Enabled = this.RemoveSource.Enabled =
					this.EpubFile.Enabled = this.MobiFile.Enabled = this.JsonFile.Enabled = this.DataFile.Enabled = state;
		}

		public delegate void UpdateButtonDelegator(string text);

		void UpdateButton(string text)
		{
			if (base.InvokeRequired)
			{
				UpdateButtonDelegator method = new UpdateButtonDelegator(this.UpdateButton);
				base.Invoke(method, new object[] { text });
			}
			else
				this.Convert.Text = text;
		}

		void InitializeEnvironent()
		{
			string linksFile = "input\\links-online.txt";
			if (File.Exists(linksFile))
			{
				string[] links = Utility.ReadTextFile(linksFile).Replace("\r", "").Split("\n".ToCharArray());
				string logs = "";
				int counter = 0;
				foreach (string link in links)
					if (!string.IsNullOrWhiteSpace(link) && this.IsValidUrl(link))
					{
						counter++;
						logs += (!logs.Equals("") ? "\r\n" : "") + "- " + link;
						this.SourceUrls.Items.Add(link);
					}
				this.UpdateLogs("Tìm thấy " + counter + " liên kết sách online (web) trong thư mục [Input]" + "\r\n" + logs + "\r\n", false);
			}

			string jsonFile = "input\\books.json";
			if (File.Exists(jsonFile))
			{
				string[] jsonBooks = Utility.ReadTextFile(jsonFile).Replace("\r", "").Split("\n".ToCharArray());
				string logs = "";
				int counter = 0;
				foreach (string jsonBook in jsonBooks)
				{
					Book book = null;
					try
					{
						book = Book.FromJson(jsonBook);
						if (this.IsValidUrl(book.SourceUri))
						{
							counter++;
							logs += (!logs.Equals("") ? "\r\n" : "") + "- " + book.Title + " [" + book.SourceUri + "]";
							this.SourceUrls.Items.Add(book.SourceUri);
						}
					}
					catch { }
				}
				this.UpdateLogs("Tìm thấy " + counter + " sách đã chuẩn hoá (JSON) trong thư mục [Input]" + "\r\n" + logs + "\r\n", false);
			}
		}

		bool IsValidUrl(string url)
		{
			return string.IsNullOrWhiteSpace(url)
							? false
							: url.Contains("vnthuquan.net") || url.Contains("isach.info");
		}

		void AddSource_Click(object sender, EventArgs e)
		{
			if (!this.IsValidUrl(this.SourceUrl.Text))
			{
				MessageBox.Show("Url của sách/truyện phải nằm trong site vnthuquan.net hoặc isach.info", "Lỗi", MessageBoxButtons.OK);
				this.SourceUrl.Focus();
				return;
			}
			else if (this.SourceUrls.FindString(this.SourceUrl.Text.Trim()) < 0)
				this.SourceUrls.Items.Add(this.SourceUrl.Text.Trim());

			this.SourceUrl.Text = "";
			this.SourceUrl.Focus();
		}

		void RemoveSource_Click(object sender, EventArgs e)
		{
			if (this.SourceUrls.Items == null || this.SourceUrls.Items.Count < 1)
				return;

			DialogResult result = MessageBox.Show("Chắc chắn muốn xoá bớt?", "Xoá bớt", MessageBoxButtons.YesNo);
			if (!result.Equals(DialogResult.Yes))
				return;

			while (this.SourceUrls.SelectedItems.Count > 0)
			{
				int index = this.SourceUrls.FindString(this.SourceUrls.SelectedItems[0].ToString());
				if (index > -1)
					this.SourceUrls.Items.RemoveAt(index);
			}

			if (this.SourceUrls.Items.Count < 1)
				this.SourceUrl.Focus();
		}

		void Convert_Click(object sender, EventArgs e)
		{
			if (this._isConverting)
				this.DoCancel();
			else
				this.DoConvert();
		}
		#endregion

		void DoConvert()
		{
			this._urls = new List<string>();
			for (int index = 0; index < this.SourceUrls.Items.Count; index++)
				this._urls.Add(this.SourceUrls.Items[index].ToString());

			if (this._urls.Count < 1)
			{
				MessageBox.Show("Phải có ít nhất 01 địa chỉ của sách/truyện online  để chuyển đổi!", "Lỗi");
				this.SourceUrl.Focus();
				return;
			}

			bool gotOne = this.EpubFile.Checked || this.MobiFile.Checked || this.JsonFile.Checked || this.DataFile.Checked;
			if (!gotOne)
			{
				MessageBox.Show("Phải chỉ định ít nhất 01 kiểu dữ liệu e-book sẽ chuyển đổi sang!", "Lỗi");
				return;
			}

			this.Logs.Text = "";
			this.UpdateLogs("\r\n" + DateTime.Now.ToString("dd/MM HH:mm") + ": -----" + "\r\n" + "Bắt đầu quá trình chuyển đổi online ebooks" + "\r\n", true);
			this.SourceUrls.Items.Clear();
			this.UpdateButton("Huỷ");
			this.UpdateStates(false);

			this._cts = new CancellationTokenSource();
			this._isConverting = true;

			this.RunConvert();

			Task.Run(async () =>
			{
				while (this._urls.Count > 0)
					await Task.Delay(789);
				this._isConverting = false;
				this.UpdateButton("Chuyển đổi");
				this.UpdateStates(true);
			}).ConfigureAwait(false);
		}

		void DoCancel()
		{
			DialogResult result = MessageBox.Show("Chắc chắn muốn huỷ bỏ?", "Huỷ", MessageBoxButtons.YesNo);
			if (result.Equals(DialogResult.Yes))
			{
				this._isConverting = false;
				this._cts.Cancel();
				this._urls.Clear();
				this.UpdateLogs("\r\n" + "....... Canceled (online books) ........" + "\r\n", true);
			}
		}

		void RunConvert()
		{
			if (this._urls.Count < 1)
				this.UpdateLogs("\r\n" + DateTime.Now.ToString("dd/MM HH:mm") + ": -----" + "\r\n" + "Hoàn thành quá trình chuyển đổi online ebooks" + "\r\n", true);

			else
				Task.Run(async () =>
				{
					await this.OnRunConvert();
				}).ConfigureAwait(false);
		}

		async Task OnRunConvert()
		{
			string url = this._urls[0];
			this.UpdateLogs("Bắt đầu lấy dữ liệu & chuyển đổi [" + url + "]", true);
			if (url.Contains("isach.info"))
				{
					try
					{
						await Utility.GetWebPageAsync("http://isach.info/robots.txt");
						await Task.Delay(Utility.GetRandomNumber(456, 789), this._cts.Token);
					}
					catch { }
					await ISach.GetBook(null, url, "temp", this._cts.Token, this.OnBookProcess, this.OnBookParsed, this.OnBookCompleted, this.OnBookError, this.OnChapterCompleted, this.OnChapterError, this.OnDownloadCompleted, this.OnDownloadError, Program.MainForm._crawlMethod);
				}
				else
					await VNThuQuan.GetBook(null, url, "temp", this._cts.Token, this.OnBookProcess, this.OnBookParsed, this.OnBookCompleted, this.OnBookError, this.OnChapterCompleted, this.OnChapterError, this.OnDownloadCompleted, this.OnDownloadError, (int)CrawMethods.Fast);
		}

		void OnNext()
		{
			this._urls.RemoveAt(0);
			this.RunConvert();
		}

		void OnBookProcess(string message)
		{
			if (!this._isConverting)
				return;

			this.UpdateLogs(message, true);
		}

		void OnBookParsed(Book book)
		{
			if (!this._isConverting)
				return;

			this.UpdateLogs("Đã phân tích xong [" + book.Name + " : " + book.SourceUri.Replace("/mobil/", "/").Replace("/mobile/", "/") + "]\r\nBắt đầu lấy dữ liệu (" + book.Chapters.Count + " chương/phần)", true);
		}

		void OnChapterCompleted(string url, List<string> data)
		{
			if (!this._isConverting)
				return;

			string message = "";
			if (data == null || data[0].Equals("") && data[1].Equals(""))
				message = "Không lấy được nội dung [" + url.Replace("/mobil/", "/").Replace("/mobile/", "/") + "] ";
			else
			{
				string counter = data.Count > 3 ? "[" + data[2] + "/" + data[3] + "] " : "";
				message = "Đã lấy xong nội dung " + counter + ": " + data[0];
			}
			this.UpdateLogs(message, true);
		}

		void OnChapterError(string url, Exception ex)
		{
			if (!this._isConverting)
				return;

			this.UpdateLogs("Đã xảy ra lỗi khi lấy nội dung  [" + url.Replace("/mobil/", "/").Replace("/mobile/", "/") + "]: " + ex.Message , true);
#if DEBUG
			this.UpdateLogs("Stack: " + ex.StackTrace, false);
#endif
		}

		void OnDownloadCompleted(string url, string filePath)
		{
			if (!this._isConverting)
				return;

			this.UpdateLogs("Đã download xong file [" + url + "]", true);
		}

		void OnDownloadError(string url, Exception ex)
		{
			if (!this._isConverting)
				return;

			this.UpdateLogs("Đã xảy ra lỗi khi download  file [" + url + "]: " + ex.Message, true);
		}

		void OnBookCompleted(Book book)
		{
			if (!this._isConverting)
				return;

			if (book == null || book.Title.Equals(""))
			{
				this.UpdateLogs("Không lấy được bộ nội dung....", true);
				this.OnNext();
			}
			else
			{
				this.UpdateLogs("Đã lấy xong toàn bộ nội dung, bắt đầu sinh các file", true);

				// generate JSON file
				if (this.JsonFile.Checked)
				{
					Book.GenerateJsonFile(book, "temp", book.Filename);
					this.UpdateLogs("Hoàn thành file JSON [books\\" + book.Filename + ".json]", true);
				}

				// generate EPUB file
				if (this.EpubFile.Checked)
					Book.GenerateEpubFile(book, "temp", book.Filename, this.OnGenerateEpubFileCompleted);

				// generate MOBI file (and related data files)
				if (this.MobiFile.Checked || this.DataFile.Checked)
					Book.GenerateMobiData(book, "temp", book.Filename, this.OnGenerateMobiFileDataCompleted, this.OnGenerateMobiFileDataError);

				// if not, then move
				else
					Book.MoveFiles("temp", "books", book.Filename, this.JsonFile.Checked, book.PermanentID, true, this.OnMoveFilesCompleted);
			}
		}

		void OnBookError(Book book, Exception ex)
		{
			if (!this._isConverting)
				return;

			this.UpdateLogs("Đã xảy ra lỗi [" + ex.Message + "]", true);
			Exception exception = ex.InnerException;
			while (exception != null)
			{
				this.UpdateLogs("\t" + exception.Message + "]", false);
#if DEBUG
				this.UpdateLogs("\tStack: " + exception.StackTrace, false);
#endif
				exception = exception.InnerException;
			}
#if DEBUG
			this.UpdateLogs("Stack: " + ex.StackTrace, false);
#endif
			this.OnNext();
		}

		void OnGenerateEpubFileCompleted(Book book, string filePath)
		{
			if (!this._isConverting)
				return;

			this.UpdateLogs("Hoàn thành file EPUB [books\\" + book.Name + ".epub]", true);
		}

		void OnGenerateMobiFileDataCompleted(Book book, string folder, string filename)
		{
			if (!this._isConverting)
				return;

			this.UpdateLogs("Hoàn thành file HTML/OPF/NCX [books\\" + book.Filename + ".html]", true);

			if (this.MobiFile.Checked)
			{
				this.UpdateLogs("Bắt đầu sinh file MOBI [books\\" + filename + ".mobi]", true);
				Book.GenerateMobiFile("vieBooks.Kindlegen.dll", folder + @"\" + filename + ".opf", this.OnGenerateMobiFileCompleted);
			}
			else if (this.DataFile.Checked)
				Book.MoveFiles("temp", "books", book.Filename, this.JsonFile.Checked || this.DataFile.Checked, book.PermanentID, true, this.OnMoveFilesCompleted);
		}

		void OnGenerateMobiFileDataError(Book book, string folder, string filename, Exception ex)
		{
			if (!this._isConverting)
				return;

			this.UpdateLogs("Đã xảy ra lỗi [" + ex.Message + "]", true);
			Exception exception = ex.InnerException;
			while (exception != null)
			{
				this.UpdateLogs("\t" + exception.Message + "]", false);
#if DEBUG
				this.UpdateLogs("\tStack: " + exception.StackTrace, false);
#endif
				exception = exception.InnerException;
			}
		}

		void OnGenerateMobiFileCompleted(object sender, EventArgs e)
		{
			if (!this._isConverting)
				return;

			string arguments = (sender as Process).StartInfo.Arguments;
			string[] info = Utility.GetFileParts(arguments);
			string filePath = info[0] + "\\" +info[1];
			string filePathMD5 = info[0] + "\\" + info[1].ToLower().GetMD5();

			this.UpdateLogs("Hoàn thành file MOBI [books\\" + info[1] + ".mobi]", true);
			this.UpdateLogs("Hoàn thành e-book [" + info[1] + "]" + "\r\n", true);

			// identifier of all media files
			string identifier = "";
			if (this.DataFile.Checked || this.JsonFile.Checked)
			{
				if (File.Exists(filePathMD5 + ".html"))
					identifier = Book.GetIdentityFromHtmlFile(filePathMD5 + ".html");
				else if (File.Exists(filePath + ".html"))
					identifier = Book.GetIdentityFromHtmlFile(filePath + ".html");
				else if (File.Exists(filePathMD5 + ".json"))
					identifier = Book.GetIdentityFromJsonFile(filePathMD5 + ".json");
				else if (File.Exists(filePath + ".json"))
					identifier = Book.GetIdentityFromJsonFile(filePath + ".json");
			}

			// rename files
			if (!this.DataFile.Checked)
				Book.CleanFiles(arguments);
			else
				Book.RenameFiles(arguments);

			// move files to destination folders
			Book.MoveFiles(info[0], "books", info[1], true, identifier, true, this.OnMoveFilesCompleted);
		}

		void OnMoveFilesCompleted()
		{
			if (!this._isConverting)
				return;

			this.UpdateLogs("Hoàn thành e-book.... " + "\r\n", true);
			this.OnNext();
		}

	}
}