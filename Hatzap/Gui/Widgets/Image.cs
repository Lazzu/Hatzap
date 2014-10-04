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

        public override GuiVertex[] GetVertices() 
        {
            if (BorderWidth == 0 || TextureRegion == null || TextureRegion.Length < 1)
                return null;

            GuiVertex[] vertices = new GuiVertex[4];

            vertices[0].Position = Position + new Vector2(-BorderWidth, -BorderWidth);
            vertices[1].Position = Position + Size + new Vector2(BorderWidth, BorderWidth);
            vertices[2].Position = Position + new Vector2(0, Size.Y) + new Vector2(-BorderWidth, BorderWidth);
            vertices[3].Position = Position + Size + new Vector2(BorderWidth, BorderWidth);
            vertices[4].Position = Position + new Vector2(-BorderWidth, -BorderWidth);
            vertices[5].Position = Position + new Vector2(Size.X, 0) + new Vector2(BorderWidth, -BorderWidth);

            vertices[0].TextureCoordinates = TextureRegion[0].Offset;
            vertices[1].TextureCoordinates = TextureRegion[0].Offset + TextureRegion[0].Size;
            vertices[2].TextureCoordinates = TextureRegion[0].Offset + new Vector2(0, TextureRegion[0].Size.Y);
            vertices[3].TextureCoordinates = TextureRegion[0].Offset + TextureRegion[0].Size;
            vertices[4].TextureCoordinates = TextureRegion[0].Offset + new Vector2(0, 0);
            vertices[5].TextureCoordinates = TextureRegion[0].Offset + new Vector2(TextureRegion[0].Size.X, 0);

            vertices[0].TexturePage = TextureRegion[0].Page;
            vertices[1].TexturePage = TextureRegion[0].Page;
            vertices[2].TexturePage = TextureRegion[0].Page;
            vertices[3].TexturePage = TextureRegion[0].Page;
            vertices[4].TexturePage = TextureRegion[0].Page;
            vertices[5].TexturePage = TextureRegion[0].Page;

            return vertices;
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
            program.SendUniform("Position", ref Position);
            program.SendUniform("Size", ref Size);

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
