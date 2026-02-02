using Godot;
using System;

public partial class Wall : Node3D
{
	double lifetime = 20.0;

	public override void _Process(double delta)
	{
		lifetime -= delta;
		if (lifetime <= 0.0)
		{
			this.QueueFree();
		}
	}
}
