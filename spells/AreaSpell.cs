using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Srolllock.Spells
{
	public partial class AreaSpell : Node3D, IEquipment, ISpell
	{
		public Texture2D Icon { get; set; }

        public double Cooldown { get; set; }

		public float Modifier { get; set; } = 1;

		public string HudSlot { get; set; } = "Ammo";

		public AreaSpell()
		{
			Icon = GD.Load<Texture2D>($"res://Spells/{GetType().Name}/{GetType().Name}.png");
		}

		public override void _Process(double delta)
		{
			Cooldown -= delta;
		}

		[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
		public void Dispel()
		{
			if (IsMultiplayerAuthority()) QueueFree();
		}
	}
}
