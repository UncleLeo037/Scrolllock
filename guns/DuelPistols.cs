using Godot;
using System;
using Srolllock.guns;

public partial class DuelPistols : Gun
{

	private RayCast3D _rayCast;
	private AnimationPlayer _anime;
	private GpuParticles3D _flash;

	public override void _Ready()
	{
		_rayCast = GetNode<RayCast3D>("RayCast3D");
		_anime = GetNode<AnimationPlayer>("AnimationPlayer");
		//_flash = GetNode<Node3D>("Node3D").GetNode<GpuParticles3D>("GPUParticles3D");
	}

	[Rpc(CallLocal = true)]
	public void ShootAnim()
	{
		_anime.Stop();
		_anime.Play("shoot");
		_flash.Restart();
		_flash.Emitting = true;
	}

	public override void Shoot()
	{
		if (_anime.CurrentAnimation != "shoot")
		{
			//add RPC shoot animation call. DO NOT move RPC from spell spawn to here! could cause spell sync issues
			//Rpc("ShootAnim");
			var target = _rayCast.GetCollider();
			if (target != null)
			{
				//should just shoot instead
				if (!string.IsNullOrEmpty(equipedSpell))
				{
					//spells will be called in different ways here in future and equipedSpell will be an object
					Vector3 point = _rayCast.GetCollisionPoint();
					SpellSpawner.instance.Rpc("RequestSpawnSpell", equipedSpell, point, new Vector3(this.GlobalRotation.X, this.GlobalRotation.Y, 0));
					equipedSpell = string.Empty;
				}
				else if (target is Player player)
				{
					player.Rpc("Damage", 35);
				}
			}
		}
	}
}
