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
        }

        public LinkableObject Parent { get; private set; }
        public LinkableObject Child { get; private set; }

        public override void Draw()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(0.25f, 0.25f, 0.25f);


            //10  1
            //11
            // * 1.1


            var childToParent = Parent.Position - Child.Position;
            var childEdge = Vector3.Multiply(childToParent.Normalized(), Child.Width);
            var p2 = Child.Position + childEdge;

            var localVector = Parent.Position - Child.Position;
            var increaseVectorLengthBy = Child.Width / localVector.Length;
            var newChildPosition = Vector3.Multiply(localVector, increaseVectorLengthBy);
            
            GL.Vertex3(Parent.Position);
            GL.Vertex3(p2);
            GL.End();
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
