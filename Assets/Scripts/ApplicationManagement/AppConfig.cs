[System.Serializable]
public class AppConfig
{
    public Lang Language { get; set; }
    public bool UseVirtualWorld { get; set; }
    public Limb[] LimbConstraints { get; set; }
}
