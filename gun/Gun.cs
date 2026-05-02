using Godot;
using System;

public partial class Gun : Node3D
{
	private PackedScene gunModel;
	private RayCast3D _rayCast;
	public string equipedSpell = string.Empty;

	public override void _Ready()
	{
		_rayCast = GetNode<RayCast3D>("RayCast3D");
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void ShootAnim()
	{
		
	}

	public void Shoot()
	{
		//add RPC shoot animation call. DO NOT move RPC from spell spawn to here! could cause spell sync issues
		this.ShootAnim();
		var target = _rayCast.GetCollider();
		if(target != null)
		{
			//should just shoot instead
			if (equipedSpell != string.Empty)
			{
				//spells will be called in different ways here in future and equipedSpell will be an object
				Vector3 point = _rayCast.GetCollisionPoint();
				SpellSpawner.CastSpell(this.Name, equipedSpell, point, new Vector3(this.GlobalRotation.X, this.GlobalRotation.Y, 0));
				//equipedSpell = string.Empty;
			}
			if (target is Player player)
			{
				player.Rpc("Damage");
			}
		}
	}
}
