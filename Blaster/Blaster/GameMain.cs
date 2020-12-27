using Autofac;
using Blaster.Systems;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Blaster
{
    public class GameMain : GameBase
    {
        private World world;
        private OrthographicCamera camera;
        private EntityFactory entityFactory;

        protected override void LoadContent()
        {
            camera = new OrthographicCamera(GraphicsDevice);
            world = new WorldBuilder()
                .AddSystem(new RenderSystem(new SpriteBatch(GraphicsDevice), camera))
                .Build();

            Components.Add(world);

            entityFactory = new EntityFactory(world, Content);

            entityFactory.CreatePlayer(new Vector2(100, 240));
        }
        protected override void RegisterDependencies(ContainerBuilder builder)
        {
            camera = new OrthographicCamera(GraphicsDevice);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }

    }
}
