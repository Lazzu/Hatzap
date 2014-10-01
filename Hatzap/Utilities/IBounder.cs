using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hatzap.Utilities
{
    public interface IBounder
    {
        ContainmentType Inside(BoundingBox box);
        ContainmentType Inside(BoundingSphere sphere);
        ContainmentType Inside(BoundingFrustum frustum);
    }
}
