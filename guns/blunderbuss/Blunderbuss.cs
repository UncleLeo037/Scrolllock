using Godot;
using Srolllock.guns;

public partial class Blunderbuss : Gun, IEquipment
{
	//will hold details that need to be shared amongst equipment types
	public Texture2D Icon {get; set;} = GD.Load<Texture2D>($"res://guns/blunderbuss/blunderbuss.png");
}
