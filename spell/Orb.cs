using Godot;
using System;

public partial class Orb : Node3D
{
	private float area = 0.0f;

	private CollisionShape3D hit;

	public override void _Ready()
	{
		//Scale = new Vector3(0, 0, 0);
		//hit = GetNode<Area3D>("Area3D").GetNode<CollisionShape3D>("CollisionShape3D");
		this.Scale = new Vector3(7, 7, 7);
		//spawn in with radius of 0
	}

	public override void _PhysicsProcess(double delta)
	{
		//over time expand till radius is equal to max radius
	}

	public void _on_area_3d_body_entered(Node3D body)
	{
		AreaOfEffect(body);
	}

	public void _on_area_3d_body_exited(Node3D body)
	{
		EndOfEffect(body);
	}

	public void SetPosition(Vector3 position)
	{
		GlobalPosition = position;
	}

	//the below methods will be overridden in child classes
	private void AreaOfEffect(Node3D body)
	{
		//need to add code for modifying other objects later
		if (body is Player player)
		{
			player.Modify(-3.0f, false);
		}
	}

	private void EndOfEffect(Node3D body)
	{
		if (body is Player player)
		{
			player.Modify();
		}
	}
}
