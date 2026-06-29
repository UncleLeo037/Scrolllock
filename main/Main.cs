using Godot;
using System;

public partial class Main : Node3D
{
	[Export] public bool IsEnet = false;
	private Control Menu;
	private Button BtnHost;
	private Button BtnJoin;

	private Control Pause;
	private Button BtnExit;

	const int PORT = 9999;
	public ENetMultiplayerPeer EnetPeer;

	public MultiplayerSpawner Spawner;
	public MultiplayerSpawner MapSpawner;

	public override void _Ready()
	{
		Spawner = GetNode<MultiplayerSpawner>("MultiplayerSpawner");
		Spawner.AddSpawnableScene("res://player/Player.tscn");
		MapSpawner = GetNode<MultiplayerSpawner>("WorldSpawner");
		MapSpawner.AddSpawnableScene("res://maps/hub/Hub.tscn");

		Menu = Spawner.GetNode<Control>("Menu");
		BtnHost = Menu.GetNode<Button>("Host");
		BtnJoin = Menu.GetNode<Button>("Join");
		Pause = Spawner.GetNode<Control>("Pause");
		BtnExit = Pause.GetNode<Button>("Exit");

		if (IsEnet)
		{
			BtnHost.Pressed += HostEnet;
			BtnJoin.Pressed += JoinEnet;
			BtnJoin.Disabled = false;
			BtnExit.Pressed += OnExit;
			EnetPeer = new ENetMultiplayerPeer();
		}
		else
		{
			Spawner.Call("manual_ready");
		}

		GetTree().Paused = false;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("escape") && !Menu.IsVisibleInTree())
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
			Pause.Show();
		}

		//should move this to resume button
		if (@event is InputEventMouseButton && !Menu.IsVisibleInTree() && Input.MouseMode != Input.MouseModeEnum.ConfinedHidden)
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
			Pause.Hide();
		}
	}


	public void HostEnet()
	{
		Menu.Hide();
		EnetPeer.CreateServer(PORT);
		Multiplayer.MultiplayerPeer = EnetPeer;
		Multiplayer.PeerConnected += AddPlayer;
		Multiplayer.PeerDisconnected += RemovePlayer;

		AddChild(GD.Load<PackedScene>("res://maps/hub/Hub.tscn").Instantiate());
		Spawner.Call("_add_player", Multiplayer.GetUniqueId());
	}
	public void JoinEnet()
	{
		Menu.Hide();
		EnetPeer.CreateClient("localhost", PORT);
		Multiplayer.MultiplayerPeer = EnetPeer;
	}

	public void AddPlayer(long id)
	{
		GetNode("MultiplayerSpawner").Call("_add_player", id);
	}

	public void RemovePlayer(long id)
	{
		GetNode("MultiplayerSpawner").Call("_remove_player", id);
	}

	public void OnExit()
	{
		GetTree().Paused = true;
		GetTree().ReloadCurrentScene();
		EnetPeer.Close();
	}
}
