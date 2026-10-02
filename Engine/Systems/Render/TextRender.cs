using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WinformsEngine.Engine.Entities;
using WinformsEngine.Engine.UI;

namespace WinformsEngine.Engine.Systems.Render
{
    public class TextRender : IUIRenderSystem
    {
        private readonly object _lock = new object();
        
        private UIItem _owner;
        public UIItem Owner { get => _owner; } 


        public Font Font;

        private Brush _textBrush;
        private Color _textColor;

        private string _content = string.Empty;
        public string Content 
        {
            get
            {
                lock (_lock) 
                    return _content;   
            }

            set 
            {
                if (value == _content) return;
                
                lock (_lock) 
                    _content = value;
            }
        }

        public Color TextColor 
        {
            get => _textColor;
            set 
            {
                if (value == _textColor) return;
                _textBrush?.Dispose();
                _textColor = value;
                _textBrush = new SolidBrush(value);
            }
        }

      

        public TextRender(Font font, Color textColor, UIItem owner) 
        {
            Font = font;
            TextColor = textColor;
            _owner = owner;
        }

        public void Dispose()
        {
            _textBrush?.Dispose();
        }

        public void Draw(Graphics g)
        {
            lock (_lock) 
                g.DrawString(_content, Font, _textBrush, _owner.Position.X, _owner.Position.Y);       
        }
    }
}