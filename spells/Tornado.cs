using Godot;
using System;
using System.Diagnostics.CodeAnalysis;

public partial class Tornado : Node3D
{
	private double lifetime = 3;
	Area3D area;
	Node3D center;
	Node3D pull;
	public override void _Ready()
	{
		area = GetNode<Area3D>("Area3D");
		center = GetNode<Node3D>("Center");
		pull = center.GetNode<Node3D>("Pull");
	}

	public override void _Process(double delta)
	{
		this.Rotation = Vector3.Zero;
		//defines tornado center pull point, need to make rotating
		center.RotateY((float)(delta * 10.0));
		foreach (Player player in area.GetOverlappingBodies())
		{
			player.Velocity = (pull.GlobalTransform.Origin - player.GlobalTransform.Origin).Normalized() * 15.0f;
		}

		lifetime -= delta;
		if (lifetime <= 0.0)
		{
			this.QueueFree();
		}
	}
}
