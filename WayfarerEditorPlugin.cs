#if TOOLS

using System;
using System.Linq;
using Godot;
using Wayfarer.Core.Systems;
using Wayfarer.ModuleSystem;
using Wayfarer.Utils.Debug;
using Wayfarer.Utils.Files;
using Wayfarer.Utils.Helpers;

namespace Wayfarer.Editor
{
    [Tool]
    public class WayfarerEditorPlugin : WayfarerModule
    {
        private static WayfarerEditorPlugin _instance;
        public static WayfarerEditorPlugin Instance => _instance;
        
        public EditorInterface EditorInterface => GetEditorInterface();
        
        public override void _EnterTreeSafe()
        {
            _instance = this;
            
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
        
        public override void _ExitTreeSafe()
        {
            try
            {
                RemoveEditorSystems();
            }
            catch (Exception e)
            {
                Log.Wf.EditorError("Couldn't remove old Editor systems", e, true);
            }
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

        public Iterator GetIterator()
        {
            Node baseControl = EditorInterface.GetBaseControl();
            Godot.Collections.Array children = baseControl.GetChildren();
            Godot.Collections.Array iterators = new Godot.Collections.Array();
            Iterator iterator;

            foreach (Node child in children)
            {
                if (child is Iterator i)
                {
                    iterators.Add(i);
                }
            }
                
            if (iterators.Count > 1)
            {
                Log.Wf.EditorError("There were MULTIPLE ITERATORS, this shouldn't happen (" + iterators.Count + ")", true);
                iterator = (Iterator) iterators.Last();

                for (int i = 0; i < iterators.Count - 1; i++)
                {
                    Iterator it = (Iterator) iterators[i];
                    try
                    {
                        it.QueueFree();
                    }
                    catch (Exception e)
                    {
                        Log.Wf.EditorError("Couldn't QueueFree the extra iterators...?", e, true);
                    }
                }
                    
                Log.Wf.Simple("    ...but we managed to remove the extra iterators, so it's fine!", true);
            }
            else
            {
                iterator = (Iterator) iterators[0];
            }

            if (iterator == null)
            {
                iterator = baseControl.GetNodeOfType<Iterator>();
            }

            iterator.Name = "EditorIterator";

            return iterator;
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