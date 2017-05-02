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

    internal interface IBoundingSphere
    {
        Vector3 Position { get; set; }
        float Radius { get; set; }
    }

    internal class Spaceship : SceneObject, IBoundingSphere
    {
        public float Radius { get; set; } = 1f;

        public Spaceship(Vector3 position)
            : base(position)
        {
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
        }
    }

    internal class Planet : SceneObject, IBoundingSphere
    {
        public bool Highlighted { get; private set; } = false;
        public float Radius { get; set; } = 1.0f;

        public readonly Color4 DefaultColour = new Color4(0f, 0f, 1f, 1f);

        public Planet(Vector3 position)
            : base(position)
        {
            Color = DefaultColour;
        }

        public override void Draw()
        {
            GL.PushMatrix();
            GL.Translate(Position);

            var slices = 50;
            var twicePi = 2f * Math.PI;

            GL.Color4(Color);
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Vertex2(0f, 0f);
            for (int i = 0; i <= slices; i++)
            {
                GL.Vertex2(
                    Radius * Math.Cos(i * twicePi / slices),
                    Radius * Math.Sin(i * twicePi / slices));
            }

            GL.End();

            GL.PopMatrix();
        }

        public override void Update(double time)
        {
        }

        public void Highlight()
        {
            Highlighted = true;
            Color = new Color4(1f, 1f, 0f, 1f);
        }
        public void UnHighlight()
        {
            Highlighted = false;
            Color = DefaultColour;
        }
    }



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
    }

    internal abstract class SceneObject
    {
        public Color4 Color { get; set; }


        public SceneObject(Vector3 position)
        {
            Position = position;
        }
        public abstract void Draw();
        public abstract void Update(double time);

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

        public void Update(double time)
        {
            foreach (var obj in _scene)
            {
                obj.Update(time);
            }
        }

        public IEnumerable<SceneObject> Objects { get { return _scene; } }

        public void Compare(Matrix4 modelview, Vector3 ray)
        {
            foreach (var item in _scene)
            {
                if (item is Planet)
                {
                    // ray is already in world coords, so is comparable to item positions.
                    // only ray is pointing in the wrong direction, i.e. from world origin to the near plane
                    Vector4 pos = new Vector4(item.Position.X, item.Position.Y, item.Position.Z, 0f);
                    Vector4.Transform(ref modelview, ref pos, out pos);

                    var nCircle = new Vector3(pos.X, pos.Y, -16f).Normalized();
                    var nRay = ray.Normalized();
                    var dot = Vector3.Dot(nCircle, nRay);
                    var angle = Math.Acos(dot);
                }
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

            scene.Add(new Planet(new Vector3(4f, 4f, 0f)));
            scene.Add(new Planet(new Vector3(-4f, -4f, 0f)));
            scene.Add(new Planet(new Vector3(4f, -4f, 0f)));
            scene.Add(new Planet(new Vector3(-4f, 4f, 0f)));
            scene.Add(new Spaceship(new Vector3(-8f, 0f, 0f)));
            scene.Add(new Spaceship(new Vector3(8f, 0f, 0f)));
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

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            scene.Update(e.Time);
        }




        private SceneObject IntersectWithScene(float x, float y, int width, int height)
        {
            if (modelview == Matrix4.Zero || projection == Matrix4.Zero)
                return null;

            var p1 = Unproject(new Vector3(x, y, -1.5f), width, height);
            var p2 = Unproject(new Vector3(x, y, 1.0f), width, height);

            Vector3 ray_pos = p1;
            Vector3 ray_dir = (p2 - p1).Normalized();

            foreach (IBoundingSphere sphere in 
                scene.Objects.Where(c => c is IBoundingSphere))
            {
                var t = Vector3.Dot((ray_pos - sphere.Position), ray_dir);
                var distanceAlongRay = -t;
                var distanceToSphereOrigin = ((ray_pos - sphere.Position) - t * ray_dir).Length;
                var isIntersect = distanceToSphereOrigin <= sphere.Radius;

                if (isIntersect)
                {
                    return (SceneObject)sphere;
                }
            }
            return null;
        }

        private Vector3 Unproject(Vector3 mouse, int width, int height)
        {
            if (modelview == Matrix4.Zero || projection == Matrix4.Zero)
                return Vector3.Zero;

            Vector4 vec;

            vec.X = 2.0f * mouse.X / (float)width - 1;
            vec.Y = -(2.0f * mouse.Y / (float)height - 1);
            vec.Z = mouse.Z;
            vec.W = 1.0f;

            Matrix4 viewInv = Matrix4.Invert(modelview);
            Matrix4 projInv = Matrix4.Invert(projection);

            Vector4.Transform(ref vec, ref projInv, out vec);
            Vector4.Transform(ref vec, ref viewInv, out vec);

            if (vec.W > float.Epsilon || vec.W < -float.Epsilon)
            {
                vec.X /= vec.W;
                vec.Y /= vec.W;
                vec.Z /= vec.W;
            }

            return new Vector3(vec.X, vec.Y, vec.Z);
        }

        public void DetectPlanet(float mouseX, float mouseY)
        {
            var sceneObject = IntersectWithScene(
                mouseX, mouseY, ClientRectangle.Width, ClientRectangle.Height);

            if (sceneObject != _mouseOver)
            {
                // if changed

                if (sceneObject != null)
                {
                    // if moved onto a planet
                    _mouseOver = sceneObject;
                    //_mouseOver.Highlight();

                }
                else
                {
                    // if moved off a planet

                    if (_attack != null)
                    {
                        // if moving towards another planet

                        if (_attackFrom != _mouseOver)
                        {
                            //_mouseOver.UnHighlight();
                        }
                    }
                    else
                    {
                        //_mouseOver.UnHighlight();
                    }

                    _mouseOver = null;
                }
            }
        }

        bool _mouseDown = false;
        PlanetLink _attack = null;
        SceneObject _attackFrom = null;
        SceneObject _mouseOver = null;

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            DetectPlanet(e.X, e.Y);

            if (_attack != null)
            {
                var worldCoord = ClickToWorld(new Vector2(e.X, e.Y));
                worldCoord.Z = 14.999f; // just in front of the near plane
                _attack.To = worldCoord;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            _mouseDown = true;

            if (_mouseOver != null)
            {
                _attack = new PlanetLink(_mouseOver.Position, _mouseOver.Position);
                _attackFrom = _mouseOver;
                scene.Add(_attack);
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            _mouseDown = false;

            if (_attack != null)
            {
                if (_mouseOver != null)
                {
                    if (_mouseOver != _attackFrom)
                    {
                        _attack.To = _mouseOver.Position;
                    }
                    else
                    {
                        scene.Remove(_attack);
                    }
                }
                else
                {
                    scene.Remove(_attack);
                }

                _attack = null;

                //_attackFrom.UnHighlight();
                _attackFrom = null;
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
