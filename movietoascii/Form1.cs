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

        public ASCIIConverter()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            font = new Font("Arial", 6f);

            string[] asciiArray = textBox1.Text.ToString().Split(" ".ToCharArray());

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
                pictureBox1.Image = bmp;

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
                bmp.Save("symbols/" + asciiArray[i] + ".bmp");
                //karakters naar array
                //vergelijk pixel in frame met karakter lijst
                //vervang door juiste karakter
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
            // Reset progress bar.
            progressBar1.Value = 0;
            // Might save some time by saving this value but it shouldn't matter (saves a few ms total).
            VideoFileReader reader = GetReader();
            int frameCount = (int)reader.FrameCount;
            reader.Close();
            // Recursion woo
            Convert(1133, start, frameCount);

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

        private void Convert(int frameNumber, DateTime start, int frameCount)
        {
            // If next file doesn't exist.
            if(!File.Exists(@"Frames\" + frameNumber + ".bmp"))
            {
                // We're done here
                TimeSpan duration = DateTime.Now - start;
                Console.WriteLine("Einde datastroom: frame " + frameNumber + ", totale tijd: " + duration.TotalMilliseconds);
                return;
            }

            // Create bitmaps
            Bitmap bm = new Bitmap("Frames/" + frameNumber + ".bmp");
            Bitmap bmp = new Bitmap(bm.Width, bm.Height);

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

            // Save frame.
            bmp.Save("new/" + String.Format("{0:0000}", frameNumber) + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            bm.Dispose();
            bmp.Dispose();

            Convert(++frameNumber, start, frameCount);
        }
    }
}
