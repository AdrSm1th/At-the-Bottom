namespace At_the_Bottom.Models
{
    public class LevelFactory
    {
        private readonly record struct Room(
            int X,
            int Y,
            int Width,
            int Height)
        {
            public Position Center =>
                new(X + Width / 2, Y + Height / 2);

            public bool Intersects(Room other)
            {
                return X <= other.X + other.Width &&
                       X + Width >= other.X &&
                       Y <= other.Y + other.Height &&
                       Y + Height >= other.Y;
            }
        }

        public static Level CreateTestLevel()
        {
            int width = 10;
            int height = 10;
            Level level = new(width, height);

            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    var tile = level.GetTile(x, y);
                    if (tile != null)
                    {
                        level.Tiles[x, y].Type = TileType.Floor;
                    }
                }
            }

            level.Tiles[width - 2, height - 2].Type = TileType.Stairs;
            level.Tiles[width - 4, height - 4].Type = TileType.Wall;

            return level;
        }


        public static GeneratedFloor GenerateRandomFloor(
            int width = 24,
            int height = 15,
            int monsterCount = 3,
            int? seed = null)
        {
            if (width < 20 || height < 14 || monsterCount < 0)
                throw new ArgumentException("Invalid level dimensions");

            Random random = seed.HasValue
                ? new Random(seed.Value)
                : Random.Shared;

            Level level = new(width, height);
            List<Room> rooms = [];

            const int maxRooms = 7;
            const int maxAttempts = 160;

            for (int attempt = 0;
                 attempt < maxAttempts && rooms.Count < maxRooms;
                 attempt++)
            {
                int roomWidth = random.Next(4, 8);
                int roomHeight = random.Next(4, 7);

                int x = random.Next(1, width - roomWidth);
                int y = random.Next(1, height - roomHeight);

                Room room = new(x, y, roomWidth, roomHeight);

                if (rooms.Any(other => room.Intersects(other)))
                    continue;

                // Пол
                for (int i = room.X; i < room.X + room.Width; i++)
                {
                    for (int j = room.Y; j < room.Y + room.Height; j++)
                    {
                        level.Tiles[i, j].Type = TileType.Floor;
                    }
                }

                // Соединяем новую комнату с предыдущей
                if (rooms.Count > 0)
                {
                    ConnectRooms(
                        level,
                        rooms[^1].Center,
                        room.Center,
                        random.Next(2) == 0
                    );
                }

                rooms.Add(room);
            }

            if (rooms.Count < 2)
                throw new InvalidOperationException(
                    "Not enough rooms generated");

            // Начальная позиция игрока
            Position playerSpawn = rooms[0].Center;

            // Для босса выбираем самую удалённую комнату
            Room bossRoom = rooms
                .OrderByDescending(room =>
                    Math.Abs(room.Center.X - playerSpawn.X) +
                    Math.Abs(room.Center.Y - playerSpawn.Y))
                .First();

            Position bossSpawn = bossRoom.Center;

            // Лестница находится в комнате босса
            Position stairs = new(bossRoom.X + 1, bossRoom.Y + 1);
            level.Tiles[stairs.X, stairs.Y].Type = TileType.Stairs;

            // Собираем возможные позиции обычных монстров
            List<Position> candidates = [];

            foreach (Room room in rooms.Skip(1))
            {
                if (room.X == bossRoom.X && room.Y == bossRoom.Y)
                {
                    continue;
                }

                for (int x = room.X; x < room.X + room.Width; x++)
                {
                    for (int y = room.Y; y < room.Y + room.Height; y++)
                    {
                        Position pos = new(x, y);

                        candidates.Add(pos);
                    }
                }
            }

            List<Position> monsterSpawns = [];

            for (int i = 0; i < monsterCount && candidates.Count > 0; i++)
            {
                int index = random.Next(candidates.Count);

                monsterSpawns.Add(candidates[index]);
                candidates.RemoveAt(index);
            }

            return new GeneratedFloor(
                level,
                playerSpawn,
                bossSpawn,
                stairs,
                monsterSpawns
            );
        }


        private static void ConnectRooms(
            Level level,
            Position from,
            Position to,
            bool horizontalFirst)
        {
            int x = from.X;
            int y = from.Y;

            void MoveHorizontal()
            {
                while (x != to.X)
                {
                    level.Tiles[x, y].Type = TileType.Floor;
                    x += Math.Sign(to.X - x);
                }
            }

            void MoveVertical()
            {
                while (y != to.Y)
                {
                    level.Tiles[x, y].Type = TileType.Floor;
                    y += Math.Sign(to.Y - y);
                }
            }

            if (horizontalFirst)
            {
                MoveHorizontal();
                MoveVertical();
            }
            else
            {
                MoveVertical();
                MoveHorizontal();
            }

            level.Tiles[x, y].Type = TileType.Floor;
        }

    }
}
