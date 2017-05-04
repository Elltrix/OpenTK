using OpenTK;

namespace OpenGL
{
    internal interface IBoundingSphere
    {
        Vector3 Position { get; set; }
        float Radius { get; set; }
    }
}
