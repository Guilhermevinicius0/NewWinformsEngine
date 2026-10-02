using WinformsEngine.Engine.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using WinformsEngine.Engine.Systems.Render;
using WinformsEngine.Engine.Systems.Inputs;
using WinformsEngine.Engine.UI;
using WinformsEngine.Engine.Systems.General;

namespace WinformsEngine.Engine.Systems
{
    public class Scene
    {
        private FpsCounter _fpsCounter { get; set; } = new FpsCounter();

        public IdleTimer Timer = new IdleTimer();
        public KeyboardMouseInput Input = new KeyboardMouseInput();
        private readonly SpatialGrid _grid = new SpatialGrid();
        private readonly RenderManager _renderManager = new RenderManager();

        private readonly HashSet<Entity> AllEntities = new HashSet<Entity>();
        private readonly HashSet<Entity> DynamicEntities = new HashSet<Entity>();
        private readonly HashSet<Entity> StaticEntities = new HashSet<Entity>();

        private readonly HashSet<UIItem> UIItems = new HashSet<UIItem>();

        public Vector2 ScreenSize;

        private readonly Dictionary<string, HashSet<Entity>> _taggedEntities = new Dictionary<string, HashSet<Entity>>();
        private readonly Dictionary<string, HashSet<UIItem>> _taggedUIElements = new Dictionary<string, HashSet<UIItem>>();

        private readonly HashSet<(Entity A, Entity B)> _collisionsSet = new HashSet<(Entity A, Entity B)>();

        private readonly Queue<Entity> _toAddEntity = new Queue<Entity>();
        private readonly HashSet<Entity> _toAddSetEntity = new HashSet<Entity>();

        private readonly Queue<Entity> _toRemoveEntitty = new Queue<Entity>();
        private readonly HashSet<Entity> _toRemoveSetEntity = new HashSet<Entity>();

        private readonly Queue<UIItem> _toAddUI = new Queue<UIItem>();
        private readonly HashSet<UIItem> _toAddSetUI = new HashSet<UIItem>();

        private readonly Queue<UIItem> _toRemoveUI = new Queue<UIItem>();
        private readonly HashSet<UIItem> _toRemoveSetUI = new HashSet<UIItem>();

        private const int MAX_QUEUE_INTERACTIONS = 100;
        private float _currentDelta = 0;

        public bool FreezePhysics = false;
        public bool IsPaused { get => _isPaused; }
        private bool _isPaused;

        private readonly HashSet<Entity> _cachedCollisions = new HashSet<Entity>();

        public void PauseScene()
        {
            foreach (var entity in DynamicEntities)
                entity.IsPaused = true;

            _isPaused = true;
        }

        public void UnPauseScene()
        {
            foreach (var entity in DynamicEntities)
                entity.IsPaused = false;

            _isPaused = false;
        }

