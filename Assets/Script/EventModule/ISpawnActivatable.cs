/// <summary>
/// Optional hook for objects spawned by ObjectSpawnObserver.
/// Implement on prefabs that need post-spawn setup (formations, AI init, etc.).
/// </summary>
public interface ISpawnActivatable
{
    void OnSpawnActivated();
}
