using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing.Imaging;
using System.Drawing;
using System.Threading;

namespace ImageCompressTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Compresslevel_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || e.Key == Key.Back || e.Key == Key.Delete || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
            {

            }
            else
                e.Handled = true;
        }
        System.Windows.Forms.OpenFileDialog file;
        private void ChooseFile_Click(object sender, RoutedEventArgs e)
        {
            file = new System.Windows.Forms.OpenFileDialog();
            file.Filter = "Jpeg|*.jpg|Jpeg|*.jpeg|PNG|*.png";
            file.Multiselect = true;
            if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (file.FileNames.Length > 1)
                {
                    filepath.Text = "包含" + file.FileNames.Length + "个文件\n";
                    foreach (string i in file.FileNames)
                        filepath.Text += i + "\n";
                }
                else
                    filepath.Text = file.FileName;
            }
        }

        private void Compress_Click(object sender, RoutedEventArgs e)
        {
            if (file != null && compresslevel.Text.Trim() != "")
            {
                if (int.Parse(compresslevel.Text) <= 100 && int.Parse(compresslevel.Text) >= 0)
                {
                    if (file.FileNames.Length > 1)
                    {
                        this.Dispatcher.Invoke(new Action(() => { compressPro.Visibility = Visibility.Visible; }));
                        compressPro.Maximum = file.FileNames.Length;
                        for (int i = 0; i < file.FileNames.Length; i++)
                        {                            
                            CompressImg(file.FileNames[i], i);
                            ProgressValue(i);
                        }                        
                    }
                    else
                    {
                        CompressImg(file.FileName,-1);
                    }
                    MessageBox.Show("保存成功");
                    compressPro.Value = 0;
                    compressPro.Visibility = Visibility.Hidden;
                }
                else
                    MessageBox.Show("压缩率不符合规范！");
            }
            else
                MessageBox.Show("为选择文件或者未输入压缩率！");
        }
        private void CompressImg(string path,int index)
        {
            using (Bitmap img = new Bitmap(path))
            {
                ImageCodecInfo imagecodeinfo=GetEncoder(ImageFormat.Jpeg);
                if (img.RawFormat == ImageFormat.Jpeg)
                {
                    imagecodeinfo = GetEncoder(ImageFormat.Jpeg);
                }
                else if(img.RawFormat==ImageFormat.Png)
                {
                    imagecodeinfo = GetEncoder(ImageFormat.Png);
                }
                System.Drawing.Imaging.Encoder myencoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters ep = new EncoderParameters(1);
                EncoderParameter temp = new EncoderParameter(myencoder,long.Parse(compresslevel.Text));
                ep.Param[0] = temp;
                if (index >= 0)
                {
                    img.Save(path + file.SafeFileNames[index], imagecodeinfo, ep);
                }
                else
                    img.Save(path + file.SafeFileName, imagecodeinfo, ep);
            }
        }
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                    return codec;
            }
            return null;
        }
        private void ProgressValue(int a)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                compressPro.Value = a + 1;
            }),System.Windows.Threading.DispatcherPriority.Background);
        }
    }
}
