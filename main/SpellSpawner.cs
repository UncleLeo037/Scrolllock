using Godot;
using Srolllock.spells;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;

public partial class SpellSpawner : Node3D
{
	private static Node3D self;
	private static Dictionary<string, PackedScene> spellDictionary;

	public override void _Ready()
	{
		self = this;

		SetSpells();
	}

	public void SetSpells()
	{
		//this string list would be grabbed from a save file
		string[] inventory = {"Force", "Wall", "Tornado", "Slick"};

		spellDictionary = new Dictionary<string, PackedScene>();

		foreach (string spell in inventory)
		{
			spellDictionary.Add(spell, GD.Load<PackedScene>($"res://spells/{spell}.tscn"));
		}
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
		Spell node = GetNode<Spell>((string)spell.Name);
		node.GlobalPosition = position;
		node.Rotation = rotation;
		node.Name = "old";
	}
}
