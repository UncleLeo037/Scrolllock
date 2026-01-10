extends CharacterBody3D;

const SPEED : float = 5.0;
const JUMP_VELOCITY : float = 5.0;
const SENSITIVITY : float = 0.08;

@onready var camera : Camera3D = $Camera3D;
@onready var body : CharacterBody3D = $"."

func _enter_tree() -> void:
	set_multiplayer_authority(name.to_int())

func _ready() -> void:
	if not is_multiplayer_authority(): return;
	
	#mouse mode is being used like bool for if player is controlable or in menu
	Input.mouse_mode = Input.MOUSE_MODE_CAPTURED;
	camera.current = true;
	
func _unhandled_input(event: InputEvent) -> void:
	if not is_multiplayer_authority(): return;
	
	if event is InputEventMouseMotion and Input.get_mouse_mode() == Input.MOUSE_MODE_CAPTURED:
		body.rotate_y(deg_to_rad(-event.relative.x * SENSITIVITY));
		camera.rotate_x(deg_to_rad(-event.relative.y * SENSITIVITY));
		camera.rotation.x = clamp(camera.rotation.x, deg_to_rad(-90), deg_to_rad(90));

func _physics_process(delta: float) -> void:
	if not is_multiplayer_authority(): return;
	
	# Add the gravity.
	if not is_on_floor():
		velocity += get_gravity() * delta;

	# Handle jump.
	if Input.is_action_just_pressed("jump") and is_on_floor():
		velocity.y = JUMP_VELOCITY;
	
	if Input.is_action_pressed("crouch"):
		camera.position.y = 0.3;
	
	if Input.is_action_just_released("crouch"):
		camera.position.y = 0.6;
	
	if Input.is_action_pressed("menu"):
		camera.position.y = 0.3;
	
	# Get the input direction and handle the movement/deceleration.
	# As good practice, you should replace UI actions with custom gameplay actions.
	var input_dir := Input.get_vector("left", "right", "forward", "backward");
	var direction := (body.transform.basis * Vector3(input_dir.x, 0, input_dir.y)).normalized();
	var totalSpeed : float = SPEED;
	if input_dir.y < 0:
		totalSpeed += float(Input.is_action_pressed("sprint")) * 3.0;
	if direction and Input.get_mouse_mode() == Input.MOUSE_MODE_CAPTURED:
		velocity.x = direction.x * totalSpeed;
		velocity.z = direction.z * totalSpeed;
	else:
		velocity.x = move_toward(velocity.x, 0, totalSpeed);
		velocity.z = move_toward(velocity.z, 0, totalSpeed);
	
	#quick save fall script
	if body.position.y < -50:
		body.position = Vector3(0, 20, 0)
	
	move_and_slide();
