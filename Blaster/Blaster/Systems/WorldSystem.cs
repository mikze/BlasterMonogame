using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blaster.Systems
{
    public class WorldSystem : EntityProcessingSystem
    {
        private readonly World _world;
        private ComponentMapper<Transform2> _transformMapper;

        public WorldSystem(AspectBuilder aspectBuilder) : base(Aspect.All(typeof(Transform2)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            throw new NotImplementedException();
        }

        public override void Process(GameTime gameTime, int entityId)
        {
            throw new NotImplementedException();
        }
    }
}
