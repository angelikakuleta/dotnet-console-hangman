using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Hangman
{
    class Game
    {
        Player player;
        string capital;
        string country;
        char[] letters;
        HashSet<char> notInWord = new HashSet<char>();

        public Game(Player player)
        {
            this.player = player;
            (this.country, this.capital) = RandomPair();
            this.letters = new char[this.capital.Length];
            for (int i = 0; i < this.capital.Length; i++)
                this.letters[i] = this.capital[i] == ' ' ? ' ' : '_';
        }

        (string, string) RandomPair()
        {
            string pathToWords = AppDomain.CurrentDomain.BaseDirectory + @"/data/countries_and_capitals.txt";
            string[] countryCapitalPairs;
            try
            {
                countryCapitalPairs = File.ReadAllLines(pathToWords);
                Random random = new Random();
                string randomPair = countryCapitalPairs[random.Next(countryCapitalPairs.Length)];
                string[] splitedRandomPair = randomPair.Split(" | ", 2).Select(x => x.Trim()).ToArray();
                return (splitedRandomPair[0], splitedRandomPair[1]);
            }
            catch (FileNotFoundException)
            {
                return ("Austria", "Vienna");
            }
        }

        public void StartGame() {
            Console.WriteLine($"Hello {player.Name}! Let's start.");
            Console.WriteLine($"{this.capital} is the capital of {this.country}.");
        }
    }
}