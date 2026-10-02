using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using WinformsEngine.Engine.Systems;
using WinformsEngine.Engine.Systems.General;
using WinformsEngine.Engine.Systems.Render;
using WinformsEngine.Engine.Systems.Sounds;

namespace WinformsEngine.Engine.Entities
{
    public abstract class Entity : Transform
    {
        protected Scene _scene { get; set; }
        public HitBox Hitbox { get; set; }
        public EntityCollisionInfo CollisionInfo { get; set; } = new EntityCollisionInfo();
        private HashSet<string> Tags { get; set; } = new HashSet<string>();

        public IEntityRenderSystem RenderSystem { get; set; } = null;
        public ISoundPlayer SoundSystem { get; set; } = null;

        protected event Action<Entity> CollisionEvent;
        protected event Action SceneEnteredEvent;
        protected event Action DestroyEvent;


        //Required entities properties
        public abstract bool IsStatic { get; set; }
        public abstract bool IsPausable { get; set; }
        public abstract int ZIndex { get; set; }

        public Vector2 Direction { get; set; }
        public Vector2 Velocity { get; set; }
        public float MaxSpeed { get; set; }
        public float Speed { get; set; }

        public bool IsOnFloor { get => CollisionInfo.IsOnFloor; }
        public bool IsOnWall { get => CollisionInfo.IsOnWall; }
        public bool IsOnCeelling { get => CollisionInfo.IsOnCeiling; }

        private bool _isPaused;
        public bool IsPaused
        {
            get => _isPaused; set
            {
                if (!IsPausable)
                {
                    _isPaused = false;
                    return;
                }
                _isPaused = value;
            }
        }


        public Entity(Scene scene, Vector2 size, Vector2 position, float speed, float maxSpeed)
        {
            _scene = scene;
            Size = size;
            Position = position;
            Speed = speed;
            MaxSpeed = maxSpeed;

            Hitbox = new HitBox
            {
                CollisionEnabled = true,
                Size = Size,
                CollisionLayer = 1,
                LayersToCollide = new List<int> { 1 }
            };

            Hitbox.SetParent(this);
            CollisionEvent += OnColide;
        }

        public void NotifyCollision(Entity entity) => CollisionEvent?.Invoke(entity);
        public void NotifySceneEntered() => SceneEnteredEvent?.Invoke();
        public void NotifyDestroyed() => DestroyEvent?.Invoke();

        public List<string> SeeAllTags()
            => Tags.ToList();

        public bool IsTaggedWith(string tag)
            => Tags.Contains(tag);

        public void AddTag(string tag)
        {
            Tags.Add(tag);
            _scene.AddTaggedItem(tag, this);
        }

        public void RemoveTag(string tag)
        {
            if (Tags.Contains(tag))
                Tags.Remove(tag);

            _scene.RemoveTaggedItem(tag, this);
        }

        protected void MoveAndSlide()
            => _scene.MoveAndSlide(this);

        protected void MoveAndCollide()
            => _scene.MoveAndCollide(this);

        public void AddTaggedItem(string tag, Entity entity)
            => _scene.AddTaggedItem(tag, entity);

        public void RemoveTaggedItem(string tag, Entity entity)
            => _scene.RemoveTaggedItem(tag, entity);

        public HashSet<Entity> GetCollisions()
            => _scene.GetCollisions(this);

        public HashSet<Entity> GetCollisions(List<Entity> entities)
            => _scene.GetCollisions(this, entities: entities);

        public HashSet<Entity> GetCollisionsWithTag(string tag)
            => _scene.GetCollisions(this, tag: tag);

        public HashSet<Entity> GetCollisionsWithTag(string tag, List<Entity> entities)
            => _scene.GetCollisions(this, tag: tag, entities: entities);

        public bool CheckWordlCollisions()
            => _scene.CheckWordlCollisions(this);

        public Vector2 GetDistanceTo(Entity target)
            => _scene.GetDistanceBetweenObjects(this, target);

        public Vector2 GetNormalTo(Entity target)
            => _scene.GetNormalBetweenObjects(this, target);

        public void Destroy()
            => _scene.RemoveEntity(this);

        public float GetCurrentDelta()
            => _scene.GetCurrentDelta();

        public abstract void ProcessPhysiques();

        protected virtual void OnColide(Entity entity) { }
        protected virtual void OnSceneEnetered() { }
        protected virtual void OnDestroyed()
        {
            RenderSystem?.Dispose();
        }
    }
}