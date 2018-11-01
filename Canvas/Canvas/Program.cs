using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Canvas
{
	static class Program
	{
		public static int TracePaint = 1;
		public static string AppName = "OpenS-CAD";
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
            //CommonTools.Tracing.EnableTrace();

            string Info;
            //xml文本比较
            if (CommonTools.XmlComparator.BothXmlLoader("E:\\111.cadxml", "E:\\222.cadxml",out Info) == false)
                Console.WriteLine(Info);

            CommonTools.Tracing.AddId(TracePaint);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWin());

            CommonTools.Tracing.Terminate();
        }
	}
}