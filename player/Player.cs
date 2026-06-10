using Godot;
using Srolllock.spells;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Threading.Tasks;
using Srolllock.guns;

public partial class Player : CharacterBody3D
{
	private const float SPEED = 5.0f;
	private const float JUMP_VELOCITY = 4.5f;
	private const float SENSITIVITY = 0.08f;
	private const float FRICTION = 0.1f;
	private const int MAX_HEALTH = 100;

	public bool hasFriction = true;

	private Camera3D _camera;
	private CharacterBody3D _body;
	private CanvasLayer _hud;
	private MultiplayerSpawner _gunSpawner;
	//private AnimationPlayer _anime;

	private Gun _gun;
	private Dictionary<Key, Node> _equipment;
	private List<string> effects = new List<string>();
	private double _health = MAX_HEALTH;
	[Export]
	private PackedScene HUD;

	public override void _EnterTree()
	{
		SetMultiplayerAuthority(Name.ToString().ToInt());
	}

	public override void _Ready()
	{
		if (!IsMultiplayerAuthority()) return;
		//start loading screen

		AddChild(HUD.Instantiate());
		_hud = GetNode<CanvasLayer>("Hud");
		_camera = GetNode<Camera3D>("Camera3D");
		_body = GetNode<CharacterBody3D>(".");
		_gunSpawner = GetNode<MultiplayerSpawner>("GunSpawner");
		
		//Hides own overhead health
		GetNode<SubViewport>("SubViewport").GetNode<ProgressBar>("ProgressBar").Visible = false;

		_equipment = new Dictionary<Key, Node>()
		{
			{Key.Key1, GD.Load<PackedScene>("res://guns/DuelPistols.tscn").Instantiate<Gun>()},
			{Key.Key2, new Force()},
			{Key.Key3, new Wall()},
			{Key.Key4, new Tornado()},
			{Key.Key5, new Slick()},
			{Key.Key6, GD.Load<PackedScene>("res://guns/Blunderbuss.tscn").Instantiate<Gun>()},
			{Key.Key7, GD.Load<PackedScene>("res://guns/RifledMusket.tscn").Instantiate<Gun>()},
			// {"Key6", null},
			// {"Key7", null},
			// {"Key8", null},
			// {"Key9", null},
			// {"Key0", null},
			// {"Equal", null},
			// {"Minus", null}
		};

		// THIS DOES NOT WORK
		foreach (var pair in _equipment)
		{
			if (pair.Value is Gun gun)
			{
				_gunSpawner.AddSpawnableScene($"res://gun/{gun.GetType().Name}.tscn");
				_camera.AddChild(gun);
			}
		}

		Input.MouseMode = Input.MouseModeEnum.Captured;
		_camera.Current = true;
		
		//stop loading screen
	}


	public override void _UnhandledInput(InputEvent @event)
	{
		if (!IsMultiplayerAuthority()) return;

		if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
		{
			if (_equipment.TryGetValue(keyEvent.PhysicalKeycode, out Node item))
			{
				switch (item)
				{
					case Spell spell:
						if (_gun != null)
						{
							_gun.equipedSpell = spell.GetType().Name;
						}
						break;
					case Gun gun:
						if (_gun != null)
						{
							Rpc("ToggleGun", _gun.Name);
						}
						_gun = gun;
						Rpc("ToggleGun", _gun.Name);
						break;
				}
			}
		}

		if (@event is InputEventMouseMotion mouseMotion &&
			Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			_body.RotateY(Mathf.DegToRad(-mouseMotion.Relative.X * SENSITIVITY));
			_camera.RotateX(Mathf.DegToRad(-mouseMotion.Relative.Y * SENSITIVITY));
			_camera.Rotation = new Vector3(
				Mathf.Clamp(_camera.Rotation.X, Mathf.DegToRad(-90), Mathf.DegToRad(90)),
				_camera.Rotation.Y,
				_camera.Rotation.Z
			);
		}

		if (Input.IsActionJustPressed("shoot"))
		{
			//only shoot if exists
			_gun?.Shoot();
		}
	}
	
	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
	public async void ToggleGun(string gunPath)
	{
		//will add code to change player animation for gun here
		var gun = _camera.GetNode<Gun>(gunPath);
		gun.Visible = !gun.Visible;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!IsMultiplayerAuthority()) return;

		float deltaFloat = (float)delta;

		// Add gravity
		if (!IsOnFloor())
		{
			Velocity += GetGravity() * deltaFloat;
		}
		else
		{
			if (Input.IsActionJustPressed("jump"))
			{
				Velocity = new Vector3(Velocity.X, JUMP_VELOCITY, Velocity.Z);
			}
		}

		// Handle crouch
		if (Input.IsActionPressed("crouch"))
		{
			_camera.Position = new Vector3(_camera.Position.X, 0.3f, _camera.Position.Z);
		}

		if (Input.IsActionJustReleased("crouch"))
		{
			_camera.Position = new Vector3(_camera.Position.X, 0.6f, _camera.Position.Z);
		}

		// Get input direction
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
		Vector3 direction = (_body.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		// Calculate speed (sprint adds speed when moving forward)
		float totalSpeed = SPEED;
		if (inputDir.Y < 0 && Input.IsActionPressed("sprint"))
		{
			totalSpeed += 3.0f;
		}
		// Apply movement if direction exists and mouse is captured
		if (direction.Length() > 0 && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			Vector3 targetVelocity = new Vector3(
				direction.X * totalSpeed,
				Velocity.Y,
				direction.Z * totalSpeed
			);

			if (IsOnFloor())
			{
				if (hasFriction)
				{
					Velocity = targetVelocity;
				}
				Velocity = Velocity.MoveToward(targetVelocity, 1f * (float)delta);
			}
			else
			{
				Velocity = Velocity.MoveToward(targetVelocity, 15f * (float)delta);
			}
		}
		else
		{
			if (IsOnFloor() && hasFriction)
			{
				Velocity = new Vector3(
					Mathf.MoveToward(Velocity.X, 0, Math.Abs(Velocity.X) * 0.1f),
					Velocity.Y,
					Mathf.MoveToward(Velocity.Z, 0, Math.Abs(Velocity.Z) * 0.1f)
				);
			}
		}

		// Quick fall recovery
		if (_body.Position.Y < -100)
		{
			Rpc("Damage", MAX_HEALTH);
		}

		//if (_anime.CurrentAnimation == "Shoot")
		//{
		//	//do nothing
		//}
		//else if (direction.Length() > 0 && IsOnFloor())
		//{
		//	_anime.Play("move");
		//}
		//else
		//{
		//	_anime.Play("idle");
		//}

		MoveAndSlide();
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void Damage(int damage)
	{
		_health -= damage;
		if (_health <= 0)
		{
			if (IsMultiplayerAuthority()) _body.Position = new Vector3(0, 0, 0);
			_health = MAX_HEALTH;
		}
		GetNode<SubViewport>("SubViewport").GetNode<ProgressBar>("ProgressBar").Value = _health;
		if (IsMultiplayerAuthority()) _hud.GetNode<ProgressBar>("ProgressBar").Value = _health;
	}

	//move all animation methods to dedicated animation class and gun animation class
	[Rpc(CallLocal = true)]
	public void PlayShoot()
	{
		//_anime.Stop();
		//_anime.Play("Shoot");
		//_flash.Restart();
		//_flash.Emitting = true;
	}

	//[Signal]
	//private delegate _on_animation_player_animation_finished()
	//{
	//if (_anime.CurrentAnimation != "Shoot")
	//{
	//_anime.Play("idle");
	//}
	//}
}
