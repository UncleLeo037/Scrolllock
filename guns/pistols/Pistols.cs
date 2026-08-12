using Godot;
using Srolllock.guns;

public partial class Pistols : Gun, IEquipment
{
    public Texture2D Icon { get; set; } = GD.Load<Texture2D>($"res://guns/pistols/pistols.png");
    public Node3D _offhand;

    public override void SpawnModel(GunSpawner rightSpawner, GunSpawner leftSpawner)
    {
        _model = (Node3D)rightSpawner.Spawn(GetType().Name);
        _offhand = (Node3D)leftSpawner.Spawn(GetType().Name);
        //_anime = model.GetNode<AnimationPlayer>("AnimationPlayer");
        //_flash = model.GetNode<GpuParticles3D>("GpuParticles3D");
    }

    public override void Despawn()
    {
        _model.QueueFree();
        _offhand.QueueFree();
    }
}
