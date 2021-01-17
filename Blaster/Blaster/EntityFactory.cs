using Blaster.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blaster
{
    public class EntityFactory
    {
        private World _world;
        private ContentManager _contentManager;

        public EntityFactory()
        { }
        public EntityFactory(World world, ContentManager contentManager)
        {
            _world = world;
            _contentManager = contentManager;
        }

        public void SetWorldAndContentManager(World w, ContentManager c) {_world = w; _contentManager = c;}

        public Entity CreatePlayer(Vector2 position, int netId, string name)
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
            entity.Attach(new NetElement() { Id = netId });
            entity.Attach(new Name { name = name });
            return entity;

        }

        public Entity CreateWall(Vector2 position)
        {
            var wallTexture = _contentManager.Load<Texture2D>("wall");
            var wallSprite = new Sprite(wallTexture);
            var entity = _world.CreateEntity();

            entity.Attach(wallSprite);
            entity.Attach(new Transform2(position, 0, Vector2.One * 4));

            return entity;
        }

        public Entity CreateText(Vector2 position, string text, int seconds)
        {
            var font = _contentManager.Load<SpriteFont>("Score");
            var entity = _world.CreateEntity();

            entity.Attach(new Transform2(position, 0, Vector2.One * 4));
            entity.Attach(new Text(text,font, seconds));
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
