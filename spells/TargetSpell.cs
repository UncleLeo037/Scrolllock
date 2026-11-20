using Godot;
using System;

public partial class TargetSpell : ISpell, IEquipment
{
	public double Cooldown { get; set; } = 0;
	public float Modifier { get; set; } = 1;
	public Texture2D Icon { get; set; }
	public string HudSlot { get; set; } = "Ammo";

	public TargetSpell()
	{
		Icon = GD.Load<Texture2D>($"res://Spells/{GetType().Name}/{GetType().Name}.png");
	}

	public virtual void Activate(object target)
	{
		//do nothing and look pretty
	}
}
