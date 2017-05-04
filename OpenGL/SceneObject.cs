using OpenTK;
using OpenTK.Graphics;

namespace OpenGL
{
    internal abstract class SceneObject
    {
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
