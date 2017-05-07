using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL
{
    internal class Scene
    {
        List<SceneObject> _scene = new List<SceneObject>();

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
                if (item is LinkableObject)
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

        internal void Init()
        {
            foreach (var item in _scene)
            {
                item.Init();
            }
        }
    }
}
