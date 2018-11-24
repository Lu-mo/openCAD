using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Deployment.Application;
using System.Web;
using System.Collections.Specialized;
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
            NameValueCollection col = new NameValueCollection();
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                string queryString = ApplicationDeployment.CurrentDeployment.ActivationUri.Query;
                col = HttpUtility.ParseQueryString(queryString);
            }
            //CommonTools.Tracing.EnableTrace();
            col["stauts"] = "出题";
            col["testID"] = "111";
            string status = col["stauts"].ToString();
            string id;
            string testID;
            string stuID;
            if (status.Equals("答题"))
            {
                id = col["id"].ToString();
                testID = col["testID"].ToString();
                stuID = col["stuID"].ToString();
            }
            else 
            {
                id = "11";
                testID = col["testID"].ToString();
                stuID = "11";
            }
          
            string Info;
            //xml文本比较
            if (CommonTools.XmlComparator.BothXmlLoader("E:\\111.cadxml", "E:\\222.cadxml", out Info) == false)
                Console.WriteLine(Info);

            CommonTools.Tracing.AddId(TracePaint);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWin(status,id,testID,stuID));

            CommonTools.Tracing.Terminate();
        }
    }
}