using Blaster.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blaster.Scene
{
    internal class GameScene : Scene
    {
        private World world;
        private OrthographicCamera camera;
        private EntityFactory entityFactory;

        public GameScene() : base() 
        {
        }

        public override void LoadContent()
        {
            camera = new OrthographicCamera(_sceneHandler._graphicsDevice);
            world = new WorldBuilder()
                .AddSystem(new RenderSystem(new SpriteBatch(_sceneHandler._graphicsDevice), camera))
                .Build();

            _sceneHandler._gameComponents.Add(world);

            entityFactory = new EntityFactory(world, _sceneHandler._content);

            entityFactory.CreatePlayer(new Vector2(100, 240));
        }

        internal override void DrawGameScene()
        {
            LoadContent();
        }

        internal override void DrawMenuScene()
        {
            
        }
    }
}
