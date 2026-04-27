using Godot;
using System;

public partial class Gun : Equipment
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

	//these need to be RPC
	public override void Equip()
	{
		//will need to call un-equip for currently equiped gun if there is one
		//will need to set self as active gun for player
		//will need to show gun model
		throw new NotImplementedException();
	}

	public override void Unequip()
	{
		//will need to hide gun model
		throw new NotImplementedException();
	}
}
