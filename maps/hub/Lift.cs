using Godot;
using System;

public partial class Lift : AnimatableBody3D
{
	[Export]
	public float top;
	[Export]
	public float bottom;

	private Vector3 target;
	const float SPEED = 1.0f;

	public override void _Ready()
	{
		target = GlobalPosition;
		SetProcess(false);
		Area3D area = GetNode<Area3D>("Area3D");
		area.BodyEntered += OnBodyEntered;
	}


	public override void _PhysicsProcess(double delta)
	{
		GlobalPosition = GlobalPosition.MoveToward(target, SPEED*(float)delta);
		if (GlobalPosition.Y == target.Y)
		{
			SetProcess(false);
		}
	}

	public void OnBodyEntered(Node3D body)
	{
		if (body is Player)
		{
			if (GlobalPosition.Y == bottom)
			{
				target.Y = top;
			}
			if (GlobalPosition.Y == top)
			{
				target.Y = bottom;
			}
			SetProcess(true);
		}
	}
}
