using Godot;
using System;

public partial class Gun : Node3D
{
	private PackedScene gunModel;
	private RayCast3D _rayCast;
	public string equipedSpell = string.Empty;

	public override void _Ready()
	{
		_rayCast = GetNode<RayCast3D>("RayCast3D");
	}

	public void Shoot()
	{
		if(_rayCast.IsColliding())
		{
			//should just shoot instead
			if (equipedSpell == string.Empty) return;

			Vector3 point = _rayCast.GetCollisionPoint();
			SpellSpawner.CastSpell(this.Name, equipedSpell, point, new Vector3(this.GlobalRotation.X, this.GlobalRotation.Y, 0));

			equipedSpell = string.Empty;
		}
	}
}
