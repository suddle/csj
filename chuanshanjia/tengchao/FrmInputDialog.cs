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
    public partial class FrmInputDialog : Form
    {
        TimeSpan dtTo = new TimeSpan(0, 50, 00);//倒计时声明
        public FrmInputDialog()
        {
            InitializeComponent();
        }
        public delegate void TextEventHandler(string strText);

        public TextEventHandler TextHandler;

        private void SureLoad(object sender, EventArgs e)
        {
            this.pictureBox1.BackgroundImage = Image.FromFile(GlobalPicapth);//获取图片路径并且显示
            timer1.Interval = 1000;//设置每次间隔1s//设置倒计时间隔
            timer1.Enabled = true;
        }
        private void SureClick(object sender, EventArgs e)
        {
            string name_id = this.textBox1.Text;
            string name_type = this.textBox2.Text;
            string name = this.textBox3.Text;
            if (this.textBox1.Text.Length == 0) {
                CommonFunc.tips("请确认是否输入修理人员id");
            }
            if (this.textBox2.Text.Length == 0)
            {
                CommonFunc.tips("请确认是否输入修理人员类型");
            }
            if (this.textBox3.Text.Length == 0)
            {
                CommonFunc.tips("请确认是否输入修理人员姓名");
            }
            int repaircount = InfoSql.SearchRepairMsg(name_id);
            if (repaircount > 0)
            {
                OperateSql.ChangeRepairMsg("update", name_id, name_type, name);//如果是存在的id  那么就更新
            }
            else
            {
                OperateSql.ChangeRepairMsg("insert", name_id, name_type, name);//如果是不存在的id  那么就插入
            }
            this.Hide();
            this.Dispose();
        }

        private void CloseClick(object sender, EventArgs e)//关闭事件
        {
            this.Hide();
            this.Dispose();
        }

        private void Timer1Tick(object sender, EventArgs e)//倒计时
        {
            dtTo = dtTo.Subtract(new TimeSpan(0, 0, 1));
            if (dtTo.TotalSeconds < 0.0)//当倒计时完毕
            {
                timer1.Enabled = false;   //其中可自行添加相应的提示框或者方法函数
                this.Hide();
                this.Dispose();
            }
        }
    }
}
