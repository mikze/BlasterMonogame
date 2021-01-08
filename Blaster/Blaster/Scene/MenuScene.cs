using Blaster.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Input;

namespace Blaster.Scene
{
    internal class MenuScene : Scene
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

            entityFactory.CreateText(new Vector2(200, 340), "Press enter to start game...", 0);
        }

        internal override void DrawScene(GameTime gameTime)
        {
            var keyboardState = KeyboardExtended.GetState();

            if (keyboardState.IsKeyDown(Keys.Enter))
            {
                world.Dispose();
                _sceneHandler.ChangeScene(new GameScene());
            }
        }

        internal override void UpdateScene(GameTime gameTime)
        {
        }
    }
}
