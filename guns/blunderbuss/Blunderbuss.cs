using System.Collections.Generic;
using System.Linq;
using Godot;
using Srolllock.guns;

public partial class Blunderbuss : Gun, IEquipment
{
	public Texture2D Icon { get; set; }
	private int _pellets = 6;

	public Blunderbuss(string name = null)
	{
		var temp = string.IsNullOrEmpty(name) ? GetType().Name : name;
		_modelPath = $"{GetType().Name}/{temp}";
		Icon = GD.Load<Texture2D>($"res://guns/{GetType().Name}/{temp}.png");
		cooldown = 0.9;
	}

	public override void Shoot()
	{
		if (ammo == 0)
		{
			Reload();
			return;
		}
		if (timer > 0.0) return;

		_playerRef.Rpc("PlayAnim", "ShootRight", -0.65f);

		timer = cooldown;
		ammo -= 1;

		for (int i = 0; i < _pellets; i++)
		{
			_rayCast.TargetPosition = new Vector3((float)GD.RandRange(-sway, sway), (float)GD.RandRange(-sway, sway), -50);

			_rayCast.ForceRaycastUpdate();

			var target = _rayCast.GetCollider();
			if (target != null)
			{
				if (!string.IsNullOrEmpty(_spell))
				{
					//only sends signal to host for spawning spells
					SpellSpawner.instance.RpcId(1, "RequestSpawnSpell", _spell, _rayCast.GetCollisionPoint(), new Vector3(this.GlobalRotation.X, this.GlobalRotation.Y, 0), 1.2f / _pellets);
				}
				else if (target is Player player)
				{
					//send damage signal to all
					player.Rpc("Damage", 15);
				}
			}
		}
		_spell = string.Empty;
	}
}
