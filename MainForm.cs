#region Related components
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Timers;

using net.vieapps.Components.Utility;
using net.vieapps.books.Components;
#endregion

namespace net.vieapps.books.Converter
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			this.InitializeComponent();

			this.InitializeEnvironment();

			if (this._crawlers != null && this._crawlers.Count > 0)
				this.StartCrawlers();

			if (!this._testingBookUri.Equals("") || !this._testingChapterUri.Equals(""))
				this.StartTestingMethods();

			if (!string.IsNullOrWhiteSpace(this._folderForNormalizing))
				this.StartNormalizeJsonFiles();
		}

		#region Event handlers
		internal static ConvertOnlineForm OnlineForm = null;

		void OpenOnlineForm(object sender, EventArgs e)
		{
			if (MainForm.OnlineForm == null)
			{
				MainForm.OnlineForm = new ConvertOnlineForm();
				MainForm.OnlineForm.Disposed += new EventHandler(this.OnlineFormDisposed);
				MainForm.OnlineForm.Show();
			}
			else
				MainForm.OnlineForm.Activate();
		}

		void OnlineFormDisposed(object sender, EventArgs e)
		{
			MainForm.OnlineForm = null;
		}

		internal static ConvertOfflineForm OfflineForm = null;

		void OpenOfflineForm(object sender, EventArgs e)
		{
			if (MainForm.OfflineForm == null)
			{
				MainForm.OfflineForm = new ConvertOfflineForm();
				MainForm.OfflineForm.Disposed += new EventHandler(this.OfflineFormDisposed);
				MainForm.OfflineForm.Show();
			}
			else
				MainForm.OfflineForm.Activate();
		}

		void OfflineFormDisposed(object sender, EventArgs e)
		{
			MainForm.OfflineForm = null;
		}

		void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			this._crawlersCTS.Cancel();
		}

		void Introduction_Click(object sender, EventArgs e)
		{
			if (this._crawlers != null && this._crawlers.Count > 0)
			{
				DialogResult result = MessageBox.Show("Stop all crawlers?", "Crawlers", MessageBoxButtons.YesNo);
				if (result.Equals(DialogResult.Yes))
					this.StopCrawlers();
			}
			else
				this.ShowIntroduction();
		}

		void ShowIntroduction()
		{
			string msg = @"vieBooks.net Converter - Công cụ chuyển đổi e-books - v1.0.1 (2017-06)"
					+ "\r\n\r\n"
					+ "Written by Quynh Nguyen - vieBooks.net"
					+ "\r\n\r\n"
					+ "Open source @ https://github.com/vieapps/Books.Converter"
					;
			MessageBox.Show(msg, "Giới thiệu");
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
				this.Introduction.Text = text;
		}
		#endregion

		List<string> _bookUrls = new List<string>(), _epubUrls = new List<string>(), _arguments = new List<string>();
		List<Crawler> _crawlers = new List<Crawler>();
		CancellationTokenSource _crawlersCTS = new CancellationTokenSource();
		string _testingBookUri = "", _testingChapterUri = "", _folderForNormalizing = "";
		internal int _crawlMethod = 1;

		#region Initialize
		void InitializeEnvironment()
		{
			// create folders
			string[] folders = new string[] { "books", "books\\media-files", "crawls", "crawls\\media-files", "input", "temp", "temp\\media-files" };
			foreach (string folder in folders)
			{
				DirectoryInfo dirInfo = new DirectoryInfo(folder);
				if (!dirInfo.Exists)
					dirInfo.Create();
			}

			// delete temporary files
			string[] filePaths = Directory.GetFiles("temp", "*.*");
			if (filePaths != null && filePaths.Length > 0)
				foreach (string tempFile in filePaths)
					File.Delete(tempFile);

			filePaths = Directory.GetFiles("temp\\media-files", "*.*");
			if (filePaths != null && filePaths.Length > 0)
				foreach (string tempFile in filePaths)
					File.Delete(tempFile);

			// arguments
			string[] arguments = Environment.GetCommandLineArgs();
			if (arguments != null && arguments.Length > 0)
			{
				string args = "";
				foreach (string argument in arguments)
				{
					args += (argument.StartsWith("/") || argument.StartsWith("-") ? (!args.Equals("") ? " " : "") + argument.Trim() : "");

					if (argument.StartsWith("/"))
						this._arguments.Add(argument);

					else if (argument.StartsWith("-normalize"))
						this._folderForNormalizing = argument.Substring(argument.IndexOf(":") + 1).Replace("\"", "");

					else if (argument.Equals("-generate-rsa"))
					{
						List<string> keys = CryptoService.GenerateRSAKeyPairs();
						Utility.WriteTextFile("temp\\Keys.txt", keys, false);
					}
				}

				if (!args.Equals(""))
					this.UpdateLogs("Arguments [" + args + "]" + "\r\n");
			}

			// crawlers
			this._crawlers = Crawler.GetCrawlers(this._arguments, "crawls", "temp", "books", this.OnCrawlerProcess, this.OnCrawlerCompleted, this.OnCrawlerError, this._crawlersCTS.Token);

			// testing methods
			foreach(string argument in this._arguments)
			{
				if (argument.StartsWith("/test-book"))
				{
					this._testingBookUri = argument.Substring(argument.IndexOf(":") + 1).Trim();
					while (this._testingBookUri.StartsWith("\""))
						this._testingBookUri = this._testingBookUri.Right(this._testingBookUri.Length - 1);
					while (this._testingBookUri.EndsWith("\""))
						this._testingBookUri = this._testingBookUri.Left(this._testingBookUri.Length - 1);
					if (!this._testingBookUri.StartsWith("http://isach.info") && !this._testingBookUri.StartsWith("http://vnthuquan.net"))
						this._testingBookUri = "";
				}
				else if (argument.StartsWith("/test-chapter"))
				{
					this._testingChapterUri = argument.Substring(argument.IndexOf(":") + 1).Trim();
					while (this._testingChapterUri.StartsWith("\""))
						this._testingChapterUri = this._testingChapterUri.Right(this._testingChapterUri.Length - 1);
					while (this._testingChapterUri.EndsWith("\""))
						this._testingChapterUri = this._testingChapterUri.Left(this._testingChapterUri.Length - 1);
					if (!this._testingChapterUri.StartsWith("http://isach.info") && !this._testingChapterUri.StartsWith("http://vnthuquan.net"))
						this._testingChapterUri = "";
				}
				else if (argument.StartsWith("/crawl-method:"))
					try
					{
						this._crawlMethod = Convert.ToInt32(argument.Substring(argument.IndexOf(":") + 1));
					}
					catch { }
			}

			// offline files (JSON/EPUB/HTML)
			string[] searchPatterns = "*.json|*.epub|*.html".Split('|');
			foreach(string searchPattern in searchPatterns)
			{
				filePaths = Directory.GetFiles("input", searchPattern);
				if (filePaths != null && filePaths.Length > 0)
				{
					this.UpdateLogs("Tìm thấy " + filePaths.Length + "  " + searchPattern.Replace("*.", "").ToUpper() + " file(s) trong thư mục [Input]");
					foreach (string filePath in filePaths)
					{
						string filename = filePath.Substring(filePath.IndexOf("\\") + 1);
						if (filename.ToLower().EndsWith(".html"))
						{
							string html = Utility.ReadTextFile(filePath, Encoding.GetEncoding(1252));
							int start = html.IndexOf("<title>");
							int end = html.IndexOf("</title>");
							if (start > 0 && end > 0)
							{
								string title = html.Substring(start + 7, end - start - 7).HtmlDecode();
								this.UpdateLogs("- " + filename + " [" + title + "]");
							}
							else
								this.UpdateLogs("- " + filename);
						}
						else
							this.UpdateLogs("- " + filename);
					}
					this.UpdateLogs("\r\n");
				}
			}

			// online books
			string linksFile = "input\\links-online.txt";
			if (File.Exists(linksFile))
			{
				string[] links = Utility.ReadTextFile(linksFile).Replace("\r", "").Split("\n".ToCharArray());
				string logs = "";
				int counter = 0;
				foreach (string link in links)
					if (!string.IsNullOrWhiteSpace(link))
					{
						counter++;
						logs += (!logs.Equals("") ? "\r\n" : "") + "- " + link;
						this._bookUrls.Add(link);
					}
				this.UpdateLogs("Tìm thấy " + counter + " liên kết sách online (web) trong thư mục [Input]" + "\r\n" + logs + "\r\n");
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
						counter++;
						logs += (!logs.Equals("") ? "\r\n" : "") + "- " + book.Title + " [" + book.SourceUri + "]";
						this._bookUrls.Add(book.SourceUri);
					}
					catch { }
				}
				this.UpdateLogs("Tìm thấy " + counter + " sách đã chuẩn hoá (JSON) trong thư mục [Input]" + "\r\n" + logs + "\r\n");
			}

			// epub
			linksFile = "input\\links-epub.txt";
			if (File.Exists(linksFile))
			{
				string[] links = Utility.ReadTextFile(linksFile).Replace("\r", "").Split("\n".ToCharArray());
				string logs = "";
				int counter = 0;
				foreach (string link in links)
					if (!string.IsNullOrWhiteSpace(link))
					{
						counter++;
						logs += (!logs.Equals("") ? "\r\n" : "") + "- " + link;
						this._epubUrls.Add(link);
					}
				this.UpdateLogs("Tìm thấy " + counter + " liên kết download sách EPUB trong thư mục [Input]" + "\r\n" + logs + "\r\n");
			}
		}
		#endregion

		#region Working with logs
		public delegate void UpdateLogsDelegator(string logs);

		internal void UpdateLogs(string logs)
		{
			if (base.InvokeRequired)
			{
				UpdateLogsDelegator method = new UpdateLogsDelegator(this.UpdateLogs);
				base.Invoke(method, new object[] { logs });
			}
			else
			{
				this.Logs.AppendText(logs + "\r\n");
				this.Logs.SelectionStart = this.Logs.TextLength;
				this.Logs.ScrollToCaret();
			}
		}

		public delegate void ClearLogsDelegator();

		internal void ClearLogs()
		{
			if (base.InvokeRequired)
			{
				ClearLogsDelegator method = new ClearLogsDelegator(this.ClearLogs);
				base.Invoke(method, new object[] {  });
			}
			else
				this.Logs.Text = "";
		}
		#endregion

		#region Crawling books
		void StartCrawlers()
		{
			System.Timers.Timer timer = new System.Timers.Timer();
			timer.Interval = 1000 * 2;
			timer.Elapsed += new ElapsedEventHandler(this.OnRunCrawlers);
			timer.AutoReset = false;
			timer.Start();

			this.UpdateButton("Stop Crawlers");
		}

		void StopCrawlers()
		{
			this._crawlersCTS.Cancel();
			this._crawlers = null;
			this.UpdateButton("Giới thiệu");
		}

		void OnRunCrawlers(object sender, ElapsedEventArgs e)
		{
			this.UpdateLogs("Start to run " + this._crawlers.Count + " crawler(s)" + "\r\n");
			foreach (Crawler crawler in this._crawlers)
				crawler.Run();
		}

		void OnCrawlerProcess(string message)
		{
			this.UpdateLogs(message);
		}

		void OnCrawlerCompleted(Crawler crawler)
		{
			this.UpdateLogs("Crawler is completed [" + crawler.Source.Replace("\\",  "/") + "]" + (crawler.MaxAttempts > 0 ? " - Max attempts: " + crawler.MaxAttempts : "") + "\r\n");
			this.CheckCrawlersCompletedState();
		}

		void OnCrawlerError(string message, Exception ex)
		{
			string msg = "\r\n" + message + "\r\n";
			if (ex is RemoteServerErrorException && !((ex as RemoteServerErrorException).InnerException is System.Net.WebException))
				msg += ex.Message + " [" + ex.GetType().ToString() + "]" + "\r\n\r\n" + "Stack: " + ex.StackTrace + "\r\n";
			this.UpdateLogs(msg);
			this.CheckCrawlersCompletedState();
		}

		void CheckCrawlersCompletedState()
		{
			bool allCompleted = true;
			if (this._crawlers != null)
				foreach (Crawler crlr in this._crawlers)
				{
					allCompleted = allCompleted && (crlr.Status.Equals("Completed") || crlr.Status.Equals("Error"));
					if (!allCompleted)
						break;
				}

			if (allCompleted)
			{
				this._crawlers = null;
				this.UpdateButton("Giới thiệu");
				this.UpdateLogs("All crawlers are completed" + "\r\n");
			}
		}
		#endregion

		#region Run testing methods
		void StartTestingMethods()
		{
			System.Timers.Timer timer = new System.Timers.Timer();
			timer.Interval = 500;
			timer.Elapsed += new ElapsedEventHandler(this.OnTestingMethods);
			timer.AutoReset = false;
			timer.Start();
		}

		void OnTestingMethods(object sender, ElapsedEventArgs e)
		{
			if (!this._testingBookUri.Equals(""))
				Task.Run(async () =>
				{
					if (this._testingBookUri.StartsWith("http://isach.info"))
						try
						{
							await Utility.GetWebPageAsync("http://isach.info/robots.txt");
						}
						catch { }
					await this.RunTestingBook();
				}).ConfigureAwait(false);

			if (!this._testingChapterUri.Equals(""))
				Task.Run(async () =>
				{
					if (this._testingChapterUri.StartsWith("http://isach.info"))
						try
						{
							await Utility.GetWebPageAsync("http://isach.info/robots.txt");
						}
						catch { }
					await this.RunTestingChapter();
				}).ConfigureAwait(false);
		}

		async Task RunTestingBook()
		{
			this.UpdateLogs("Start to test: " + this._testingBookUri);
			try
			{
				Book book = null;
				if (this._testingBookUri.StartsWith("http://isach.info"))
					book = await ISach.ParseBook(this._testingBookUri, CancellationToken.None);
				else
					book = await VNThuQuan.ParseBook(this._testingBookUri, CancellationToken.None);

				string message = "- Total of chapters: " + book.Chapters.Count + "\r\n";
				if (book.Chapters.Count > 1)
					for (int index = 0; index < book.Chapters.Count; index++)
						message += " + " + (index + 1) + ": " + book.ChapterUrls[index] + "\r\n";
				this.UpdateLogs(message);
			}
			catch (Exception ex)
			{
				this.UpdateLogs("Error while testing: " + ex.Message + " [" + ex.GetType().ToString() + "]" + "\r\n-Stack: " + ex.StackTrace);
			}
		}

		async Task RunTestingChapter()
		{
			this.UpdateLogs("Start to test: " + this._testingChapterUri);
			try
			{
				List<string> contents = null;
				if (this._testingChapterUri.StartsWith("http://isach.info"))
					contents = await ISach.GetChapter(this._testingChapterUri, ISach.ReferUri, CancellationToken.None);
				else
					contents = await VNThuQuan.GetChapter(this._testingChapterUri, VNThuQuan.ReferUri, CancellationToken.None);

				string message = "";
				if (contents != null && (!contents[0].Equals("") || !contents[1].Equals("")))
				{
					if (this._testingChapterUri.StartsWith("http://isach.info"))
						message = "- Title: " + contents[0] + "\r\n" + "- Body: " + contents[1].Replace(">\n</", ">\r\n</");
					else
					{
						List<string> data = VNThuQuan.ParseChapter(contents);
						if (data != null && data.Count > 1)
							message = "- Title: " + data[0] + "\r\n" + "- Body: " + data[1].Replace(">\n</", ">\r\n</");
						else
							message = "No matched data is found";
					}
				}
				else
					message = "No matched data is found";
				this.UpdateLogs(message);
			}
			catch (Exception ex)
			{
				this.UpdateLogs("Error while testing: " + ex.Message + " [" + ex.GetType().ToString() + "]" + "\r\n-Stack: " + ex.StackTrace);
			}
		}
		#endregion

		#region Run normalize methods
		void StartNormalizeJsonFiles()
		{
			Task.Run(async () =>
			{
				try
				{
					await Task.Delay(1234);
					await this.NormalizeJsonFiles();
				}
				catch (OperationCanceledException) { }
				catch (Exception ex)
				{
					this.UpdateLogs("Error occurred while normalizing: " + ex.Message);
#if DEBUG
					this.UpdateLogs("Stack: " + ex.StackTrace.Replace("\t", "") + "\r\n");
#endif
				}
			}).ConfigureAwait(false);
		}

		async Task NormalizeJsonFiles()
		{
			// get files
			List<string> filePaths = Utility.GetFilePaths(this._folderForNormalizing, "*.json", true, new List<string>() { Utils.MediaFolder });
			if (filePaths == null || filePaths.Count < 1)
				return;

			// normalize
			this._crawlersCTS.Token.ThrowIfCancellationRequested();
			this.UpdateLogs("Start to normalize " + filePaths.Count + " JSON file(s) in [" + this._folderForNormalizing + "]" + "\r\n");
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			foreach (string filePath in filePaths)
				await this.NormalizeJsonFile(filePath);

			stopwatch.Stop();
			this.UpdateLogs("Process to normalize " + filePaths.Count + " JSON file(s) is completed. Total processing times: " + stopwatch.GetElapsedTimes() + " ..... \r\n");
		}

		async Task NormalizeJsonFile(string filePath)
		{
			if (!File.Exists(filePath))
			{
				this.UpdateLogs("The JSON file is not existed [" + filePath + "]");
				return;
			}

			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			Book book = Book.FromJson(await Utility.ReadTextFileAsync(filePath));
			this._crawlersCTS.Token.ThrowIfCancellationRequested();

			string[] fileParts = Utility.GetFileParts(filePath);
			this.UpdateLogs("Start to verify and normalize the book [" + book.Name + "]");

			book.Verify();
			this.UpdateLogs("Verify is completed." + (!book.IsCompleted ? " - The book is NOT COMPLETED, total of missing chapters: " + book.MissingChapters.Count : ""));
			this._crawlersCTS.Token.ThrowIfCancellationRequested();

			book.Normalize();
			this.UpdateLogs("Chapters & TOC are normalized");
			this._crawlersCTS.Token.ThrowIfCancellationRequested();

			if (!filePath.EndsWith(book.Filename + ".json"))
			{
				this.UpdateLogs("Rename JSON file the book [" + fileParts[1] + ".json --> " + book.Filename + ".json]");
				if (File.Exists(fileParts[0] + "\\" + book.Filename + ".json"))
					File.Delete(fileParts[0] + "\\" + book.Filename + ".json");

				File.Delete(filePath);
				if (File.Exists(fileParts[0] + "\\" + fileParts[1] + ".epub"))
					File.Delete(fileParts[0] + "\\" + fileParts[1] + ".epub");
				if (File.Exists(fileParts[0] + "\\" + fileParts[1] + ".mobi"))
					File.Delete(fileParts[0] + "\\" + fileParts[1] + ".mobi");
			}

			Book.GenerateJsonFile(book, fileParts[0], book.Filename);
			this.UpdateLogs("JSON file is re-generated [" + fileParts[0] + "\\" + book.Filename + ".json]");

			stopwatch.Stop();
			this.UpdateLogs("Normalize process is completed in " + stopwatch.GetElapsedTimes() + " ........ \r\n");
		}
		#endregion

	}
}