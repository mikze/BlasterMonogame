using Blaster.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Blaster.Systems
{
    class PlayerSystem : EntityProcessingSystem
    {
        private ComponentMapper<Player> _playerMapper;
        private ComponentMapper<AnimatedSprite> _spriteMapper;
        private ComponentMapper<Transform2> _transformMapper;

        public PlayerSystem()
            : base(Aspect.All(typeof(Player), typeof(Transform2), typeof(AnimatedSprite)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _playerMapper = mapperService.GetMapper<Player>();
            _spriteMapper = mapperService.GetMapper<AnimatedSprite>();
            _transformMapper = mapperService.GetMapper<Transform2>();
        }

        public override void Process(GameTime gameTime, int entityId)
        {
            var keyboardState = KeyboardExtended.GetState();

            if (keyboardState.WasKeyJustUp(Keys.Up))
            {

            }
        }
    }
}
