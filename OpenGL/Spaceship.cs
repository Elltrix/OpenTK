using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace OpenGL
{
    internal class Spaceship : LinkableObject, IBoundingSphere
    {
        public Accumulator _power { get; set; }
        public float Radius { get; set; } = 1f;
        private Label _label;

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
            GL.Color4(1f, 1f, 1f, 1f);
            foreach (var item in Children)
            {
                item.Draw();
            }
            GL.PopMatrix();
            GL.Color4(1f, 1f, 1f, 1f);
        }

        public override void Update(double time)
        {
            _power.Update(time);
            var powerVal = (int)_power.Value;
            _label.UpdateText(powerVal.ToString());
        }

        public override void Init()
        {
            _power = new Accumulator { Rate = -1, Value = 20, Limit = 20, Enabled = false };
            _label = new Label(new Vector3(0f, 0f, 0f), "0");
            Children.Add(_label);
            foreach (var item in Children)
            {
                item.Init();
            }
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
