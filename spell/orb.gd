extends Node3D

@onready var area : Area3D = $Area3D

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	var targets = area.get_overlapping_bodies()
	#change for loop to call spell specific function
	#might add this in c# since it's probably computationally heavy
	for target : CharacterBody3D in targets:
		if !target.is_on_floor():
			target.velocity -= target.get_gravity() * delta
