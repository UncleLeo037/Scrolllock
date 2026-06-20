using Godot;
using Srolllock.spells;
using System;

public partial class Wall : Spell
{
	double lifetime = 20.0;

	public override void _PhysicsProcess(double delta)
	{
		lifetime -= delta;
		if (lifetime <= 0.0)
		{
			this.QueueFree();
		}
	}
}
