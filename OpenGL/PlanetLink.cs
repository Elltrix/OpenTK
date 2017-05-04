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
    internal class PlanetLink : SceneObject
    {
        public Vector3 To { get; set; }

        public PlanetLink(Vector3 from, Vector3 to)
            : base(from)
        {
            To = to;
            Color = new Color4(1f, 0f, 1f, 1f);
        }

        public override void Draw()
        {
            GL.Color4(Color);

            GL.LineWidth(3);

            GL.Begin(PrimitiveType.Lines);

            //GL.Vertex3(Position);

            int dashes = 100;
            float fiftheth = 1f / dashes;
            var diff = To - Position;
            var inc = Vector3.Multiply(diff, fiftheth);
            var start = Vector3.Multiply(inc, (float)_animation);
            var acc = new Vector3(Position.X, Position.Y, Position.Z);
            acc = Vector3.Add(acc, start);

            for (int i = 0; i < dashes; i++)
            {
                //acc = Vector3.Add(acc, inc);
                GL.Vertex3(acc);
                acc = Vector3.Add(acc, inc);
            }

            //GL.Vertex3(To);

            GL.End();
        }

        private double _animation = 0d;

        public override void Update(double time)
        {
            _animation += time;
            if (_animation > 1d)
            {
                _animation = 0.0;
            }
        }

        public override void Init()
        {
            throw new NotImplementedException();
        }
    }
}
