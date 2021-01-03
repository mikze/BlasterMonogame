using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blaster.Components
{
    public class Text
    {
        public Text(string text, SpriteFont font)
        {
            this.text = text;
            this.font = font;
        }
        public string text { get; set; }
        public SpriteFont font { get; set; }
    }
}
