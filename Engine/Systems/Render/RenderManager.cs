using System;
using System.Collections.Generic;
using System.Drawing;
using WinformsEngine.Engine.Entities;
using WinformsEngine.Engine.UI;

namespace WinformsEngine.Engine.Systems.Render
{
    public class RenderManager
    {
        private readonly List<Entity> _entities = new List<Entity>();
        private readonly List<UIItem> _ui = new List<UIItem>();

        public void AddEntity(Entity entity)
        {
            int index = _entities.BinarySearch(entity, Comparer<Entity>
                .Create((a, b) => (a.ZIndex)
                .CompareTo(b.ZIndex)));

            if (index < 0)
                index = ~index;

            _entities.Insert(index, entity);
        }

        public void RemoveEntity(Entity entity)
        {
            _entities.Remove(entity);
        }

        public void AddUIElement(UIItem element)
        {
            int index = _ui.BinarySearch(element, Comparer<UIItem>
                .Create((a, b) => (a.ZIndex)
                .CompareTo(b.ZIndex)));

            if (index < 0)
                index = ~index;

            _ui.Insert(index, element);

        }

        public void RemoveUIElement(UIItem element)
        {
            _ui.Remove(element);
        }

        public void Render(Graphics g)
        {
            for (int i = 0; i < _entities.Count; i++)
            {
                var entity = _entities[i];

                if (entity.RenderSystem == null)
                    continue;

                entity.RenderSystem.Draw(g);
            }

            for (int i = 0; i < _ui.Count; i++)
            {
                var ui = _ui[i];

                if (ui.RenderSystem == null)
                    continue;

                ui.RenderSystem.Draw(g);
            }
        }
    }
}
