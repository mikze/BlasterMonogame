using BlasterServer.Collision;
using BlasterServer.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace BlasterServer.Entities
{
    public class WallEntity : Entity, IWallEntity
    {
        public WallEntity(int id, Vector2 position, Hitbox Hitbox) : base(id, position, Hitbox)
        {
        }
    }
}
