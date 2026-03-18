using System.Threading.Tasks;

public interface IAssetLoader
{
    Task Load();
    void Unload();
    bool IsLoaded { get; }
}