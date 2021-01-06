using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blaster.Components
{
    public class NetElement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public string ComponentType { get; set; }
    }
}
