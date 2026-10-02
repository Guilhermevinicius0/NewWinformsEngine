using WinformsEngine.Engine.Entities;
using System;
using System.Collections.Generic;
using System.Numerics;
using WinformsEngine.Engine.Systems.General;

namespace WinformsEngine.Engine.Systems
{
    public class HitBox : Transform
    {
        private Entity _parent;

        public bool CollisionEnabled { get; set; } = true;
        public int CollisionLayer = 1;
        public List<int> LayersToCollide = new List<int>() { 1 };

        public bool CanTestCollision(HitBox target)
        {
            if (target == null) return false;
            if (!target.CollisionEnabled) return false;
            if (!CollisionEnabled) return false;
            if (Size == Vector2.Zero || target.Size == Vector2.Zero) return false;
            if (!LayersToCollide.Contains(target.CollisionLayer)) return false;
            if (!target.LayersToCollide.Contains(CollisionLayer)) return false;

            return true;
        }

        public bool IsColliding(HitBox target, out Vector2 collisionNormal, bool touchAsCollision = true)
        {
            collisionNormal = Vector2.Zero;

            if (!CanTestCollision(target))
                return false;

            bool overlap_x = touchAsCollision ?
                            Right >= target.Left && Left <= target.Right :
                            Right > target.Left && Left < target.Right;

            bool overlap_y = touchAsCollision ?
                            Bottom >= target.Top && Top <= target.Bottom :
                            Bottom > target.Top && Top < target.Bottom;

            if (!overlap_x || !overlap_y)
                return false;

            collisionNormal = GetNearestAxysNormal(target);
            return true;
        }

        public float GetOverLappingX(HitBox target)
        {
            return Math.Min(Right, target.Right) - Math.Max(Left, target.Left);
        }

        public float GetOverLappingY(HitBox target)
        {
            return Math.Min(Bottom, target.Bottom) - Math.Max(Top, target.Top);
        }

        public Vector2 GetNearestAxysNormal(HitBox target)
        {
            float overlapX = GetOverLappingX(target);
            float overlapY = GetOverLappingY(target);

            if (overlapX <= 0 || overlapY <= 0)
                return Vector2.Zero;

            if (Math.Abs(overlapX - overlapY) < 0.001f)
                return new Vector2(Math.Sign(CenteredX - target.CenteredX), 0);

            if (overlapX < overlapY)
                return new Vector2(Math.Sign(CenteredX - target.CenteredX), 0);

            return new Vector2(0, Math.Sign(CenteredY - target.CenteredY));
        }

        public Vector2 GetDistanceTo(HitBox target)
        {
            return CenteredPosition - target.CenteredPosition;
        }

        public Vector2 GetNormalTo(HitBox target
            )
        {
            var normal = GetDistanceTo(target);

            if (normal.Length() > 1)
                return Vector2.Normalize(normal);

            return normal;
        }

        internal void SetParent(Entity entity)
        {
            _parent = entity;
        }
    }
}