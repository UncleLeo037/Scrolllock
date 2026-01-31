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

	public static async void SpawnSpell(Node spell, Vector3 point, float size = 0.0f)
	{
		string temp = spell.Name;
		self.AddChild(spell);
		Orb orb = self.GetNode<Orb>(temp);
		orb.GlobalPosition = point;
		//add in scale
		//orb.Scale += new Vector3(size, size, size);
		orb.Name = "old";
	}
}
