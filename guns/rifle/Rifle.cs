using Godot;
using Srolllock.guns;

public partial class Rifle : Gun, IEquipment
{
	public Texture2D Icon {get; set;} = GD.Load<Texture2D>($"res://guns/rifle/rifle.png");
	public Rifle(string name = null)
	{
		var temp = string.IsNullOrEmpty(name) ? GetType().Name : name;
		_modelName = $"{GetType().Name}/{temp}";
	}
}
