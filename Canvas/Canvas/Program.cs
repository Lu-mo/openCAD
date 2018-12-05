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
            //col["status"] = "出题";
            if (col["status"] != null)
            {
                string status = col["status"].ToString();
                string examName;
                string testID;
                string stuID;
                string stuName;
                string className;
                string school;
                //MessageBox.Show(status);
                if (status.Equals("答题") || status.Equals("修改答案"))
                {
                    examName = col["examName"].ToString();
                    testID = col["testID"].ToString();
                    stuID = col["stuID"].ToString();
                    stuName = col["stuName"].ToString();
                    className = col["className"].ToString();
                    school = col["school"].ToString();
                    //examName = "juan10";
                    //testID = "1";
                    //stuID = "16240070";
                    //stuName = "yxt";
                    //className = "1701";
                    //school = "深职院";
                    //MessageBox.Show(examName+','+testID+','+stuID+','+stuName+','+className+school);
                }
                else
                {
                    examName = "11";
                    testID = col["testID"].ToString();
                    //testID = "9";
                    stuID = "11";
                    stuName = "11";
                    className = "11";
                    school = "11";
                }

                string Info;
                //xml文本比较
                //if (CommonTools.XmlComparator.BothXmlLoader("E:\\111.cadxml", "E:\\222.cadxml", out Info) == false)
                //    Console.WriteLine(Info);

                CommonTools.Tracing.AddId(TracePaint);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainWin(status, examName, testID, stuID, stuName, className, school));

                CommonTools.Tracing.Terminate();
            }
        }
    }
}