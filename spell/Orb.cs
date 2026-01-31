using Godot;
using System;

public partial class Orb : Node3D
{
	private double lifetime = 10.0;
	private float size = 6.0f;

	public override void _Ready()
	{
		//GD.Print(this.EditorDescription);
		//Add code here to decide collision shape based on EditorDescription
		//Will also decide visual effects and scale and lifetime here

		//create lookup system for spell attributes

		Scale = new Vector3(size, size, size);

		Area3D area = GetNode<Area3D>("Area3D");
		area.BodyEntered += _on_area_3d_body_entered;
		area.BodyExited += _on_area_3d_body_exited;

		string shape = "Sphere";

		//enable collision shape and decide lifetime at end alongside eachother
		area.GetNode<CollisionShape3D>($"Collision{shape}3D").Disabled = false;
		//this will be changed to handle more than instants
		if (this.EditorDescription.Contains("instant"))
		{
			lifetime = 0.1;
		}
	}

	public override void _Process(double delta)
	{
		lifetime -= delta;
		if (lifetime <= 0.0)
		{
			this.QueueFree();
		}
	}

	public void _on_area_3d_body_entered(Node3D body)
	{
		if (body is Player player)
		{
			if (this.EditorDescription.Contains("instant"))
			{
				//call instant effect method
				Vector3 origin = this.GlobalTransform.Origin;
				player.Velocity += (player.GlobalTransform.Origin - origin).Normalized() * 7.0f;
			}
			else
			{
				player.AddEffect(this.EditorDescription);
			}
		}
	}

	public void _on_area_3d_body_exited(Node3D body)
	{
		if (body is Player player)
		{
			//needs to skip if instant effect
			if (!this.EditorDescription.Contains("instant"))
			{
				player.RemoveEffect(this.EditorDescription);
			}
		}
	}
}
