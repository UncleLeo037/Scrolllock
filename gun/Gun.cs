using Godot;
using System;

public partial class Gun : Equipment
{
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
