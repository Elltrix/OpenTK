using OpenTK;
using OpenTK.Graphics;
using System.Collections.Generic;

namespace OpenGL
{
    internal abstract class SceneObject
    {
        public List<SceneObject> Children { get; set; }
            = new List<SceneObject>();

        public Color4 Color { get; set; }
        
        public SceneObject(Vector3 position)
        {
            Position = position;
        }
        public abstract void Draw();
        public abstract void Update(double time);
        public abstract void Init();

        public Vector3 Position { get; set; }
    }
}
