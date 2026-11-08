using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace At_the_Bottom.Models
{
    public static class MonsterFactory
    {
        public static Monster CreateRandom(
            Position position,
            int floor)
        {
            int type = Random.Shared.Next(
                floor >= 3 ? 3 : 2
            );

            int healthBonus = (floor - 1) * 2;
            int damageBonus = (floor - 1) / 3;

            return type switch
            {
                0 => new Monster(
                    position, 'g', "Гоблин",
                    10 + healthBonus,
                    2 + damageBonus,
                    10, 5),

                1 => new Monster(
                    position, 's', "Скелет",
                    14 + healthBonus,
                    3 + damageBonus,
                    15, 8),

                2 => new Monster(
                    position, 'O', "Орк",
                    22 + healthBonus,
                    5 + damageBonus,
                    25, 12),

                _ => throw new InvalidOperationException()
            };
        }
    }
}
