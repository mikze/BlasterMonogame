using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace Blaster.Components
{
    public class Text
    {
        public System.Timers.Timer timer;
        public bool Destroy = false;

        public Text(string text, SpriteFont font, int seconds)
        {
            this.text = text;
            this.font = font;
            if (seconds > 0)
            {
                timer = new System.Timers.Timer(seconds * 1000);
                timer.Elapsed += (object sender, ElapsedEventArgs e) => { Destroy = true; };
                timer.Start();
            }
        }
        public string text { get; set; }
        public SpriteFont font { get; set; }
    }
}
