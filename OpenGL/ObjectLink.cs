using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL
{
    internal class ObjectLink : SceneObject
    {
        public ObjectLink(Vector3 position, LinkableObject parent, LinkableObject child)
            : base(position)
        {
            Parent = parent;
            Child = child;

            Parent.Links.Add(this);
            Child.Links.Add(this);
        }

        public LinkableObject Parent { get; private set; }
        public LinkableObject Child { get; private set; }

        public override void Draw()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(0.25f, 0.25f, 0.25f);
            
            // draw the line between the edges of the two objects, rather than from their centres
            var p1 = ScaleBy(Child.Position, Parent.Position, Parent.Width);
            var p2 = ScaleBy(Parent.Position, Child.Position, Child.Width);
                        
            GL.Vertex3(p1);
            GL.Vertex3(p2);
            GL.End();
        }

        /// <summary>
        /// Scale the vector defined by p1 and p2
        /// </summary>
        /// <param name="p1">Starting point of vector</param>
        /// <param name="p2">Ending point of vector</param>
        /// <param name="scaler">Factor to scale the vector by</param>
        /// <returns>The scaled p2</returns>
        private static Vector3 ScaleBy(Vector3 p1, Vector3 p2, float scaler)
        {
            var direction = (p1 - p2).Normalized();
            var reduction = Vector3.Multiply(direction, scaler);
            return p2 + reduction;
        }

        public override void Init()
        {
        }

        public override void Update(double time)
        {
        }

        public override bool Intersect(Ray ray)
        {
            return false;
        }
    }
}
