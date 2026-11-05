using At_the_Bottom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace At_the_Bottom.Services
{
    public class BattleService
    {
        public int Attack(
            Entity attacker,
            Entity defender,
            bool isDefending = false)
        {
            int damage = Math.Max(
                1,
                attacker.Damage - defender.Defense
            );

            if (isDefending)
            {
                damage = Math.Max(1, (damage + 1) / 2);
            }

            defender.TakeDamage(damage);

            return damage;
        }
    }
}
