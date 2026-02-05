using Godot;
using System;

public partial class SpellSpawner : Node3D
{
	private static Node3D self;

	public override void _Ready()
	{
		self = this;
	}

	public static void CastSpell(string playerName, string spellName, Vector3 position, Vector3 rotation)
	{
		self.Rpc("SpawnSpell", playerName, spellName, position, rotation);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void SpawnSpell(string playerName, string spellName, Vector3 position, Vector3 rotation)
	{
		PackedScene spellScene = GD.Load<PackedScene>($"res://spells/{spellName}.tscn");
		Node spell = spellScene.Instantiate();
		spell.Name = playerName;
		AddChild(spell);
		Node3D node = GetNode<Node3D>((string)spell.Name);
		node.GlobalPosition = position;
		node.Rotation = rotation;
		node.Name = "old";
	}
}
