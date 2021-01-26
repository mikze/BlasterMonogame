using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace BlasterServer.Collision
{
    public class Hitbox
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }

        public Hitbox(Vector2 Position, Vector2 Size)
        {
            this.Position = Position;
            this.Size = Size;
        }

        public bool IsCollision(Hitbox hitbox)
        {
            var x = false;
            var y = false;

            if (hitbox.Position.X >= Position.X && hitbox.Position.X <= Position.X + Size.X)
                x = true;

            if (hitbox.Position.X + hitbox.Size.X >= Position.X && hitbox.Position.X + hitbox.Size.X <= Position.X + Size.X)
                x = true;

            if (hitbox.Position.Y >= Position.Y && hitbox.Position.Y <= Position.Y + Size.Y)
                y = true;

            if (hitbox.Position.Y + hitbox.Size.Y >= Position.Y && hitbox.Position.Y + hitbox.Size.Y <= Position.Y + Size.Y)
                y = true;

            return x && y;
        }
    }
}
