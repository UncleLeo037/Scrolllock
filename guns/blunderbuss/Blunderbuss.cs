using Godot;
using Srolllock.guns;

public partial class Blunderbuss : Gun, IEquipment
{
	public Texture2D Icon { get; set; } = GD.Load<Texture2D>($"res://guns/blunderbuss/blunderbuss.png");

	public Blunderbuss(string name = null)
	{
		var temp = string.IsNullOrEmpty(name) ? GetType().Name : name;
		_modelName = $"{GetType().Name}/{temp}";
	}

	public override void Shoot()
	{
		if (_anime?.CurrentAnimation.ToString().Contains("Shoot") == false)
		{
			_anime.GetParent().Rpc("PlayAnim", "ShootRight", -0.65f, true);
			var target = _rayCast.GetCollider();
			if (target != null)
			{
				//should just shoot instead
				if (!string.IsNullOrEmpty(_spell))
				{
					//spells will be called in different ways here in future
					Vector3 point = _rayCast.GetCollisionPoint();
					//only sends signal to host for spawning spells
					SpellSpawner.instance.RpcId(1, "RequestSpawnSpell", _spell, point, new Vector3(this.GlobalRotation.X, this.GlobalRotation.Y, 0));
					_spell = string.Empty;
				}
				else if (target is Player player)
				{
					//send damage signal to all
					player.Rpc("Damage", 35);
				}
			}
		}
	}
}
