extends Node3D

var lobby_id : int = 0
var peer : SteamMultiplayerPeer
#@export var player_scene : PackedScene
var is_host : bool = false
var is_joining : bool = false
var is_ip : bool = false

@onready var menu : Control = $Menu
@onready var btn_host : Button = $Menu/Host
@onready var btn_join : Button = $Menu/Join
@onready var txt_input : LineEdit = $Menu/Prompt
@onready var btn_ip : Button = $Menu/Lan

@onready var pause : Control = $Pause
@onready var btn_exit : Button = $Pause/Exit
@onready var btn_copy : Button = $Pause/Copy
@onready var display_id = $Pause/ID

@onready var multiplayer_spawner : MultiplayerSpawner = $MultiplayerSpawner

const PORT = 9999
var enet_peer = ENetMultiplayerPeer.new()

func _ready():
	multiplayer_spawner.add_spawnable_scene("res://player/Player.tscn")
	if Steam.isSteamRunning():
		Steam.steamInitEx(480, true)
		Steam.initRelayNetworkAccess()
		Steam.lobby_created.connect(_on_lobby_created)
		Steam.lobby_joined.connect(_on_lobby_joined)
	else:
		_on_lan_pressed()

func _unhandled_input(event : InputEvent) -> void:
	if event.is_action_pressed("escape") and not menu.is_visible_in_tree():
		Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
		pause.show()
	if event is InputEventMouseButton and not menu.is_visible_in_tree():
		Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
		pause.hide()

func host_lobby():
	Steam.createLobby(Steam.LobbyType.LOBBY_TYPE_FRIENDS_ONLY, 16)
	is_host = true

func _on_lobby_created(result : int, lobby_id : int):
	if result == Steam.Result.RESULT_OK:
		self.lobby_id = lobby_id
		display_id.text = str(lobby_id)
		
		peer = SteamMultiplayerPeer.new()
		peer.server_relay = true
		peer.create_host()
		multiplayer.multiplayer_peer = peer
		multiplayer.peer_connected.connect(_add_player)
		multiplayer.peer_disconnected.connect(_remove_player)
		_add_player()
		
		print("Lobby created, join code: ", lobby_id)
		menu.hide()

func join_lobby(lobby_id : int):
	if not Steam.isLobby(lobby_id):
		return
	is_joining = true
	Steam.joinLobby(lobby_id)
	
func _on_lobby_joined(lobby_id : int, permissions : int, locked : bool, response : int):
	if !is_joining:
		return
	
	self.lobby_id = lobby_id
	display_id.text = str(lobby_id)
	peer = SteamMultiplayerPeer.new()
	peer.server_relay = true
	peer.create_client(Steam.getLobbyOwner(lobby_id))
	multiplayer.multiplayer_peer = peer

	is_joining = false
	menu.hide()

func _add_player(id : int = 1):
	var player = preload("res://player/Player.tscn").instantiate()
	player.name = str(id)
	call_deferred("add_child", player)

func _remove_player(id : int):
	if !self.has_node(str(id)):
		return
	
	self.get_node(str(id)).queue_free()


func _on_host_pressed() -> void:
	if is_ip:
		host_enet()
	else:
		host_lobby()


func _on_prompt_text_changed(new_text: String) -> void:
	btn_join.disabled = (new_text.length() == 0)


func _on_join_pressed() -> void:
	if is_ip:
		join_enet()
	else:
		join_lobby(txt_input.text.to_int())


func _on_exit_pressed() -> void:
	peer.close()
	Steam.steamShutdown()
	get_tree().reload_current_scene()


func _on_copy_pressed() -> void:
	DisplayServer.clipboard_set(str(lobby_id))

func _on_lan_pressed() -> void:
	is_ip = !is_ip
	if is_ip:
		btn_ip.text = "Localhost"
		txt_input.hide()
		btn_join.disabled = false
	else:
		btn_ip.text = "SteamInit"
		txt_input.show()
		btn_join.disabled = true
	if !Steam.isSteamRunning() and !is_ip:
		_on_lan_pressed()

func host_enet() -> void:
	menu.hide()
	enet_peer.create_server(PORT)
	multiplayer.multiplayer_peer = enet_peer
	multiplayer.peer_connected.connect(_add_player)
	multiplayer.peer_disconnected.connect(_remove_player)

	_add_player(multiplayer.get_unique_id())

func join_enet():
	menu.hide()
	enet_peer.create_client("localhost", PORT)
	multiplayer.multiplayer_peer = enet_peer
