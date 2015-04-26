using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Models;
using Hatzap.Shaders;
using Hatzap.Textures;

namespace Hatzap.Rendering
{
    public abstract class Renderable : IRenderable
    {
        readonly Transform transform = new Transform();

        /// <summary>
        /// Transform object of the current renderable object.
        /// </summary>
        public Transform Transform { get { return transform; } }

        public virtual ShaderProgram Shader
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public virtual Texture Texture
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public virtual Material Material
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public virtual int Triangles
        {
            get { throw new NotImplementedException(); }
        }

        public virtual void Render()
        {
            throw new NotImplementedException();
        }

        ITransformable ITransformable.Parent { get; set; }

        List<ITransformable> children;

        public List<ITransformable> Children
        {
            get { return children; }
        }

        public IEnumerable<ITransformable> GetChildren()
        {
            if (children == null)
                yield break;

            for(int i = 0; i < children.Count; i++)
            {
                yield return children[i];
            }
        }

        public IEnumerable<ITransformable> GetChildren(Func<ITransformable, bool> comparer)
        {
            if (children == null)
                yield break;

            for (int i = 0; i < children.Count; i++)
            {
                if(comparer(children[i])) yield return children[i];
            }
        }

        /// <summary>
        /// Gets or sets if the object should be visible or not
        /// </summary>
        public bool Visible
        {
            get;
            set;
        }
    }
}
