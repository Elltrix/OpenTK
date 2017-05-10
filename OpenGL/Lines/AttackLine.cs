using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace OpenGL
{
    public class AttackLine : UserLine
    {
        public AttackLine(Vector3 from, Vector3 to) 
            : base(from, to)
        {
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
