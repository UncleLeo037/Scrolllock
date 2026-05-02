using Godot;
using Srolllock.spells;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

public partial class Player : CharacterBody3D
{
	private const float SPEED = 5.0f;
	private const float JUMP_VELOCITY = 4.5f;
	private const float SENSITIVITY = 0.08f;
	private const float FRICTION = 0.1f;

	public bool hasFriction = true;

	private Camera3D _camera;
	private CharacterBody3D _body;
	//private AnimationPlayer _anime;

	private Gun _gun;
	private Dictionary<Key, object> _equipment;

	private List<string> effects = new List<string>();

	public override void _EnterTree()
	{
		SetMultiplayerAuthority(Name.ToString().ToInt());
	}

	public override void _Ready()
	{
		GD.Print(this.Name);
		if (!IsMultiplayerAuthority()) return;

		_camera = GetNode<Camera3D>("Camera3D");
		_body = GetNode<CharacterBody3D>(".");
		//_anime = GetNode<AnimationPlayer>("AnimationPlayer");
		//add ref to player animation player to Gun so gun can trigger player animations when shooting
		//_flash = _camera.GetNode<Node3D>("Gun").GetNode<GpuParticles3D>("Flash");
		_gun = _camera.GetNode<Gun>("Gun");
		_gun.Name = this.Name;

		_equipment = new Dictionary<Key, object>()
		{
			{Key.Key1, new Force()},
			{Key.Key2, new Wall()},
			{Key.Key3, new Tornado()},
			{Key.Key4, new Slick()}
			// {"Key5", null},
			// {"Key6", null},
			// {"Key7", null},
			// {"Key8", null},
			// {"Key9", null},
			// {"Key0", null},
			// {"Equal", null},
			// {"Minus", null}
		};

		Input.MouseMode = Input.MouseModeEnum.Captured;
		_camera.Current = true;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (!IsMultiplayerAuthority()) return;

		if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
		{
			if (_equipment.TryGetValue(keyEvent.PhysicalKeycode, out object item))
			{
				switch (item)
				{
					case Spell spell:
						_gun.equipedSpell = spell.GetType().Name;
						break;
					case Gun gun:
						//need to change this to rpc method call so other players see changed gun
						_gun = gun;
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

		if (Input.IsActionJustPressed("shoot")/* && _anime.CurrentAnimation != "Shoot"*/)
		{
			//Rpc("PlayShoot");
			_gun.Shoot();
		}
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
			this.Respawn();
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
	public async void Damage()
	{
		//will later apply damage and then call respawn which will check if player is dead
		this.Respawn();
	}

	private async void Respawn()
	{
		//this location will later be set to a proper respawn point
		//add check to see if health is less than 0 in here
		_body.Position = new Vector3(0, 0, 0);
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
