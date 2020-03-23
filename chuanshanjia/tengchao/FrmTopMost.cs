using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static tengchao.PublicDefine;


namespace tengchao
{
    public partial class FrmTopMost : Form
    {
        //MainWind mw = new MainWind();
        public FrmTopMost()
        {
            InitializeComponent();
        }
        public FrmTopMost(MainWind main)
        {
            InitializeComponent();
            pParent = main;
        }
        private Point ptMouseCurrrnetPos, ptMouseNewPos, ptFormPos, ptFormNewPos;
        private void SwitchToMain()
        {
            pParent.RestoreWindow();
            this.Hide();
        }

        private void frmTopMost_MouseDoubleClick_1(object sender, MouseEventArgs e)
        {
            SwitchToMain();
        }

        private void frmTopMost_MouseUp_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                blnMouseDown = false;
        }

        private void frmTopMost_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                blnMouseDown = true;
                ptMouseCurrrnetPos = Control.MousePosition;
                ptFormPos = Location;
            }
        }
        private void frmTopMost_MouseMove(object sender, MouseEventArgs e)
        {
            if (blnMouseDown)
            {
                ptMouseNewPos = Control.MousePosition;
                ptFormNewPos.X = ptMouseNewPos.X - ptMouseCurrrnetPos.X + ptFormPos.X;
                ptFormNewPos.Y = ptMouseNewPos.Y - ptMouseCurrrnetPos.Y + ptFormPos.Y;
                Location = ptFormNewPos;
                ptFormPos = ptFormNewPos;
                ptMouseCurrrnetPos = ptMouseNewPos;
            }
        }

        private bool blnMouseDown = false;
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void check_main(object sender, EventArgs e)
        {
            SwitchToMain();
        }

        private MainWind pParent;
        private void frmTopMost_Load(object sender, EventArgs e)
        {
            this.Opacity =0.9;
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.Show();
            this.Top = 600;
            this.Left = Screen.PrimaryScreen.Bounds.Width - 1000;
            this.Width = 100;
            this.Height = 100;
        }
        /// <summary>
        ///  更新数据以及log记录
        /// </summary>
        public void update_hwind_txt() {
            // 更新统计数据
            this.textBox1.Text = OperateSql.CountsFromSqlite("true").ToString();
            this.textBox2.Text = OperateSql.CountsFromSqlite("false").ToString();
            this.textBox3.Text = OperateSql.GetRuChangNum("上传成功").ToString() + "/" + OperateSql.GetChuchangNum("上传成功").ToString();
            logg.Info("上传结果"+OperateSql.GetRuChangNum("上传成功").ToString() + "/" + OperateSql.GetChuchangNum("上传成功").ToString());
            try {
                // 更新主窗体页面log以及统计数据
                pParent.LoadTextboxEnd();
            }
            catch (Exception e) {
                logg.Info(e.ToString()+ "更新主窗体页面log以及统计数据");
            }
        }
    }
}
