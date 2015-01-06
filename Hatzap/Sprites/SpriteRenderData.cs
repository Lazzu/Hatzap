using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Hatzap.Sprites
{
    public class SpriteRenderData
    {
        /// <summary>
        /// The sprite position on screen in pixels
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// The sprite size on screen in pixels
        /// </summary>
        public Vector2 Size { get; set; }

        /// <summary>
        /// The sprite rotation around z-axis in radians
        /// </summary>
        public float Rotation { get; set; }
    }
}
