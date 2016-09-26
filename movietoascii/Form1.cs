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
        int symbolSize = 6;
		bool hq = false;

		Bitmap bmp;

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

			progressBar1.Value = 0;
        }


        private void btConvertClick(object sender, EventArgs e)
        {
            Bitmap resolutionBitmap = new Bitmap("Frames/0.bmp");
            writer.Open("new/video.mp4", resolutionBitmap.Width, resolutionBitmap.Height, 24, VideoCodec.MPEG4, 8000000);
            btGetFrames.Enabled = false;
            btConvert.Enabled = false;
            btAsciiCharacters.Enabled = false;
            txCharacters.Enabled = false;
			btAsciiCharacters.Enabled = false;

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
        }

        private void scanCharacters ()
        {
            listBox1.Items.Clear();
            frameNumber = 0;
            font = new Font("Arial", symbolSize - .5f);
            string[] asciiArray = txCharacters.Text.ToString().Split(" ".ToCharArray());
            symbolList = new List<Symbol>();

            for (int i = 0; i < asciiArray.Length; i++)
            {
                //string naar bitmap;
                Bitmap bmp = new Bitmap(symbolSize, symbolSize);
                Graphics g = Graphics.FromImage(bmp);
                string asciiCharacter = asciiArray[i];

                g.CompositingMode = CompositingMode.SourceCopy;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawString(asciiCharacter, font, new SolidBrush(Color.Green), 0, 0);

                decimal tb = 0;
                for (int w = 0; w < bmp.Width; w++)
                {
                    for (int h = 0; h < bmp.Height; h++)
                    {
                        decimal br = (decimal)bmp.GetPixel(w, h).GetBrightness();
                        tb += br;
                    }
                }

                decimal brightness = tb / (bmp.Width * bmp.Height);

                Symbol s = new Symbol(brightness, asciiCharacter, bmp);
                symbolList.Add(s);
            }
            
            symbolList = BubbleSort.Sort(symbolList);
            
            decimal previousValue = -1;
            for (int i = 0; i < symbolList.Count; i++)
            {
                if (Math.Abs(symbolList[i].Brightness - previousValue) <= .0001m)
                {
                    symbolList.RemoveAt(i);
                    i--;
                }
                else
                {
                    previousValue = symbolList[i].Brightness;
                }
            }

            foreach (Symbol symbol in symbolList)
            {
                listBox1.Items.Add(symbol.Character + " " + symbol.Brightness);
            }
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
				Bitmap bmp = new Bitmap(bm.Width, bm.Height);
                
                // Create graphics, maybe set lower quality?
                Graphics g = Graphics.FromImage(bmp);
                g.CompositingMode = CompositingMode.SourceCopy;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                for (int w = 1; w < bm.Width; w += symbolSize)
                {
                    for (int h = 1; h < bm.Height; h += symbolSize)
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
							if (hq)
							{
								SolidBrush brush = new SolidBrush(bm.GetPixel(w, h));
								RectangleF rectf = new RectangleF(w, h, symbolSize, symbolSize);
								g.DrawString(symbolList[result].Character, font, brush, rectf);
							}
							else
							{
								Image tempImage = ChangeColorOfImage(bm.GetPixel(w, h), symbolList[result].Image);
								g.DrawImage(tempImage, new PointF(w, h));
							}
                        }
                        else
                        {
							if (hq)
							{
								RectangleF rectf = new RectangleF(w, h, symbolSize, symbolSize);
								g.DrawString(symbolList[result].Character, font, Brushes.Green, rectf);
							}
							else
							{
								g.DrawImage(symbolList[result].Image, w, h);
							}
							
                        }
                    }
                }

                g.Dispose();

                // Save frame.
                // TODO: Option to save frames or video or both.
                // TODO: Option to pick where to save and load.
                bmp.Save("new/" + string.Format("{0:0000}", frameNumber) + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
				ShowImage(bmp);
				// writer.WriteVideoFrame(bmp);
				Console.WriteLine(frameNumber);
                
            }
            progressBar2.Invoke(((Action)(() => progressBar2.Value = (int)(((float)frameNumber++ / (float)frameCount) * 100))));
            
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

            return bmp;            
        }

		private void ShowImage(Image image) 
		{
			if (this.InvokeRequired)
			{
				// no, so call this method again but this
				// time use the UI thread!
				// the heavy-lifting for switching to the ui-thread
				// is done for you
				this.Invoke(new MethodInvoker(delegate { ShowImage(image); }));
			}
			// we are now for sure on the UI thread
			// so update the image
			this.pictureBox1.Image = image;
		}

        private void SetAsciiCharacters(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            txCharacters.Text = "";
            for (int i = 31; i < 255; i++)
            {
                txCharacters.Text += " " + (char)i;
            }
            scanCharacters();
        }

        private void btUpdateCharacters_Click(object sender, EventArgs e)
        {
            scanCharacters();
        }

		private void cbHQ_Checked_Changed(object sender, EventArgs e)
		{
			hq = ((CheckBox)sender).Checked;
		}
	}
}
