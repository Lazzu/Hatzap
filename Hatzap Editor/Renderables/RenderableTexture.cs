using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap;
using Hatzap.Models;
using Hatzap.Rendering;
using Hatzap.Shaders;
using Hatzap.Textures;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Hatzap_Editor.Renderables
{
    class RenderableTexture : RenderableAsset
    {
        Model model;

        public RenderableTexture(Texture texture)
        {
            Vector2 halfSize = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);

            model = new Model();
            model.Material = new Material() { 
                Transparent = false,
                UniformData = new List<IUniformData>
                {
                    new UniformDataVector4()
                    {
                        Name = "Color",
                        Data = Vector4.One
                    },
                    new UniformDataInt()
                    {
                        Name = "textureSampler",
                        Data = 0
                    }
                }
            };
            model.Texture = texture;
            model.Shader = ShaderManager.Get("TexturedQuad");
            var mesh = new Mesh();
            mesh.Type = PrimitiveType.TriangleStrip;
            /*mesh.Vertices = new Vector3[]{
                new Vector3(-halfSize.X, -halfSize.Y, 0) * 100.0f,
                new Vector3(halfSize.X, -halfSize.Y, 0) * 100.0f,
                new Vector3(-halfSize.X, halfSize.Y, 0) * 100.0f,
                new Vector3(halfSize.X, halfSize.Y, 0) * 100.0f,
            };*/
            mesh.Vertices = new Vector3[]{
                new Vector3(-1, -1, 0),
                new Vector3(1, -1, 0),
                new Vector3(-1, 1, 0),
                new Vector3(1, 1, 0),
            };
            mesh.UV = new Vector3[] {
                new Vector3(0,1,0),
                new Vector3(1,1,0),
                new Vector3(0,0,0),
                new Vector3(1,0,0),
            };
            mesh.Indices = new uint[]{
                0,1,2,3
            };
            model.Mesh = mesh;
            model.Transform.Position = new Vector3(0, 0, 0);
            model.Transform.Scale = new Vector3(1, 1, 1);
            model.Transform.Rotation = Quaternion.Identity;

            Camera camera = new Camera(null);
            camera.SetAsCurrent();

            float w = EditorRenderer.Width / 2.0f;
            float h = EditorRenderer.Height / 2.0f;

            camera.Projection = Matrix4.CreateOrthographicOffCenter(-w, w, -h, h, -1, 1);
            camera.View = Matrix4.Identity;
        }

        public override Renderable Renderable
        {
            get
            {
                return model;
            }
        }
    }
}
