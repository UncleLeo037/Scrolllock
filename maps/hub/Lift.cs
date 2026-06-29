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
		//set authority to host
        if (Multiplayer.IsServer())
            SetMultiplayerAuthority(1);

        if (IsMultiplayerAuthority())
        {
            target = Position;
            Area3D area = GetNode<Area3D>("Area3D");
            area.BodyEntered += OnBodyEntered;
        }
		else
		{
			SetPhysicsProcess(false);
		}
	}


	public override void _PhysicsProcess(double delta)
	{
		Position = Position.MoveToward(target, SPEED*(float)delta);
		if (Position.Y == target.Y)
		{
			SetProcess(false);
		}
	}

	public void OnBodyEntered(Node3D body)
	{
		if (body is Player)
		{
			if (Position.Y == bottom)
			{
				target.Y = top;
			}
			if (Position.Y == top)
			{
				target.Y = bottom;
			}
			SetProcess(true);
		}
	}
}
