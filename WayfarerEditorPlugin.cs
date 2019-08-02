#if TOOLS

using Godot;
using Wayfarer.Core.Systems;

namespace Wayfarer.Editor
{
    public class WayfarerEditorPlugin : EditorPlugin
    {
        public EditorInterface EditorInterface => GetEditorInterface();
        
        public override void _EnterTree()
        {
            AddEditorSystems();
        }

        public override void _ExitTree()
        {
            
        }

        private void AddEditorSystems()
        {
            Iterator iterator = new Iterator();
            iterator.Name = "EditorIterator";
            EditorInterface.GetBaseControl().AddChild(iterator);
        }

        private void RemoveEditorSystems()
        {
            
        }
    }
}

#endif