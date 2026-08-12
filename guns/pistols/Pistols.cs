using Godot;
using Srolllock.guns;

public partial class Pistols : Gun, IEquipment
{
    public Texture2D Icon { get; set; } = GD.Load<Texture2D>($"res://guns/pistols/pistols.png");
    private Node3D _offhand;
    private string _sideName;

    public Pistols(string mainName = null, string sideName = null)
	{
		var type = string.IsNullOrEmpty(mainName) ? GetType().Name : mainName;
		_modelName = $"{GetType().Name}/{type}";
        type = string.IsNullOrEmpty(sideName) ? GetType().Name : sideName;
        _sideName = $"{GetType().Name}/{type}";
	}

    public override void SpawnModel(GunSpawner rightSpawner, GunSpawner leftSpawner)
    {
        _model = (Node3D)rightSpawner.Spawn(_modelName);
        _offhand = (Node3D)leftSpawner.Spawn(_sideName);
        //_anime = model.GetNode<AnimationPlayer>("AnimationPlayer");
        //_flash = model.GetNode<GpuParticles3D>("GpuParticles3D");
    }

    public override void Despawn()
    {
        _model.QueueFree();
        _offhand.QueueFree();
    }
}
