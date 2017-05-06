using OpenTK;
using OpenTK.Graphics;
using System.Collections.Generic;

namespace OpenGL
{
    /// <summary>
    /// A node in the scene graph
    /// An object with visual representation in the game world
    /// </summary>
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
        public abstract bool Intersect(Ray ray);

        // location relative to it's parent scene object
        // if no parent then the position is in world coordinates
        public Vector3 Position { get; set; }

        // used for bounding tests
        public float Width { get; set; }
        public float Height { get; set; }
    }
}
