using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Shaders;
using Hatzap.Textures;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Hatzap.Gui.Widgets
{
    public class Image : Widget
    {
        public float BorderWidth { get; set; }

        /// <summary>
        /// The image texture to be rendered
        /// </summary>
        public Texture Texture { get; set; }

        /// <summary>
        /// The texture coordinates used when rendering the texture.
        /// </summary>
        public Vector2[] TextureCoordinates { get; set; }

        Vector2[] tcoord = null;

        string _sortID = "_default";
        string _renderLayer = "Image._deafult";
        public string SortID { get { return _sortID; } set { _sortID = value; RebuildRenderLayer(); } }

        public Image()
        {
            BorderWidth = 0;
            tcoord = new Vector2[]{
                new Vector2(0,0),
                new Vector2(1,1),
                new Vector2(0,1),
                new Vector2(1,1),
                new Vector2(0,0),
                new Vector2(1,0),
            };
            RebuildRenderLayer();
        }

        public override void GetVertices(List<Vector2> v, List<Vector3> u, List<Vector4> c) 
        {
            if (BorderWidth == 0 || TextureRegion == null || TextureRegion.Length < 1)
                return;

            var vertices = new Vector2[4];
            var uv = new Vector3[4];
            var colors = new Vector4[4];

            vertices[0] = Position + new Vector2(-BorderWidth, -BorderWidth);
            vertices[1] = Position + Size + new Vector2(BorderWidth, BorderWidth);
            vertices[2] = Position + new Vector2(0, Size.Y) + new Vector2(-BorderWidth, BorderWidth);
            vertices[3] = Position + Size + new Vector2(BorderWidth, BorderWidth);
            vertices[4] = Position + new Vector2(-BorderWidth, -BorderWidth);
            vertices[5] = Position + new Vector2(Size.X, 0) + new Vector2(BorderWidth, -BorderWidth);

            uv[0] = new Vector3(TextureRegion[0].Offset.X, TextureRegion[0].Offset.Y, TextureRegion[0].Page);
            uv[1] = new Vector3(TextureRegion[0].Offset.X + TextureRegion[0].Size.X, TextureRegion[0].Offset.Y + TextureRegion[0].Size.Y, TextureRegion[0].Page);
            uv[2] = new Vector3(TextureRegion[0].Offset.X, TextureRegion[0].Offset.Y + TextureRegion[0].Size.Y, TextureRegion[0].Page);
            uv[3] = new Vector3(TextureRegion[0].Offset.X + TextureRegion[0].Size.X, TextureRegion[0].Offset.Y + TextureRegion[0].Size.Y, TextureRegion[0].Page);
            uv[4] = new Vector3(TextureRegion[0].Offset.X, TextureRegion[0].Offset.Y, TextureRegion[0].Page);
            uv[5] = new Vector3(TextureRegion[0].Offset.X + TextureRegion[0].Size.X, TextureRegion[0].Offset.Y, TextureRegion[0].Page);

            colors[0] = Color;
            colors[1] = Color;
            colors[2] = Color;
            colors[3] = Color;
            colors[4] = Color;
            colors[5] = Color;

            v.AddRange(vertices);
            u.AddRange(uv);
            c.AddRange(colors);
        }

        int vbo = 0, vao = 0;

        public override void CustomRender()
        {
            bool generate = false;
            if (vbo == 0)
            {
                vbo = GL.GenBuffer();
                vao = GL.GenVertexArray();
                generate = true;
            }

            ShaderProgram program = ShaderManager.Get("Gui.Image");

            program.Enable();

            Texture.Bind();

            GL.BindVertexArray(vao);

            if(generate)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

                Vector2[] vertices = new Vector2[6];

                vertices[0] = new Vector2(0, 0);
                vertices[1] = new Vector2(0, 1);
                vertices[2] = new Vector2(1, 1);

                vertices[3] = new Vector2(1, 1);
                vertices[4] = new Vector2(1, 0);
                vertices[5] = new Vector2(0, 0);

                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, BlittableValueType.StrideOf(vertices), 0);

                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(6 * Vector2.SizeInBytes), vertices, BufferUsageHint.StaticDraw);
            }

            program.SendUniform("Projection", ref GuiRoot.Root.Projection);
            program.SendUniform("Position", ref position);
            program.SendUniform("Size", ref size);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            GL.BindVertexArray(0);

            program.Disable();
        }

        public override string CustomRenderLayer
        {
            get
            {
                return _renderLayer;
            }
        }

        void RebuildRenderLayer()
        {
            _renderLayer = string.Format("Image.{0}", SortID);
        }
    }
}
