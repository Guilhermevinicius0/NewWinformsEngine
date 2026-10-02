using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WinformsEngine.Engine.Systems;
using WinformsEngine.Engine.Systems.Render;

namespace WinformsEngine.Engine.UI
{
    public abstract class UIItem
    {
        public Vector2 Position { get; set; }
        public bool IsVisible { get; set; }
        public abstract int ZIndex { get; set; }
        public IRenderSystem RenderSystem {get;set;}

        private Scene _scene;
        private HashSet<string> Tags { get; set; } = new HashSet<string>();
       
        public UIItem(Scene scene)
        {
            _scene = scene;
        }
            
        public List<string> SeeAllTags()
            => Tags.ToList();

        public bool IsTaggedWith(string tag)
            => Tags.Contains(tag);

        public void AddTag(string tag)
        {
            Tags.Add(tag);
            _scene.AddTaggedUI(tag, this);
        }

        public void RemoveTag(string tag)
        {
            if (Tags.Contains(tag))
                Tags.Remove(tag);

            _scene.RemoveTaggedUI(tag, this);
        }
    }
}
