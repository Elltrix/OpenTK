using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace OpenGL
{
    internal class Spaceship : LinkableObject, IBoundingSphere
    {
        public float Radius { get; set; } = 1f;

        public Spaceship(Vector3 position)
            : base(position)
        {
        }

        public override void Draw()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.PushMatrix();
            GL.Translate(Position);

            GL.Color4(0f, 1f, 0f, 1f);
            GL.Begin(PrimitiveType.Triangles);
            GL.Vertex2(-1.0f, -1.0f);
            GL.Vertex2(1.0f, -1.0f);
            GL.Vertex2(0.0f, 1.0f);
            GL.End();

            GL.PopMatrix();
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
