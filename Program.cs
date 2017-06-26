using System;
using System.Windows.Forms;

namespace net.vieapps.books.Converter
{
	internal static class Program
	{
		internal static MainForm MainForm = null;

		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Program.MainForm = new MainForm();
			Application.Run(Program.MainForm);
		}
	}
}
