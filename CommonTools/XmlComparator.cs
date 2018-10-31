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
        public static bool BothXmlLoader(string StandardXmlFilePath, string AnswerXmlFilePath)
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
                return false;
            }
            return XmlCompare();
            //return true;
        }
        public static bool XmlCompare()
        {
            if (standard == null || answer == null)
                return false;
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
                    if(NodeCompare(Snode,Anode))
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
                return true;
            else
                return false;
        }

        public static bool NodeCompare(XmlNode s,XmlNode a)
        {
            if (!s.Name.Equals(a.Name)) return false;
            //获得该节点的子节点（即：该节点的第一层子节点）
            XmlNodeList sProperty = s.ChildNodes;
            XmlNodeList aProperty = a.ChildNodes;
            //Console.WriteLine(sProperty.ToString());
            for(int i=0;i<6;i++)
            {
                if (!sProperty.Item(i).Attributes.Item(0).Value.Equals(aProperty.Item(i).Attributes.Item(0).Value)) return false;
                if (!sProperty.Item(i).Attributes.Item(1).Value.Equals(aProperty.Item(i).Attributes.Item(1).Value)) return false;
                //Console.WriteLine(sProperty.Item(i).Attributes.Item(1).Value.ToString());
            }
            return true;
        }
    }
    
}
