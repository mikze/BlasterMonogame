using Blaster.Components;
using Blaster.Network;
using Blaster.Scene;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Text;
using static Blaster.Network.NetworkHelper;

namespace Blaster.Systems
{
    public class RenderSystem : EntityDrawSystem
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly OrthographicCamera _camera;
        private ComponentMapper<AnimatedSprite> _animatedSpriteMapper;
        private ComponentMapper<Sprite> _spriteMapper;
        private ComponentMapper<Transform2> _transforMapper;
        private ComponentMapper<Text> _textMapper;
        private ComponentMapper<TextBox> _textBoxMapper;

        public RenderSystem(SpriteBatch spriteBatch, OrthographicCamera camera) 
            : base(Aspect.All(typeof(Transform2)).One(typeof(AnimatedSprite), typeof(Sprite), typeof(Text)))
        {
            _spriteBatch = spriteBatch;
            _camera = camera;
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.GetViewMatrix());

            foreach (var entity in ActiveEntities)
            {
                bool isText = _textMapper.Has(entity);

                if (!isText)
                {
                    bool isTextBox = _textBoxMapper.Has(entity);

                    if (!isTextBox)
                    {
                        var sprite = _animatedSpriteMapper.Has(entity)
                            ? _animatedSpriteMapper.Get(entity)
                            : _spriteMapper.Get(entity);
                        var transform = _transforMapper.Get(entity);

                        if (sprite is AnimatedSprite animatedSprite)
                            animatedSprite.Update(gameTime.GetElapsedSeconds());

                        _spriteBatch.Draw(sprite, transform);
                    }
                    else
                    {
                        var textBox = _textBoxMapper.Get(entity);
                        //textBox.Draw()
                    }
                }
                else
                {
                    var text = _textMapper.Get(entity);
                    _spriteBatch.DrawString(text.font, text.text, _transforMapper.Get(entity).Position, Color.White);
                }

            }

            _spriteBatch.End();
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _transforMapper = mapperService.GetMapper<Transform2>();
            _animatedSpriteMapper = mapperService.GetMapper<AnimatedSprite>();
            _spriteMapper = mapperService.GetMapper<Sprite>();
            _textMapper = mapperService.GetMapper<Text>();
            _textBoxMapper = mapperService.GetMapper<TextBox>();
        }
    }
}
