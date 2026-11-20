using System.Collections.Generic;
using System.Linq;
using Godot;
using Srolllock.Guns;

public partial class Blunderbuss : Gun
{
	private int _pellets = 6;

	public Blunderbuss(string name = null)
	{
		var temp = string.IsNullOrEmpty(name) ? GetType().Name : name;
		_modelPath = $"{GetType().Name}/{temp}";
		Icon = GD.Load<Texture2D>($"res://Guns/{GetType().Name}/{temp}.png");
		cooldown = 0.9;
		_modifier = 1.2f / _pellets;
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
				_action?.Invoke(_rayCast.GetCollisionPoint(), target);
			}
		}
	}
}
