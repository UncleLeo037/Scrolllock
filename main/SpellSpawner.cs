using Godot;
using System;
using System.Collections.Generic;

public partial class SpellSpawner : Node3D
{
	private static Node3D self;
	private static Dictionary<string, PackedScene> spellDictionary;

	public override void _Ready()
	{
		self = this;

		//adjust this to be based on equiped spells
		//will need to account for spells of all players in game
		spellDictionary = new Dictionary<string, PackedScene>()
		{
			{"Force", GD.Load<PackedScene>("res://spells/Force.tscn")},
			{"Wall", GD.Load<PackedScene>("res://spells/Wall.tscn")},
			{"Tornado", GD.Load<PackedScene>("res://spells/Tornado.tscn")}
		};
	}

	public static void CastSpell(string playerName, string spellName, Vector3 position, Vector3 rotation)
	{
		self.Rpc("SpawnSpell", playerName, spellName, position, rotation);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void SpawnSpell(string playerName, string spellName, Vector3 position, Vector3 rotation)
	{
		PackedScene spellScene = spellDictionary[spellName];
		Node spell = spellScene.Instantiate();
		spell.Name = playerName;
		AddChild(spell);
		Node3D node = GetNode<Node3D>((string)spell.Name);
		node.GlobalPosition = position;
		node.Rotation = rotation;
		node.Name = "old";
	}
}
