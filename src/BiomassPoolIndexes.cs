namespace Landis.Extension.Succession.PnETForC
{
    /// <summary>
    /// BiomassPoolIDs used to index directly into calculations.
    /// Note that these are 1-based, unlike BiomassPoolTypes.
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

    /// <summary>
    /// BiomassPoolTypes used to index directly into calculations.
    /// Note that these are 0-based, unlike BiomassPoolIDs.
    /// </summary>
    public enum BiomassPoolTypes  // The biomass component type.
    {
        MERCHANTABLE = 0,  // The merchantable biomass component.
        FOLIAGE,  // The foliage biomass component.
        OTHER,  // The other biomass component.
        SUBMERCHANTABLE,  // The submerchantable biomass component.
        COARSEROOT,  // The coarse root biomass component.
        FINEROOT  // The fine root biomass component.
    };  
}