using System;
using System.Collections.Generic;
using System.Linq;

namespace BaldaEngine
{
    public class GameFieldWorker
    {
        public enum PathFinderMoves : int
        {
            None = 0,
            Up = 1,
            Right = 2,
            Down = 3,
            Left = 4,
        }

        private readonly GameField _gameField;

        private readonly string[] _dictionary;
        private readonly int _depth; //max word length + 1

        //(e.g. 01|1|11|2 -- y0x1 is a position, 1 is PathFinderMoves as int (next move); the result word)

        public GameFieldWorker(GameField parsedGameField, string[] dictionary)
        {
            _depth = 7;
            _gameField = parsedGameField;
            _dictionary = dictionary;
        }

        public string GetWordFromLocation(Location startingLocation)
        {
            var movesCache = new Dictionary<string, string>();
            var initialMoves = getInitialMoves(startingLocation);

            movesCache = movesCache.Concat(initialMoves).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            int offset = 0;
            for (int currentDepth = 1; currentDepth < _depth; currentDepth++)
            {
                int movesLastCacheSize = movesCache.Count;
                for (int j = offset; j < movesLastCacheSize; j++)
                {
                    var tempMoves = getMoveByCache(movesCache, j);
                    movesCache = movesCache.Concat(tempMoves).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                }
                offset += movesLastCacheSize;
            }

            var words = new List<string>();

            foreach(var s in movesCache.Values)
            {
                string reversed = new string(s.Reverse().ToArray());

                if (checkWordAvailability(s))
                    words.Add(s);

                if(checkWordAvailability(reversed))
                    words.Add(reversed);
            }

            //https://stackoverflow.com/questions/7975935/is-there-a-linq-function-for-getting-the-longest-string-in-a-list-of-strings
            return words.Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur);
        }

        private IDictionary<string, string> getMoveByCache(IDictionary<string, string> cache, int cacheIndex)
        {
            var res = new Dictionary<string, string>();
            var currentSquare = cache.ElementAt(cacheIndex);
            //TODO: доставать уже пройденные координаты из кеша и не давать "наступать" на них снова
            string[] moves = currentSquare.Key.Split('|');

            var currentSquareLocation = Location.FromString(moves[moves.Length - 3]);
            var lastMove = (PathFinderMoves)int.Parse(moves[moves.Length - 2]);

            for (int j = 1; j <= 4; j++)
            {
                var currentMove = (PathFinderMoves)j;
                var calculatedLocation = createSpecifedLocation(currentSquareLocation, currentMove);

                string fromNewMove = checkMoveValidity(currentMove, currentSquareLocation, lastMove) 
                    ? _gameField[calculatedLocation] 
                    : null;

                if (fromNewMove == null)
                    continue;

                res.Add(currentSquare.Key + $"{calculatedLocation}|{j}|", currentSquare.Value + fromNewMove);
            }

            return res;
        }
        //TODO: уничтожить
        private IDictionary<string, string> getInitialMoves(Location startingLocation)
        {
            var res = new Dictionary<string, string>();
            string startingChar = _gameField[startingLocation].ToString();

            for (int j = 1; j <= 4; j++)
            {
                var currentMove = (PathFinderMoves)j;
                var calculatedLocation = createSpecifedLocation(startingLocation, currentMove);

                string fromNewMove = checkMoveValidity(currentMove, startingLocation, PathFinderMoves.None) 
                    ? startingChar + _gameField[calculatedLocation] 
                    : null;

                if (fromNewMove == null)
                    continue;

                res.Add($"{calculatedLocation}|{j}|", fromNewMove);
            }

            return res;
        }

        private bool checkWordAvailability(string word)
        {
            return _dictionary.Contains(word);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="location">current location</param>
        /// <param name="lastMove">move before reaching current location</param>
        /// <returns></returns>
        private bool checkMoveValidity(PathFinderMoves path, Location location, PathFinderMoves lastMove)
        {
            Location calculatedLocation = createSpecifedLocation(location, path);

            bool result = path switch
            {
                PathFinderMoves.Up =>    (location.Y - 1) >= 0 && lastMove != PathFinderMoves.Down,
                PathFinderMoves.Right => (location.X + 1) <= 4 && lastMove != PathFinderMoves.Left, 
                PathFinderMoves.Down =>  (location.Y + 1) <= 4 && lastMove != PathFinderMoves.Up,
                PathFinderMoves.Left =>  (location.X - 1) >= 0 && lastMove != PathFinderMoves.Right,
                _ => false,
            };

            return result && checkCharAvailability(calculatedLocation);
        }

        /// <summary>
        /// checks if there is a letter on specified location
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private bool checkCharAvailability(Location location)
        {
            string res = _gameField[location];
            return char.IsLetter(res[0]); //it is not possible to get null-reference here
        }

        private Location createSpecifedLocation(Location startingLocation, PathFinderMoves move)
        {
            var res = move switch
            {
                PathFinderMoves.Up => new Location(startingLocation.Y - 1, startingLocation.X),
                PathFinderMoves.Right => new Location(startingLocation.Y, startingLocation.X + 1),
                PathFinderMoves.Down => new Location(startingLocation.Y + 1, startingLocation.X),
                PathFinderMoves.Left => new Location(startingLocation.Y, startingLocation.X - 1),
                _ => throw new NotImplementedException(),
            };

            return res;
        }

        //locations of squares that has no characters at the moment
        private Location[] getPossibleMoves()
        {
            var possibleMoves = new List<Location>();

            for (int i = 0; i <= 4; i++)
            {
                for (int j = 0; j <= 4; j++)
                {
                    var currentLocation = new Location(i, j);

                    if (_gameField[currentLocation] != "0")
                        continue;

                    var moves = getInitialMoves(currentLocation);

                    if (moves.Count == 0)
                        continue;

                    possibleMoves.Add(currentLocation);
                }
            }

            return possibleMoves.ToArray();
        }

        public string[] GetStartRecommendations()
        {
            char[] cyrillicAlphabet = Extensions.GetCyrillicAlphabet();
            var startingSquares = getPossibleMoves();
            char[,] cachedGameField = _gameField.ParsedGameField;
            string lastBestWord = null;

            var iterationCache = new List<string>();

            foreach (var location in startingSquares)
            {
                foreach (char c in cyrillicAlphabet)
                {
                    _gameField[location] = c.ToString();
                    string res = GetWordFromLocation(location);

                    if (res?.Length < lastBestWord?.Length)
                        continue;

                    lastBestWord = res;
                    iterationCache.Add(res);
                }

                _gameField.ParsedGameField = cachedGameField;
            }

            return iterationCache.OrderBy(x => x.Length).Reverse().Take(10).ToArray();
        }
    }
}
