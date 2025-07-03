using Godot;
using System;
using System.Collections.Generic;

namespace MagicSystem
{
    public class ObjectPool
    {
        public List<Node> pool = new List<Node>();
        PackedScene scene;
        int current = 0;
        int max = 3;

        public ObjectPool(string sceneName, int max)
        {
            scene = GlobalData.GetPackedScene(sceneName);
            this.max = max;
        }

        public ObjectPool(PackedScene objectScene, int max)
        {
            scene = objectScene;
            this.max = max;
        }

        ~ObjectPool()
        {
            foreach (Node n in pool)
            {
                n.QueueFree();
            }    
        }

        public virtual Node GetCurrentObject()
        {
            if (pool.Count > current && pool[current] != null)
            {
                return pool[current];
            }
            else if (pool.Count > current && pool[current] == null)
            {
                pool[current] = scene.Instantiate();
                LevelManager.currentLevel.GetTree().Root.AddChild(pool[current]);
                return pool[current];
            }
            else
            {
                pool.Add(scene.Instantiate());
                LevelManager.currentLevel.GetTree().Root.AddChild(pool[current]);
                return pool[current];
            }
        }

        public virtual Node GetNext()
        {
            current++;
            current %= max;
            return GetCurrentObject();
        }
    }
}