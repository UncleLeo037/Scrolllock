using Godot;
using Srolllock.guns;

public partial class Pistols : Gun
{
    public override Texture2D Icon {get; set;} = GD.Load<Texture2D>($"res://equipment/guns/pistols/pistols.png");
}
