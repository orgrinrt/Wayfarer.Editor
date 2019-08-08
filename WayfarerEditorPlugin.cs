﻿#if TOOLS

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
            GetIterator();
            GetMouseManager();
        }

        public MouseManager GetMouseManager()
        {
            if (!IsInstanceValid(_mouseManager) || _mouseManager == null)
            {
                MouseManager mouseManager = new MouseManager() { Name = "MouseManager" };
                try
                {
                    AddChild(mouseManager);
                    _mouseManager = mouseManager;
                }
                catch (Exception e)
                {
                    Log.Wf.EditorError("Couldn't add MouseManager to the base node of editor...?", e, true);
                }
            }
            
            return _mouseManager;
        }

        public Iterator GetIterator()
        {
            if (!IsInstanceValid(_iterator) || _iterator == null)
            {
                Iterator iterator = new Iterator { Name = "EditorIterator" };
                try
                {
                    AddChild(iterator);
                    _iterator = iterator;
                }
                catch (Exception e)
                {
                    Log.Wf.EditorError("Couldn't add EditorIterator to the base node of editor...?", e, true);
                }
            }
            
            return _iterator;
        }

        public void RemoveEditorSystems()
        {
            try
            {
                _iterator.QueueFree();
            }
            catch (Exception e)
            {
                Log.Wf.EditorError("Couldn't remove the old iterator", e, true);
            }
            
            try
            {
                _mouseManager.QueueFree();
            }
            catch (Exception e)
            {
                Log.Wf.EditorError("Couldn't remove the old MouseManager", e, true);
            }
        }
    }
}

#endif