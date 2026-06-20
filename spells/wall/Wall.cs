using Godot;
using Srolllock.spells;
using System;

public partial class Wall : Spell, IEquipment
{
	double lifetime = 20.0;
	public Texture2D Icon {get; set;} = GD.Load<Texture2D>($"res://spells/wall/wall.png");
	
	public override void _PhysicsProcess(double delta)
	{
		lifetime -= delta;
		if (lifetime <= 0.0)
		{
			this.QueueFree();
		}
	}
}
