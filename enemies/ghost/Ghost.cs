using Godot;
using System;

public partial class Ghost : CharacterBody3D
{
	private NavigationAgent3D navAgent;



	private float speed = 3.0f;
	private Player target;
	private float memory = 10.0f;

	public override void _Ready()
	{
		navAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
		navAgent.PathDesiredDistance = 0.5f;
		navAgent.TargetDesiredDistance = 0.5f;

		Area3D vision = GetNode<Area3D>("Area3D");
		vision.BodyEntered += Detect;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!IsMultiplayerAuthority()) return;
		if (!IsOnFloor())
		{
			Velocity += GetGravity() * (float)delta;
		}
		else
		{
			if (target != null)
			{
				navAgent.TargetPosition = target.GlobalTransform.Origin;
				memory -= 1 * (float)delta;
				if (memory < 0)
				{
					target = null;
					memory = 5.0f;
				}
			}
			if (navAgent.IsNavigationFinished())
			{
				return;
			}

			Vector3 currentAgentPosition = GlobalTransform.Origin;
			Vector3 nextPathPosition = navAgent.GetNextPathPosition();

			Velocity = currentAgentPosition.DirectionTo(nextPathPosition) * speed;

			//forgets player direction when player is null but can add look about code then
			var angle = Rotation.AngleTo(new Vector3(0, Velocity.X, Velocity.Z));
			Rotation = Rotation.Lerp(new Vector3(0, angle, 0), (float)delta * 5);
		}
		MoveAndSlide();
	}

	public void Detect(Node3D node)
	{
		if (node is Player player)
		{
			target = player;
		}
	}
}
