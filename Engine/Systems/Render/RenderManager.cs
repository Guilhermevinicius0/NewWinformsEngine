using System;
using System.Collections.Generic;
using System.Drawing;
using WinformsEngine.Engine.Entities;
using WinformsEngine.Engine.Systems.General;
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

        public void Render(Graphics g, Camera camera)
        {
            for (int i = 0; i < _entities.Count; i++)
            {
                var entity = _entities[i];

                if (entity.RenderSystem == null)
                    continue;

                var screenPos = camera.WorldToScreen(entity.Position);
                var screenSize = entity.Size * camera.Zoom;

                bool visible =
                    screenPos.X + screenSize.X > 0 &&
                    screenPos.X < camera.Size.X * camera.Zoom &&
                    screenPos.Y + screenSize.Y > 0 &&
                    screenPos.Y < camera.Size.Y * camera.Zoom;

                if (!visible) continue;

                entity.RenderSystem.Draw(g, camera);
            }

            for (int i = 0; i < _ui.Count; i++)
            {
                var ui = _ui[i];

                if (ui.RenderSystem == null)
                    continue;

                 
                ui.RenderSystem.Draw(g, camera);
            }
        }
    }
}
