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
                        //_mouseOver.Highlight();

                    }
                    else
                    {
                        // if moved off a planet

                        if (_userLink != null)
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

            
        }

        bool _mouseDown = false;
        UserLine _userLink = null;

        LinkableObject _attackFrom = null;
        LinkableObject _mouseOver = null;

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            DetectPlanet(e.X, e.Y);

            if (_userLink != null)
            {
                var worldCoord = ClickToWorld(new Vector2(e.X, e.Y));
                worldCoord.Z = 14.999f; // just in front of the near plane
                _userLink.To = worldCoord;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            _mouseDown = true;

            if (_mouseOver != null)
            {
                _userLink = new UserLine(_mouseOver.Position, _mouseOver.Position);
                _attackFrom = _mouseOver;
                scene.Add(_userLink);
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            _mouseDown = false;

            if (_userLink != null)
            {
                // you are dragging a line

                if (_mouseOver != null)
                {
                    // you are currently over a planet

                    if (_mouseOver != _attackFrom)
                    {
                        // you are on a different planet

                        LinkObjects(_attackFrom, _mouseOver);                        
                    }
                }

                scene.Remove(_userLink);
                _userLink = null;
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
