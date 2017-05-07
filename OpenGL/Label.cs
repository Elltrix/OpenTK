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
            
            DrawText(_text);
        }

        public override void Init()
        {
            LoadTexture("test.png");
        }


        public override void Update(double time)
        {
        }

        private void DrawText(string text)
        {
            GL.Begin(PrimitiveType.Quads);

            float glyphWidth = (float)Settings.GlyphWidth / (float)_textureWidth;
            float glyphHeight = (float)Settings.GlyphHeight / (float)_textureHeight;

            float letterWidth = 0.5f;
            float letterHeight = 1f;
            float lettersToDraw = text.Length;
            float widthOfLabel = letterWidth * lettersToDraw;
            float pos_x = -(widthOfLabel / 2);
            float pos_y = -(letterHeight / 2);

            for (int n = 0; n < text.Length; n++)
            {
                char idx = text[n];
                float tex_x = (float)(idx % Settings.GlyphsPerLine) * glyphWidth;
                float tex_y = (float)(idx / Settings.GlyphsPerLine) * glyphHeight;
                

                // bottom left 
                GL.TexCoord2(
                    tex_x,
                    tex_y + glyphHeight);

                GL.Vertex2(
                    pos_x, 
                    pos_y);

                // bottom right
                GL.TexCoord2(
                    tex_x + glyphWidth,
                    tex_y + glyphHeight);

                GL.Vertex2(
                    pos_x + letterWidth, 
                    pos_y);
                
                // top right
                GL.TexCoord2(
                    tex_x + glyphWidth, 
                    tex_y);

                GL.Vertex2(
                    pos_x + letterWidth, 
                    pos_y + letterHeight);

                // top left
                GL.TexCoord2(
                    tex_x, 
                    tex_y);

                GL.Vertex2(
                    pos_x, 
                    pos_y + letterHeight);


                pos_x += letterWidth;
            }

            GL.End();
        }

        public override bool Intersect(Ray ray)
        {
            return false;
        }
    }
}
