using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Srolllock.spell
{
    public static class SpellFactory
    {
        public static Spell CreateSpell(string spellType)
        {
            return spellType switch
            {
                "force" => new ForceSpell(),
                "gravity" => new GravitySpell(),
                _ => throw new ArgumentException("Invalid spell type"),
            };
        }
        public static readonly Dictionary<string, Spell> SpellLookup = new Dictionary<string, Spell>
        {
            { "force", new ForceSpell() },
            { "gravity", new GravitySpell() }
        };
    }

    public abstract class Spell
    {
        public double lifetime { get; set; }
        public Vector3 size { get; set; }
        public string shape { get; set; }

        public Spell(double lifetime, Vector3 size, string shape)
        {
            this.lifetime = lifetime;
            this.size = size;
            this.shape = shape;
        }

        //make overridable method for spell effect
        public abstract void OnEnter(Node3D body, Node3D spell);

        public abstract void OnExit(Node3D body, Node3D spell);
    }

    class ForceSpell : Spell
    {
        public ForceSpell() : base(0.1f, new Vector3(6.0f, 6.0f, 6.0f), "Sphere")
        {
        }

        public override void OnEnter(Node3D body, Node3D spell)
        {
            if (body is Player player)
            {
                Vector3 origin = spell.GlobalTransform.Origin;
                player.Velocity += (player.GlobalTransform.Origin - origin).Normalized() * 7.0f;
            }
        }

        public override void OnExit(Node3D body, Node3D spell)
        {
            //do nothing for instant effect
        }
    }

    class GravitySpell : Spell
    {
        public GravitySpell() : base(10.0f, new Vector3(6.0f, 6.0f, 6.0f), "Sphere")
        {
        }
        public override void OnEnter(Node3D body, Node3D spell)
        {
            if (body is Player player)
            {
                player.AddEffect("gravity");
            }
        }
        public override void OnExit(Node3D body, Node3D spell)
        {
            if (body is Player player)
            {
                player.RemoveEffect("gravity");
            }
        }
    }
}
