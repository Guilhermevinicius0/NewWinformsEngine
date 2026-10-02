using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinformsEngine.Engine.UI;

namespace WinformsEngine.Engine.Systems.Render
{
    public interface IUIRenderSystem : IRenderSystem
    {
         UIItem Owner { get; }
    }
}
