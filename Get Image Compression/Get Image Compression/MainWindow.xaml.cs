using System;
using System.Windows;
using Microsoft.Win32;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Get_Image_Compression
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        static string[] fileNames = new string[1];
        public MainWindow()
        {
            InitializeComponent();
            btnSelectImages.Click += BtnSelectImages_Click;
            btnStartToImageCompression.Click += BtnStartToImageCompression_Click;//加载按钮
            btnStartToImageCompression.IsEnabled = false;
        }

        private void BtnSelectImages_Click(object sender, RoutedEventArgs e)
        {
            
            //打开文件夹对话框
            OpenFileDialog open = new OpenFileDialog();
            open.Multiselect = true;
            open.Title = "选择图片文件";
            open.Filter = "图片|*.jpg;*.png;*.gif;*bmp";
            if (open.ShowDialog() == true)
            {
                fileNames = open.FileNames;
                tbxDirFile.Text = fileNames[0];
            }
            pbarGo.Value = 0;
            pbarGo.Maximum = fileNames.Length;
            btnStartToImageCompression.IsEnabled = true;
        }

        private void BtnStartToImageCompression_Click(object sender, RoutedEventArgs e)
        {
            lbState.Content = "开始压缩";
            var task = new List<Task<bool>>();

            foreach (var item in fileNames)
            {
                task.Add(Task.Run(() => GetPicThumbnail(item)));
            }
        }
        internal bool GetPicThumbnail(Object sFile)
        {
            bool rv=false;
            int iWidth = 0;
            int iHeight = 0;
            var fileName = System.IO.Path.GetFileName(sFile.ToString());
            var dFile = System.IO.Path.GetDirectoryName(sFile.ToString()) + @"\compImage\";
            System.IO.Directory.CreateDirectory(dFile);
            dFile = dFile + fileName;
            using (System.Drawing.Image iSize=System.Drawing.Image.FromFile(sFile.ToString()))
            {
                iWidth = iSize.Width;
                iHeight = iSize.Height;
            }
            if (System.IO.Path.GetExtension(fileName)!=".gif")
            {
                rv = new LibImageCompression.GetImageCompression().GetPicThumbnail(sFile.ToString(), dFile, iHeight, iWidth, 90);
            }
            if (System.IO.Path.GetExtension(fileName) == ".gif")
            {
                rv = new LibImageCompression.GetGifCompression().GetGifThumbnail(sFile.ToString(), dFile, iHeight, iWidth, 90);
            }
            //Application.Current.Dispatcher.Invoke(new Action(() =>
            //{
            //    pbarGo.Value += 1;
            //}));
            this.Dispatcher.Invoke(new Action(() =>
            {
                pbarGo.Value += 1;
                if (pbarGo.Value==pbarGo.Maximum)
                {
                    lbState.Content = "已完成！";
                    pbarGo.Value = 0;
                }
            }));
            return rv;
        }
        delegate void mydelegate(int i);
        public void PbarGo(int i)
        {
            this.pbarGo.Value += i;
        }

    }
}
