using Godot;
using System;

namespace Srolllock.guns
{
	public partial class Gun : Node3D
	{
		public string equipedSpell = string.Empty;
		protected RayCast3D _rayCast;
		protected AnimationPlayer _anime;
		protected GpuParticles3D _flash;
		protected Node _model;

		public override void _Ready()
		{
			_rayCast = GetNode<RayCast3D>("RayCast3D");
			//need to move this logic out of the local node
			//_anime = GetNode<AnimationPlayer>("AnimationPlayer");
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

		//pistol will have multiple flash nodes so will need to make an override method for this in Pistols.cs
		public virtual void SetModel(Node model)
		{
			_model = model;
			//_anime = model.GetNode<AnimationPlayer>("AnimationPlayer");
			//_flash = model.GetNode<GpuParticles3D>("GpuParticles3D");
		}

		public virtual void Shoot()
		{
			if (!(_anime?.CurrentAnimation == "shoot"))
			{
				//Rpc("ShootAnim");
				var target = _rayCast.GetCollider();
				if (target != null)
				{
					//should just shoot instead
					if (!string.IsNullOrEmpty(equipedSpell))
					{
						//spells will be called in different ways here in future
						Vector3 point = _rayCast.GetCollisionPoint();
						//only sends signal to host for spawning spells
						SpellSpawner.instance.RpcId(1, "RequestSpawnSpell", equipedSpell, point, new Vector3(this.GlobalRotation.X, this.GlobalRotation.Y, 0));
						equipedSpell = string.Empty;
					}
					else if (target is Player player)
					{
						//send damage signal to all
						player.Rpc("Damage", 35);
					}
				}
			}
		}
	}
}