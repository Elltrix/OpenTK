using OpenTK;
using OpenTK.Graphics;

namespace OpenGL
{
    public abstract class UserLine : SceneObject
    {
        public Vector3 To { get; set; }

        public UserLine(Vector3 from, Vector3 to)
            : base(from)
        {
            To = to;
            Color = new Color4(1f, 1f, 0f, 1f);
        }

        
    }
}
