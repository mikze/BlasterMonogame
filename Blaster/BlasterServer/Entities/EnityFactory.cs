using System;
using System.Collections.Generic;
using System.Text;

namespace BlasterServer.Entities
{
    public static class EnityFactory
    {
        static int _neutralId = 1000;
        static int NeutralEntityId => _neutralId++;
        public static Entity CreatePlayer(int id)
        {
            var position = new System.Numerics.Vector2(300, 300);
            var hitboxSize = new System.Numerics.Vector2(50, 60);
            return new PlayerEntity(id, position, new Collision.Hitbox(position, hitboxSize));
        }

        public static Entity CreateWall(float X, float Y)
        {
            var position = new System.Numerics.Vector2(X, Y);
            var hitboxSize = new System.Numerics.Vector2(53, 55);
            return new WallEntity(NeutralEntityId, position, new Collision.Hitbox(position, hitboxSize));
        }

        public static Entity CreateBomb(float X, float Y)
        {
            var position = new System.Numerics.Vector2(X, Y);
            var hitboxSize = new System.Numerics.Vector2(53, 55);
            return new BombEntity(NeutralEntityId, position, new Collision.Hitbox(position, hitboxSize));
        }
    }
}
