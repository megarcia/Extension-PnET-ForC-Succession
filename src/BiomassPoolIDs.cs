namespace Landis.Extension.Succession.PnETForC
{
    /// <summary>
    /// BiomassPoolIDs - IDs used to index directly into the calculations.
    /// Note that unlike Soils.ComponentType, these are 1-based.
    /// </summary>
    public enum BiomassPoolIDs
    {
        Merchantable = 1,
        Foliage,
        Other,
        SubMerchantable,
        CoarseRoot,
        FineRoot
    };
}