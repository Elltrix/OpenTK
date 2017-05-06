using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OpenGL
{
    internal abstract class LinkableObject : TexturedObject
    {
        public List<ObjectLink> Links { get; set; }

        public LinkableObject(Vector3 position) 
            : base(position)
        {
            Links = new List<ObjectLink>();
        }
    }
}
