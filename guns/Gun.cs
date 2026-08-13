using Godot;
using Srolllock.spells;
using System;

namespace Srolllock.guns
{
	public partial class Gun : Node3D
	{
		protected string _spell = string.Empty;
		protected RayCast3D _rayCast;
		protected AnimationPlayer _anime;
		protected Node3D _model;
		protected string _modelName;

		protected double cooldown = 0.5;
		protected double timer = 0.0;

		public override void _Ready()
		{
			_rayCast = new RayCast3D();
			AddChild(_rayCast);
			_rayCast.TargetPosition = new Vector3(0, 0, -100);
			_rayCast.SetCollisionMaskValue(1, true);
			_rayCast.SetCollisionMaskValue(2, true);
			_rayCast.SetCollisionMaskValue(3, true);
			//need to move this logic out of the local node
			//_anime = GetNode<AnimationPlayer>("AnimationPlayer");
			//_flash = GetNode<Node3D>("Node3D").GetNode<GpuParticles3D>("GPUParticles3D");
		}

		public override void _PhysicsProcess(double delta)
		{
			if (timer > 0.0) timer -= 1 * delta;
		}

		public void Setup(AnimationPlayer anim, Camera3D camera)
		{
			_anime = anim;
			camera.AddChild(this);
		}

		//pistol will have multiple flash nodes so will need to make an override method for this in Pistols.cs
		public virtual void SpawnModel(GunSpawner spawner, GunSpawner temp)
		{
			_model = (Node3D)spawner.Spawn(_modelName);
			//_anime = model.GetNode<AnimationPlayer>("AnimationPlayer");
			//_flash = model.GetNode<GpuParticles3D>("GpuParticles3D");
		}

		public virtual void Despawn()
		{
			_model.QueueFree();
		}

		public void SetSpell(Spell spell)
		{
			_spell = spell?.GetType().Name ?? string.Empty;
		}

		public virtual void Shoot()
		{
			if (timer <= 0.0)
			{
				timer = cooldown;
				_anime.GetParent().Rpc("PlayAnim", "ShootRight", -1f, true);
				var target = _rayCast.GetCollider();
				if (target != null)
				{
					//should just shoot instead
					if (!string.IsNullOrEmpty(_spell))
					{
						//spells will be called in different ways here in future
						Vector3 point = _rayCast.GetCollisionPoint();
						//only sends signal to host for spawning spells
						SpellSpawner.instance.RpcId(1, "RequestSpawnSpell", _spell, point, new Vector3(this.GlobalRotation.X, this.GlobalRotation.Y, 0));
						_spell = string.Empty;
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