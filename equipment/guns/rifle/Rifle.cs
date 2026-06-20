using Godot;
using Srolllock.guns;

public partial class Rifle : Gun
{
	public override Texture2D Icon {get; set;} = GD.Load<Texture2D>($"res://equipment/guns/rifle/rifle.png");
}
