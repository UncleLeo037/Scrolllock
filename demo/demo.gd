extends Node3D

var lobby_id : int = 0
var peer : SteamMultiplayerPeer
@export var player_scene : PackedScene
@onready var menu : Control = $Menu
var is_host : bool = false
var is_joining : bool = false

@onready var btn_host : Button = $Menu/Host
@onready var btn_join : Button = $Menu/Join
@onready var txt_input : LineEdit = $Menu/Prompt

func _ready():
	if !Steam.steamInitEx():
		print("Steam initialised: ", Steam.steamInitEx(480, true))
	Steam.initRelayNetworkAccess()
	Steam.lobby_created.connect(_on_lobby_created)
	Steam.lobby_joined.connect(_on_lobby_joined)

func _unhandled_input(event : InputEvent) -> void:
	if event.is_action_pressed("escape") and not menu.is_visible_in_tree():
		Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
		$Exit.show()
	if event is InputEventMouseButton and not menu.is_visible_in_tree():
		Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
		$Exit.hide()

func host_lobby():
	Steam.createLobby(Steam.LobbyType.LOBBY_TYPE_PUBLIC, 16)
	is_host = true

func _on_lobby_created(result : int, lobby_id : int):
	if result == Steam.Result.RESULT_OK:
		self.lobby_id = lobby_id
		
		peer = SteamMultiplayerPeer.new()
		peer.server_relay = true
		peer.create_host()
		multiplayer.multiplayer_peer = peer
		multiplayer.peer_connected.connect(_add_player)
		multiplayer.peer_disconnected.connect(_remove_player)
		menu.hide()
		_add_player()
		
		print("Lobby created, join code: ", lobby_id)

func join_lobby(lobby_id : int):
	is_joining = true
	Steam.joinLobby(lobby_id)

func _on_lobby_joined(lobby_id : int, permissions : int, locked : bool, response : int):
	if !is_joining:
		return
	
	self.lobby_id = lobby_id
	peer = SteamMultiplayerPeer.new()
	peer.server_relay = true
	peer.create_client(Steam.getLobbyOwner(lobby_id))
	multiplayer.multiplayer_peer = peer
	
	is_joining = false

func _add_player(id : int = 1):
	var player = player_scene.instantiate()
	player.name = str(id)
	call_deferred("add_child", player)

func _remove_player(id : int):
	if !self.has_node(str(id)):
		return
	
	self.get_node(str(id)).queue_free()


func _on_host_pressed() -> void:
	host_lobby()


func _on_prompt_text_changed(new_text: String) -> void:
	btn_join.disabled = (new_text.length() == 0)


func _on_join_pressed() -> void:
	join_lobby(txt_input.text.to_int())


func _on_exit_pressed() -> void:
	$Exit.hide()
	menu.show()
	if is_host:
		peer.close()
	get_tree().reload_current_scene()
