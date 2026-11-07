namespace At_the_Bottom.Models
{
    public record GeneratedFloor(
        Level Level,
        Position PlayerSpawn,
        Position BossSpawn,
        Position StairsPosition,
        IReadOnlyList<Position> MonsterSpawns
    );
}
