using System;
using System.Linq;

namespace BaldaEngine
{
    internal class Program
    {
        internal static void Main()
        {
            while (true)
            {
                printRecommendations();
            }
        }

        private static void printRecommendations()
        {
            Console.WriteLine("Calculating...");

            FileWorker parser = new FileWorker();
            string[] plainTextGameField = parser.GetGameFieldLines(nameof(plainTextGameField));
            string[] dictionary = parser.GetDictionary(nameof(dictionary));

            char[,] parsedGameField = parser.GetParsedGameField(plainTextGameField);

            var gameField = new GameField(parsedGameField);
            var gameFieldWorker = new GameFieldWorker(gameField, dictionary);

            //string word = gameFieldWorker.GetWordFromLocation(new Location(1, 2));
            string[] word = gameFieldWorker.GetStartRecommendations();

            Console.Clear();

            word.ToList().ForEach(x => Console.WriteLine(x));
            Console.WriteLine("-----------------------");
            Console.ReadKey();
        }
    }
}
