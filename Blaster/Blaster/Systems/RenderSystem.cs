﻿using Blaster.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Sprites;

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
        private ComponentMapper<Name> _nameMapper;
        private ContentManager _contentManager;
        public RenderSystem(SpriteBatch spriteBatch, OrthographicCamera camera, ContentManager contentManager) 
            : base(Aspect.All(typeof(Transform2)).One(typeof(AnimatedSprite), typeof(Sprite), typeof(Text), typeof(NetElement)))
        {
            _spriteBatch = spriteBatch;
            _camera = camera;
            _contentManager = contentManager;
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.GetViewMatrix());

            foreach (var entity in ActiveEntities)
            {
                bool isText = _textMapper.Has(entity);
                

                if (!isText)
                {
                    bool hasName = _nameMapper.Has(entity);
                    var sprite = _animatedSpriteMapper.Has(entity)
                        ? _animatedSpriteMapper.Get(entity)
                        : _spriteMapper.Get(entity);
                    var transform = _transforMapper.Get(entity);

                    if (sprite is AnimatedSprite animatedSprite)
                        animatedSprite.Update(gameTime.GetElapsedSeconds());

                    _spriteBatch.Draw(sprite, transform);
                    if (hasName)
                    {
                        var name = _nameMapper.Get(entity);
                        var font = _contentManager.Load<SpriteFont>("Score");
                        var position = new Vector2(_transforMapper.Get(entity).Position.X-15, _transforMapper.Get(entity).Position.Y - 50);
                        _spriteBatch.DrawString(font, name.name, position, Color.White);
                    }
                }
                else
                {
                    var text = _textMapper.Get(entity);
                    if (text.Destroy)
                        DestroyEntity(entity);
                    else
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
            _nameMapper = mapperService.GetMapper<Name>();
        }
    }
}
