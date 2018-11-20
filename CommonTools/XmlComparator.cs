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
                

                //string temp = sProperty.Item(i).Attributes.Item(1).Value;
                //if(temp.Contains("{X="))
                //    Console.WriteLine(sProperty.Item(i).Attributes.Item(1).Value);
                //if (!sProperty.Item(i).Attributes.Item(0).Value.Equals(aProperty.Item(i).Attributes.Item(0).Value)) return false;
                //if (!sProperty.Item(i).Attributes.Item(1).Value.Equals(aProperty.Item(i).Attributes.Item(1).Value)) return false;
                //Console.WriteLine(sProperty.Item(i).Attributes.Item(1).Value.ToString());

            }

            if(s.Name.Equals("line"))
            if(s.Name.Equals("arc"))
            {

            }
            if (s.Name.Equals("circle"))
            {
                
            }
            return true;
        }

        public static bool isAllowableDeviation(double s,double a)
        {
            double allowableDeviation = 0.001;
            if(Math.Abs(s-a)<=allowableDeviation)
                return true;
            return false;
        }
    }
    
}
