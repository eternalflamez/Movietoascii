using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge;
using AForge.Video.FFMPEG;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading;

namespace movietoascii
{
    public partial class ASCIIConverter : Form
    {
        List<Symbol> symbolList;
        Font font;
        VideoFileWriter writer = new VideoFileWriter();
        int frameNumber;
        bool inColor = false;
        DateTime start;
        int frameCount = 0;
        Thread convertThread;

        public ASCIIConverter()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            frameNumber = 0;
            writer.Open("new/video.mp4", 1920, 1080, 24, VideoCodec.MPEG4, 8000000);

            listBox1.Items.Clear();
            font = new Font("Arial", 6f);

            textBox1.Text = "";
            for (int i = 65; i < 255; i++)
            {
                textBox1.Text += " " + (char)i;
            }

            string[] asciiArray = textBox1.Text.ToString().Split(" ".ToCharArray());
            symbolList = new List<Symbol>();

            for (int i = 0; i < asciiArray.Length; i++)
            {
                //string naar bitmap;
                Bitmap bmp = new Bitmap(6, 6);
                Graphics g = Graphics.FromImage(bmp);
                string randomString = asciiArray[i];
                Font myFont = new Font("Arial", 5.5f);
                PointF rect = new PointF(0, 0);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawString(randomString, myFont, new SolidBrush(Color.White), rect);

                decimal tb = 0;
                for (int w = 0; w < 6; w++)
                {
                    for (int h = 0; h < 6; h++)
                    {
                        decimal br = (decimal)bmp.GetPixel(w, h).GetBrightness();
                        tb += br;

                        if(br != 0)
                        {
                            bmp.SetPixel(w, h, Color.Green);
                        }
                    }
                }

                decimal brightness = tb / 36;
                string character = asciiArray[i];

                Symbol s = new Symbol(brightness, character, bmp);
                symbolList.Add(s);

                listBox1.Items.Add(asciiArray[i] + " " + brightness);
            }
            
            symbolList = BubbleSort.Sort(symbolList);
            
            decimal previousValue = -1;
            for (int i = 0; i < symbolList.Count; i++)
            {
                if (symbolList[i].Brightness == previousValue)
                {
                    symbolList.RemoveAt(i);
                    i--;
                }
                else
                {
                    previousValue = symbolList[i].Brightness;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            btConvert.Enabled = false;

            progressBar1.Value = 0;

            VideoFileReader reader = GetReader();
            for (int i = 0; i < reader.FrameCount; i++)
            {
                Bitmap videoFrame = reader.ReadVideoFrame();
                videoFrame.Save("Frames/" + i + ".bmp");
                videoFrame.Dispose();

                progressBar1.Value = (int) (((float)i / (float)reader.FrameCount) * 100);
            }
            reader.Close();

            button1.Enabled = true;
            btConvert.Enabled = true;
        }


        private void btConvertClick(object sender, EventArgs e)
        {
            button1.Enabled = false;
            btConvert.Enabled = false;

            if (checkBox1.Checked == true)
            {
                inColor = true;
            }
            else
            {
                inColor = false;
            }

            // Save time to compare for speed.
            start = DateTime.Now;
            frameCount = Directory.GetFiles("Frames\\").Length - 2;
            convertThread = new Thread(new ThreadStart(Convert));
            convertThread.Start();
            // Recursion woo
        }

        private VideoFileReader GetReader()
        {
            VideoFileReader reader = new VideoFileReader();
            // open video file
            reader.Open("movie.mp4");
            return reader;
        }

        private void Convert()
        {
            // If next file doesn't exist.
            if(!File.Exists(@"Frames\" + frameNumber + ".bmp"))
            {
                // We're done here
                TimeSpan duration = DateTime.Now - start;
                Console.WriteLine("Einde datastroom: frame " + frameNumber + ", totale tijd: " + duration.TotalMilliseconds);
                writer.Close();

                button1.Invoke(((Action)(() => button1.Enabled = true)));
                btConvert.Invoke(((Action)(() => btConvert.Enabled = true)));
                return;
            }
            
            using (Bitmap bm = new Bitmap("Frames/" + frameNumber + ".bmp"))
            {
                using (Bitmap bmp = new Bitmap(bm.Width, bm.Height))
                {
                    // Create graphics, maybe set lower quality?
                    Graphics g = Graphics.FromImage(bmp);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    for (int w = 1; w < bm.Width; w += 6)
                    {
                        for (int h = 1; h < bm.Height; h += 6)
                        {
                            int result = 0;

                            // Get brightness and color for generating to ASCII

                            decimal fb = (decimal)bm.GetPixel(w, h).GetBrightness();
                            
                            decimal distance = decimal.MaxValue;
                            decimal previousDistance = distance;

                            // Determine which character to use
                            for (int b = 0; b < symbolList.Count; b++)
                            {
                                decimal currentDistance = Math.Abs(symbolList[b].Brightness - fb);

                                if (currentDistance > previousDistance)
                                {
                                    // Because the list is sorted, moving away from our point would mean all upcoming values will fail the search anyway.
                                    break;
                                }

                                previousDistance = currentDistance;

                                if (currentDistance < distance)
                                {
                                    distance = currentDistance;
                                    result = b;
                                }
                            }
                            
                            if (inColor == true)
                            {
                                SolidBrush brush = new SolidBrush(bm.GetPixel(w, h));
                                RectangleF rectf = new RectangleF(w, h, 6, 6);
                                g.DrawString(symbolList[result].Character.ToString(), font, brush, rectf);

                                // TODO: Faster recoloring?! DrawImage is 30x faster but changecolor is super slow.
                                // Image tempImage = ChangeColorOfImage(bm.GetPixel(w, h), symbolList[result].Image);
                                // g.DrawImage(tempImage, new PointF(w, h));
                            }
                            else
                            {
                                g.DrawImage(symbolList[result].Image, new PointF(w, h));
                            }
                        }
                    }

                    g.Dispose();

                    // Save frame.
                    bmp.Save("new/" + string.Format("{0:0000}", frameNumber) + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                    // TODO: Split video up in fragments
                    writer.WriteVideoFrame(bmp);// Errors when using a non full hd bmp
                    Console.WriteLine(frameNumber);
                }
            }

            frameNumber++;
            progressBar2.Invoke(((Action)(() => progressBar2.Value = (int)(((float)frameNumber / (float)frameCount) * 100))));
            
            Convert();
        }

        private void ASCIIConverter_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Close thread on formclosing
            if (convertThread != null)
            {
                convertThread.Abort();
            }
        }

        private Image ChangeColorOfImage(Color color, Image image)
        {
            Bitmap bmp = new Bitmap(image);

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    if (bmp.GetPixel(i, j).A >= 150)
                    {
                        bmp.SetPixel(i, j, color);
                    }
                }
            }

            return (Image) bmp;
        }
    }
}
