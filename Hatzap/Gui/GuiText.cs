using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Gui.Fonts;
using Hatzap.Shaders;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Gui
{
    public class GuiText
    {
        string text;

        int vao, vbo, ebo;

        bool dirty = true;
        bool mustDescribe = true;

        GPUGlyph[] vertices = null;
        int[] indices = null;


        float weight, border, smooth, glyphSpacing, lineheight, fontsize;
        Vector4 borderColor;
        HorizontalAlignment horizontalAlignment;
        VerticalAlignment verticalAlignment;

        public Font Font { get; set; }
        public float FontSize { get { return fontsize; } set { fontsize = value; } }
        public Vector4 Color { get; set; }
        public float Weight { get { return weight; } set { weight = value; } }
        public float Border { get { return border; } set { border = value; } }
        public float Smooth { get { return smooth; } set { smooth = value; } }
        public float LetterSpacing { get { return glyphSpacing; } set { glyphSpacing = value; dirty = true; } }
        public float LineHeight { get { return lineheight; } set { lineheight = value; dirty = true; } }
        public Vector4 BorderColor { get { return borderColor; } set { borderColor = value; dirty = true; } }
        public HorizontalAlignment HorizontalAlignment { get { return horizontalAlignment; } set { horizontalAlignment = value; dirty = true; } }
        public VerticalAlignment VerticalAlignment { get { return verticalAlignment; } set { verticalAlignment = value; dirty = true; } }

        public string Text
        {
            get { return text; }
            set
            {
                string tmp = value;

                if(tmp != text)
                {
                    dirty = true;
                    text = tmp;
                }
            }
        }

        public GuiText()
        {
            vbo = GL.GenBuffer();
            ebo = GL.GenBuffer();
            vao = GL.GenVertexArray();

            FontSize = 10;
            Color = Vector4.One;
            Weight = 1;
            Border = 0;
            LineHeight = 1.5f;
            LetterSpacing = 1.0f;
            BorderColor = new Vector4(1, 0, 0, 1);
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
        }

        public void Draw(ShaderProgram shader)
        {
            if (text == string.Empty)
                return;

            if(dirty)
            {
                dirty = false;

                Upload();
            }

            if (indices == null || vertices == null)
                return;

            Font.Texture.Bind();

            GL.BindVertexArray(vao);

            if (mustDescribe)
            {
                int stride = BlittableValueType.StrideOf(vertices);

                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

                // Vertex
                int vertexLocation = shader.GetAttribLocation("vertex");
                if(vertexLocation > -1)
                {
                    GL.EnableVertexAttribArray(vertexLocation);
                    GL.VertexAttribPointer(vertexLocation, 2, VertexAttribPointerType.Float, false, stride, 0);
                }
                else
                {
                    Console.WriteLine("Bad attribute location for vertex: " + vertexLocation);
                }
                
                // TCoord
                int uvLocation = shader.GetAttribLocation("uv");
                if(uvLocation > -1)
                {
                    GL.EnableVertexAttribArray(uvLocation);
                    GL.VertexAttribPointer(uvLocation, 2, VertexAttribPointerType.Float, false, stride, Vector2.SizeInBytes);
                }
                else
                {
                    Console.WriteLine("Bad attribute location for uv: " + uvLocation);
                }
                // Color
                int colorLocation = shader.GetAttribLocation("vColor");
                if(colorLocation > -1)
                {
                    GL.EnableVertexAttribArray(colorLocation);
                    GL.VertexAttribPointer(colorLocation, 4, VertexAttribPointerType.Float, false, stride, Vector2.SizeInBytes * 2);
                }
                else
                {
                    Console.WriteLine("Bad attribute location for vColor: " + colorLocation);
                }
                // BorderColor
                int borderLocation = shader.GetAttribLocation("vBorderColor");
                if (borderLocation > -1)
                {
                    GL.EnableVertexAttribArray(borderLocation);
                    GL.VertexAttribPointer(borderLocation, 4, VertexAttribPointerType.Float, false, stride, Vector2.SizeInBytes * 2 + Vector4.SizeInBytes);
                }
                else
                {
                    Console.WriteLine("Bad attribute location for vBorderColor: " + borderLocation);
                }
                // Settings
                /*int settingsLocation = shader.GetAttribLocation("vSettings");
                GL.EnableVertexAttribArray(settingsLocation);
                GL.VertexAttribPointer(settingsLocation, 3, VertexAttribPointerType.Float, false, stride, Vector2.SizeInBytes * 2 + Vector4.SizeInBytes * 2);*/
                
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

                mustDescribe = false;
            }

            float smooth = (1f - (fontsize / 150f)) * 0.5f * this.weight * this.smooth;

            if (smooth < 0f)
                smooth = 0f;
            else if (smooth > 0.5f)
                smooth = 0.5f;

            //float weight = Hatzap.Utilities.MathHelper.Lerp(0f, 2f, 1f - (fontsize / 100f * this.weight));
            float weight = (float)Math.Pow(1.5f - (fontsize / 150f), this.weight) * this.weight * 0.75f;

            if (weight < 0.75f)
                weight = 0.75f;

            CalculatedWeight = weight;

            Vector4 settings = new Vector4(fontsize, weight, border, smooth);

            shader.SendUniform("Settings", ref settings);
            
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);

            GL.BindVertexArray(0);
        }

        public float CalculatedWeight { get; protected set; }

        void Upload()
        {
            TextSettings settings = new TextSettings()
            {
                Size = 1,
                Weight = weight,
                Border = border,
                Smooth = smooth,
                Color = Color,
                BorderColor = borderColor,
                LineHeight = lineheight,
                GlyphSpacing = glyphSpacing,
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment,
                MaxWidth = 0
            };

            vertices = Font.TextToVertices(text, settings);

            List<int> tmp = new List<int>();

            for (int i = 0; i < vertices.Length / 4; i++)
            {
                // 1st triangle
                tmp.Add(i * 4 + 0);
                tmp.Add(i * 4 + 1);
                tmp.Add(i * 4 + 2);
                // 2nd triangle
                tmp.Add(i * 4 + 2);
                tmp.Add(i * 4 + 1);
                tmp.Add(i * 4 + 3);
            }

            indices = tmp.ToArray();
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(vertices.Length * GPUGlyph.SizeInBytes), vertices, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(indices.Length * sizeof(int)), indices, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

    }
}
