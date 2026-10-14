using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinformsEngine.Engine.UI;

namespace WinformsEngine.Engine.Systems.Scene
{
    public partial class Scene
    {
        #region Fields
        
        private readonly HashSet<UIItem> UIItems = new HashSet<UIItem>();
        private readonly Dictionary<string, HashSet<UIItem>> _taggedUIElements = new Dictionary<string, HashSet<UIItem>>();
        
        private readonly Queue<UIItem> _toAddUI = new Queue<UIItem>();
        private readonly HashSet<UIItem> _toAddSetUI = new HashSet<UIItem>();

        private readonly Queue<UIItem> _toRemoveUI = new Queue<UIItem>();
        private readonly HashSet<UIItem> _toRemoveSetUI = new HashSet<UIItem>();

        #endregion

        #region Methods

        public void AddUIElement(UIItem element)
        {
            if (element == null || !_toAddSetUI.Add(element))
                return;

            _toAddUI.Enqueue(element);
        }

        public void RemoveUIElement(UIItem element)
        {
            if (element == null || !_toRemoveSetUI.Add(element))
                return;

            _toRemoveUI.Enqueue(element);
        }

        public void ProcessUIQueues()
        {
            int interactions = 0;

            while (_toRemoveUI.Count > 0 && interactions < MAX_QUEUE_INTERACTIONS)
            {
                var element = _toRemoveUI.Dequeue();
                var tags = element.SeeAllTags();

                if (tags != null && tags.Count > 0)
                {
                    for (int i = 0; i < tags.Count; i++)
                        RemoveTaggedUI(tags[i], element);
                }

                _toRemoveSetUI.Remove(element);
                UIItems.Remove(element);
                _renderManager.RemoveUIElement(element);
                interactions++;
            }

            while (_toAddUI.Count > 0 && interactions < MAX_QUEUE_INTERACTIONS)
            {
                var element = _toAddUI.Dequeue();
                var tags = element.SeeAllTags();

                if (tags != null && tags.Count > 0)
                {
                    for (int i = 0; i < tags.Count; i++)
                        AddTaggedUI(tags[i], element);
                }

                _toAddSetUI.Remove(element);
                UIItems.Add(element);
                _renderManager.AddUIElement(element);
                interactions++;
            }
        }

        public void AddTaggedUI(string tag, UIItem element)
        {
            if (!_taggedUIElements.TryGetValue(tag, out var set))
            {
                set = new HashSet<UIItem>();
                _taggedUIElements.Add(tag, set);
            }

            set.Add(element);
        }

        public void RemoveTaggedUI(string tag, UIItem element)
        {
            if (!_taggedUIElements.TryGetValue(tag, out var hash)) return;
            hash.Remove(element);
            if (hash.Count == 0) _taggedUIElements.Remove(tag);
        }

        #endregion
    }
}
