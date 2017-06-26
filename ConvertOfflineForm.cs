#region Related components
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using net.vieapps.Components.Utility;
using net.vieapps.books.Components;
#endregion

namespace net.vieapps.books.Converter
{
	public partial class ConvertOfflineForm : Form
	{
		List<string> _files = new List<string>();
		List<Book> _updatingBooks = new List<Book>();
		bool _isConverting = false, _isCrawling = false;
		CancellationTokenSource _cts = null;
		int _kindlegenProcesID = -1;

		#region Event handlers
		public ConvertOfflineForm()
		{
			this.InitializeComponent();
			this.GetSourceFiles();
			this.Convert.Enabled = this._files.Count > 0;
			if (this._files.Count < 1)
				this.UpdateLogs("KHÔNG Tìm thấy file JSON/EPUB/HTML nào trong thư mục [Input] để có thể thực hiện hoạt động chuyển đổi", false);
		}

		void Convert_Click(object sender, EventArgs e)
		{
			if (this._isConverting || this._isCrawling)
				this.DoCancel();
			else
				this.DoConvert();
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
		#endregion

		#region Get source files
		void GetSourceFiles()
		{
			string[] searchPatterns = "*.json|*.epub|*.html".Split('|');
			foreach (string searchPattern in searchPatterns)
			{
				string[] filePaths = Directory.GetFiles("input", searchPattern);
				if (filePaths != null && filePaths.Length > 0)
				{
					this.UpdateLogs("Tìm thấy " + filePaths.Length + "  " + searchPattern.Replace("*.", "").ToUpper() + " file(s)  trong thư mục [Input]", false);
					foreach (string filePath in filePaths)
					{
						string[] info = Utility.GetFileParts(filePath, false);
						if (!info[1].ToLower().Equals("books.json"))
							this._files.Add(info[1]);

						if (info[1].ToLower().EndsWith(".html"))
						{
							string html = Utility.ReadTextFile(filePath, Encoding.GetEncoding(1252));
							int start = html.IndexOf("<title>");
							int end = html.IndexOf("</title>");
							if (start > 0 && end > 0)
							{
								string title = html.Substring(start + 7, end - start - 7).HtmlDecode();
								this.UpdateLogs("- " + info[1] + " [" + title + "]", false);
							}
							else
								this.UpdateLogs("- " + info[1], false);
						}
						else if (!info[1].ToLower().Equals("books.json"))
							this.UpdateLogs("- " + info[1], false);
					}
					this.UpdateLogs("\r\n", false);
				}
			}
		}
		#endregion

		void DoConvert()
		{
			if (this._files.Count < 1)
			{
				MessageBox.Show("Phải có ít nhất 01 file dữ liệu e-book để chuyển đổi!", "Lỗi");
				return;
			}

			bool gotOne = this.EpubFile.Checked || this.MobiFile.Checked || this.JsonFile.Checked || this.DataFile.Checked;
			if (!gotOne)
			{
				MessageBox.Show("Phải chỉ định ít nhất 01 kiểu dữ liệu e-book sẽ chuyển đổi sang!", "Lỗi");
				return;
			}

			this.UpdateButton("Huỷ");
			this.UpdateStates(false);

			this._cts = new CancellationTokenSource();
			this._isConverting = true;

			this.UpdateLogs("\r\n" + DateTime.Now.ToString("dd/MM HH:mm") + ": -----" + "\r\n" + "Bắt đầu quá trình chuyển đổi files JSON/EPUB/HTML" + "\r\n", true);
			this.RunConvert();

			Task.Run(async () =>
			{
				try
				{
					while (this._files.Count > 0 || this._updatingBooks.Count > 0)
						await Task.Delay(2345, this._cts.Token);
				}
				catch (OperationCanceledException) { }
				catch (Exception) { }

				this._isConverting = false;
				this._isCrawling = false;
				this.UpdateButton("Chuyển đổi");
				this.UpdateStates(true);
			}).ConfigureAwait(false);
		}

		void DoCancel()
		{
			DialogResult result = MessageBox.Show("Chắc chắn muốn huỷ bỏ?", "Huỷ", MessageBoxButtons.YesNo);
			if (result.Equals(DialogResult.Yes))
			{
				this._cts.Cancel();
				this._files.Clear();
				this._updatingBooks.Clear();
				this.UpdateLogs("\r\n" + "....... Canceled (offline books) ........" + "\r\n", true);
			}
		}

		void RunConvert()
		{
			if (this._files.Count < 1)
			{
				this.UpdateLogs("\r\n" + DateTime.Now.ToString("dd/MM HH:mm") + ": -----" + "\r\n" + "Hoàn thành quá trình chuyển đổi files JSON/EPUB/HTML", true);
				this.UpdateLogs((this._updatingBooks.Count > 0 ? "Đang đợi hoàn thành quá trình lấy các dữ liệu còn thiếu..... [" + this._updatingBooks.Count + "]" : "") + "\r\n", true);

				this._isConverting = false;
				return;
			}

			System.Timers.Timer timer = new System.Timers.Timer();
			timer.Interval = 10;
			timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnRunConvert);
			timer.AutoReset = false;
			timer.Start();
		}

