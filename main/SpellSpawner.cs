using Godot;
using Godot.Collections;
using Srolllock.spells;

public partial class SpellSpawner : MultiplayerSpawner
{
	public static SpellSpawner self;
	public override void _Ready()
	{
		SpawnFunction = Callable.From<Variant, Node>(SpawnSpell);
		self = this;
	}

	public Node SpawnSpell(Variant detail)
	{
		Dictionary spell = (Dictionary)detail;
		PackedScene spellScene = GD.Load<PackedScene>($"res://{spell["path"]}.tscn");
		Spell spellInstance = spellScene.Instantiate<Spell>();
		spellInstance.Position = (Vector3)spell["position"];
		spellInstance.Rotation = (Vector3)spell["rotation"];
		return spellInstance;
	}

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void RequestCastSpell(string path, Vector3 pos, Vector3 rot)
    {
        if (!IsMultiplayerAuthority())
            return;

        Spawn(new Dictionary<string, Variant>
        {
            {"path", path},
            {"position", pos},
            {"rotation", rot}
        });
    }

    public static void CastSpell(string path, Vector3 pos, Vector3 rot)
    {
        if (self.IsMultiplayerAuthority())
        {
            self.Spawn(new Dictionary<string, Variant>
            {
                {"path", path},
                {"position", pos},
                {"rotation", rot}
            });
        }
        else
        {
            self.Rpc("RequestCastSpell", path, pos, rot);
        }
    }
}
