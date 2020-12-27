﻿using Blaster.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blaster
{
    public class EntityFactory
    {
        private readonly World _world;
        private readonly ContentManager _contentManager;

        public EntityFactory(World world, ContentManager contentManager)
        {
            _world = world;
            _contentManager = contentManager;
        }

        public Entity CreatePlayer(Vector2 position)
        {
            var dudeTexture = _contentManager.Load<Texture2D>("hero");
            var dudeAtlas = TextureAtlas.Create("dudeAtlas", dudeTexture, 16, 16);
            var entity = _world.CreateEntity();
            var spriteSheet = new SpriteSheet { TextureAtlas = dudeAtlas };

            AddAnimationCycle(spriteSheet, "idle", new[] { 0, 1, 2, 1 });
            AddAnimationCycle(spriteSheet, "walk", new[] { 6, 7, 8, 9, 10, 11 });
            entity.Attach(new AnimatedSprite(spriteSheet, "idle"));
            entity.Attach(new Transform2(position, 0, Vector2.One * 4));
            entity.Attach(new Player());
            return entity;

        }

        private void AddAnimationCycle(SpriteSheet spriteSheet, string name, int[] frames, bool isLooping = true, float frameDuration = 0.1f)
        {
            var cycle = new SpriteSheetAnimationCycle();
            foreach (var f in frames)
            {
                cycle.Frames.Add(new SpriteSheetAnimationFrame(f, frameDuration));
            }

            cycle.IsLooping = isLooping;

            spriteSheet.Cycles.Add(name, cycle);
        }
    }
}