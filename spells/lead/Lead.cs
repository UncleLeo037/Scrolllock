using Godot;
using System;
using Srolllock.Spells;

public partial class Lead : TargetSpell
{
	public override void Activate(object target)
	{
		if (target is Player player) player.Rpc("Damage", 75 * Modifier);
	}
}