		void OnRunConvert(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (this._files[0].ToLower().EndsWith(".html"))
				this.ConvertHtml();
			else if (this._files[0].ToLower().EndsWith(".json"))
				this.ConvertJson();
			else if (this._files[0].ToLower().EndsWith(".epub"))
				this.ConvertEpub();
		}

		void GoNext()
		{
			this._isConverting = true;
			this.RunConvert();
		}

		#region Convert HTML files
		void ConvertHtml()
		{
			this.UpdateLogs("\r\n" + DateTime.Now.ToString("dd/MM HH:mm") + ": -----" + "\r\n" + "Bắt đầu chuyển đổi file HTML [input\\" + this._files[0] + "]", true);
			string html = Utility.ReadTextFile("input\\" + this._files[0], Encoding.GetEncoding(1252));
			MsWordHtml.Normalize(html, true, false, true, true, true, this.OnHtmlNormalizing, this.OnHtmlNormalized);
		}

		void OnHtmlNormalizing(string logs)
		{
			if (!this._isConverting)
				return;

			this.UpdateLogs(logs, true);
		}

		void OnHtmlNormalized(string html)
		{
			if (!this._isConverting)
				return;

			this.UpdateLogs("Đã chuẩn hoá xong mã HTML, bắt đầu chuyển đổi", true);
			MsWordHtml.ToBook(html, "input", "temp", null, this.OnHtmlConverted);
		}

		void OnHtmlConverted(Book book)
		{
			if (!this._isConverting)
				return;

			this.UpdateLogs("Đã chuyển đổi xong, bắt đầu sinh các file nội dung", true);

			// generate JSON file
			if (this.JsonFile.Checked)
			{
				Book.GenerateJsonFile(book, "temp", book.Filename);
				this.UpdateLogs("Hoàn thành file JSON [input\\" + book.Filename + ".json]", true);
			}

			// generate EPUB file
			if (this.EpubFile.Checked)
				Book.GenerateEpubFile(book, "temp", book.Filename, this.OnGenerateEpubFileCompleted);

			// generate MOBI file
			if (this.MobiFile.Checked || this.DataFile.Checked)
				Book.GenerateMobiData(book, "temp", book.Filename, this.OnGenerateMobiFileDataCompleted, null);

			// if not, then move files
			else
				Book.MoveFiles("temp", "books", book.Filename, this.JsonFile.Checked || this.DataFile.Checked, book.PermanentID, true, this.OnMoveFilesCompleted);
		}
		#endregion

