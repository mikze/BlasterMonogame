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
using static Blaster.Network.NetworkHelper;
using Blaster.Network;
using System.Linq;

namespace Blaster.Systems
{
    class NetElementSystem : EntityProcessingSystem
    {
        private ComponentMapper<NetElement> _netElemMapper;
        private ComponentMapper<Player> _playerMapper;
        private ComponentMapper<Transform2> _transformMapper;
        private EntityFactory _entityFactory;

        public NetElementSystem(EntityFactory entityFactory)
            : base(Aspect.All(typeof(NetElement), typeof(Transform2), typeof(AnimatedSprite)))
        {
            _entityFactory = entityFactory;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _netElemMapper = mapperService.GetMapper<NetElement>();
            _playerMapper = mapperService.GetMapper<Player>();
            _transformMapper = mapperService.GetMapper<Transform2>();
        }

        public override void Process(GameTime gameTime, int entityId)
        {
            var p = _netElemMapper.Get(entityId);
            var frames = BlasterClient.GetFrames(p.Id);

            HandleMovements(frames.Where(x => (FrameKind)x.FrameKind == FrameKind.movement), entityId);
            HandlePlayerState(frames.Where(x => (FrameKind)x.FrameKind == FrameKind.setPlayerState), entityId);
            HandleNewPlayer(frames.Where(x => (FrameKind)x.FrameKind == FrameKind.newPLayer));
            HandlePlayerDisconnected(frames.Where(x => (FrameKind)x.FrameKind == FrameKind.playerDisconnected), p.Id, entityId);
            HandleChat(frames.Where(x => (FrameKind)x.FrameKind == FrameKind.chat), entityId);
            HandlePlayerNameChange(frames.Where(x => (FrameKind)x.FrameKind == FrameKind.setName), p);
            HandleBomb(frames.Where(x => (FrameKind)x.FrameKind == FrameKind.createBomb));
        }

        private void HandleBomb(IEnumerable<Frame> enumerable)
        {
            var position = enumerable.First().body.Split('-');
            _entityFactory.CreateBomb(new Vector2(float.Parse(position[0]), float.Parse(position[1])));
        }

        private void HandlePlayerState(IEnumerable<Frame> enumerable, int entityId)
        {
            if (enumerable.Any())
            {
                var isPlayer = _playerMapper.Has(entityId);
                if (isPlayer)
                {
                    var stateFromBody = enumerable.First().body;
                    if (stateFromBody == "idle")
                        _playerMapper.Get(entityId).State = State.Idle;
                }
            }
        }

        private void HandlePlayerNameChange(IEnumerable<Frame> enumerable, NetElement netElem)
        {
            if (enumerable.Any())
            {
                var newName = enumerable.First().body;
                netElem.Name = newName;
            }
            
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
                _entityFactory.CreateText(new Vector2(t.Position.X-35, t.Position.Y - 80), enumerable.First().body, 3);
            }
        }

        private void HandleNewPlayer(IEnumerable<Frame> enumerable)
        {
            foreach(var e in enumerable)
            {
                var newElem = splitNetElementFromFrameBody(e.body);
                _entityFactory.CreatePlayer(newElem.Position, e.id, newElem.Name);
            }
        }

        internal void HandleMovements(IEnumerable<Frame> frames, int entityId)
        {
            var t = _transformMapper.Get(entityId);
            var p = _netElemMapper.Get(entityId);
            var isPlayer = _playerMapper.Has(entityId);

            foreach (var f in frames)
            {
                if (p.Id == f.id)
                {
                    var splittedBody = f.body.Split('-');
                    var X = float.Parse(splittedBody[0]);
                    var Y = float.Parse(splittedBody[1]);

                    var newPosition = new Vector2(X, Y);

                    if(isPlayer)
                    {
                        var player = _playerMapper.Get(entityId);
                        UpdatePlayerState(player, t.Position, newPosition);
                    }
                    t.Position = newPosition;
                }
            }
        }

        private void UpdatePlayerState(Player player, Vector2 position, Vector2 newPosition)
        {
            bool right = position.X < newPosition.X;
            bool left = position.X > newPosition.X;
            bool up = position.Y > newPosition.Y;
            bool down = position.Y < newPosition.Y;

            if (right)
            {
                player.Facing = Facing.Right;
                player.State = State.Walking;
            }

            else if (left)
            {
                player.Facing = Facing.Left;
                player.State = State.Walking;
            }

            else if (down || up)
                player.State = State.Walking;

            else
                player.State = State.Idle;
        }
    }
}
