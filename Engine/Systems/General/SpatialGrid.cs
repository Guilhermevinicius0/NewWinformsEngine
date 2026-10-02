using WinformsEngine.Engine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WinformsEngine.Engine.Systems
{
    public class SpatialGrid
    {
        public float CellSize = 64;
        public int SearchRadius = 1;

        private Dictionary<Vector2, HashSet<Entity>> _grid = new Dictionary<Vector2, HashSet<Entity>>();
        private List<Entity> _nearbyCache = new List<Entity>();

        public Vector2 GetCellKey(Vector2 position)
        {
            return new Vector2((int)Math.Floor(position.X / CellSize),
                               (int)Math.Floor(position.Y / CellSize));
        }

        public void AddItem(Entity entity)
        {
            var key = GetCellKey(entity.Position);

            if (!_grid.ContainsKey(key))
                _grid.Add(key, new HashSet<Entity>());

            _grid[key].Add(entity);
        }

        public void RemoveItem(Entity entity)
        {
            var key = GetCellKey(entity.Position);

            if (!_grid.ContainsKey(key))
                return;

            var hash = _grid[key];

            if (hash.Contains(entity))
                hash.Remove(entity);

            if (hash.Count == 0)
                _grid.Remove(key);
        }

        public void UpdateItemCell(Entity entity, Vector2 previewsPosition)
        {
            if (entity.Position == previewsPosition)
                return;

            var oldKey = GetCellKey(previewsPosition);
            var newKey = GetCellKey(entity.Position);

            if (oldKey == newKey)
                return;

            if (!_grid.ContainsKey(newKey))
                _grid.Add(newKey, new HashSet<Entity>());

            _grid[newKey].Add(entity);

            if (_grid.TryGetValue(oldKey, out var hash))
            {
                if (hash.Contains(entity))
                    hash.Remove(entity);

                if (hash.Count == 0)
                    _grid.Remove(oldKey);
            }
        }

        private List<Entity> CalculateNearbyItens(Vector2 position, int? customRadius = null)
        {
            _nearbyCache.Clear();
            var targetKey = GetCellKey(position);
            var radius = customRadius != null ? customRadius.Value : SearchRadius;

            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    var key = new Vector2(targetKey.X + i, targetKey.Y + j);

                    if (_grid.TryGetValue(key, out var itens))
                        _nearbyCache.AddRange(itens);
                }
            }

            return _nearbyCache;
        }

        public List<Entity> GetNearbyItens(Entity entity, int? customRadius = null)
            => CalculateNearbyItens(entity.Position, customRadius);

        public List<Entity> GetNearbyItens(Vector2 position, int? customRadius = null)
            => CalculateNearbyItens(position, customRadius);
    }
}
