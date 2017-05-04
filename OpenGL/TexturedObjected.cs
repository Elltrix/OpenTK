using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;

namespace OpenGL
{
    internal abstract class TexturedObject : SceneObject
    {
        protected int _textureId;
        protected int _textureWidth;
        protected int _textureHeight;

        public TexturedObject(Vector3 position)
            : base(position)
        {
        }

        protected void LoadTexture(string filename)
        {
            using (var bitmap = new Bitmap(filename))
            {
                _textureId = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, _textureId);

                BitmapData data = bitmap.LockBits(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height), 
                    ImageLockMode.ReadOnly, 
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(
                    TextureTarget.Texture2D, 
                    0, 
                    PixelInternalFormat.Rgba, 
                    bitmap.Width, 
                    bitmap.Height, 
                    0, 
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, 
                    PixelType.UnsignedByte, 
                    data.Scan0);

                bitmap.UnlockBits(data);

                GL.TexParameter(
                    TextureTarget.Texture2D, 
                    TextureParameterName.TextureMinFilter, 
                    (int)TextureMinFilter.Nearest);

                GL.TexParameter(
                    TextureTarget.Texture2D, 
                    TextureParameterName.TextureMagFilter, 
                    (int)TextureMagFilter.Nearest);

                _textureWidth = bitmap.Width;
                _textureHeight = bitmap.Height;
                
            }
        }
    }
}
