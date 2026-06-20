using Godot;
using Srolllock.guns;

public partial class Blunderbuss : Gun
{
	public override Texture2D Icon {get; set;} = GD.Load<Texture2D>($"res://equipment/guns/blunderbuss/blunderbuss.png");
}
