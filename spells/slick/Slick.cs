using Godot;
using System;
using Srolllock.spells;

public partial class Slick : Spell, IEquipment
{
	private double lifetime = 5;
	public Texture2D Icon { get; set; } = GD.Load<Texture2D>($"res://spells/slick/slick.png");

	public override void _Ready()
	{
		Area3D area = GetNode<Area3D>("Area3D");
		area.BodyEntered += _on_area_3d_body_entered;
		area.BodyExited += _on_area_3d_body_exited;
		Scale = Scale * (0.5f + (0.5f * Modifier));
	}

	public override void _PhysicsProcess(double delta)
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
			if (!player.effects.Contains(GetType().Name))
			{
				player.HasFriction = false;
			}
			player.effects.Add(GetType().Name);
		}
	}

	public void _on_area_3d_body_exited(Node3D body)
	{
		if (body is Player player)
		{
			player.effects.Remove(GetType().Name);
			if (!player.effects.Contains(GetType().Name))
			{
				player.HasFriction = true;
			}
		}
	}
}
