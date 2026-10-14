using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WinformsEngine.Engine.Entities;
using WinformsEngine.Engine.UI;

namespace WinformsEngine.Engine.Systems.Scene
{
    public partial class Scene
    {
        #region Fetch Entityes
        #region general

        public List<Entity> GetAllEntities()
        {
            return AllEntities.ToList();
        }

        public List<Entity> GetAllDynamicEntities()
        {
            return DynamicEntities.ToList();
        }

        public List<Entity> GetAllStaticEntities()
        {
            return StaticEntities.ToList();
        }

        #endregion
        #region by type

        public List<T> GetAllEntities<T>() where T : Entity
        {
            return AllEntities.OfType<T>().ToList();
        }

        public List<T> GetAllDynamicEntities<T>() where T : Entity
        {
            return DynamicEntities.OfType<T>().ToList();
        }

        public List<T> GetAllStaticEntities<T>() where T : Entity
        {
            return StaticEntities.OfType<T>().ToList();
        }

        #endregion
        #region by tag (and type)

        public List<Entity> GetEntitiesTaggedWith(string tag)
        {
            _taggedEntities.TryGetValue(tag, out var hash);
            if (hash == null) return new List<Entity>();
            return hash.ToList();
        }

        public List<T> GetEntitiesTaggedWith<T>(string tag) where T : Entity
        {
            _taggedEntities.TryGetValue(tag, out var hash);
            if (hash == null) return new List<T>();
            return hash.OfType<T>().ToList();
        }
        #endregion
        #region by radius

        public List<Entity> GetEntitiesInRadius(Vector2 center, float radius)
        {
            var result = new List<Entity>();
            var nearby = _grid.GetNearbyItens(center);

            float radiusSquared = radius * radius;
            foreach (var entity in nearby)
            {
                float distSquared = Vector2.DistanceSquared(center, entity.CenteredPosition);
                if (distSquared <= radiusSquared)
                    result.Add(entity);
            }

            return result;
        }

        public List<T> GetEntitiesInRadius<T>(Vector2 center, float radius) where T : Entity
        {
            var result = new List<T>();
            var nearby = _grid.GetNearbyItens(center).OfType<T>();

            float radiusSquared = radius * radius;
            foreach (var entity in nearby)
            {
                float distSquared = Vector2.DistanceSquared(center, entity.CenteredPosition);
                if (distSquared <= radiusSquared)
                    result.Add(entity);
            }

            return result;
        }

        #endregion
        #endregion

        #region Fetch UI
        #region General

        public List<UIItem> GetAllUIElements() 
        {
            return UIItems.ToList();
        }

        public List<T> GetAllUIElements<T>() where T : UIItem 
        {
            return UIItems.OfType<T>().ToList();
        }

        #endregion
        #region Tagged

        public List<UIItem> GetUIElementsTaggedWith(string tag)
        {
            _taggedUIElements.TryGetValue(tag, out var hash);
            if (hash == null) return new List<UIItem>();
            return hash.ToList();
        }

        public List<T> GetUIElementsTaggedWith<T>(string tag) where T : UIItem
        {
            _taggedUIElements.TryGetValue(tag, out var hash);
            if (hash == null) return new List<T>();
            return hash.OfType<T>().ToList();
        }

        #endregion
        #endregion
    }
}
