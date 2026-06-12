using Godot;
using System;

public partial class Ghost : CharacterBody3D
{
	private NavigationAgent3D navAgent;

	private float speed = 2.0f;
    private Player target;

	public override void _Ready()
	{
		navAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
		navAgent.PathDesiredDistance = 0.5f;
		navAgent.TargetDesiredDistance = 0.5f;

		Area3D area = GetNode<Area3D>("Area3D");
		area.BodyEntered += Detect;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (!IsMultiplayerAuthority()) return;
		base._PhysicsProcess(delta);
		if (target != null)
		{
			navAgent.TargetPosition = target.GlobalTransform.Origin;
		}
        if (navAgent.IsNavigationFinished())
        {
            return;
        }

        Vector3 currentAgentPosition = GlobalTransform.Origin;
        Vector3 nextPathPosition = navAgent.GetNextPathPosition();

        Velocity = currentAgentPosition.DirectionTo(nextPathPosition) * speed;
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
