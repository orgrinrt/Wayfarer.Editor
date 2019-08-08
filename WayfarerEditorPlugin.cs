#if TOOLS

using System;
using System.Linq;
using Godot;
using Wayfarer.Core.Systems;
using Wayfarer.Core.Systems.Managers;
using Wayfarer.ModuleSystem;
using Wayfarer.Utils.Debug;
using Wayfarer.Utils.Files;
using Wayfarer.Utils.Helpers;
using Object = Godot.Object;

namespace Wayfarer.Editor
{
    [Tool]
    public class WayfarerEditorPlugin : WayfarerModule
    {
        private static WayfarerEditorPlugin _instance;
        private Iterator _iterator;
        private MouseManager _mouseManager;
        
        public static WayfarerEditorPlugin Instance => _instance;
        public EditorInterface EditorInterface => GetEditorInterface();
        public Iterator Iterator => GetIterator();
        public MouseManager MouseManager => GetMouseManager();
        
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
            //RemoveEditorSystems();
            
            Iterator iterator = new Iterator { Name = "EditorIterator" };
            try
            {
                EditorInterface.GetBaseControl().AddChild(iterator);
                _iterator = iterator;
            }
            catch (Exception e)
            {
                Log.Wf.EditorError("Couldn't add EditorIterator to the base node of editor...?", e, true);
            }
            
            Log.Wf.Editor("Iterator added!", true);

            MouseManager mouseManager = new MouseManager() { Name = "MouseManager" };
            try
            {
                EditorInterface.GetBaseControl().AddChild(mouseManager);
                _mouseManager = mouseManager;
            }
            catch (Exception e)
            {
                Log.Wf.EditorError("Couldn't add MouseManager to the base node of editor...?", e, true);
            }
        }

        public MouseManager GetMouseManager()
        {
            if (!IsInstanceValid(_mouseManager) || _mouseManager == null)
            {
                Node baseControl = EditorInterface.GetBaseControl();
                Godot.Collections.Array children = baseControl.GetChildren();
                Godot.Collections.Array mouseManagers = new Godot.Collections.Array();
                MouseManager mouseManager;

                foreach (Node child in children)
                {
                    if (child is MouseManager i)
                    {
                        mouseManagers.Add(i);
                    }
                }

                if (mouseManagers.Count > 1)
                {
                    Log.Wf.EditorError("There were MULTIPLE MOUSEMANAGERS, this shouldn't happen (" + mouseManagers.Count + ")",
                        true);
                    mouseManager = (MouseManager) mouseManagers.Last();

                    for (int i = 0; i < mouseManagers.Count - 1; i++)
                    {
                        MouseManager mm = (MouseManager) mouseManagers[i];
                        try
                        {
                            mm.Name = "BeingFreed";
                            mm.QueueFree();
                        }
                        catch (Exception e)
                        {
                            Log.Wf.EditorError("Couldn't QueueFree the extra MouseManagers...?", e, true);
                        }
                    }

                    Log.Wf.Simple("    ...but we managed to remove the extra MouseManagers, so it's fine!", true);
                }
                else
                {
                    mouseManager = (MouseManager) mouseManagers[0];
                }

                if (mouseManager == null)
                {
                    mouseManager = baseControl.GetNodeOfType<MouseManager>();
                }

                mouseManager.Name = "MouseManager";

                _mouseManager = mouseManager;
            }
            
            return _mouseManager;
        }

        public Iterator GetIterator()
        {
            if (!IsInstanceValid(_iterator) || _iterator == null)
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
                    Log.Wf.EditorError("There were MULTIPLE ITERATORS, this shouldn't happen (" + iterators.Count + ")",
                        true);
                    iterator = (Iterator) iterators.Last();

                    for (int i = 0; i < iterators.Count - 1; i++)
                    {
                        Iterator it = (Iterator) iterators[i];
                        try
                        {
                            it.Name = "BeingFreed";
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

                _iterator = iterator;
            }
            
            return _iterator;
        }

        public void RemoveEditorSystems()
        {
            try
            {
                RemoveOldIterator();
            }
            catch (Exception e)
            {
                Log.Wf.EditorError("Couldn't remove the old iterator", e, true);
            }
            
            try
            {
                RemoveOldMouseManager();
            }
            catch (Exception e)
            {
                Log.Wf.EditorError("Couldn't remove the old MouseManager", e, true);
            }
        }

        private void RemoveOldMouseManager()
        {
            try
            {
                Node editorBase = EditorInterface.GetBaseControl();
                
                foreach (Node node in editorBase.GetChildren())
                {
                    if (node is MouseManager)
                    {
                        try
                        {
                            node.QueueFree();
                            Log.Wf.Editor("Removed old MouseManager (QueueFree)", true);
                        }
                        catch (Exception e)
                        {
                            Log.Wf.EditorError("Tried to QueueFree() MouseManager, but couldn't", e, true);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Wf.EditorError("Couldn't remove the old MouseManager", e, true);
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