using Blaster.Components;
using Blaster.Network;
using Blaster.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using static Blaster.Network.NetworkHelper;

namespace Blaster.Scene
{
    internal class GameScene : Scene
    {
        World world;
        EntityFactory entityFactory;
        public GameScene() : base() 
        {
            BlasterClient.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6666));
        }

        public override void LoadContent()
        {
            var camera = new OrthographicCamera(_sceneHandler._graphicsDevice);

            entityFactory = new EntityFactory();

            world = new WorldBuilder()
                .AddSystem(new RenderSystem(new SpriteBatch(_sceneHandler._graphicsDevice), camera))
                .AddSystem(new NetElementSystem(entityFactory))
                .Build();

           _sceneHandler._gameComponents.Add(world);

            entityFactory.SetWorldAndContentManager(world, _sceneHandler._content);

            var entitiesToBuild = DownloadElementsFromServer();

            foreach(var e in entitiesToBuild)
            {
                entityFactory.CreatePlayer(e.Position, e.Id);
            }
        }

        internal override void DrawScene()
        {
            var keyboardState = KeyboardExtended.GetState();

            if (keyboardState.IsKeyDown(Keys.Space))
            {
                world.Dispose();
                BlasterClient.Disconnect();
                _sceneHandler.ChangeScene(new MenuScene());
            }
            SendMovement(keyboardState);
        }

        internal void SendMovement(KeyboardStateExtended keyState)
        {
            if (keyState.IsKeyDown(Keys.Right))
                BlasterClient.Send(new Frame() { id = 1, FrameKind = (int)FrameKind.movement, body = "right" });

            if (keyState.IsKeyDown(Keys.Left))
                BlasterClient.Send(new Frame() { id = 1, FrameKind = (int)FrameKind.movement, body = "left" });

            if (keyState.IsKeyDown(Keys.Up))
                BlasterClient.Send(new Frame() { id = 1, FrameKind = (int)FrameKind.movement, body = "up" });

            if (keyState.IsKeyDown(Keys.Down))
                BlasterClient.Send(new Frame() { id = 1, FrameKind = (int)FrameKind.movement, body = "down" });
        }
     
    }
}
