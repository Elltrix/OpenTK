using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace OpenGL
{
    internal class LinePrimitive: SceneObject
    {
        public Vector3 To { get; set; }

        public LinePrimitive(Vector3 from, Vector3 to)
            : base(from)
        {
            To = to;
        }

        public override void Draw()
        {
            GL.Begin(PrimitiveType.Lines);

            GL.Vertex3(Position);

            GL.Vertex3(To);

            GL.End();
        }
    }

    internal class CirclePrimitive : SceneObject
    {
        public CirclePrimitive(Vector3 position)
            : base(position) { }

        public override void Draw()
        {
            GL.PushMatrix();
            GL.Translate(Position);

            var radius = 1f;
            var slices = 50;
            var twicePi = 2f * Math.PI;

            GL.Color3(0f, 0f, 1f);
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Vertex2(0f,0f);
            for (int i = 0; i <= slices; i++)
            {
                GL.Vertex2(
                    radius * Math.Cos(i * twicePi / slices),
                    radius * Math.Sin(i * twicePi / slices));
            }

            GL.End();

            GL.PopMatrix();
        }
    }

    internal abstract class SceneObject
    {
        public SceneObject(Vector3 position)
        {
            Position = position;
        }
        public abstract void Draw();

        public Vector3 Position { get; set; }
    }

    internal class Scene
    {
        List<SceneObject> _scene
            = new List<SceneObject>();

        public void Add(SceneObject obj)
        {
            _scene.Add(obj);
        }

        public void Remove(SceneObject obj)
        {
            _scene.Remove(obj);
        }

        public void Draw()
        {
            foreach (var obj in _scene)
            {
                obj.Draw();
            }
        }
    }

    internal class Game : GameWindow
    {
        private Matrix4 projection;
        private Matrix4 modelview;

        Scene scene = new Scene();

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Title = "Hello World";

            GL.ClearColor(Color4.CornflowerBlue);

            Location = new System.Drawing.Point(50, 500);
            Size = new System.Drawing.Size(1024, 768);
            
            scene.Add(new CirclePrimitive(new Vector3(4f, 4f, 0f)));
            scene.Add(new CirclePrimitive(new Vector3(-4f, -4f, 0f)));
            scene.Add(new CirclePrimitive(new Vector3(4f, -4f, 0f)));
        }

        private static void Triangle()
        {
            GL.Color3(0f, 1f, 0f);
            GL.Begin(PrimitiveType.Triangles);
            GL.Vertex2(-1.0f, -1.0f);
            GL.Vertex2(1.0f, -1.0f);
            GL.Vertex2(0.0f, 1.0f);
            GL.End();
        }

        private static void Square()
        {
            GL.Color3(1f, 0f, 0f);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(-1f, -1f);
            GL.Vertex2(1f, -1f);
            GL.Vertex2(1f, 1f);
            GL.Vertex2(-1f, 1f);
            GL.End();
        }

        private static void Point()
        {
            GL.PointSize(5);
            GL.Begin(PrimitiveType.Points);
            GL.Color3(1.0, 0.0, 0.0);
            GL.Vertex2(0f, 0f);
            GL.End();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var eye = Vector3.Zero;
            eye.Z = 16;

            modelview = Matrix4.LookAt(eye, Vector3.UnitZ, Vector3.UnitY);

            GL.MatrixMode(MatrixMode.Modelview);

            GL.LoadMatrix(ref modelview);

            scene.Draw();

            SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            projection = Matrix4.CreatePerspectiveFieldOfView(
                (float)Math.PI / 4, Width / (float)Height, 1.0f, 20.0f);

            GL.MatrixMode(MatrixMode.Projection);

            GL.LoadMatrix(ref projection);
        }
        
        LinePrimitive line = null;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (line == null)
            {
                var worldCoord = ClickToWorld(new Vector2(e.X, e.Y));
                worldCoord.Z = 14.999f;

                line = new LinePrimitive(worldCoord, worldCoord);
                scene.Add(line);
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (line != null)
            {
                scene.Remove(line);
                line = null;
            }
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);

            if (line != null)
            {
                var worldCoord = ClickToWorld(new Vector2(e.X, e.Y));
                worldCoord.Z = 14.999f;
                line.To = worldCoord;
            }
        }

        private Vector3 ClickToWorld(Vector2 p)
        {
            var vec = new Vector4(
                p.X / (float)this.Width * 2f - 1f,
                -(p.Y / (float)this.Height * 2f - 1f),
                0f, 1f);

            var viewInv = Matrix4.Invert(modelview);
            var projInv = Matrix4.Invert(projection);

            Vector4.Transform(ref vec, ref projInv, out vec);
            Vector4.Transform(ref vec, ref viewInv, out vec);

            return new Vector3(vec.X, vec.Y, vec.Z);
        }

        private string Format(float value)
        {
            return Math.Round(value, 2).ToString().PadLeft(8);
        }

        private void Log(Vector3 vector)
        {
            Console.WriteLine($"{Format(vector.X)} {Format(vector.Y)} {Format(vector.Z)}");
        }
    }
}
