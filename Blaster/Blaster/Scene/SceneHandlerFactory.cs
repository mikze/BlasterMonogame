using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blaster.Scene
{
    internal static class SceneHandlerFactory
    {
        public static SceneHandler CreateSceneHandler(GameComponentCollection gameComponents, GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            var scene = new GameScene();
            var sceneHandler = new SceneHandler(scene, gameComponents, graphicsDevice, contentManager);
            scene.SetSceneHandler(sceneHandler);

            return sceneHandler;
        }
    }
}
