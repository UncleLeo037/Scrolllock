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
		protected Player _playerRef;
		protected Node3D _model;
		protected string _modelPath;
		protected RandomNumberGenerator rand = new RandomNumberGenerator();

		protected double cooldown = 0.6;
		protected double reload = 1.5;
		protected double timer = 0.0;
		protected double sway = 3.0;

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

		public virtual void Aim()
		{
			_playerRef.IsAiming = true;
			_playerRef.Rpc("PlayAnim", "AimRight", 0);
		}

		public virtual async void Shoot()
		{
			//do nothing and look pretty
		}
	}
}