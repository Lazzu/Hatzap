using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatzap.Models
{
    public interface ITransformable
    {
        /// <summary>
        /// The read-only Transform instance that contains the object's position, rotation and scale.
        /// </summary>
        Transform Transform { get; }

        /// <summary>
        /// The parent object of the transformable instance.
        /// </summary>
        ITransformable Parent { get; set; }

        /// <summary>
        /// Child renderables.
        /// </summary>
        List<ITransformable> Children { get; }

        /// <summary>
        /// Gets all immediate child objects.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ITransformable> GetChildren();

        /// <summary>
        /// Gets all immediate child objects.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ITransformable> GetChildren(Func<ITransformable, bool> comparer);
    }
}
