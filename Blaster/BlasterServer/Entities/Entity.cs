using BlasterServer.Collision;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace BlasterServer.Entities
{
    public abstract class Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        Vector2 _position;
        public Vector2 Position { get => _position; set => Hitbox.Position = _position = value; }
        public Hitbox Hitbox { get; set; }

        public Entity(int id, Vector2 position, Hitbox Hitbox)
        {
            this.Hitbox = Hitbox;
            Id = id;
            Position = position;      
        }
    }
}
