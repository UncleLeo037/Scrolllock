using Godot;
using System;
using Srolllock.spells;

public partial class Slick : Spell
{
	private double lifetime = 5;

	public override void _Ready()
	{
		Area3D area = GetNode<Area3D>("Area3D");
		area.BodyEntered += _on_area_3d_body_entered;
		area.BodyExited += _on_area_3d_body_exited;
	}

	public override void _Process(double delta)
	{
		//change to use animation waiting system to wait for spell effect to finish playing
		lifetime -= delta;
		if (lifetime <= 0.0)
		{
			this.QueueFree();
		}
	}

	public void _on_area_3d_body_entered(Node3D body)
	{
		//need to change this to add no firction to effects stack as it will cause issues if there are multiple slick spells
		if (body is Player player)
		{
			player.hasFriction = false;
		}
	}

	public void _on_area_3d_body_exited(Node3D body)
	{
		//this will remove one from the stack
		if (body is Player player)
		{
			player.hasFriction = true;
		}
	}
}
