using Godot;
using Srolllock.guns;

public partial class Pistols : Gun, IEquipment
{
    public Texture2D Icon {get; set;} = GD.Load<Texture2D>($"res://guns/pistols/pistols.png");
}
