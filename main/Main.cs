using Godot;
using System;

public partial class Main : Node3D
{
	private Control Menu;
	private Button BtnHost;
	private Button BtnJoin;


	private Button BtnIP;

	public override void _Ready()
	{
		GetNode("MultiplayerSpawner").Call("manual_ready");
	}

	public override void _Process(double delta)
	{
	}
}
