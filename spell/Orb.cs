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

	public void _on_area_3d_body_entered(Node3D body)
	{
		AreaOfEffect(body);
	}

	public void _on_area_3d_body_exited(Node3D body)
	{
		EndOfEffect(body);
	}

	//the below methods will be overridden in child classes
	private void AreaOfEffect(Node3D body)
	{
		if (body is Player player)
		{
			player.AddEffect(this.EditorDescription);
		}
	}

	private void EndOfEffect(Node3D body)
	{
		if (body is Player player)
		{
			player.RemoveEffect(this.EditorDescription);
		}
	}
}
