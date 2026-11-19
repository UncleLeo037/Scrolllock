using Godot;
using Srolllock.Spells;
using System;

public partial class Fireball : AreaSpell
{
	private double lifetime = 0.5;
	
	public override void _Ready()
	{
		Scale = Scale * (0.5f + (0.5f * Modifier));
		Area3D area = GetNode<Area3D>("Area3D");
		area.BodyEntered += _on_area_3d_body_entered;
	}

	public override void _PhysicsProcess(double delta)
	{
		lifetime -= delta;
		if (lifetime <= 0.0)
		{
			Dispel();
		}
	}

	public void _on_area_3d_body_entered(Node3D body)
	{
		if (body is Player player)
		{
			//player.Rpc("Damage", 75 * Modifier);
			player.Damage(75 * Modifier);
			player.Velocity += (player.GlobalTransform.Origin - this.GlobalTransform.Origin).Normalized() * (10.0f * Modifier);
		}
	}
}
