using Godot;
using Srolllock.spells;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Srolllock.guns
{
	public partial class Gun : Node3D
	{
		protected string _spell = string.Empty;
		protected List<RayCast3D> _rayCasts = new List<RayCast3D>();
		protected Player _playerRef;
		protected Node3D _model;
		protected string _modelPath;
		protected RandomNumberGenerator rand = new RandomNumberGenerator();

		protected double cooldown = 0.6;
		protected double timer = 0.0;
		protected double sway = 3.0;

		public override void _Ready()
		{
			RayCast3D temp = new RayCast3D();
			temp.SetCollisionMaskValue(1, true);
			temp.SetCollisionMaskValue(2, true);
			temp.SetCollisionMaskValue(3, true);
			temp.TargetPosition = new Vector3((float)GD.RandRange(-sway, sway), (float)GD.RandRange(-sway, sway), -100);
			_rayCasts.Add(temp);
			AddChild(temp);
			//need to move this logic out of the local node
			//_anime = GetNode<AnimationPlayer>("AnimationPlayer");
			//_flash = GetNode<Node3D>("Node3D").GetNode<GpuParticles3D>("GPUParticles3D");
		}

		public override void _PhysicsProcess(double delta)
		{
			if (timer > 0.0) timer -= 1 * delta;
		}

		public void Setup(Camera3D camera)
		{
			_playerRef = camera.GetParent<Player>();
			camera.AddChild(this);
		}

		//pistol will have multiple flash nodes so will need to make an override method for this in Pistols.cs
		public virtual void SpawnModel(GunSpawner spawner, GunSpawner temp)
		{
			SetPhysicsProcess(true);
			_model = (Node3D)spawner.Spawn(_modelPath);

			// use something like this paired with a dynamic shared autospawn list
			//PackedScene gunScene = GD.Load<PackedScene>($"res://guns/{_modelPath}.gltf");
			//_playerRef.GetNode("Camera3D/Left").AddChild(gunScene.Instantiate());
		}

		public virtual void Despawn()
		{
			_model.QueueFree();
			SetPhysicsProcess(false);
		}

		public void SetSpell(Spell spell)
		{
			_spell = spell?.GetType().Name ?? string.Empty;
		}

		public virtual async void Shoot()
		{
			if (timer <= 0.0)
			{
				timer = cooldown;
				_playerRef.Rpc("PlayAnim", "ShootRight", -1f, true);
				var target = _rayCasts.First().GetCollider();
				if (target != null)
				{
					//should just shoot instead
					if (!string.IsNullOrEmpty(_spell))
					{
						//spells will be called in different ways here in future
						Vector3 point = _rayCasts.First().GetCollisionPoint();
						//only sends signal to host for spawning spells
						SpellSpawner.instance.RpcId(1, "RequestSpawnSpell", _spell, point, new Vector3(this.GlobalRotation.X, this.GlobalRotation.Y, 0), 1);
						_spell = string.Empty;
					}
					else if (target is Player player)
					{
						//send damage signal to all
						player.Rpc("Damage", 50);
					}
				}
				_rayCasts.First().TargetPosition = new Vector3((float)GD.RandRange(-sway, sway), (float)GD.RandRange(-sway, sway), -100);
			}
		}
	}
}