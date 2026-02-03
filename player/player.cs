using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;

public partial class Player : CharacterBody3D
{
	private const float SPEED = 5.0f;
	private const float JUMP_VELOCITY = 4.5f;
	private const float SENSITIVITY = 0.08f;

	private Camera3D _camera;
	private CharacterBody3D _body;
	private AnimationPlayer _anime;
	private GpuParticles3D _flash;
	private Node3D _model;
	private RayCast3D _bullet;

	private string equipedSpell = string.Empty;
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
		_anime = GetNode<AnimationPlayer>("AnimationPlayer");
		_flash = _camera.GetNode<Node3D>("Pistol").GetNode<GpuParticles3D>("Flash");
		_model = GetNode<Node3D>("Model");
		_bullet = _camera.GetNode<RayCast3D>("RayCast3D");

		_model.Hide();

		Input.MouseMode = Input.MouseModeEnum.Captured;
		_camera.Current = true;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (!IsMultiplayerAuthority()) return;

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

		if (Input.IsActionJustPressed("one"))
		{
			equipedSpell = "Force";
		}
		if (Input.IsActionJustPressed("two"))
		{
			equipedSpell = "Wall";
		}
		if (Input.IsActionJustPressed("three"))
		{
			equipedSpell = "Tornado";
		}

		if (Input.IsActionJustPressed("shoot") && _anime.CurrentAnimation != "Shoot")
		{
			Rpc("PlayShoot");
			if (_bullet.IsColliding())
			{
				if (equipedSpell == string.Empty) return;

				Vector3 point = _bullet.GetCollisionPoint();
				SpellSpawner.CastSpell(this.Name, equipedSpell, point, new Vector3(_camera.Rotation.X, this.Rotation.Y, 0));

				//equipedSpell = string.Empty;
			}
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

		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			Velocity = new Vector3(Velocity.X, Velocity.Y + JUMP_VELOCITY, Velocity.Z);
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
				Velocity = targetVelocity;
			}
			else
			{
				Velocity = Velocity.MoveToward(targetVelocity, 15f * (float)delta);
			}
		}
		else
		{
			if (IsOnFloor())
			{
				Velocity = new Vector3(
					Mathf.MoveToward(Velocity.X, 0, Math.Abs(Velocity.X) * 0.1f),
					Velocity.Y,
					Mathf.MoveToward(Velocity.Z, 0, Math.Abs(Velocity.Z) * 0.1f)
				);
			}
		}

		// Quick fall recovery
		if (_body.Position.Y < -50)
		{
			_body.Position = new Vector3(0, 20, 0);
		}

		if (_anime.CurrentAnimation == "Shoot")
		{
			//do nothing
		}
		else if (direction.Length() > 0 && IsOnFloor())
		{
			_anime.Play("move");
		}
		else
		{
			_anime.Play("idle");
		}

		MoveAndSlide();
	}

	[Rpc(CallLocal = true)]
	public void PlayShoot()
	{
		_anime.Stop();
		_anime.Play("Shoot");
		_flash.Restart();
		_flash.Emitting = true;
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
