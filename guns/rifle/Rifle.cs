using Godot;
using Srolllock.guns;

public partial class Rifle : Gun, IEquipment
{
	public Texture2D Icon { get; set; }
	private int _piercing = 6;
	private (float x, float y) _savedSway; 
	public Rifle(string name = null)
	{
		var temp = string.IsNullOrEmpty(name) ? GetType().Name : name;
		_modelPath = $"{GetType().Name}/{temp}";
		Icon = GD.Load<Texture2D>($"res://guns/{GetType().Name}/{temp}.png");
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

			if (!string.IsNullOrEmpty(_spell))
			{
				Vector3 point = _rayCast.GetCollisionPoint();
				//only sends signal to host for spawning spells
				SpellSpawner.instance.RpcId(1, "RequestSpawnSpell", _spell, point, new Vector3(this.GlobalRotation.X, this.GlobalRotation.Y, 0), 1);
			}
			else if (collider is Player player)
			{
				//send damage signal to all
				player.Rpc("Damage", 50);
			}

			if (collider is CollisionObject3D colObj)
			{
				_rayCast.AddExceptionRid(colObj.GetRid());
			}

			if (collider is StaticBody3D) break;
			if (collider is CsgShape3D) break; //for demo maps
		}
		_spell = string.Empty;
		_rayCast.ClearExceptions();
	}
}
