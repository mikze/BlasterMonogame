using Autofac;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Blaster
{
    public abstract class GameBase : Game
    {
        protected GraphicsDeviceManager GraphicsDeviceManager { get; }
        public int Width { get; }
        public int Height { get; }

        public GameBase(int width = 800, int height = 480)
        {
            Width = width;
            Height = height;
            GraphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = width,
                PreferredBackBufferHeight = height
            };
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected abstract void RegisterDependencies(ContainerBuilder builder);
    }
}
