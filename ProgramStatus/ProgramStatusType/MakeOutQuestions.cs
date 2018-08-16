using Canvas;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProgramStatus.ProgramStatusType
{
    public partial class MakeOutQuestions : Form
    {
        DocumentForm documentform;

        public MakeOutQuestions()
        {
            InitializeComponent();

            //DocumentForm documentForm = new DocumentForm(string.Empty);
            //documentForm.Dock = DockStyle.Fill;
            ////documentForm.FormBorderStyle = FormBorderStyle.None;
            //documentForm.TopLevel = false;
            //this.Controls.Add(documentForm);
            //documentForm.Show();

            MainWin mainWin = new MainWin();
            mainWin.ShowDialog();
            OpenDocumentWhileMainWinClosed(mainWin.filePathList);
            this.PerformLayout();
        }

        private void OpenDocumentWhileMainWinClosed(List<string> filePathList)
        {
            int listLength = filePathList.Count;
            int x = 20 * (listLength - 1);
            int y = 20 * (listLength - 1);
            for (int i = 0; i < listLength; i++)
            {
                documentform = new DocumentForm(filePathList[i]);
                documentform.TopLevel = false;
                documentform.SetDesktopLocation(x, y);
                //documentform.Dock = DockStyle.Fill;
                //documentform.FormBorderStyle = FormBorderStyle.None;
                this.Controls.Add(documentform);
                documentform.Show();
                x -= 20;
                y -= 20;
            }
        }
    }
}
