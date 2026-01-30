using Godot;
using System;

public partial class DemoMap : Node3D
{
	private static Node3D self;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		self = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public static void SpawnSpell(PackedScene spell, Vector3 point)
	{
		self.AddChild(spell.Instantiate());
		Orb orb = self.GetNode<Orb>("Orb");
		string newName = "Orb_" + Guid.NewGuid().ToString();
		orb.Name = newName;
		Orb temp = self.GetNode<Orb>(newName);
		temp.SetPosition(point);
	}
}
