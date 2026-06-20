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

	public bool HasFriction = true;

	private Camera3D _camera;
	private CharacterBody3D _body;
	private Hud _hud;
	private MultiplayerSpawner _gunSpawner;
	//private AnimationPlayer _anime;
	private Gun _gunLocal;
	private Node _gunRemote;
	private Dictionary<Key, object> _equipment;
	private List<string> _effects = new List<string>();
	private double _health = MAX_HEALTH;

	public override void _EnterTree()
	{
		SetMultiplayerAuthority(Name.ToString().ToInt());
	}

	public override void _Ready()
	{
		if (!IsMultiplayerAuthority())
		{
			//disable processing & inputs for non auth player
			SetPhysicsProcess(false);
			SetProcessInput(false);
			return;
		}
		//start loading screen

		AddChild(GD.Load<PackedScene>($"res://player/Hud.tscn").Instantiate());
		_hud = GetNode<Hud>("Hud");
		_camera = GetNode<Camera3D>("Camera3D");
		_body = GetNode<CharacterBody3D>(".");
		_gunSpawner = GetNode<MultiplayerSpawner>("Camera3D/GunSpawner");
		_gunSpawner.SetMultiplayerAuthority(int.Parse(Name));

		//Hides own overhead health
		GetNode<ProgressBar>("SubViewport/ProgressBar").Visible = false;

		_equipment = new Dictionary<Key, object>()
		{
			{Key.Key1, new Pistols()},
			{Key.Key2, new Force()},
			{Key.Key3, new Wall()},
			{Key.Key4, new Tornado()},
			{Key.Key5, new Slick()},
			{Key.Key6, new Blunderbuss()},
			{Key.Key7, new Rifle()}
		};

		foreach (var pair in _equipment)
		{
			if (pair.Value is Gun gun)
			{
				_camera.AddChild(gun);
			}
		}

		Input.MouseMode = Input.MouseModeEnum.Captured;
		_camera.Current = true;

		//stop loading screen
	}

	//should phase out this whole method
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
						if (_gunLocal != null)
						{
							_gunLocal.equipedSpell = spell.GetType().Name;
						}
						break;
					case Gun gun:
						if (_gunRemote != null)
						{
							_gunRemote.QueueFree();

						}
						_gunRemote = _gunSpawner.Spawn(gun.GetType().Name);
						_gunLocal = gun;
						_gunLocal.SetModel(_gunRemote); //for animation interaction
						break;
				}
			}
		}
	}

	public override void _Input(InputEvent @event)
	{
		//prevents fps controls when in menu
		if (Input.MouseMode != Input.MouseModeEnum.Captured) return;

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
			//only shoot if exists
			_gunLocal?.Shoot();
		}
		// else if (Input.IsActionJustPressed("aim"))
		// {
		// 	//aim anim
		// }
		// else if (Input.IsActionJustPressed("reload"))
		// {
		// 	//load normal bullets
		// }
		// else if (Input.IsActionJustPressed("load"))
		// {
		// 	//load active spell
		// }
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
}
