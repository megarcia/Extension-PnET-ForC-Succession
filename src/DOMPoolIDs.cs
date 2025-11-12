namespace Landis.Extension.Succession.PnETForC
{
    /// <summary>
    /// DOMPoolIDs - IDs used to index directly into the calculations.
    /// Note that unlike Soils.DOMPoolType, these are 1-based.
    /// </summary>
    public enum DOMPoolIDs
    {
        VeryFastAG = 1,
        VeryFastBG,
        FastAG,
        FastBG,
        Medium,
        SlowAG,
        SlowBG,
        StemSnag,
        BranchSnag,
        SpareCPool
    };
}
