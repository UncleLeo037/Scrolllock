using Godot;
using Godot.Bridge;
using Srolllock.Spells;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Srolllock.Guns
{
	public partial class Gun : Node3D, IEquipment
	{
		public Texture2D Icon { get; set; }
		public string HudSlot { get; set; } = "Gun";
		protected Action<Vector3, object> _method { get; set; } = null;
		protected Action<Vector3, object> _action { get; set; } = null; 
		protected Player _playerRef;
		protected float _modifier = 1;
		protected Node3D _model;
		protected string _modelPath;
		protected RandomNumberGenerator rand = new RandomNumberGenerator();
		protected RayCast3D _rayCast = new RayCast3D();

		protected double cooldown = 0.6;
		protected double timer = 0.0;
		protected double sway = 3.0;
		protected int maxAmmo = 1;
		protected int ammo = 1;

		public override void _Ready()
		{
			_rayCast.SetCollisionMaskValue(1, true);
			_rayCast.SetCollisionMaskValue(2, true);
			_rayCast.SetCollisionMaskValue(3, true);
			AddChild(_rayCast);
			_action = (vec, obj) => (new Lead()).Activate(obj);
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

		public virtual async void SetSpell(ISpell spell)
		{
			switch (spell)
			{
				case TargetSpell targetSpell:
					targetSpell.Modifier = _modifier;
					_method = (vec, obj) => targetSpell.Activate(obj);
					break;
				case AreaSpell areaSpell:
					_method = (vec, obj) => SpellSpawner.instance.RpcId(1, "RequestSpawnSpell", areaSpell.GetType().Name, vec, new Vector3(this.GlobalRotation.X, this.GlobalRotation.Y, 0), _modifier);
					break;
				//case TargetSpell selfSpell:
				//	method = vec => Callable.From(() => targetSpell.Activate(_playerRef)).CallDeferred();
				//	break;
				default:
					return;
			}
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

		public virtual async void Reload(bool forceReload = false) {
			string reloadAnim = "ReloadRight";//$"Reload{GetType().Name}";
			if (forceReload)
			{
				//continue
			}
			else if (_playerRef.AnimPlayer.CurrentAnimation == reloadAnim || ammo == maxAmmo)
			{
				return;
			}
			_playerRef.Rpc("PlayAnim", reloadAnim, 0);
			Variant[] anim = await ToSignal(_playerRef.AnimPlayer, AnimationPlayer.SignalName.AnimationFinished);
			if (anim[0].AsString() == reloadAnim)
			{
				ammo = maxAmmo;
				_action = _method;
			}
		}
	}
}