using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace OpenGL
{
    internal class Spaceship : SceneObject, IBoundingSphere
    {
        public Accumulator Power { get; set; }

        public float Radius { get; set; } = 1f;

        public Spaceship(Vector3 position)
            : base(position)
        {
            Power = new Accumulator
            {
                Rate = 1,
                Value = 0,
                Limit = 10
            };
        }

        public override void Draw()
        {
            GL.PushMatrix();
            GL.Translate(Position);

            GL.Color3(0f, 1f, 0f);
            GL.Begin(PrimitiveType.Triangles);
            GL.Vertex2(-1.0f, -1.0f);
            GL.Vertex2(1.0f, -1.0f);
            GL.Vertex2(0.0f, 1.0f);
            GL.End();

            GL.PopMatrix();
        }

        public override void Update(double time)
        {
            Power.Update(time);
        }

        public override void Init()
        {
        }
    }

}
