using Blaster.Components;
using Blaster.Network;
using Blaster.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Input;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using static Blaster.Network.NetworkHelper;

namespace Blaster.Scene
{
    internal class GameScene : Scene
    {
        World world;
        EntityFactory entityFactory;
        GuiSystem _guiSystem;
        TextBox chatText;
        string _playerName;
        bool broadcastState = false;

        public GameScene(string playerName) : base()
        {
            _playerName = playerName;
            BlasterClient.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6666)); //54.37.139.37
        }

        public override void LoadContent()
        {
            try
            {            
                var camera = new OrthographicCamera(_sceneHandler._graphicsDevice);

                entityFactory = new EntityFactory();

                world = new WorldBuilder()
                    .AddSystem(new RenderSystem(new SpriteBatch(_sceneHandler._graphicsDevice), camera, _sceneHandler._content))
                    .AddSystem(new NetElementSystem(entityFactory))
                    .AddSystem(new PlayerSystem())
                    .Build();

                _sceneHandler._gameComponents.Add(world);

                entityFactory.SetWorldAndContentManager(world, _sceneHandler._content);
                PlayerConnect(_playerName);
                var entitiesToBuild = DownloadElementsFromServer();
                

                foreach (var e in entitiesToBuild)
                {
                    if(e.ComponentType == "player")
                        entityFactory.CreatePlayer(e.Position, e.Id, e.Name);
                    if (e.ComponentType == "wall")
                        entityFactory.CreateWall(e.Position);
                }

                LoadGui();
            }
            catch(Exception e)
            {
                BlasterClient.Disconnect();
                _sceneHandler.ChangeScene(new ErrorScene());
            }
        }

        internal override void DrawScene(GameTime gameTime)
        {
            _guiSystem.Draw(gameTime);
            var keyboardState = KeyboardExtended.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                world.Dispose();
                BlasterClient.Disconnect();
                _sceneHandler.ChangeScene(new MenuScene());
            }
            if (keyboardState.IsKeyDown(Keys.Enter))
            {
                BlasterClient.Send(new Frame() { id = 1, FrameKind = (int)FrameKind.chat, body = chatText.Text });
            }
            SendMovement(keyboardState);
        }

        internal override void UpdateScene(GameTime gameTime)
        {
            _guiSystem.Update(gameTime);
        }

        internal void SendMovement(KeyboardStateExtended keyState)
        {
            if (keyState.IsKeyDown(Keys.Right))
            {
                broadcastState = true;
                BlasterClient.Send(new Frame() { id = 1, FrameKind = (int)FrameKind.movement, body = "right" });
            }

            if (keyState.IsKeyDown(Keys.Left))
            {
                broadcastState = true;
                BlasterClient.Send(new Frame() { id = 1, FrameKind = (int)FrameKind.movement, body = "left" });
            }

            if (keyState.IsKeyDown(Keys.Up))
            {
                broadcastState = true;
                BlasterClient.Send(new Frame() { id = 1, FrameKind = (int)FrameKind.movement, body = "up" });
            }

            if (keyState.IsKeyDown(Keys.Down))
            {
                broadcastState = true;
                BlasterClient.Send(new Frame() { id = 1, FrameKind = (int)FrameKind.movement, body = "down" });
            }

            if (broadcastState && !keyState.IsKeyDown(Keys.Right) && !keyState.IsKeyDown(Keys.Left) && !keyState.IsKeyDown(Keys.Up) && !keyState.IsKeyDown(Keys.Down))
            {
                broadcastState = false;

                BlasterClient.Send(new Frame() { id = 1, FrameKind = (int)FrameKind.setPlayerState, body = "idle" });
            }
        }

        void LoadGui()
        {
            var viewportAdapter = new DefaultViewportAdapter(_sceneHandler._graphicsDevice);
            var guiRenderer = new GuiSpriteBatchRenderer(_sceneHandler._graphicsDevice, () => Matrix.Identity);
            var font = _sceneHandler._content.Load<BitmapFont>("Sensation");
            BitmapFont.UseKernings = false;
            Skin.CreateDefault(font);
            chatText = new TextBox { Text = "Send message", Position = new Point(0, 150) };
            var controlTest = new StackPanel
            {
                Items =
                {
                    chatText
                }
                ,
                Position = new Point(0, 150),
                VerticalAlignment = VerticalAlignment.Bottom
            };


            var demoScreen = new Screen
            {
                Content = controlTest
            };

            _guiSystem = new GuiSystem(viewportAdapter, guiRenderer) { ActiveScreen = demoScreen };
        }
    }
}
