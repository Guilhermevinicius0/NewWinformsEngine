using WinformsEngine.Engine.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

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

        public void Draw(Graphics g)
        {
            if (_owner.Size == Vector2.Zero)
                return;

            if (Show_Wires) 
            {
                g.DrawLine(_backColorPen, _owner.Left, _owner.Top, _owner.Right, _owner.Top);
                g.DrawLine(_backColorPen, _owner.Right, _owner.Top, _owner.Right, _owner.Bottom);
                g.DrawLine(_backColorPen, _owner.Right, _owner.Bottom, _owner.Left, _owner.Bottom);
                g.DrawLine(_backColorPen, _owner.Left, _owner.Bottom, _owner.Left, _owner.Top);
                return;
            }

            g.FillRectangle(_backColorBrush, _owner.Position.X, _owner.Position.Y, _owner.Size.X, _owner.Size.Y);
        }

        public void Dispose()
        {
            _backColorBrush?.Dispose();
            _backColorPen?.Dispose();
        }
    }
}
