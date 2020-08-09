namespace BaldaEngine
{
    public class Location
    {
        public Location(int y, int x)
        {
            Y = y;
            X = x;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public override string ToString()
        {
            return $"{Y}{X}";
        }

        public static Location FromString(string cache)
        {
            //TODO: сделать че-нибудь с этим уебищем
            return new Location(int.Parse(cache[0].ToString()), int.Parse(cache[1].ToString()));
        }
    }

    public class GameField
    {
        public char[,] ParsedGameField;
        public GameField(char[,] parsedGameField)
        {
            ParsedGameField = parsedGameField;
        }

        public string this[Location key]
        {
            get => ParsedGameField[key.Y, key.X].ToString();
            set => ParsedGameField[key.Y, key.X] = char.Parse(value);
        }
    }
}
