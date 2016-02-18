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

namespace movietoascii
{
    public partial class ASCIIConverter : Form
    {
        List<decimal> characterBrightnessList;
        List<string> characterList;
        Font font;
        VideoFileWriter writer = new VideoFileWriter();
        int frameNumber;

        public ASCIIConverter()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            frameNumber = 0;
            writer.Open("new/video.mp4", 1920, 1080, 24, VideoCodec.MPEG4);

            listBox1.Items.Clear();
            font = new Font("Arial", 6f);

            textBox1.Text = "";
            for (int i = 33; i < 255; i++)
            {
                textBox1.Text += " " + (char)i;
            }

            string[] asciiArray = textBox1.Text.ToString().Split(" ".ToCharArray());

            for (int i = 0; i < asciiArray.Length; i++)
            {
                //string naar bitmap;
                using (Bitmap bmp = new Bitmap(6, 6))
                {
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
                            tb += (decimal)bmp.GetPixel(w, h).GetBrightness();
                        }
                    }

                    decimal er = tb / 36;
                    listBox1.Items.Add(asciiArray[i] + " " + er);
                }
            }

            List<decimal> lis = new List<decimal>();
            List<string> pis = new List<string>();

            for (int t = 0; t < listBox1.Items.Count; t++)
            {
                lis.Add(decimal.Parse(listBox1.Items[t].ToString().Split(" ".ToCharArray())[1]));
                pis.Add(listBox1.Items[t].ToString().Split(" ".ToCharArray())[0]);
            }

            characterBrightnessList = lis;
            characterList = pis;

            characterBrightnessList = BubbleSort.Sort(characterBrightnessList, characterList);
            characterList = BubbleSort.GetSortedSecondaryList();
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
                progressBar1.Value += 1;
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

            // Save time to compare for speed.
            DateTime start = DateTime.Now;
            // Recursion woo
            Convert(start);

            button1.Enabled = true;
            btConvert.Enabled = true;
        }

        private VideoFileReader GetReader()
        {
            VideoFileReader reader = new VideoFileReader();
            // open video file
            reader.Open("movie.mp4");
            return reader;
        }

        private void Convert(DateTime start)
        {
            // If next file doesn't exist.
            if(!File.Exists(@"Frames\" + frameNumber + ".bmp"))
            {
                // We're done here
                TimeSpan duration = DateTime.Now - start;
                Console.WriteLine("Einde datastroom: frame " + frameNumber + ", totale tijd: " + duration.TotalMilliseconds);
                writer.Close();
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

                            decimal fb = (decimal)bm.GetPixel(w, h).GetBrightness();

                            decimal distance = decimal.MaxValue;
                            decimal previousDistance = distance;

                            // Determine which character to use
                            for (int b = 0; b < characterBrightnessList.Count; b++)
                            {
                                decimal currentDistance = Math.Abs(characterBrightnessList[b] - fb);

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

                            // Draw character
                            RectangleF rectf = new RectangleF(w, h, 6, 6);
                            g.DrawString(characterList[result].ToString(), font, Brushes.Green, rectf);
                        }
                    }

                    g.Dispose();

                    // Save frame.
                    bmp.Save("new/" + string.Format("{0:0000}", frameNumber) + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                    // TODO: Split video up in fragments
                    writer.WriteVideoFrame(bmp);
                }
            }

            frameNumber++;
            
            Convert(start);
        }
    }
}