        public void AddEntity(Entity entity)
        {
            if (entity == null || !_toAddSetEntity.Add(entity))
                return;

            _toAddEntity.Enqueue(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            if (entity == null || !_toRemoveSetEntity.Add(entity))
                return;

            _toRemoveEntitty.Enqueue(entity);
        }

        public void AddUIElement(UIItem element)
        {
            if (element == null || !_toAddSetUI.Add(element))
                return;

            _toAddUI.Enqueue(element);
        }

        public void RemoveUIElement(UIItem element)
        {
            if (element == null || !_toRemoveSetUI.Add(element))
                return;

            _toRemoveUI.Enqueue(element);
        }

        public void ProcessEntityQueues()
        {
            int interactions = 0;

            while (_toRemoveEntitty.Count > 0 && interactions < MAX_QUEUE_INTERACTIONS)
            {
                var entity = _toRemoveEntitty.Dequeue();
                _toRemoveSetEntity.Remove(entity);

                var tags = entity.SeeAllTags();

                if (tags != null && tags.Count > 0)
                {
                    for (int i = 0; i < tags.Count; i++)
                        RemoveTaggedItem(tags[i], entity);
                }

                AllEntities.Remove(entity);
                StaticEntities.Remove(entity);
                DynamicEntities.Remove(entity);
                
                _grid.RemoveItem(entity);
                _renderManager.RemoveEntity(entity);

                entity.NotifyDestroyed();
                interactions++;
            }

            while (_toAddEntity.Count > 0 && interactions < MAX_QUEUE_INTERACTIONS)
            {
                var entity = _toAddEntity.Dequeue();
                _toAddSetEntity.Remove(entity);

                var tags = entity.SeeAllTags();

                if (tags != null && tags.Count > 0)
                {
                    for (int i = 0; i < tags.Count; i++)
                        AddTaggedItem(tags[i], entity);
                }

                AllEntities.Add(entity);

                if (entity.IsStatic)
                    StaticEntities.Add(entity);
                else
                    DynamicEntities.Add(entity);

                _grid.AddItem(entity);
                _renderManager.AddEntity(entity);

                entity.NotifySceneEntered();
                interactions++;
            }
        }

        public void ProcessUIQueues()
        {
            int interactions = 0;

            while (_toRemoveUI.Count > 0 && interactions < MAX_QUEUE_INTERACTIONS)
            {
                var element = _toRemoveUI.Dequeue();
                var tags = element.SeeAllTags();

                if (tags != null && tags.Count > 0)
                {
                    for (int i = 0; i < tags.Count; i++)
                        RemoveTaggedUI(tags[i], element);
                }

                _toRemoveSetUI.Remove(element);
                UIItems.Remove(element);
                _renderManager.RemoveUIElement(element);
                interactions++;
            }
            
            while (_toAddUI.Count > 0 && interactions < MAX_QUEUE_INTERACTIONS)
            {
                var element = _toAddUI.Dequeue();
                var tags = element.SeeAllTags();

                if (tags != null && tags.Count > 0)
                {
                    for (int i = 0; i < tags.Count; i++)
                        AddTaggedUI(tags[i], element);
                }

                _toAddSetUI.Remove(element);
                UIItems.Add(element);
                _renderManager.AddUIElement(element);
                interactions++;
            }
        }


        internal float GetCurrentDelta() => _currentDelta;

        public void ProcessPhysics(float delta)
        {
            _currentDelta = delta;

            foreach (var entity in DynamicEntities)
            {
                if (entity.IsPaused) continue;
                entity.ProcessPhysiques();
            }

            foreach (var entity in DynamicEntities)
                ProcessCollisions(entity);

            foreach (var entity in DynamicEntities)
                SolveOverLapping(entity);

            foreach(var (A, B) in _collisionsSet) 
            {
                A.NotifyCollision(B);
                B.NotifyCollision(A);
            }

            _collisionsSet.Clear();
        }

        public void ProcessWorldTick(float delta)
        {
            ProcessEntityQueues();
            ProcessUIQueues();

            if (!FreezePhysics)
                ProcessPhysics(delta);

            Input.UpdatePreviewsInputs();

            //_fpsCounter.Tick(delta);
            //Console.WriteLine(_fpsCounter.Fps);
        }

        public void AddTaggedItem(string tag, Entity entity)
        {
            if (!_taggedEntities.TryGetValue(tag, out var set))
            {
                set = new HashSet<Entity>();
                _taggedEntities.Add(tag, set);
            }

            set.Add(entity);
        }

        public void RemoveTaggedItem(string tag, Entity entity)
        {
            if (!_taggedEntities.TryGetValue(tag, out var hash)) return;
            hash.Remove(entity);
            if (hash.Count == 0) _taggedEntities.Remove(tag);
        }

        public void AddTaggedUI(string tag, UIItem element)
        {
            if (!_taggedUIElements.TryGetValue(tag, out var set))
            {
                set = new HashSet<UIItem>();
                _taggedUIElements.Add(tag, set);
            }

            set.Add(element);
        }

        public void RemoveTaggedUI(string tag, UIItem element)
        {
            if (!_taggedUIElements.TryGetValue(tag, out var hash)) return;
            hash.Remove(element);
            if (hash.Count == 0) _taggedUIElements.Remove(tag);
        }

        public List<Entity> GetEntitiesTaggedWith(string tag) 
        {
            _taggedEntities.TryGetValue(tag, out var hash);
            if (hash == null) return new List<Entity>();
            return (hash.ToList());
        }

        public List<UIItem> GetUIElementsTaggedWith(string tag) 
        {
            _taggedUIElements.TryGetValue(tag, out var hash);
            if (hash == null) return new List<UIItem>();
            return (hash.ToList());
        }

        public void Render(Graphics g)
        {
            _renderManager.Render(g);
        }

        public HashSet<Entity> GetCollisions(Entity entity, string tag = null, List<Entity> entities = null)
        {
            if (entity == null)
                return _cachedCollisions;

            _cachedCollisions.Clear();

            var targets = entities ?? _grid.GetNearbyItens(entity);

            for (int i = 0; i < targets.Count; i++)
            {
                var target = targets[i];

                if (target == null || target == entity || (tag != null && !target.IsTaggedWith(tag)))
                    continue;

                if (!entity.Hitbox.IsColliding(target.Hitbox, out _))
                    continue;

                _cachedCollisions.Add(target);
            }

            return _cachedCollisions;
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

                if (target == null || target == entity || (tag != null && !target.IsTaggedWith(tag)))
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

        public bool CheckWordlCollisions(Entity entity)
        {
            return GetCollisions(entity).Count > 0;
        }

        public Vector2 GetDistanceBetweenObjects(Entity a, Entity b)
        {
            return a.CenteredPosition - b.CenteredPosition;
        }

        public Vector2 GetNormalBetweenObjects(Entity a, Entity b)
        {
            var normal = GetDistanceBetweenObjects(a, b);
            return normal.LengthSquared() > 1.0f ? Vector2.Normalize(normal) : normal;
        }

        public void MoveAndCollide(Entity entity)
        {
            Vector2 displacement = entity.Velocity;
            if (_currentDelta != 0)
                displacement *= _currentDelta;

            if (displacement.X != 0)
            {
                var previews = entity.Position;
                entity.Position = new Vector2(entity.Position.X + displacement.X, entity.Position.Y);
                _grid.UpdateItemCell(entity, previews);
            }

            if (displacement.Y != 0)
            {
                var previews = entity.Position;
                entity.Position = new Vector2(entity.Position.X, entity.Position.Y + displacement.Y);
                _grid.UpdateItemCell(entity, previews);
            }
        }

        public void MoveAndSlide(Entity entity)
        {
            MoveAndCollide(entity);
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
                    _grid.UpdateItemCell(entity, previews);
                }
                else
                {
                    float dir = Math.Sign(entity.CenteredY - closestTarget.CenteredY);
                    if (dir == 0) dir = 1;

                    var previews = entity.Position;
                    entity.Position = new Vector2(entity.Position.X, entity.Position.Y + overlapY * dir);
                    _grid.UpdateItemCell(entity, previews);
                }

                interactions++;
            }
        }
    }
}