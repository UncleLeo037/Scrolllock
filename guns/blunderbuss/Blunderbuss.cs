using System.Collections.Generic;
using System.Linq;
using Godot;
using Srolllock.guns;

public partial class Blunderbuss : Gun, IEquipment
{
	public Texture2D Icon { get; set; }
	private List<RayCast3D> _rayCasts = new List<RayCast3D>();
	private int _shots = 6;

	public Blunderbuss(string name = null)
	{
		var temp = string.IsNullOrEmpty(name) ? GetType().Name : name;
		_modelPath = $"{GetType().Name}/{temp}";
		Icon = GD.Load<Texture2D>($"res://guns/{GetType().Name}/{temp}.png");
		cooldown = 0.9;
	}

	public override void _Ready()
	{
		for (int i = 0; i < _shots; i++)
		{
			RayCast3D temp = new RayCast3D();
			temp.SetCollisionMaskValue(1, true);
			temp.SetCollisionMaskValue(2, true);
			temp.SetCollisionMaskValue(3, true);
			temp.TargetPosition = new Vector3((float)GD.RandRange(-sway, sway), (float)GD.RandRange(-sway, sway), -50);
			_rayCasts.Add(temp);
			AddChild(temp);
		}
	}

	public override void Shoot()
	{
		if (timer <= 0.0)
		{
			timer = cooldown;
			_playerRef.Rpc("PlayAnim", "ShootRight", -0.65f);

			foreach (RayCast3D ray in _rayCasts)
			{
				var target = ray.GetCollider();
				if (target != null)
				{
					if (!string.IsNullOrEmpty(_spell))
					{
						//only sends signal to host for spawning spells
						SpellSpawner.instance.RpcId(1, "RequestSpawnSpell", _spell, ray.GetCollisionPoint(), new Vector3(this.GlobalRotation.X, this.GlobalRotation.Y, 0), 1.2f/_rayCasts.Count);
					}
					else if (target is Player player)
					{
						//send damage signal to all
						player.Rpc("Damage", 15);
					}
				}
			}
			_spell = string.Empty;
			foreach (RayCast3D ray in _rayCasts)
			{
				ray.TargetPosition = new Vector3((float)GD.RandRange(-sway, sway), (float)GD.RandRange(-sway, sway), -50);
			}
		}
	}
}
