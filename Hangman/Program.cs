using System;

namespace Hangman
{
    class Program
    {
        static Player player;
        const int LifePoints = 5;

        static void Main(string[] args)
        {
            StartGame();
        }

        static void StartGame()
        {
            Console.WriteLine("Satrting game...");
            string playerName = AskForPlayerName();
            player = new Player(playerName, LifePoints);
            Console.WriteLine($"Hello {player.Name}! Let's start");
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
