using System.IO;
using System.Text;

namespace BaldaEngine
{
    public class FileWorker
    {
        public string[] GetGameFieldLines(string path)
        {
            return File.ReadAllLines(path);
        }

        public string[] GetDictionary(string path)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            string[] dict = File.ReadAllLines(path, Encoding.GetEncoding("windows-1251"));

            for(int i = 0; i < dict.Length; i++) 
            { 
                dict[i] = dict[i].Trim().ToLower();
            }

            return dict;
        }

        public char[,] GetParsedGameField(string[] fileLines)
        {
            char[,] gameField = new char[5, 5];

            for(int i = 0; i < gameField.GetLength(0); i++)
            {
                fileLines[i] = fileLines[i].ToLower();
                for(int j = 0; j < gameField.GetLength(1); j++)
                {
                    gameField[i, j] = fileLines[i][j];
                }
            }

            return gameField;
        }
    }
}
