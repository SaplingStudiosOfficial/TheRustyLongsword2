// =============================================================
// BOON ID
// =============================================================
//
// Stable identity for each boon.
//
// Explicit numeric values because this enum is persisted in
// save data - reordering the list must never change what an
// existing save means.
// =============================================================

public enum BoonId
{
    Torch = 0,
    WardingSigil = 1,
    BoneCharm = 2,
    HolyWater = 3,
    BlackGrimoire = 4,
    BrokenMirror = 5
}
