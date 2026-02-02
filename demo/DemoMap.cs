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

	public static void CastSpell(string playerName, string spellName, Vector3 position, Vector3 rotation)
	{
		self.Rpc("SpawnSpell", playerName, spellName, position, rotation);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void SpawnSpell(string playerName, string spellName, Vector3 position, Vector3 rotation)
	{
		PackedScene spellScene = GD.Load<PackedScene>($"res://spells/{spellName}");
		Node spell = spellScene.Instantiate();
		spell.Name = playerName;
		AddChild(spell);
		Node3D node = GetNode<Node3D>((string)spell.Name);
		node.GlobalPosition = position;
		node.Rotation = rotation;
		node.Name = "old";
	}
}
