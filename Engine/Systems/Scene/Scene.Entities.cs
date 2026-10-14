using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WinformsEngine.Engine.Entities;

namespace WinformsEngine.Engine.Systems.Scene
{
    public partial class Scene
    {
        #region Fields

        private readonly HashSet<Entity> AllEntities = new HashSet<Entity>();
        private readonly HashSet<Entity> DynamicEntities = new HashSet<Entity>();
        private readonly HashSet<Entity> StaticEntities = new HashSet<Entity>();

        private readonly Dictionary<string, HashSet<Entity>> _taggedEntities = new Dictionary<string, HashSet<Entity>>();

        private readonly Queue<Entity> _toAddEntity = new Queue<Entity>();
        private readonly HashSet<Entity> _toAddSetEntity = new HashSet<Entity>();

        private readonly Queue<Entity> _toRemoveEntitty = new Queue<Entity>();
        private readonly HashSet<Entity> _toRemoveSetEntity = new HashSet<Entity>();

        #endregion

        #region Methods

        public void OnEntityPositionChanged(Entity entity, Vector2 previewsPosition)
        {
            if (entity.IsStatic)
                return;

            _grid.UpdateItemCell(entity, previewsPosition);
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

        public void AddTaggedEntity(string tag, Entity entity)
        {
            if (!_taggedEntities.TryGetValue(tag, out var set))
            {
                set = new HashSet<Entity>();
                _taggedEntities.Add(tag, set);
            }

            set.Add(entity);
        }

        public void RemoveTaggedEntity(string tag, Entity entity)
        {
            if (!_taggedEntities.TryGetValue(tag, out var hash)) return;
            hash.Remove(entity);
            if (hash.Count == 0) _taggedEntities.Remove(tag);
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
                        RemoveTaggedEntity(tags[i], entity);
                }

                AllEntities.Remove(entity);
                StaticEntities.Remove(entity);
                DynamicEntities.Remove(entity);

                _grid.RemoveItem(entity);
                _renderManager.RemoveEntity(entity);

                if (!entity.IsStatic)
                    entity.EntityPositionChangedEvent -= OnEntityPositionChanged;

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
                        AddTaggedEntity(tags[i], entity);
                }

                AllEntities.Add(entity);

                if (entity.IsStatic)
                    StaticEntities.Add(entity);
                else
                {
                    DynamicEntities.Add(entity);
                    entity.EntityPositionChangedEvent += OnEntityPositionChanged;
                }

                _grid.AddItem(entity);
                _renderManager.AddEntity(entity);

                entity.NotifySceneEntered();
                interactions++;
            }
        }

        #endregion
    }
}
