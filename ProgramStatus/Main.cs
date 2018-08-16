using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProgramStatus
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 程序态跳转
        /// </summary>
        /// <param name="programStatusTypeEnum"></param>
        private void ProgramStatusJump(ProgramStatusTypeEnum programStatusTypeEnum)
        {
            string typeStr = programStatusTypeEnum.ToString().Substring(5);
            //通过反射加载窗体
            Form form = (Form)Assembly.Load("ProgramStatus").CreateInstance("ProgramStatus.ProgramStatusType." + typeStr);
            
            form.Dock = DockStyle.Fill;
            form.TopLevel = false;
            //移除除菜单栏外的其他控件
            if (this.Controls.Count > 1)
            {
                this.Controls.RemoveAt(1);
            }

            this.Controls.Add(form);
            form.Show();
        }

        private void 出题态ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProgramStatusJump(ProgramStatusTypeEnum.TYPE_MakeOutQuestions);
        }

        private void 制作标准答案态ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProgramStatusJump(ProgramStatusTypeEnum.TYPE_MakeStandardAnswers);
        }

        private void 学生答题态ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProgramStatusJump(ProgramStatusTypeEnum.TYPE_StudentsAnswer);
        }

        private void 查阅态ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProgramStatusJump(ProgramStatusTypeEnum.TYPE_Query);
        }
    }
}