		#region Convert JSON files
		void ConvertJson()
		{
			bool isValid = this.EpubFile.Checked || this.MobiFile.Checked || this.DataFile.Checked;
			if (!isValid)
			{
				this._files.RemoveAt(0);
				this.GoNext();
				return;
			}

			this.UpdateLogs("\r\n" + DateTime.Now.ToString("dd/MM HH:mm") + ": -----" + "\r\n" + "Bắt đầu chuyển đổi file JSON [input\\" + this._files[0] + "]", true);
			Book book = null;
			try
			{
				book = Book.FromJsonFile("input\\" + this._files[0]);
				book.GetMediaFiles();
			}
			catch (Exception ex)
			{
				this.UpdateLogs("Đã xảy ra lỗi khi phân tích file JSON: " + ex.Message, true);
#if DEBUG
				this.UpdateLogs("Stack: " + ex.StackTrace, false);
#endif
				this._files.RemoveAt(0);
				this.GoNext();
				return;
			}

			if (!this._isConverting)
				return;

			this.UpdateLogs("Đã phân tích xong JSON, bắt đầu kiểm tra", false);
			book.Verify();

			if (book.IsCompleted)
			{
				this.UpdateLogs("Đã kiểm tra xong JSON, bắt đầu chuẩn hoá", false);
				book.Normalize();
				this.UpdateLogs("Đã chuẩn hoá xong, bắt đầu sinh các file nội dung", false);
			}
			else
			{
				this.UpdateLogs("Sách [" + book.Name + "] bị thiếu một số chương. Bắt đầu lấy thông tin các chương còn thiếu.....", true);
				this._updatingBooks.Add(book);

				this._files.RemoveAt(0);
				this.GoNext();

				this.RunCrawler();
				return;
			}

			if (!this._isConverting)
				return;

			Book.GenerateJsonFile(book, "input", book.Filename);
			this.UpdateLogs("Hoàn thành file JSON [input\\" + book.Filename + ".json]", true);

			if (this.EpubFile.Checked)
				Book.GenerateEpubFile(book, "input", book.Filename, this.OnGenerateEpubFileCompleted);

			if (this.MobiFile.Checked || this.DataFile.Checked)
				Book.GenerateMobiData(book, "input", book.Filename, this.OnGenerateMobiFileDataCompleted, null);

			else
			{
				// clean-up
				if (!this.DataFile.Checked)
					Book.CleanFiles("input\\" + book.Filename);
				else
					Book.RenameFiles("input\\" + book.Filename);
				this.OnMoveFilesCompleted();
			}
		}
		#endregion

		#region Convert EPUB files
		void ConvertEpub()
		{
			this._files.RemoveAt(0);
			this.GoNext();
		}
		#endregion

		#region Generate EPUB/JSON/MOBI file
		void OnGenerateEpubFileCompleted(Book book, string filePath)
		{
			if (!this._isConverting)
				return;

			this.UpdateLogs("Hoàn thành file EPUB [input\\" + book.Name + ".epub]", true);
		}

