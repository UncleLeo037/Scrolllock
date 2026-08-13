using Godot;
using Srolllock.guns;

public partial class Rifle : Gun, IEquipment
{
	public Texture2D Icon {get; set;}
	public Rifle(string name = null)
	{
		var temp = string.IsNullOrEmpty(name) ? GetType().Name : name;
		_modelPath = $"{GetType().Name}/{temp}";
		Icon = GD.Load<Texture2D>($"res://guns/{GetType().Name}/{temp}.png");
	}
}
