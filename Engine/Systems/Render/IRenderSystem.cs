using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformsEngine.Engine.Systems.Render
{
    public interface IRenderSystem : IDisposable
    {
        void Draw(Graphics g);
    }
}
