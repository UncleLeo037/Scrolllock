using Godot;
using Srolllock.guns;

public partial class Blunderbuss : Gun, IEquipment
{
	public Texture2D Icon {get; set;} = GD.Load<Texture2D>($"res://guns/blunderbuss/blunderbuss.png");

	public Blunderbuss(string name = null)
	{
		_modelName = string.IsNullOrEmpty(name) ? GetType().Name : name;
	}
}
