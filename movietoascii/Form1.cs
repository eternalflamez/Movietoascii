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
        List<decimal> characterBrightnessList;
        List<string> characterList;
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
            scanCharacters();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            btGetFrames.Enabled = false;
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

            btGetFrames.Enabled = true;
            btConvert.Enabled = true;
        }


        private void btConvertClick(object sender, EventArgs e)
        {
            Bitmap resolutionBitmap = new Bitmap("Frames/0.bmp");
            writer.Open("new/video.mp4", resolutionBitmap.Width, resolutionBitmap.Height, 24, VideoCodec.MPEG4, 8000000);
            btGetFrames.Enabled = false;
            btConvert.Enabled = false;
            btAsciiCharacters.Enabled = false;
            txCharacters.Enabled = false;

            if (chInColor.Checked == true)
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

        private void scanCharacters ()
        {
            listBox1.Items.Clear();
            frameNumber = 0;
            font = new Font("Arial", 5f);
            string[] asciiArray = txCharacters.Text.ToString().Split(" ".ToCharArray());

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

                btGetFrames.Invoke(((Action)(() => btGetFrames.Enabled = true)));
                btConvert.Invoke(((Action)(() => btConvert.Enabled = true)));
                btAsciiCharacters.Invoke(((Action)(() => btAsciiCharacters.Enabled = true)));
                txCharacters.Invoke(((Action)(() => txCharacters.Enabled = true)));
                progressBar2.Invoke(((Action)(() => progressBar2.Value = 0)));
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
                            SolidBrush brush = new SolidBrush(Color.FromArgb(bm.GetPixel(w, h).ToArgb()));

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

                            if (inColor == true)
                            {
                                g.DrawString(characterList[result].ToString(), font, brush, rectf);
                            }
                            else
                            {
                                g.DrawString(characterList[result].ToString(), font, new SolidBrush(Color.Green), rectf);
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
            try
            {
                convertThread.Abort();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.Write("Error: " + ex);
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            txCharacters.Text = "";
            for (int i = 33; i < 255; i++)
            {
                txCharacters.Text += " " + (char)i;
            }
            scanCharacters();
        }

        private void btUpdateCharacters_Click(object sender, EventArgs e)
        {
            scanCharacters();
        }
    }
}
