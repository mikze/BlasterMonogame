﻿using Blaster.Components;
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
using static Blaster.Network.NetworkHelper;
using Blaster.Network;
using System.Linq;

namespace Blaster.Systems
{
    class NetElementSystem : EntityProcessingSystem
    {
        private ComponentMapper<NetElement> _playerMapper;
        private ComponentMapper<AnimatedSprite> _spriteMapper;
        private ComponentMapper<Transform2> _transformMapper;
        private EntityFactory _entityFactory;

        public NetElementSystem(EntityFactory entityFactory)
            : base(Aspect.All(typeof(NetElement), typeof(Transform2), typeof(AnimatedSprite)))
        {
            _entityFactory = entityFactory;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _playerMapper = mapperService.GetMapper<NetElement>();
            _spriteMapper = mapperService.GetMapper<AnimatedSprite>();
            _transformMapper = mapperService.GetMapper<Transform2>();
        }

        public override void Process(GameTime gameTime, int entityId)
        {
            var p = _playerMapper.Get(entityId);
            var frames = BlasterClient.GetFrames(p.Id);
            HandleMovements(frames.Where(x => (FrameKind)x.FrameKind == FrameKind.movement), entityId);
            HandleNewPlayer(frames.Where(x => (FrameKind)x.FrameKind == FrameKind.newPLayer));
            HandlePlayerDisconnected(frames.Where(x => (FrameKind)x.FrameKind == FrameKind.playerDisconnected), p.Id, entityId);
            HandleChat(frames.Where(x => (FrameKind)x.FrameKind == FrameKind.chat), entityId);
        }

        private void HandlePlayerDisconnected(IEnumerable<Frame> frames, int playerId, int entityId)
        {
            foreach(var f in frames)
            {
                if(f.id == playerId)
                    DestroyEntity(entityId);
            }
        }

        private void HandleChat(IEnumerable<Frame> enumerable, int entityId)
        {
            if (enumerable.Any())
            {
                var t = _transformMapper.Get(entityId);
                _entityFactory.CreateText(new Vector2(t.Position.X, t.Position.Y - 50), enumerable.First().body, 3);
            }
        }

        private void HandleNewPlayer(IEnumerable<Frame> enumerable)
        {
            foreach(var e in enumerable)
            {
                var newElem = splitNetElementFromFrameBody(e.body);
                _entityFactory.CreatePlayer(newElem.Position, e.id);
            }
        }

        internal void HandleMovements(IEnumerable<Frame> frames, int entityId)
        {
            var t = _transformMapper.Get(entityId);
            var p = _playerMapper.Get(entityId);
            foreach (var f in frames)
            {
                if (p.Id == f.id)
                {
                    var splittedBody = f.body.Split('-');
                    var X = float.Parse(splittedBody[0]);
                    var Y = float.Parse(splittedBody[1]);

                    var newPosition = new Vector2(X, Y);

                    t.Position = newPosition;
                }
            }
        }
    }
}
