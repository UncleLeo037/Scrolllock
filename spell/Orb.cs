using Godot;
using Srolllock.spell;
using System;

public partial class Orb : Node3D
{
	private double lifetime;
	Spell s;

	public override void _Ready()
	{
		Area3D area = GetNode<Area3D>("Area3D");
		area.BodyEntered += _on_area_3d_body_entered;
		area.BodyExited += _on_area_3d_body_exited;

		s = SpellFactory.CreateSpell(this.EditorDescription);

		this.Scale = s.size;

		//enable collision shape and decide lifetime at end alongside eachother
		area.GetNode<CollisionShape3D>($"Collision{s.shape}3D").Disabled = false;
		lifetime = s.lifetime;
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
		s.OnEnter(body, this);
	}

	public void _on_area_3d_body_exited(Node3D body)
	{
		s.OnExit(body, this);
	}
}
