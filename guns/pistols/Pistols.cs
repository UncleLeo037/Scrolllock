using System.Linq;
using System.Runtime.CompilerServices;
using Godot;
using Srolllock.guns;

public partial class Pistols : Gun, IEquipment
{
    public Texture2D Icon { get; set; }
    private Node3D _offhand;
    private string _sideName;
    private bool _isRight = true;

    public Pistols(string mainName = null, string sideName = null)
    {
        var type = string.IsNullOrEmpty(mainName) ? GetType().Name : mainName;
        _modelPath = $"{GetType().Name}/{type}";
        type = string.IsNullOrEmpty(sideName) ? GetType().Name : sideName;
        _sideName = $"{GetType().Name}/{type}";
        Icon = GD.Load<Texture2D>($"res://guns/{GetType().Name}/{mainName}.png");
        cooldown = 0.3;
    }

    public override void SpawnModel(GunSpawner rightSpawner, GunSpawner leftSpawner)
    {
        SetPhysicsProcess(true);
        _model = (Node3D)rightSpawner.Spawn(_modelPath);
        _offhand = (Node3D)leftSpawner.Spawn(_sideName);
        //_anime = model.GetNode<AnimationPlayer>("AnimationPlayer");
        //_flash = model.GetNode<GpuParticles3D>("GpuParticles3D");
    }

    public override void Despawn()
    {
        _model.QueueFree();
        _offhand.QueueFree();
        SetPhysicsProcess(false);
    }

    public override async void Shoot()
    {
        if (timer <= 0.0)
        {
            timer = cooldown;
            string animation = _isRight ? "ShootRight" : "ShootLeft";
            _playerRef.Rpc("PlayAnim", animation, -0.25f, _isRight);
            _isRight = !_isRight;

            var target = _rayCasts.First().GetCollider();
            if (target != null)
            {
                //should just shoot instead
                if (!string.IsNullOrEmpty(_spell))
                {
                    //spells will be called in different ways here in future
                    Vector3 point = _rayCasts.First().GetCollisionPoint();
                    //only sends signal to host for spawning spells
                    SpellSpawner.instance.RpcId(1, "RequestSpawnSpell", _spell, point, new Vector3(this.GlobalRotation.X, this.GlobalRotation.Y, 0), 1);
                    _spell = string.Empty;
                }
                else if (target is Player player)
                {
                    //send damage signal to all
                    player.Rpc("Damage", 25);
                }
            }
            _rayCasts.First().TargetPosition = new Vector3((float)GD.RandRange(-sway, sway), (float)GD.RandRange(-sway, sway), -100);
        }
    }
}
