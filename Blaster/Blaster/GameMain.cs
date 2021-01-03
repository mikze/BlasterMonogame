using Autofac;
using Blaster.Systems;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Blaster.Scene;

namespace Blaster
{
    public class GameMain : GameBase
    {
        SceneHandler sceneHandler;
        protected override void LoadContent()
        {
            sceneHandler = SceneHandlerFactory.CreateSceneHandler(Components, GraphicsDevice, Content);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            sceneHandler.DrawScene();
            base.Draw(gameTime);
        }
    }
}
