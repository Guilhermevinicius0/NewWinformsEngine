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

        public bool IsAttachedToParent { get; set; } = true;
        public bool CollisionEnabled { get; set; } = true;
        public int CollisionLayer = 1;
        public List<int> LayersToCollide = new List<int>() { 1 };

        public override Vector2 Size
        {
            get 
            {
                if (IsAttachedToParent)
                    return _parent?.Size ?? Vector2.Zero;

                return base.Size;
            }
            set
            {
                if (IsAttachedToParent)
                    return;

                base.Size = value;
            }
        }

        public override Vector2 Position
        {
            get 
            {
                if (IsAttachedToParent)
                    return _parent?.Position ?? Vector2.Zero;

                return base.Position;
            }
            set
            {
                if (IsAttachedToParent)
                    return;

                base.Position = value;
            }
        }


        public override float Left 
        {
            get 
            {
                if (IsAttachedToParent)
                    return _parent?.Left ?? 0f;

                return base.Left;
            }
            set
            {
                if (IsAttachedToParent)
                    return;

                base.Left = value;
            }
        }

        public override float Right 
        {
            get
            {
                if (IsAttachedToParent)
                    return _parent?.Right ?? 0f;

                return base.Right;
            }
            set
            {
                if (IsAttachedToParent)
                    return;

                base.Right = value;
            }
        }
        
        public override float Top 
        {
            get
            {
                if (IsAttachedToParent)
                    return _parent?.Top ?? 0f;

                return base.Top;
            }
            set
            {
                if (IsAttachedToParent)
                    return;

                base.Top = value;
            }
        }
        
        public override float Bottom 
        {
            get
            {
                if (IsAttachedToParent)
                    return _parent?.Bottom ?? 0f;

                return base.Bottom;
            }
            set
            {
                if (IsAttachedToParent)
                    return;

                base.Bottom = value;
            }
        }

        public override float CenteredX 
        {
            get
            {
                if (IsAttachedToParent)
                    return _parent?.CenteredX ?? 0f;

                return base.CenteredX;
            }
            set
            {
                if (IsAttachedToParent)
                    return;

                base.CenteredX = value;
            }
        }
        
        public override float CenteredY 
        {
            get
            {
                if (IsAttachedToParent)
                    return _parent?.CenteredY ?? 0f;

                return base.CenteredY;
            }
            set
            {
                if (IsAttachedToParent)
                    return;

                base.CenteredY = value;
            }
        }

        public override Vector2 CenteredPosition 
        {
            get
            {
                if (IsAttachedToParent)
                    return _parent?.CenteredPosition ?? Vector2.Zero;

                return base.CenteredPosition;
            }
            set
            {
                if (IsAttachedToParent)
                    return;

                base.CenteredPosition = value;
            }
        }

        public HitBox(Entity parent, Vector2 size, List<int> layers, bool attached, bool colisionEnable, int collisionLayer) 
        {
            _parent = parent;
            IsAttachedToParent = attached;
            CollisionEnabled = colisionEnable;
            CollisionLayer = collisionLayer;
            Size = size;
            LayersToCollide = layers;
        }

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