namespace PlantsVSZombies;
/// <summary>
/// dictates what ImageHaver should be shown on a given pixel 
/// </summary>
public enum Layers
{
    //order relient
    Cursor,
    InnerIntoractible,
    Inermenue,
    InerFrame,
    Frame,
    Menue,
    Forground,
    Sun,
    Attack,
    Plant,
    Special,
    BZombie,
    SZombie,
    PlantSpace,
    Background,
}

/// <summary>
///  a struct for showing what an object is, and how visible it should be
/// </summary>
/// <param name="layer"></param>
public readonly struct LayerID(Layers layer)
{
    //this dictionary is used to tell the computer which layer should be shown, if both objects have
    //the same Layers. It is only used for the enitilisation of a LayerID
    static readonly Dictionary<Layers, float> layerList;
    /// <summary>
    /// Finds the next sub Layer in <see cref="layerList"/>, then increases the sublayer.
    /// </summary>
    /// <returns>
    /// The next number in the subLayer
    /// </returns>
    static float ReturnNextInLayer(Layers layer)
    {

        float building = layerList[layer];
        layerList[layer] += 0.001f;
        return building;
    }
    static LayerID()
    {
        layerList = [];
        try
        {
            for (int i = 0; i < 100; i++)
            {
                layerList.Add((Layers)i, 0);
            }
        }
        catch (ArgumentOutOfRangeException)
        { }
    }
    //unik Layers and float combination
    public readonly float layerID = ReturnNextInLayer(layer);
    public readonly Layers layer = layer;
    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    public override string ToString() => layer.ToString();
    public static bool operator ==(LayerID a, LayerID b)
    {
        if (a.layer != b.layer)
            return false;

        if (a.layerID != b.layerID)
            return false;
        return true;
    }
    public static bool operator !=(LayerID a, LayerID b)
    {
        return !(a == b);
    }
    public static implicit operator Layers(LayerID d) => d.layer;
    /// <summary>
    /// True if <see href="a"/> layer is less visible then <see href="b"/>.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns> False if <see href="a"/>.layer is lower then <see href="b"/>.layer, 
    /// or they are equal and <see href="a"/>.layerID is lower then <see href="b"/>.layerID.</returns>
    public static bool operator >(LayerID a, LayerID b)
    {
        if ((int)a.layer > (int)b.layer)
            return true;

        if ((int)a.layer == (int)b.layer)
            if (a.layerID > b.layerID)
                return true;
        return false;
    }
    /// <summary>
    /// True if <see href="a"/> layer is more visible then <see href="b"/>.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns> False if <see href="a"/>.layer is higher then <see href="b"/>.layer, 
    /// or they are equal and <see href="a"/>.layerID is higher then <see href="b"/>.layerID.</returns>
    public static bool operator <(LayerID a, LayerID b)
    {
        if (a == b)
            return false;
        return !(a > b);
    }
    public static bool operator ==(LayerID a, Layers layer)
    {
        if (a.layer == layer)
            return true;
        return false;
    }
    public static bool operator !=(LayerID a, Layers layer)
    {
        return !(a == layer);
    }
    public LayerID() : this(Layers.Background) { }
}