		void OnGenerateMobiFileDataCompleted(Book book, string folder, string filename)
		{
			if (!this._isConverting)
				return;

			this.UpdateLogs("Hoàn thành file HTML/OPF/NCX [input\\" + book.Filename + ".html]", true);

			// generate MOBI file
			if (this.MobiFile.Checked)
			{
				this.UpdateLogs("Bắt đầu sinh file MOBI [input\\" + filename + ".mobi]", true);
				this._kindlegenProcesID = Book.GenerateMobiFile("vieBooks.Kindlegen.dll", folder + @"\" + filename + ".opf", this.OnGenerateMobiFileCompleted);

				Task.Run(async () =>
				{
					try
					{
						while (this._kindlegenProcesID > 0)
							await Task.Delay(780, this._cts.Token);
					}
					catch (OperationCanceledException)
					{
						if (this._kindlegenProcesID > 0)
							Utility.KillProcess(this._kindlegenProcesID);
					}
					catch (Exception)
					{
						this._files.RemoveAt(0);
					}
				}).ConfigureAwait(false);
			}
			else
			{
				// clean-up
				if (!this.DataFile.Checked)
					Book.CleanFiles(folder + "\\" + filename);
				else
					Book.RenameFiles(folder + "\\" + filename);

				this.OnMoveFilesCompleted();
			}
		}

		void OnGenerateMobiFileCompleted(object sender, EventArgs e)
		{
			if (!this._isConverting)
				return;

			this._kindlegenProcesID = -1;

			string arguments = (sender as Process).StartInfo.Arguments;
			string[] info = Utility.GetFileParts(arguments);
			string filePath = info[0] + "\\" + info[1];
			string filePathMD5 = info[0] + "\\" + info[1].ToLower().GetMD5();

			this.UpdateLogs("Hoàn thành file MOBI [input\\" + info[1] + ".mobi]", true);
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

			// clean-up
			if (!this.DataFile.Checked)
				Book.CleanFiles(arguments);
			else
				Book.RenameFiles(arguments);

			this.OnMoveFilesCompleted();
		}

		void OnMoveFilesCompleted()
		{
			this.UpdateLogs("-----" + "\r\n" + DateTime.Now.ToString("dd/MM HH:mm") + ": Hoàn thành e-book..." + "\r\n", true);
			this._files.RemoveAt(0);
			this.GoNext();
		}
		#endregion

		#region Crawl missing contents/chapters
		void RunCrawler()
		{
			if (this._isCrawling)
				return;

			this._isCrawling = true;

			Task.Run(async () =>
			{
				while (this._updatingBooks.Count > 0)
				{
					await Task.Delay(100);
					await this.CrawlUrlOfMissingChapters();
					await this.CrawlMissingChapters();
				}
			}).ConfigureAwait(false);

			Task.Run(async () =>
			{
				try
				{
					while (this._updatingBooks.Count > 0)
						await Task.Delay(2345, this._cts.Token);
				}
				catch (OperationCanceledException) { }
				catch (Exception) { }

				this._isCrawling = false;
			}).ConfigureAwait(false);
		}

		async Task CrawlUrlOfMissingChapters()
		{
			try
			{
				// parse book
				Book book = null;
				if (this._updatingBooks[0].SourceUri.StartsWith("http://isach.info"))
				{
					try
					{
						await Utility.GetWebPageAsync("http://isach.info/robots.txt");
					}
					catch { }

					await Task.Delay(Utility.GetRandomNumber(1234, 3210), this._cts.Token);
					book = await ISach.ParseBook(this._updatingBooks[0].SourceUri, this._cts.Token);
				}
				else if (this._updatingBooks[0].SourceUri.StartsWith("http://vnthuquan.net"))
					book = await VNThuQuan.ParseBook(this._updatingBooks[0].SourceUri, this._cts.Token);

				if (book == null)
					throw new InformationNotFoundException("Không có thông tin chính xác về sách [" + this._updatingBooks[0].SourceUri + "]");

				// update url of missing chapters
				if (this._updatingBooks[0].MissingChapters.Count < 1)
				{
					book.ID = this._updatingBooks[0].ID;
					book.PermanentID = this._updatingBooks[0].PermanentID;
					this._updatingBooks[0] = book;
				}
				else
					for (int index = 0; index < this._updatingBooks[0].MissingChapters.Count; index++)
						this._updatingBooks[0].ChapterUrls[this._updatingBooks[0].MissingChapters[index]] = book.ChapterUrls[this._updatingBooks[0].MissingChapters[index]];
			}
			catch (OperationCanceledException) { }
			catch (Exception ex)
			{
				this.UpdateLogs("Đã xảy ra lỗi trong quá trình lấy dữ liệu [" + this._updatingBooks[0].Name + "]: " + ex.Message, true);
#if DEBUG
				this.UpdateLogs("Stack: " + ex.StackTrace, false);
#endif
				this._updatingBooks.RemoveAt(0);
			}
		}

		async Task CrawlMissingChapters()
		{
			try
			{
				Book book = this._updatingBooks[0];
				if (book.SourceUri.StartsWith("http://isach.info"))
					book = await ISach.FetchChapters(book, "input", this._cts.Token, this.CrawlerOnProcess, this.CrawlerOnChapterCompleted, this.CrawlerOnChapterError, this.CrawlerOnDownloadCompleted, this.CrawlerOnDownloadError, Program.MainForm._crawlMethod);
				else if (book.SourceUri.StartsWith("http://vnthuquan.net"))
					book = await VNThuQuan.FetchChapters(book, "input", this._cts.Token, this.CrawlerOnProcess, this.CrawlerOnChapterCompleted, this.CrawlerOnChapterError, this.CrawlerOnDownloadCompleted, this.CrawlerOnDownloadError, (int)CrawMethods.Fast);

				Book.GenerateJsonFile(book, "input", book.Filename);
				this._files.Add(book.Filename + ".json");

				this._updatingBooks.RemoveAt(0);
				if (!this._isConverting)
				{
					await Task.Delay(234);
					this.GoNext();
				}
			}
			catch (OperationCanceledException) { }
			catch (Exception ex)
			{
				this.UpdateLogs("Đã xảy ra lỗi trong quá trình lấy dữ liệu: " + ex.Message, false);
#if DEBUG
				this.UpdateLogs("Stack: " + ex.StackTrace, false);
#endif
				this._updatingBooks.RemoveAt(0);
			}
		}

		void CrawlerOnBookCompleted(Book book)
		{
			Book.GenerateJsonFile(book, "input", book.Filename);
			this._files.Add(book.Filename + ".json");

			this._updatingBooks.RemoveAt(0);
			if (!this._isConverting)
				this.GoNext();
		}

		void CrawlerOnBookError(Book book, Exception ex)
		{
			if (!this._isCrawling)
				return;

			this.UpdateLogs("Đã xảy ra lỗi trong quá trình lấy dữ liệu : " + ex.Message, false);
		}

		void CrawlerOnProcess(string message)
		{
			if (!this._isCrawling)
				return;

			this.UpdateLogs(message, false);
		}

		void CrawlerOnBookParsed(Book book)
		{
			if (!this._isCrawling)
				return;

			string message = "The book is parsed [" + book.Name + "]. Start to fetch contents"
												+ "\r\n"
												+ "- Uri: " + book.SourceUri
												+ "\r\n"
												+ "- Total of chapters: " + book.Chapters.Count;
			this.UpdateLogs(message, false);
		}

		void CrawlerOnChapterCompleted(string url, List<string> data)
		{
			if (!this._isCrawling)
				return;

			string message = "";
			if (data == null || data[0].Equals("") && data[1].Equals(""))
				message = "Không lấy được nội dung [" + url + "] ";
			else
			{
				string counter = data.Count > 3 ? "[" + data[2] + "/" + data[3] + "] " : "";
				message = "Đã lấy xong nội dung " + counter + ": " + data[0];
			}
			this.UpdateLogs(message, false);
		}

		void CrawlerOnChapterError(string url, Exception ex)
		{
			if (!this._isCrawling)
				return;

			this.UpdateLogs("Đã xảy ra lỗi khi lấy nội dung  [" + url + "]: " + ex.Message, false);
#if DEBUG
			this.UpdateLogs("Stack: " + ex.StackTrace, false);
#endif
		}

		void CrawlerOnDownloadCompleted(string url, string filePath)
		{
			if (!this._isCrawling)
				return;

			this.UpdateLogs("Đã download xong file [" + url + "]", false);
		}

		void CrawlerOnDownloadError(string url, Exception ex)
		{
			this.UpdateLogs("Đã xảy ra lỗi khi download  file [" + url + "]: " + ex.Message, false);
		}
		#endregion

	}
}