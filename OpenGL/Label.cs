using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace OpenGL
{
    internal class Label : TexturedObject
    {
        private string _text;

        public void UpdateText(string text)
        {
            _text = text;
        }

        public Label(Vector3 position, string text) : base(position)
        {
            _text = text;
        }

        public override void Draw()
        {
            GL.BindTexture(TextureTarget.Texture2D, _textureId);

            //GL.PushMatrix();
            //GL.Translate(Position);
            DrawText(0, 0, _text);
            //GL.PopMatrix();
        }

        public override void Init()
        {
            LoadTexture("test.png");
        }


        public override void Update(double time)
        {
        }

        private void DrawText(int x, int y, string text)
        {
            float disp_x = 0f;

            GL.Begin(PrimitiveType.Quads);

            float glyphWidth = (float)Settings.GlyphWidth / (float)_textureWidth;
            float glyphHeight = (float)Settings.GlyphHeight / (float)_textureHeight;

            for (int n = 0; n < text.Length; n++)
            {
                char idx = text[n];
                float u = (float)(idx % Settings.GlyphsPerLine) * glyphWidth;
                float v = (float)(idx / Settings.GlyphsPerLine) * glyphHeight;

                float letterWidth = 0.5f;
                float letterHeight = 1f;

                GL.TexCoord2(u, v);
                GL.Vertex2(disp_x - letterWidth/2, letterHeight/2);

                GL.TexCoord2(u, v + glyphHeight);
                GL.Vertex2(disp_x - letterWidth/2, -letterHeight/2);

                GL.TexCoord2(u + glyphWidth, v + glyphHeight);
                GL.Vertex2(disp_x + letterWidth/2, -letterHeight/2);

                GL.TexCoord2(u + glyphWidth, v);
                GL.Vertex2(disp_x + letterWidth/2, letterHeight/2);

                disp_x += letterWidth;


            }

            GL.End();
        }

    }
}
