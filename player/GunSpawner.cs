using Godot;
using Srolllock.guns;

public partial class GunSpawner : MultiplayerSpawner
{
    public Gun _SpawnFunction(Variant data)
    {
        string gunPath = data.AsString();

        PackedScene gunScene = GD.Load<PackedScene>(gunPath);
        Gun gunInstance = gunScene.Instantiate<Gun>();
        return gunInstance;
    }

    public override void _Ready()
    {
        SpawnFunction = Callable.From<Variant, Node>(_SpawnFunction);
    }
}
