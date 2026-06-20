using Godot;
using Srolllock.guns;
using System;
using System.Collections.Generic;

public partial class Radial : Control
{
	[Export] public Color BackColor;
	public List<object> Options;
	public int Select = 0;
	private int _outerRadius = 256;
	private int _innerRadius = 128;
	private Vector2 offset = Vector2.One * 64 / -2;

    public override void _Process(double delta)
    {
        float angle = (GetLocalMousePosition().Angle() - float.Tau / 2 / Options.Count + float.Tau / 4) * -1;
		if (angle < 0.0f)
		{
			angle += float.Tau;
		}
		float rad = angle % float.Tau;
		Select = (int)Math.Ceiling(rad / float.Tau * Options.Count);
		GD.Print(Select);
    }

	public override void _Draw()
	{
		DrawArc(Vector2.Zero, _innerRadius, 0, float.Tau, 128, BackColor, 4, true);
		int i = 0;
		foreach (var item in Options)
		{
			i++;
			var angle = (float.Tau * (i - 1) / Options.Count) - float.Tau / 4;
			var midle = (_innerRadius + _outerRadius) / 2;
			//GD.Print(Vector2.FromAngle(angle));
			var pos = midle * Vector2.FromAngle(angle) + offset;

			string name = item.GetType().Name;
			string type = item.GetType().BaseType.Name;
			DrawTexture(
				GD.Load<Texture2D>($"res://{type}s/{name}/{name}.tres"),
				pos
			);
		}
	}
}
