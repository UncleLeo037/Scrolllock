using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Srolllock.spells
{
	public partial class Spell : Node3D, IEquipment
	{
		public Texture2D Icon { get; set; }

		public double Cooldown;

		public float Modifier = 1;

		public Spell()
		{
			Icon = GD.Load<Texture2D>($"res://spells/{GetType().Name}/{GetType().Name}.png");
		}

		public override void _Process(double delta)
		{
			Cooldown -= delta;
		}

		[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
		public void Dispel()
		{
			this.QueueFree();
		}
	}
}
