using Godot;
using Srolllock.guns;

public partial class GunSpawner : MultiplayerSpawner
{
    public Node _SpawnFunction(string path)
    {
        PackedScene gunScene = GD.Load<PackedScene>($"res://{path}.tscn");
        Node gunInstance = gunScene.Instantiate();
        return gunInstance;
    }

    public override void _Ready()
    {
        SpawnFunction = Callable.From<string, Node>(_SpawnFunction);
    }
}
