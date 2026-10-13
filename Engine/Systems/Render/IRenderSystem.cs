using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinformsEngine.Engine.Systems.General;

namespace WinformsEngine.Engine.Systems.Render
{
    public interface IRenderSystem : IDisposable
    {
        void Draw(Graphics g, Camera camera);
    }
}
