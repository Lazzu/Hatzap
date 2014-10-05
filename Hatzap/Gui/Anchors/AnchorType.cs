using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatzap.Gui.Anchors
{
    /// <summary>
    /// Anchortype defines how anchors behave.
    /// 
    /// None - No anchoring. Move freely.
    /// Snap - Snap to parent's anchoring coordinates.
    /// Absolute - Keep the coordinates.
    /// Relative - Keep the relative distance from parent's anchoring coordinates.
    /// </summary>
    public enum AnchorType
    {
        None,
        Snap,
        Absolute,
        Relative
    }
}
