﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hatzap.Models;
using Hatzap.Shaders;
using Hatzap.Textures;
using OpenTK;

namespace Hatzap.Rendering
{
    public interface IRenderable : ITransformable
    {
        /// <summary>
        /// The material data of the renderable object.
        /// </summary>
        Material Material { get; set; }

        /// <summary>
        /// Object's triangle count.
        /// </summary>
        int Triangles { get; }

        /// <summary>
        /// If object should be hidden or not
        /// </summary>
        bool Visible { get; set; }


        /// <summary>
        /// The render method issues a render call of the current object.
        /// </summary>
        void Render();
    }
}
