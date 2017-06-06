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
            Width = Height = 1f;
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
            Vector3 ray_dir = (ray.To - ray.From).Normalized();
            var t = Vector3.Dot((ray.From - Position), ray_dir);
            var distanceAlongRay = -t;
            var distanceToSphereOrigin = ((ray.From - Position) - t * ray_dir).Length;
            var isIntersect = distanceToSphereOrigin <= Width;
            return isIntersect;
        }
    }

}
