using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WinformsEngine.Engine.Entities;

namespace WinformsEngine.Engine.Systems.Scene
{
    public partial class Scene
    {
        #region Fields
        
        private readonly HashSet<(Entity A, Entity B)> _collisionsSet = new HashSet<(Entity A, Entity B)>();
        private readonly HashSet<Entity> _cachedCollisions = new HashSet<Entity>();

        #endregion

        #region Methods

        public HashSet<Entity> GetCollisions(Entity entity, string tag = null, List<Entity> entities = null)
        {
            if (entity == null)
                return _cachedCollisions;

            _cachedCollisions.Clear();

            var targets = entities ?? _grid.GetNearbyItens(entity);

            for (int i = 0; i < targets.Count; i++)
            {
                var target = targets[i];

                if (target == null || target == entity || tag != null && !target.IsTaggedWith(tag))
                    continue;

                if (!entity.Hitbox.IsColliding(target.Hitbox, out _))
                    continue;

                _cachedCollisions.Add(target);
            }

            return _cachedCollisions;
        }

        public HashSet<T> GetCollisions<T>(Entity entity, string tag = null, List<T> entities = null) where T : Entity
        {
            HashSet<T> result = new HashSet<T>();

            if (entity == null)
                return result;

            var targets = entities ?? _grid.GetNearbyItens(entity).OfType<T>().ToList();

            for (int i = 0; i < targets.Count; i++)
            {
                var target = targets[i];

                if (target == null || target == entity || tag != null && !target.IsTaggedWith(tag))
                    continue;

                if (!entity.Hitbox.IsColliding(target.Hitbox, out _))
                    continue;

                result.Add(target);
            }

            return result;
        }

        public void ProcessCollisions(Entity entity, string tag = null, List<Entity> entities = null)
        {
            if (entity == null)
                return;

            entity.CollisionInfo.Clear();
            var targets = entities ?? _grid.GetNearbyItens(entity);

            for (int i = 0; i < targets.Count; i++)
            {
                var target = targets[i];

                if (target == null || target == entity || tag != null && !target.IsTaggedWith(tag))
                    continue;

                if (!entity.Hitbox.IsColliding(target.Hitbox, out var axys))
                    continue;

                entity.CollisionInfo.Targets.Add(target);

                var pair = RuntimeHelpers.GetHashCode(entity) < RuntimeHelpers.GetHashCode(target)
                    ? (entity, target)
                    : (target, entity);

                _collisionsSet.Add(pair);

                if (axys.Y < 0) entity.CollisionInfo.IsOnCeiling = true;
                if (axys.Y > 0) entity.CollisionInfo.IsOnFloor = true;
                if (axys.X < 0) entity.CollisionInfo.IsOnLeftWall = true;
                if (axys.X > 0) entity.CollisionInfo.IsOnRightWall = true;
            }
        }

        public bool CheckWorldCollisions(Entity entity)
        {
            return GetCollisions(entity).Count > 0;
        }

        private void SolveOverLapping(Entity entity)
        {
            const int MAX_INTERACTIONS = 10;
            int interactions = 0;

            while (interactions < MAX_INTERACTIONS)
            {
                var targets = GetCollisions(entity).ToList();
                if (targets.Count == 0) break;

                Entity closestTarget = null;
                float minDistance = float.MaxValue;

                for (int i = 0; i < targets.Count; i++)
                {
                    var t = targets[i];
                    float d = Vector2.DistanceSquared(entity.CenteredPosition, t.CenteredPosition);
                    if (d < minDistance) { minDistance = d; closestTarget = t; }
                }

                if (closestTarget == null) break;

                float overlapX = entity.Hitbox.GetOverLappingX(closestTarget.Hitbox);
                float overlapY = entity.Hitbox.GetOverLappingY(closestTarget.Hitbox);

                if (overlapX <= 0 || overlapY <= 0) break;

                if (overlapX < overlapY)
                {
                    float dir = Math.Sign(entity.CenteredX - closestTarget.CenteredX);
                    if (dir == 0) dir = 1;

                    var previews = entity.Position;
                    entity.Position = new Vector2(entity.Position.X + overlapX * dir, entity.Position.Y);
                }
                else
                {
                    float dir = Math.Sign(entity.CenteredY - closestTarget.CenteredY);
                    if (dir == 0) dir = 1;

                    var previews = entity.Position;
                    entity.Position = new Vector2(entity.Position.X, entity.Position.Y + overlapY * dir);
                }

                interactions++;
            }
        }

        #endregion
    }
}
