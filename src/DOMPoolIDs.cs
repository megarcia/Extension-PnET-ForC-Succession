namespace Landis.Extension.Succession.PnETForC
{
    /// <summary>
    /// DOMPoolIDs used to index directly into calculations.
    /// Note that these are 1-based, unlike DOMPoolTypes.
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

    /// <summary>
    /// DOMPoolTypes used to index directly into calculations.
    /// Note that these are 0-based, unlike DOMPoolIDs.
    /// </summary>
    public enum DOMPoolTypes
    {
        VERYFASTAG = 0,
        VERYFASTBG,
        FASTAG,
        FASTBG,
        MEDIUM,
        SLOWAG,
        SLOWBG,
        STEMSNAG,
        BRANCHSNAG,
        SPARECPOOL
    };    
}
