using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using WinformsEngine.Engine.Systems;
using WinformsEngine.Engine.Systems.Enums;
using WinformsEngine.Engine.Systems.General;
using WinformsEngine.Engine.Systems.Render;
using WinformsEngine.Engine.Systems.Scene;
using WinformsEngine.Engine.Systems.Sounds;

namespace WinformsEngine.Engine.Entities
{
    public abstract class Entity : Transform
    {
        #region Fields

        protected Scene _scene { get; set; }
        public HitBox Hitbox { get; set; }
        public EntityCollisionInfo CollisionInfo { get; set; } = new EntityCollisionInfo();
        private HashSet<string> Tags { get; set; } = new HashSet<string>();

        public IEntityRenderSystem RenderSystem { get; set; } = null;
        public ISoundPlayer SoundSystem { get; set; } = null;

        public Vector2 Direction { get; set; }
        public Vector2 Velocity { get; set; }
        public float MaxSpeed { get; set; }
        public float Speed { get; set; }
        public float GravityForce { get; set; }
        public float MaxGravityForce { get; set; }

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

        public Vector2 ScreenSize => _scene?.ScreenSize ?? Vector2.Zero;
        public Vector2 ScreenCenter => _scene?.ScreenCenter ?? Vector2.Zero;
        public float ScreenRight => _scene?.ScreenRight ?? 0;
        public float ScreenBottom => _scene?.ScreenBottom ?? 0;
        public float ScreenLeft => _scene?.ScreenLeft ?? 0;
        public float ScreenTop => _scene?.ScreenTop ?? 0;


        //Required entities properties
        public abstract bool IsStatic { get; set; }
        public abstract bool IsPausable { get; set; }
        public abstract int ZIndex { get; set; }

        #endregion
        #region Events

        public event Action<Entity, Vector2> EntityPositionChangedEvent;
        public event Action<Entity, Vector2> EntitySizeChangedEvent;

        protected event Action<Entity> CollisionEvent;
        protected event Action SceneEnteredEvent;
        protected event Action DestroyEvent;

        #endregion
        #region Entity Methods

        public Entity(Scene scene, Vector2 size, Vector2 position, float speed, float maxSpeed)
        {
            _scene = scene;
            Size = size;
            Position = position;
            Speed = speed;
            MaxSpeed = maxSpeed;

            Hitbox = new HitBox(this, Size, new List<int> { 1 }, true, true, 1);
            Hitbox.SetParent(this);
            CollisionEvent += OnColide;
            base.PositionChangedEvent += OnTransformPositionChanged;
            base.SizeChangedEvent += OnTransformSizeChanged;
        }

        private void OnTransformSizeChanged(Vector2 previewsSize)
        {
            EntitySizeChangedEvent?.Invoke(this, previewsSize);
        }

        private void OnTransformPositionChanged(Vector2 previewsPosition)
        {
            EntityPositionChangedEvent?.Invoke(this, previewsPosition);
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
            _scene.AddTaggedEntity(tag, this);
        }

        public void RemoveTag(string tag)
        {
            if (Tags.Contains(tag))
                Tags.Remove(tag);

            _scene.RemoveTaggedEntity(tag, this);
        }

        public void EnableInputListener()
        {
            _scene.Input.KeyPressedEvent += OnKeyPressed;
            _scene.Input.KeyReleasedEvent += OnKeyReleased;
        }

        public void DisableInputListener()
        {
            _scene.Input.KeyPressedEvent -= OnKeyPressed;
            _scene.Input.KeyReleasedEvent -= OnKeyReleased;
        }

        public abstract void ProcessPhysiques();

        protected virtual void OnColide(Entity entity) { }
        protected virtual void OnKeyPressed(Keys key) { }
        protected virtual void OnKeyReleased(Keys key) { }
        protected virtual void OnSceneEntered() { }
        protected virtual void OnDestroyed()
        {
            RenderSystem?.Dispose();
            CollisionEvent -= OnColide;
            base.PositionChangedEvent -= OnTransformPositionChanged;
            base.SizeChangedEvent -= OnTransformSizeChanged;
            DisableInputListener();
        }

        #endregion
        #region Scene Wrappers

        protected void MoveAndCollide()
            => _scene.MoveAndCollide(this);

        public List<Entity> GetAllEntities()
            => _scene.GetAllEntities();

        public List<T> GetAllEntities<T>() where T : Entity
           => _scene.GetAllEntities<T>();

        public List<Entity> GetEntitiesTaggedWith(string tag)
            => _scene.GetEntitiesTaggedWith(tag);

        public List<T> GetEntitiesTaggedWith<T>(string tag) where T : Entity
            => _scene.GetEntitiesTaggedWith<T>(tag);

        public HashSet<Entity> GetCollisions()
            => _scene.GetCollisions(this);

        public HashSet<T> GetCollisions<T>() where T : Entity
           => _scene.GetCollisions<T>(this);

        public HashSet<Entity> GetCollisionsWithTag(string tag)
            => _scene.GetCollisions(this, tag: tag);

        public HashSet<T> GetCollisionsWithTag<T>(string tag) where T : Entity
            => _scene.GetCollisions<T>(this, tag: tag);

        public bool CheckWorldCollisions()
            => _scene.CheckWorldCollisions(this);

        public Vector2 GetDistanceTo(Entity target)
            => _scene.GetDistanceBetweenObjects(this, target);

        public Vector2 GetNormalTo(Entity target)
            => _scene.GetNormalBetweenObjects(this, target);

        public void ChangeSceneState(SceneStates state)
            => _scene.ChangeSceneState(state);

        public void Destroy()
            => _scene.RemoveEntity(this);

        public float GetCurrentDelta()
            => _scene.GetCurrentDelta();

        #endregion
    }
}