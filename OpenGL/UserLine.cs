using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL
{
    internal class UserLine : SceneObject
    {
        public Vector3 To { get; set; }

        public UserLine(Vector3 from, Vector3 to)
            : base(from)
        {
            To = to;
            Color = new Color4(1f, 1f, 0f, 1f);
        }

        public override void Draw()
        {

            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.Color4(Color);

            GL.LineWidth(3);

            GL.Begin(PrimitiveType.Lines);

            GL.Vertex3(Position);

            GL.Vertex3(To);

            GL.End();
        }

        public override void Update(double time)
        {
        }

        public override void Init()
        {
        }

        public override bool Intersect(Ray ray)
        {
            return false;
        }
    }
}
