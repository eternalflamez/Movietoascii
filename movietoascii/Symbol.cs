using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace movietoascii
{
    public class Symbol
    {
        private decimal brightness;
        private string character;
        private Image image;
        
        public decimal Brightness
        {
            get { return brightness; }
        }

        public string Character
        {
            get { return character; }
        }

        public Image Image
        {
            get { return image; }
        }

        public Symbol(decimal brightness, string character, Image image)
        {
            this.brightness = brightness;
            this.character = character;
            this.image = image;
        }

    }
}
