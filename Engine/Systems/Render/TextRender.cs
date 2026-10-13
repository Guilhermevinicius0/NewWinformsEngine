using System.Drawing;
using WinformsEngine.Engine.Systems.General;
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

        public void Draw(Graphics g, Camera camera)
        {
            lock (_lock)
            {
                if (_owner.IsFixedToScreen)
                {
                    g.DrawString(_content, Font, _textBrush, _owner.Position.X, _owner.Position.Y);
                    return;
                }

                var position = camera.WorldToScreen(_owner.Position);
                g.DrawString(_content, Font, _textBrush, position.X, position.Y);
            }
        }
    }
}