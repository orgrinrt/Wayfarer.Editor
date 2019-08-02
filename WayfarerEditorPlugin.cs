#if TOOLS

using System;
using Godot;
using Wayfarer.Core.Systems;
using Wayfarer.Core.Utils.Debug;
using Wayfarer.Core.Utils.Files;
using Wayfarer.Core.Utils.Helpers;

namespace Wayfarer.Editor
{
    [Tool]
    public class WayfarerEditorPlugin : EditorPlugin
    {
        public EditorInterface EditorInterface => GetEditorInterface();
        
        public override void _EnterTree()
        {
            EnablePlugin();
            
            try
            {
                Log.Initialize();
            }
            catch (Exception e)
            {
                Log.Wayfarer.Error("Couldn't initialize Log (static)", e, true);
            }
            try
            {
                Directories.Initialize();
            }
            catch (Exception e)
            {
                Log.Wayfarer.Error("Couldn't initialize Directories (static)", e, true);
            }

            try
            {
                AddEditorSystems();
            }
            catch (Exception e)
            {
                Log.Wayfarer.Error("Couldn't AddEditorSystems", e, true);
            }
        }
        
        public override void _ExitTree()
        {
            try
            {
                RemoveEditorSystems();
            }
            catch (Exception e)
            {
                Log.Wf.EditorError("Couldn't remove old Editor systems", e, true);
            }

            DisablePlugin();
        }

        private void AddEditorSystems()
        {
            RemoveEditorSystems();
            
            Iterator iterator = new Iterator { Name = "EditorIterator" };
            try
            {
                EditorInterface.GetBaseControl().AddChild(iterator);
            }
            catch (Exception e)
            {
                Log.Wf.EditorError("Couldn't add EditorIterator to the base node of editor...?", e, true);
            }
            
            Log.Wf.Editor("Iterator added!", true);
        }

        public void RemoveEditorSystems()
        {
            try
            {
                RemoveOldIterator();
            }
            catch (Exception e)
            {
                Log.Wf.EditorError("Couldn't remove the old systems", e, true);
            }
        }
        
        private void RemoveOldIterator()
        {
            try
            {
                Node editorBase = EditorInterface.GetBaseControl();
                
                foreach (Node node in editorBase.GetChildren())
                {
                    if (node is Iterator)
                    {
                        try
                        {
                            node.QueueFree();
                            Log.Wf.Editor("Removed old Iterator (QueueFree)", true);
                        }
                        catch (Exception e)
                        {
                            Log.Wf.EditorError("Tried to QueueFree() Iterator, but couldn't", e, true);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Wf.EditorError("Couldn't remove the old iterator", e, true);
            }
        }
    }
}

#endif