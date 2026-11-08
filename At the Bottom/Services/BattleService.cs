using At_the_Bottom.Models;

namespace At_the_Bottom.Services
{
    public class BattleService
    {
        public int Attack(
            Entity attacker,
            Entity defender,
            bool isDefending = false,
            int damageBonus = 0,
            int damageMultiplier = 1)
        {
            int damage = Math.Max(
                1,
                attacker.Damage * damageMultiplier + damageBonus - defender.Defense
            );

            if (isDefending)
            {
                damage = Math.Max(1, (damage + 3) / 4);
            }

            defender.TakeDamage(damage);

            return damage;
        }
    }
}
