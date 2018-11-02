using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace LibImageCompression
{
    /// <summary>
    /// 压缩Gif
    /// </summary>
    public class GetGifCompression
    {
        #region GetGifThumbnail
        /// <summary>
        /// 无损压缩GIF图片
        /// </summary>
        /// <param name="sFile">原图片</param>
        /// <param name="dFile">压缩后保存位置</param>
        /// <param name="dHeight">高度</param>
        /// <param name="dWidth">宽度</param>
        /// <param name="flag">压缩质量 1-100(参数废弃不用)</param>
        /// <returns>bool</returns>
        public bool GetGifThumbnail(string sFile, string dFile, int dHeight, int dwidth, int flag)
        {
            Image img = Image.FromFile(sFile);

            //新图第一帧
            Image new_img = new Bitmap(dwidth, dHeight);
            //新图其他帧
            Image new_imgs = new Bitmap(dwidth, dHeight);
            //新图第一帧GDI+绘图对象
            Graphics g_new_img = Graphics.FromImage(new_img);
            //新图其他帧GDI+绘图对象
            Graphics g_new_imgs = Graphics.FromImage(new_imgs);
            //配置新图第一帧GDI+绘图对象
            g_new_img.CompositingMode = CompositingMode.SourceCopy;
            g_new_img.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g_new_img.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g_new_img.SmoothingMode = SmoothingMode.HighQuality;
            g_new_img.Clear(Color.FromKnownColor(KnownColor.Transparent));
            //配置其他帧GDI+绘图对象
            g_new_imgs.CompositingMode = CompositingMode.SourceCopy;
            g_new_imgs.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g_new_imgs.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g_new_imgs.SmoothingMode = SmoothingMode.HighQuality;
            g_new_imgs.Clear(Color.FromKnownColor(KnownColor.Transparent));
            try
            {
                //遍历维数
                foreach (Guid guid in img.FrameDimensionsList)
                {
                    //因为是缩小GIF文件所以这里要设置Time
                    //如果是TFTT文件这里要设置为PAGE
                    FrameDimension f = FrameDimension.Time;
                    //获取总帧数
                    int count = img.GetFrameCount(f);
                    //保存标识参数
                    Encoder encoder = Encoder.SaveFlag;
                    //
                    EncoderParameters ep = new EncoderParameters();
                    long[] py = new long[1];
                    py[0] = flag;//设置压缩的比例1-100
                    EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, py);
                    
                    //图像编码、解码器
                    ImageCodecInfo ici = null;
                    //图片编码、解码器合集
                    ImageCodecInfo[] icis = ImageCodecInfo.GetImageDecoders();
                    //为图片编码、解码器 对象赋值
                    foreach (ImageCodecInfo ic in icis)
                    {
                        if (ic.FormatID == ImageFormat.Gif.Guid)
                        {
                            ici = ic;
                            break;
                        }
                    }
                    for (int i = 0; i < count; i++)
                    {
                        //选择由维度索引指定的帧
                        img.SelectActiveFrame(f, i);
                        //第一帧
                        if (i == 0)
                        {
                            //将原图第一帧画给新图第一帧
                            g_new_img.DrawImage(img, new Rectangle(0, 0, dwidth, dHeight), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                            //把振频和透明背景调色板等设置复制给新图第一帧
                            for (int j = 0; j < img.PropertyItems.Length; j++)
                            {
                                new_img.SetPropertyItem(img.PropertyItems[j]);
                            }

                            //第一帧需要设置为MultiFrame
                            ep.Param[0] = eParam;
                            //保存第一帧
                            new_img.Save(dFile, ici, ep);
                        }
                        //其他帧
                        else
                        {
                            //把原图的其他帧画给新图的其他帧
                            g_new_imgs.DrawImage(img, new Rectangle(0, 0, dwidth, dHeight), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                            //把振频和透明背景调色板等设置复制给新图第一帧
                            for (int j = 0; j < img.PropertyItems.Length; j++)
                            {
                                new_imgs.SetPropertyItem(img.PropertyItems[j]);
                            }
                            //如果是GIF这里设置为FrameDimensionTime
                            //如果为TIFF则设置为FrameDimensionPage
                            ep.Param[0] = eParam;
                            //向新图添加一帧
                            new_img.SaveAdd(new_imgs, ep);
                        }
                    }
                    
                    //关闭多帧文件流
                    ep.Param[0] = eParam;
                    new_img.SaveAdd(ep);
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }
            finally
            {
                img.Dispose();
                new_img.Dispose();
                new_imgs.Dispose();
                g_new_img.Dispose();
                g_new_imgs.Dispose();
            }
        }

        #endregion
    }
}
