using Godot;
using System;

public partial class Ghost : CharacterBody3D
{
	private NavigationAgent3D _navAgent;
	private Player _target;
	public float speed = 3.0f;
	public float memory = 15.0f;

	// public Ghost(float speed, float size, float memory)
	// {
	// 	this.speed = speed;
	// 	Scale = Scale*size;
	// 	this.memory = memory;
	// }

	public override void _Ready()
	{
		_navAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
		_navAgent.PathDesiredDistance = 0.5f;
		_navAgent.TargetDesiredDistance = 0.5f;

		Area3D vision = GetNode<Area3D>("Area3D");
		vision.BodyEntered += Detect;
		vision.BodyExited += Hidden;
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
			if (_target != null)
			{
				_navAgent.TargetPosition = _target.GlobalTransform.Origin;
			}
			if (_navAgent.IsNavigationFinished())
			{
				Attack();
				return;
			}

			Vector3 currentAgentPosition = GlobalTransform.Origin;
			Vector3 nextPathPosition = _navAgent.GetNextPathPosition();

			Velocity = currentAgentPosition.DirectionTo(nextPathPosition) * speed;

			//forgets player direction when player is null but can add look about code then
			var angle = Rotation.AngleTo(new Vector3(0, Velocity.X, Velocity.Z));
			Rotation = Rotation.Lerp(new Vector3(0, angle, 0), (float)delta * 5);
		}
		MoveAndSlide();
	}

	public override void _Process(double delta)
	{
		memory -= 1 * (float)delta;
		if (memory < 0)
		{
			_target = null;
			SetProcess(false);
		}
	}


	public void Attack()
	{
		if (_target != null)
		{
			//check that _target Player is in attack area and then deal damage
			//also add some delay between attacks and before can move again
		}
	}

	public void Detect(Node3D node)
	{
		if (node is Player player)
		{
			_target = player;
			SetProcess(false);
		}
	}

	public void Hidden(Node3D node)
	{
		if (node is Player)
		{		
			memory = 5.0f;
			SetProcess(true);
		}
	}
}
