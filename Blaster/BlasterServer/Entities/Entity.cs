using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace BlasterServer.Entities
{
    public class Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public string ComponentType { get; set; }

        public Entity(int id, Vector2 position, string componentType)
        {
            Id = id;
            Position = position;
            ComponentType = componentType;
        }
    }
}
