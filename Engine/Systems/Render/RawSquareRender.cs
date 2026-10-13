using WinformsEngine.Engine.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using WinformsEngine.Engine.Systems.General;

namespace WinformsEngine.Engine.Systems.Render
{
    public class RawSquareRender : IEntityRenderSystem
    {
        public bool Show_Wires = false;
        private readonly Entity _owner;
        public Entity Owner { get => _owner; }

        private Brush _backColorBrush;
        private Pen _backColorPen;

        private Color _backColor;
        public Color BackColor { get => _backColor; set 
            {
                if (value == _backColor) return;

                _backColorBrush?.Dispose();
                _backColorPen?.Dispose();

                _backColor = value;
                _backColorBrush = new SolidBrush(value);
                _backColorPen = new Pen(value);
            } 
        }

        public RawSquareRender(Entity entity, Color color) 
        {
            _owner = entity;
            BackColor = color;
        }

        public void Draw(Graphics g, Camera camera)
        {
            if (_owner.Size == Vector2.Zero)
                return;

            var position = camera.WorldToScreen(_owner.Position);
            
            float left = position.X;
            float top = position.Y;
            float right = left + _owner.Size.X;
            float bottom = top + _owner.Size.Y;
            float width = _owner.Size.X * camera.Zoom;
            float height = _owner.Size.Y * camera.Zoom;


            if (Show_Wires) 
            {
                g.DrawLine(_backColorPen, left, top, width, top);
                g.DrawLine(_backColorPen, width, top, width, height);
                g.DrawLine(_backColorPen, width, height, left, height);
                g.DrawLine(_backColorPen, left, height, left, top);
                return;
            }

            g.FillRectangle(_backColorBrush, left, top, width, height);
        }

        public void Dispose()
        {
            _backColorBrush?.Dispose();
            _backColorPen?.Dispose();
        }
    }
}