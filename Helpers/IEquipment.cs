using Godot;
using System;

public partial interface IEquipment
{
	Texture2D Icon { get; set; }

	string HudSlot { get; set; }
}
