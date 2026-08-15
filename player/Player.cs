using Godot;
using Srolllock.spells;
using System;
using System.Collections.Generic;
using Srolllock.guns;

public partial class Player : CharacterBody3D
{
	private const float SPEED = 5.0f;
	private const float JUMP_VELOCITY = 4.5f;
	private const float SENSITIVITY = 0.08f;
	private const float FRICTION = 0.1f;
	private const int MAX_HEALTH = 100;

	private Camera3D _camera;
	private CharacterBody3D _body;
	private Hud _hud;
	private GunSpawner _rightSpawner;
	private GunSpawner _leftSpawner;
	private GpuParticles3D _rightFlash;
	private GpuParticles3D _leftFlash;
	private AnimationPlayer _anime;
	private Gun _gun;
	private Spell _spell;
	private double _health = MAX_HEALTH;

	public List<string> effects = new List<string>();
	//this should be expanded into a list/dict of bools that turn effects on and off
	public bool HasFriction = true;
	public bool IsAiming = false;

	public override void _EnterTree()
	{
		SetMultiplayerAuthority(Name.ToString().ToInt());
	}

	public override void _Ready()
	{
		_anime = GetNode<AnimationPlayer>("AnimationPlayer");
		_rightFlash = GetNode<GpuParticles3D>("Camera3D/Right/MuzzleFlash");
		_leftFlash = GetNode<GpuParticles3D>("Camera3D/Left/MuzzleFlash");

		if (!IsMultiplayerAuthority())
		{
			//disable processing & inputs for non auth player
			SetPhysicsProcess(false);
			SetProcessInput(false);
			return;
		}
		//loading screen should probably be started by join or host button

		AddChild(GD.Load<PackedScene>($"res://player/Hud.tscn").Instantiate());
		_hud = GetNode<Hud>("Hud");
		_camera = GetNode<Camera3D>("Camera3D");
		_body = GetNode<CharacterBody3D>(".");

		_rightSpawner = GetNode<GunSpawner>("Camera3D/Right/GunSpawner");
		_rightSpawner.SetMultiplayerAuthority(int.Parse(Name));

		_leftSpawner = GetNode<GunSpawner>("Camera3D/Left/GunSpawner");
		_leftSpawner.SetMultiplayerAuthority(int.Parse(Name));

		//Hides own overhead health
		GetNode<ProgressBar>("SubViewport/ProgressBar").Visible = false;

		foreach (var item in _hud.Loadout) if (item is Gun gun) gun.Setup(_camera);

		Input.MouseMode = Input.MouseModeEnum.Captured;
		_camera.Current = true;

		//stop loading screen because camera is present
	}

	public override void _Input(InputEvent @event)
	{
		//prevents fps controls when in menu
		if (Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			if (@event is InputEventMouseMotion mouseMotion)
			{
				_body.RotateY(Mathf.DegToRad(-mouseMotion.Relative.X * SENSITIVITY));
				_camera.RotateX(Mathf.DegToRad(-mouseMotion.Relative.Y * SENSITIVITY));
				_camera.Rotation = new Vector3(
					Mathf.Clamp(_camera.Rotation.X, Mathf.DegToRad(-90), Mathf.DegToRad(90)),
					_camera.Rotation.Y,
					_camera.Rotation.Z
				);
			}
			else if (Input.IsActionJustPressed("shoot"))
			{
				_gun?.Shoot();
				IsAiming = false;
			}
			else if (Input.IsActionJustPressed("load"))
			{
				_gun?.SetSpell(_spell);
			}
			else if (Input.IsActionJustPressed("aim"))
			{
				_gun?.Aim();
			}
			else if (Input.IsActionJustReleased("aim"))
			{
				if (IsAiming)
				{
					_anime.PlayBackwards("AimRight");
					IsAiming = false;
				}
			}
			// else if (Input.IsActionJustPressed("reload"))
			// {
			// 	//load normal bullets
			// }
			// else if (Input.IsActionJustPressed("load"))
			// {
			// 	//load active spell
			// }
		}
		if (Input.MouseMode == Input.MouseModeEnum.ConfinedHidden && (Input.IsActionJustPressed("shoot") || Input.IsActionJustPressed("aim")))
		{
			switch (_hud.CloseRadial())
			{
				case Spell spell:
					_spell = spell;
					break;
				case Gun gun:
					_gun?.Despawn(); //despawn equiped model if exists
					_gun = gun;
					_gun.SpawnModel(_rightSpawner, _leftSpawner);
					break;
				default:
					break;
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		//fall detection should be moved out of player and replaced with incredibly high tic damage
		if (_body.Position.Y < -100)
		{
			Rpc("Damage", 100);
		}
		float floatDelta = (float)delta;

		// Add gravity
		if (!IsOnFloor())
		{
			Velocity += GetGravity() * floatDelta;
		}
		else
		{
			if (Input.IsActionJustPressed("jump"))
			{
				Velocity = new Vector3(Velocity.X, JUMP_VELOCITY, Velocity.Z);
			}
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
		if (direction.Length() > 0 && Input.MouseMode != Input.MouseModeEnum.Visible)
		{
			Vector3 targetVelocity = new Vector3(
				direction.X * totalSpeed,
				Velocity.Y,
				direction.Z * totalSpeed
			);

			if (IsOnFloor())
			{
				if (HasFriction)
				{
					Velocity = targetVelocity;
				}
				Velocity = Velocity.MoveToward(targetVelocity, 1f * floatDelta);
			}
			else
			{
				Velocity = Velocity.MoveToward(targetVelocity, 15f * floatDelta);
			}
		}
		else
		{
			if (IsOnFloor() && HasFriction)
			{
				Velocity = new Vector3(
					Mathf.MoveToward(Velocity.X, 0, Math.Abs(Velocity.X) * 0.1f),
					Velocity.Y,
					Mathf.MoveToward(Velocity.Z, 0, Math.Abs(Velocity.Z) * 0.1f)
				);
			}
		}

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
		if (IsMultiplayerAuthority()) _hud.HealthBar.Value = _health;
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void PlayAnim(string animation, float pos)
	{
		//sets up muzzle flash position and node for shoot animation
		if (animation.Contains("Shoot"))
		{
			var temp = animation.Contains("Right") ? _rightFlash : _leftFlash;
			temp.Position = new Vector3(0, 0, pos);
			temp.Restart();
		}
		_anime.Play(animation);
		if (!animation.Contains("Aim")) _anime.Queue("RESET"); //prevents guns getting stuck
	}
}
