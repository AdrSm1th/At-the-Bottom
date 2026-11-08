using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace At_the_Bottom.Models
{
    public class Boss(Position pos, int floor)
        : Monster(
            pos,
            'B',
            "Страж глубин",
            24 + (floor - 1) * 5, // Максимальное здоровье
            4 + (floor - 1) / 3, // Урон
            50 + (floor - 1) * 10, // Опыт
            30 + (floor - 1) * 5) // Золото
    {
    }
}
