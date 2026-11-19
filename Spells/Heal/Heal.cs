using Godot;
using Srolllock.Spells;
using System;
using System.Collections.Generic;

public partial class Heal : AreaSpell
{
	private double lifetime = 2;
	private List<Player> players = new();
	
	public override void _Ready()
	{
		Scale = Scale * (0.5f + (0.5f * Modifier));
		Area3D area = GetNode<Area3D>("Area3D");
		area.BodyEntered += _on_area_3d_body_entered;
	}

	public override void _PhysicsProcess(double delta)
	{
		lifetime -= delta;
		foreach (Player player in players)
		{
			player?.Damage(-0.5 * Modifier);
		}
		if (lifetime <= 0.0)
		{
			Dispel();
		}
	}

	public void _on_area_3d_body_entered(Node3D body)
	{
		if (body is Player player)
		{
			players.Add(player);
		}
	}
}
