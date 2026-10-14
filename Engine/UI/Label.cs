using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WinformsEngine.Engine.Systems.Render;
using WinformsEngine.Engine.Systems.Scene;

namespace WinformsEngine.Engine.UI
{
    public class Label : UIItem
    {
      

        public TextRender TextRender
        {
            get 
            {
                return (RenderSystem is TextRender text ? text : null);
            }
        }

        private string _content { get; set; }
        public string Content 
        {
            get => _content;
            set 
            {
                if (value == _content) return;
                _content = value;

                if (TextRender != null)
                    TextRender.Content = _content;
            }
        }

        private Font _font { get; set; }

        public Font Font 
        {
            get => _font;
            set 
            {
                if (_font == value) return;
                _font = value;

                if (TextRender != null)
                    TextRender.Font = _font;
            }
        }

        private Color _textColor { get; set; }
        public Color TextColor 
        {
            get => _textColor;
            set 
            {
                if (value == _textColor) return;

                _textColor = value;
                if (TextRender != null)
                    TextRender.TextColor = _textColor;
            }
        }

        public override int ZIndex { get; set; } = 1;
        public override bool IsFixedToScreen { get; set; } = true;

        public Label(Font font, Color textColor, Scene scene) : base(scene) 
        {
            _font = font;
            _textColor = textColor;
            IsVisible = true;
            RenderSystem = new TextRender(font, textColor, this);
        }

        public void Dispose()
        {
            RenderSystem?.Dispose();
        }
    }
}