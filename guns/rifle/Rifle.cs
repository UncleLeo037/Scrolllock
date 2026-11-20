using Godot;
using Srolllock.Guns;
using Srolllock.Spells;

public partial class Rifle : Gun
{
	private int _piercing = 6;
	private (float x, float y) _savedSway; 
	public Rifle(string name = null)
	{
		var temp = string.IsNullOrEmpty(name) ? GetType().Name : name;
		_modelPath = $"{GetType().Name}/{temp}";
		Icon = GD.Load<Texture2D>($"res://Guns/{GetType().Name}/{temp}.png");
	}

	public override void SpawnModel(GunSpawner rightSpawner, GunSpawner leftSpawner)
	{
		SetPhysicsProcess(true);
		_model = (Node3D)rightSpawner.Spawn(_modelPath);
		//_anime = model.GetNode<AnimationPlayer>("AnimationPlayer");
		//_flash = model.GetNode<GpuParticles3D>("GpuParticles3D");
	}

	public override void Shoot()
	{
		if (ammo == 0)
		{
			Reload();
			return;
		}
		if (timer > 0.0) return;

		_playerRef.Rpc("PlayAnim", "ShootRight", -1f);

		timer = cooldown;
		ammo -= 1;

		_savedSway.x = (float)GD.RandRange(-sway, sway);
		_savedSway.y = (float)GD.RandRange(-sway, sway);

		for (int i = 0; i < _piercing; i++)
		{
			_rayCast.TargetPosition = new Vector3(_savedSway.x, _savedSway.y, -100);

			_rayCast.ForceRaycastUpdate();

			var collider = _rayCast.GetCollider();
			if (collider == null) break;

			_action?.Invoke(_rayCast.GetCollisionPoint(), collider);

			if (collider is CollisionObject3D colObj)
			{
				_rayCast.AddExceptionRid(colObj.GetRid());
			}

			if (collider is StaticBody3D) break;
			if (collider is CsgShape3D) break; //for demo maps
		}
		_rayCast.ClearExceptions();
	}
}
