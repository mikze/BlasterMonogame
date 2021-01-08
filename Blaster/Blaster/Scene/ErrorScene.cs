using Blaster.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blaster.Scene
{
    internal class ErrorScene : Scene
    {
        World world;
        public override void LoadContent()
        {
            var camera = new OrthographicCamera(_sceneHandler._graphicsDevice);
            world = new WorldBuilder()
                .AddSystem(new RenderSystem(new SpriteBatch(_sceneHandler._graphicsDevice), camera))
                .Build();

            _sceneHandler._gameComponents.Add(world);

            var entityFactory = new EntityFactory(world, _sceneHandler._content);

            entityFactory.CreateText(new Vector2(200, 340), "Something went wrong... \n Press enter to back...", 0);
        }

        internal override void DrawScene(GameTime gameTime)
        {
            var keyboardState = KeyboardExtended.GetState();

            if (keyboardState.IsKeyDown(Keys.Enter))
            {
                world.Dispose();
                _sceneHandler.ChangeScene(new MenuScene());
            }
        }

        internal override void UpdateScene(GameTime gameTime)
        {
        }
    }
}
