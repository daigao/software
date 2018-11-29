using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskUI线程调用
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            progressBar1.Maximum = 100;
            button1.Click += Button1_Click;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Task task = Task.Run(()=> {
                    //调用UI线程
                    progressBar1.Invoke(new Action(() =>
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            progressBar1.Value += 1;
                            Thread.Sleep(5);
                        }
                    }));
                
            });
        }
    }
}
