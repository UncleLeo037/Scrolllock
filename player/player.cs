using Godot;
using System;

public partial class player : CharacterBody3D
{
	private const float SPEED = 5.0f;
	private const float JUMP_VELOCITY = 4.5f;
	private const float SENSITIVITY = 0.08f;
	
	private Camera3D _camera;
	private CharacterBody3D _body;
	private AnimationPlayer _anime;
	private GpuParticles3D _flash;
	private Node3D _model;
	
	public override void _EnterTree()
	{
		SetMultiplayerAuthority(Name.ToString().ToInt());
	}
	
	public override void _Ready()
	{
		if (!IsMultiplayerAuthority()) return;
		
		_camera = GetNode<Camera3D>("Camera3D");
		_body = GetNode<CharacterBody3D>(".");
		_anime = GetNode<AnimationPlayer>("AnimationPlayer");
		_flash = _camera.GetNode<Node3D>("Pistol").GetNode<GpuParticles3D>("Flash");
		_model = GetNode<Node3D>("Model");
		
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
		
		if (Input.IsActionJustPressed("shoot") && _anime.CurrentAnimation != "Shoot")
		{
			Rpc(MethodName.PlayShoot);
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
		
		// Handle jump
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			Velocity = new Vector3(Velocity.X, JUMP_VELOCITY, Velocity.Z);
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
			Velocity = new Vector3(
				direction.X * totalSpeed,
				Velocity.Y,
				direction.Z * totalSpeed
			);
		}
		else
		{
			Velocity = new Vector3(
				Mathf.MoveToward(Velocity.X, 0, totalSpeed),
				Velocity.Y,
				Mathf.MoveToward(Velocity.Z, 0, totalSpeed)
			);
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
