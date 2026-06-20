using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Srolllock.spells
{
	public partial class Spell : Node3D
	{
		public double Cooldown;

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
