using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Canvas
{
    public partial class Form1 : Form
    {
        public Form1(string status, string id, string testID)
        {
            Main(status,id, testID);
        }
        public static int TracePaint = 1;
        public static string AppName = "";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string status,string id,string testID)
        {
            AppName = id.ToString();
            //CommonTools.Tracing.EnableTrace();
            CommonTools.Tracing.AddId(TracePaint);

            Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWin(status,id, testID));
            CommonTools.Tracing.Terminate();
        }
    }
}
