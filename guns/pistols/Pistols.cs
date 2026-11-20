using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Godot;
using Srolllock.Guns;
using Srolllock.Spells;

public partial class Pistols : Gun
{
    private Node3D _offhand;
    private string _sideName;
    private double _sideTimer;
    protected Action<Vector3, object> _sideAction { get; set; } = null;
    private int _sideAmmo = 1;

    public Pistols(string mainName = null, string sideName = null)
    {
        var type = string.IsNullOrEmpty(mainName) ? GetType().Name : mainName;
        _modelPath = $"{GetType().Name}/{type}";
        type = string.IsNullOrEmpty(sideName) ? GetType().Name : sideName;
        _sideName = $"{GetType().Name}/{type}";
        Icon = GD.Load<Texture2D>($"res://Guns/{GetType().Name}/{mainName}.png");
        _modifier = 0.6f;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (timer > 0.0) timer -= 1 * delta;
        if (_sideTimer > 0.0) _sideTimer -= 1 * delta;
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

    public override async void SetSpell(ISpell spell)
    {
        switch (spell)
        {
            case TargetSpell targetSpell:
                targetSpell.Modifier = _modifier;
                _method = (vec, obj) => targetSpell.Activate(obj);
                break;
            case AreaSpell areaSpell:
                _method = (vec, obj) => SpellSpawner.instance.RpcId(1, "RequestSpawnSpell", areaSpell.GetType().Name, vec, new Vector3(this.GlobalRotation.X, this.GlobalRotation.Y, 0), _modifier);
                break;
            default:
                //probably do misfire animation for lols
                break;
        }
    }

    public override void Aim()
    {
        if (_sideAmmo == 0)
        {
            Reload();
            return;
        }
        if (_sideTimer > 0.0) return;

        _playerRef.Rpc("PlayAnim", "ShootRight", -0.25f);

        _sideTimer = cooldown;
        _sideAmmo -= 1;

        _rayCast.TargetPosition = new Vector3((float)GD.RandRange(-sway, sway), (float)GD.RandRange(-sway, sway), -75);

        var target = _rayCast.GetCollider();
        if (target == null) return;

        _sideAction?.Invoke(_rayCast.GetCollisionPoint(), target);
    }


    public override void Shoot()
    {
        if (ammo == 0)
        {
            Reload();
            return;
        }
        if (timer > 0.0) return;

        _playerRef.Rpc("PlayAnim", "ShootLeft", -0.25f);

        timer = cooldown;
        ammo -= 1;

        _rayCast.TargetPosition = new Vector3((float)GD.RandRange(-sway, sway), (float)GD.RandRange(-sway, sway), -75);

        var target = _rayCast.GetCollider();
        if (target == null) return;

        _action?.Invoke(_rayCast.GetCollisionPoint(), target);
    }

    public override async void Reload(bool forceReload = false)
    {
        if (forceReload)
        {
            //continue
        }
        else if (ammo == maxAmmo && _sideAmmo != maxAmmo)
        {
            ReloadRight();
            return;
        }
        else if (ammo != maxAmmo && _sideAmmo == maxAmmo)
        {
            ReloadLeft();
            return;
        }
        else if (ammo == maxAmmo && _sideAmmo == maxAmmo) return;

        string reloadAnim = "ReloadBoth";//$"Reload{GetType().Name}";
        if (_playerRef.AnimPlayer.CurrentAnimation == reloadAnim) return;
        _playerRef.Rpc("PlayAnim", reloadAnim, 0);
        Variant[] anim = await ToSignal(_playerRef.AnimPlayer, AnimationPlayer.SignalName.AnimationFinished);
        if (anim[0].AsString() == reloadAnim)
        {
            ammo = _sideAmmo = maxAmmo;
            _action = _method;
            _sideAction = _method;
        }
    }

    public async void ReloadLeft()
    {
        string reloadAnim = "ReloadLeft";//$"Reload{GetType().Name}";
        if (_playerRef.AnimPlayer.CurrentAnimation == reloadAnim) return;
        _playerRef.Rpc("PlayAnim", reloadAnim, 0);
        Variant[] anim = await ToSignal(_playerRef.AnimPlayer, AnimationPlayer.SignalName.AnimationFinished);
        if (anim[0].AsString() == reloadAnim)
        {
            ammo = maxAmmo;
            _action = _method;
        }
    }

    public async void ReloadRight()
    {
        string reloadAnim = "ReloadRight";//$"Reload{GetType().Name}";
        if (_playerRef.AnimPlayer.CurrentAnimation == reloadAnim) return;
        _playerRef.Rpc("PlayAnim", reloadAnim, 0);
        Variant[] anim = await ToSignal(_playerRef.AnimPlayer, AnimationPlayer.SignalName.AnimationFinished);
        if (anim[0].AsString() == reloadAnim)
        {
            _sideAmmo = maxAmmo;
            _sideAction = _method;
        }
    }
}
