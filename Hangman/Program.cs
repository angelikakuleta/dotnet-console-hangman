using System;

namespace Hangman
{
    class Program
    {
        const int LifePoints = 5;

        static void Main(string[] args)
        {
            string playerName = AskForPlayerName();
            Player player = new Player(playerName, LifePoints);
            Console.WriteLine("Satrting the game...");
            new Game(player).StartGame();
        }

        static string AskForPlayerName()
        {
            Console.WriteLine("Enter your name");
            string input = Console.ReadLine();

            if (input.Length < 3)          
                AskForPlayerName();

            return input;
        }
    }
}
