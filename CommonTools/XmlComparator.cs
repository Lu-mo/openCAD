using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CommonTools
{
    public class XmlComparator
    {
        static public XmlDocument standard=null;
        static public XmlDocument answer=null;
        //public static XmlComparator(string StandardXmlFilePath, string AnswerXmlFilePath)
        //{
        //    if (BothXmlLoader(StandardXmlFilePath, AnswerXmlFilePath) == false)
        //        return;
        //}
        public static bool BothXmlLoader(string StandardXmlFilePath, string AnswerXmlFilePath, out string Info)
        {
            try
            {
                //初始化
                standard = new XmlDocument();
                answer = new XmlDocument();
                //加载xml文件（参数为xml文件的路径）
                standard.Load(StandardXmlFilePath);
                answer.Load(AnswerXmlFilePath);
            }
            catch (Exception)
            {
                Info = "Loading Error";
                return false;
            }
            //Info = "Loading successful";
            bool flag= XmlCompare(out Info);
            return flag;
            //return true;
        }
        public static bool XmlCompare(out string Info)
        {
            if (standard == null || answer == null)
            {
                Info = "Xml Empty";
                return false;
            }
            //读取根节点
            XmlNode SrootNode = standard.SelectSingleNode("CanvasDataModel").SelectSingleNode("layer").SelectSingleNode("items");
            XmlNode ArootNode = answer.SelectSingleNode("CanvasDataModel").SelectSingleNode("layer").SelectSingleNode("items");

            //Console.WriteLine(SrootNode.Name);

            //获得该节点的子节点（即：该节点的第一层子节点）
            XmlNodeList SfirstLevelNodeList = SrootNode.ChildNodes;
            XmlNodeList AfirstLevelNodeList = ArootNode.ChildNodes;

            bool flag = false;
            int sameNum = 0;
            foreach (XmlNode Snode in SfirstLevelNodeList)
            {
                foreach (XmlNode Anode in AfirstLevelNodeList)
                {
                    if (NodeCompare(Snode, Anode))
                    {
                        flag = true;
                    }
                }
                if (flag == true)
                {
                    sameNum++;
                    flag = false;
                }

            }
            if (sameNum == SfirstLevelNodeList.Count)
            {
                Info ="correct graph number:"+ sameNum.ToString() + "/" + "total correct graph number:" + SfirstLevelNodeList.Count.ToString()+"/" + "Your answer graph number:" + AfirstLevelNodeList.Count.ToString();
                return true;
            }
            else
            {
                Info = "correct graph number:" + sameNum.ToString() + "/" + "total correct graph number:" + SfirstLevelNodeList.Count.ToString() + "/" + "Your answer graph number:" + AfirstLevelNodeList.Count.ToString();
                return false;
            }
        }
        public static bool NodeCompare(XmlNode s,XmlNode a)
        {
            if (!s.Name.Equals(a.Name)) return false;
            //获得该节点的子节点（即：该节点的第一层子节点）
            XmlNodeList sProperty = s.ChildNodes;
            XmlNodeList aProperty = a.ChildNodes;

            Dictionary<string, string> sd = new Dictionary<string, string>();
            Dictionary<string, string> ad = new Dictionary<string, string>();
            //Console.WriteLine(sProperty.ToString());
            for (int i=0;i<sProperty.Count;i++)
            {
                string style = sProperty.Item(i).Attributes.Item(0).Value;
                //if (!style.Equals("UseLayerWidth") && !style.Equals("UseLayerColor") && !style.Equals("Width") && !style.Equals("Color"))
                
                    sd.Add(sProperty.Item(i).Attributes.Item(0).Value, sProperty.Item(i).Attributes.Item(1).Value);
                    ad.Add(aProperty.Item(i).Attributes.Item(0).Value, aProperty.Item(i).Attributes.Item(1).Value);
                //Console.WriteLine(sProperty.Item(i).Attributes.Item(0).Value);

                //string temp = sProperty.Item(i).Attributes.Item(1).Value;
                //if(temp.Contains("{X="))
                //    Console.WriteLine(sProperty.Item(i).Attributes.Item(1).Value);
                //if (!sProperty.Item(i).Attributes.Item(0).Value.Equals(aProperty.Item(i).Attributes.Item(0).Value)) return false;
                //if (!sProperty.Item(i).Attributes.Item(1).Value.Equals(aProperty.Item(i).Attributes.Item(1).Value)) return false;
                //Console.WriteLine(sProperty.Item(i).Attributes.Item(1).Value.ToString());

            }
            //valueEqual(sd["P1"], ad["P1"]);
            //Console.WriteLine()
            if (s.Name.Equals("line"))//线没有起点终点之分
            {
                if (valueEqual(sd["P1"], ad["P1"]) && valueEqual(sd["P2"], ad["P2"]))
                {
                    //Console.WriteLine("if (s.Name.Equals(\"line\"))");
                    return true;
                }
                else if (valueEqual(sd["P1"], ad["P2"]) && valueEqual(sd["P2"], ad["P1"]))
                    return true;
                Console.WriteLine("line unpair");
                return false;
            }
            else if (s.Name.Equals("rectangle"))//四种一模一样的矩形
            {
                if (valueEqual(sd["P1"], ad["P1"]) && valueEqual(sd["P2"], ad["P2"])) return true;
                else if (valueEqual(sd["P1"], ad["P2"]) && valueEqual(sd["P2"], ad["P1"])) return true;
                else if(valueEqual(sd["P1"], ad["P1"].Split(',')[0] + ',' + ad["P2"].Split(',')[1]) && valueEqual(sd["P2"], ad["P2"].Split(',')[0] + ',' + ad["P1"].Split(',')[1])) return true;
                else if(valueEqual(sd["P1"], ad["P2"].Split(',')[0] + ',' + ad["P1"].Split(',')[1]) && valueEqual(sd["P2"], ad["P1"].Split(',')[0] + ',' + ad["P2"].Split(',')[1])) return true;
                //ad["P1"].Split(',')[0] + ad["P2"].Split(',')[1];
                //Console.WriteLine(ad["P1"].Split(',')[0]+','+ ad["P2"].Split(',')[1]);
                Console.WriteLine("rectangle unpair");
                return false;
            }
            else if(s.Name.Equals("arc"))
            {
                if(!sd["Direction"].Equals(ad["Direction"]))
                {
                    string temp = ad["StartAngle"];
                    ad["StartAngle"] = ad["EndAngle"];
                    ad["EndAngle"] = temp;
                }
                if (valueEqual(sd["Center"], ad["Center"]) && valueEqual(sd["Radius"], ad["Radius"]) && valueEqual(sd["StartAngle"], ad["StartAngle"]) && valueEqual(sd["EndAngle"], ad["EndAngle"])) return true;
                Console.WriteLine("arc unpair");
                return false;
            }
            else if (s.Name.Equals("circle"))
            {
                if (valueEqual(sd["Center"], ad["Center"]) && valueEqual(sd["Radius"], ad["Radius"])) return true;
                Console.WriteLine("circle unpair");
                return false;
            }
            else if(s.Name.Equals("bezierCurve"))
            {
                //Console.WriteLine("\n else if(s.Name.Equals(\"bezierCurve\"))");
                if (valueEqual(sd["P1"], ad["P1"]) && valueEqual(sd["P2"], ad["P2"]) && valueEqual(sd["P3"], ad["P3"]) && valueEqual(sd["P4"], ad["P4"])) return true;
                else if (valueEqual(sd["P1"], ad["P4"]) && valueEqual(sd["P2"], ad["P3"]) && valueEqual(sd["P3"], ad["P2"]) && valueEqual(sd["P4"], ad["P1"])) return true;
                Console.WriteLine("bezierCurve unpair");
                return false;
            }
            else
            {
                Console.WriteLine("dataModel error");
                return false;
            }
            return true;
        }
        public static bool valueEqual(string s,string a)
        {

            //Console.WriteLine(s.Substring(s.IndexOf(",") + 4, s.IndexOf("}") - s.IndexOf(",") - 4));
            if (s.Contains("{X="))
            {
                double sX = Double.Parse(s.Substring(s.IndexOf("X") + 2, s.IndexOf(",") - s.IndexOf("X") - 2));
                double sY = Double.Parse(s.Substring(s.IndexOf(",") + 4, s.IndexOf("}") - s.IndexOf(",") - 4));
                double aX = Double.Parse(a.Substring(a.IndexOf("X") + 2, a.IndexOf(",") - a.IndexOf("X") - 2));
                double aY = Double.Parse(a.Substring(a.IndexOf(",") + 4, a.IndexOf("}") - a.IndexOf(",") - 4));
                //Console.WriteLine(sX+", "+sY+"   "+aX+", "+aY);
                    if (isAllowableDeviation(sX, aX) && isAllowableDeviation(sY, aY))
                    return true;
            }
            else
            {
                try
                {
                    return isAllowableDeviation(Double.Parse(s), Double.Parse(a));
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            return false;
        }
        public static bool isAllowableDeviation(double s,double a)
        {
            double allowableDeviation = 0.1;
            if(Math.Abs(s-a)<=allowableDeviation)
                return true;
            return false;
        }
    }
    
}
