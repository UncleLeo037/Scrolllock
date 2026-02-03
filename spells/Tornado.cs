using Godot;
using System;
using System.Diagnostics.CodeAnalysis;

public partial class Tornado : Node3D
{
	private double lifetime = 3;
	Area3D area;
	public override void _Ready()
	{
		area = GetNode<Area3D>("Area3D");
	}

	public override void _Process(double delta)
	{
		this.Rotation = Vector3.Zero;
		//defines tornado center pull point, need to make rotating
		Vector3 pull = new Vector3(0, 5, 0);
		foreach (Player player in area.GetOverlappingBodies())
		{
			player.Velocity = ((this.GlobalTransform.Origin + pull) - player.GlobalTransform.Origin).Normalized() * 20.0f;
		}

		lifetime -= delta;
		if (lifetime <= 0.0)
		{
			this.QueueFree();
		}
	}
}
