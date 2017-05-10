using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Linq;

namespace OpenGL
{
    internal class Game : GameWindow
    {
        private Matrix4 projection;
        private Matrix4 modelview;

        Scene scene = new Scene();
        

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Title = "Hello World";


            GL.Enable(EnableCap.Texture2D);

            GL.ClearColor(Color4.CornflowerBlue);

            Location = new System.Drawing.Point(50, 500);
            Size = new System.Drawing.Size(1024, 768);

            scene.Add(new Spaceship(new Vector3(-8f, 0f, 0f)));
            scene.Add(new Spaceship(new Vector3(8f, 0f, 0f)));

            scene.Add(new Planet(new Vector3(4f, 4f, 0f)));
            scene.Add(new Planet(new Vector3(-4f, -4f, 0f)));
            scene.Add(new Planet(new Vector3(4f, -4f, 0f)));
            scene.Add(new Planet(new Vector3(-4f, 4f, 0f)));
            
            scene.Init();
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


            //GL.Disable(EnableCap.Blend);
            //Blt(10, 40, TextureWidth, TextureHeight);
            GL.Enable(EnableCap.Blend);


            
            //GL.Enable(EnableCap.Blend);
            //GL.PushMatrix();
            //GL.Scale(0.05, 0.05, 0.05);
            //DrawText(0, 0, "Test");
            //GL.PopMatrix();
            //GL.Disable(EnableCap.Blend);
            //GL.Disable(EnableCap.Texture2D);




            scene.Draw();

            SwapBuffers();

            _ready = true;
        }


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            projection = Matrix4.CreatePerspectiveFieldOfView(
                (float)Math.PI / 4, Width / (float)Height, 1.0f, 20.0f);

            GL.MatrixMode(MatrixMode.Projection);

            GL.LoadMatrix(ref projection);

            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
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

            var ray = new Ray
            {
                From = Unproject(new Vector3(x, y, -1.5f), width, height),
                To = Unproject(new Vector3(x, y, 1.0f), width, height)
            };

            foreach (SceneObject sceneObject in scene.Objects)
            {
                if (sceneObject.Intersect(ray))
                {
                    return sceneObject;
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

        static bool LinesIntersect(Vector3 A, Vector3 B, Vector3 C, Vector3 D)
        {
            var CmP = new Vector2(C.X - A.X, C.Y - A.Y);
            var r = new Vector2(B.X - A.X, B.Y - A.Y);
            var s = new Vector2(D.X - C.X, D.Y - C.Y);

            float CmPxr = CmP.X * r.Y - CmP.Y * r.X;
            float CmPxs = CmP.X * s.Y - CmP.Y * s.X;
            float rxs = r.X * s.Y - r.Y * s.X;

            if (CmPxr == 0f)
            {
                // Lines are collinear, and so intersect if they have any overlap

                return ((C.X - A.X < 0f) != (C.X - B.X < 0f))
                    || ((C.Y - A.Y < 0f) != (C.Y - B.Y < 0f));
            }

            if (rxs == 0f)
                return false; // Lines are parallel.

            float rxsr = 1f / rxs;
            float t = CmPxs * rxsr;
            float u = CmPxr * rxsr;

            return (t >= 0f) && (t <= 1f) && (u >= 0f) && (u <= 1f);
        }

        public void DetectPlanet(float mouseX, float mouseY)
        {
            SceneObject sceneObject = IntersectWithScene(
                mouseX, mouseY, ClientRectangle.Width, ClientRectangle.Height);

            if (sceneObject is LinkableObject)
            {
                var linkableObject = (LinkableObject)sceneObject;

                if (linkableObject != _mouseOver)
                {
                    // if changed

                    if (linkableObject != null)
                    {
                        // if moved onto a planet
                        _mouseOver = linkableObject;

                    }                    
                }
            }
            else
            {
                // just moved off a planet

                if (_mouseOver != null)
                {
                    _mouseOver = null;
                }
            }

            
        }

        bool _mouseDown = false;
        UserLine _line = null;

        LinkableObject _attackFrom = null;
        LinkableObject _mouseOver = null;
        Vector3 _mouseWorldLocation = Vector3.Zero;
        bool _ready = false;

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (!_ready) return;
            base.OnMouseMove(e);
            DetectPlanet(e.X, e.Y);

            _mouseWorldLocation = ClickToWorld(new Vector2(e.X, e.Y));
            _mouseWorldLocation.Z = 14.99f; // just in front of the near plane

            if (_line != null)
            {
                
                _line.To = _mouseWorldLocation;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            _mouseDown = true;

            if (_mouseOver != null)
            {
                _line = new AttackLine(_mouseOver.Position, _mouseOver.Position);
                _attackFrom = _mouseOver;
            }
            else
            {
                _line = new CutLine(_mouseWorldLocation, _mouseWorldLocation);                
            }
            scene.Add(_line);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            _mouseDown = false;

            if (_line != null)
            {
                // you are dragging a line

                if (_line is AttackLine)
                {
                    if (_mouseOver != null)
                    {
                        // you are currently over a planet

                        if (_mouseOver != _attackFrom)
                        {
                            // you are on a different planet

                            LinkObjects(_attackFrom, _mouseOver);
                        }
                    }
                }
                else
                {
                    // do cutting here

                    foreach (ObjectLink link in scene.Objects.Where(o => o is ObjectLink))
                    {
                        var isIntersect 
                            = LinesIntersect(_line.Position, _line.To, 
                            link.Parent.Position, link.Child.Position);

                        if (isIntersect)
                        {
                            Console.WriteLine("Intersect");
                        }
                    }
                }

                scene.Remove(_line);
                _line = null;
                _attackFrom = null;
            }
        }

        private void LinkObjects(LinkableObject parent, LinkableObject child)
        {
            bool alreadyLinked = false;

            foreach (var existingLink in parent.Links)
            {
                if (existingLink.Child == child)
                {
                    alreadyLinked = true;
                    break;
                }
            }

            if (!alreadyLinked)
            {
                var newLink = new ObjectLink(
                    Vector3.Zero,
                    _attackFrom, 
                    _mouseOver);                

                scene.Add(newLink);
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
