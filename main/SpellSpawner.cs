using Godot;
using Godot.Collections;
using System;
using Srolllock.spells;

public partial class SpellSpawner : MultiplayerSpawner
{
	public static SpellSpawner instance;
	public override void _Ready()
	{
		SpawnFunction = Callable.From<Variant, Node>(SpawnSpell);
		instance = this;
	}

	public Node SpawnSpell(Variant detail)
	{
		Dictionary spell = (Dictionary)detail;
		PackedScene spellScene = GD.Load<PackedScene>($"res://equipment/spells/{spell["name"].ToString().ToLower()}/{spell["name"]}.tscn");
		Spell spellInstance = spellScene.Instantiate<Spell>();
		spellInstance.Position = (Vector3)spell["position"];
		spellInstance.Rotation = (Vector3)spell["rotation"];
		return spellInstance;
	}

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public void RequestSpawnSpell(string name, Vector3 pos, Vector3 rot)
    {
        if (!IsMultiplayerAuthority())
            return;

        Spawn(new Dictionary<string, Variant>
        {
            {"name", name},
            {"position", pos},
            {"rotation", rot}
        });
    }
}
