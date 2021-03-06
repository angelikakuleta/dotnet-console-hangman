using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using ConsoleTables;

namespace Hangman
{
    class Game
    {
        Player player;
        string capital;
        string country;
        char[] letters;
        HashSet<char> notInWord = new HashSet<char>();
        public bool IsFinished { get; private set; } = false;
        private DateTime startTime;
        public DateTime StartTime
        {
            get => startTime;
            private set 
            {
                //  only if there was no previously assigned value
                if (startTime == default(DateTime))
                    startTime = value; 
            }
        }
        private DateTime endTime;
        public DateTime EndTime 
        { 
            get => endTime; 
            private set
            {
                if (value > this.StartTime)
                    endTime = value;
            }
        }

        public Game(Player player)
        {
            this.player = player;
            (this.country, this.capital) = GetRandomPair();
            this.letters = new char[this.capital.Length];
            for (int i = 0; i < this.capital.Length; i++)
                this.letters[i] = this.capital[i] == ' ' ? ' ' : '_';
        }

        public void StartGame() {
            Console.WriteLine($"Hello {this.player.Name}! Let's start.");

            this.StartTime = DateTime.Now;

            do
            {    
                this.DisplayGameState();

                Console.WriteLine("Would you like to guess the whole word(s)? y/n");
                char input = Char.ToLower(Console.ReadKey().KeyChar);    
                DisplayGameState();

                if (input == 'y')
                {
                    string enteredWord = this.AskForWord();
                    if (!this.CheckWord(enteredWord))
                        this.player.LifePoints -= 2;
                }
                else
                {
                    char enteredLetter = this.AskForLetter();           
                    if (!this.CheckLetter(enteredLetter))
                        this.player.LifePoints--;
                }       
                
            } while (!this.IsFinished && this.player.LifePoints > 0);
            this.EndTime = DateTime.Now;

            this.player.CheckIfHasWon(capital);

            this.DisplayGameState();
            this.DisplayResult();
        }

        (string, string) GetRandomPair()
        {
            string pathToWords = AppDomain.CurrentDomain.BaseDirectory + @"/data/countries_and_capitalss.txt";
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

        void DisplayMaskedWord()
        {
            foreach (char c in this.letters)
                Console.Write(c);
            Console.WriteLine("\n");
        }

        bool CheckLetter(char enteredLetter)
        {
            bool guessed = false;

            for (int i = 0; i < this.capital.Length; i++)
            {
                if (enteredLetter == Char.ToLower(this.capital[i]))
                {
                    this.letters[i] = this.capital[i];
                    guessed = true;
                }
            }

            if (guessed)
            {
                if (this.capital == new string(this.letters))
                    this.IsFinished = true;
            }
            else
            {
                if (!this.notInWord.Contains(enteredLetter))
                    this.notInWord.Add(enteredLetter);
            }

            return guessed;
        }

        bool CheckWord(string enteredWord)
        {
            bool guessed = false;

            if (enteredWord == this.capital.ToLower())
            {
                for (int i = 0; i < this.capital.Length; i++)
                {
                    this.letters[i] = this.capital[i];
                    guessed = true;
                }
            }
            if (guessed)
            {
                if (this.capital == new string(this.letters))
                    this.IsFinished = true;
            }
            return guessed;
        }

        void DisplayNotInWord()
        {
            if (this.notInWord.Count > 0) {
                Console.Write("Not in word(s): ");
                foreach (char c in this.notInWord)
                    Console.Write($"{c} ");
                Console.WriteLine("\n");
            }        
        }

        string GetGuessedLetters()
        {
            return String.Join("", this.letters);
        }

        void DisplayGameState()
        {
            Console.Clear();
            Console.WriteLine($"Life points: {this.player.LifePoints}");
            Console.WriteLine();           
            this.DisplayMaskedWord();     
            this.DisplayNotInWord();

            if (this.player.LifePoints == 1)
                Console.WriteLine($"Hint: The capital of {this.country}\n"); 
                
            Console.WriteLine();
        }

        string AskForWord()
        {
            string input;
            do
            {
                Console.WriteLine("Enter the capital");
                input = Console.ReadLine();
            } while (input.Length < 2);          

            return input.ToLower();
        }

        char AskForLetter()
        {
            char input;
            do
            {
                Console.WriteLine("Enter a letter");
                input = Console.ReadKey().KeyChar;
                Console.WriteLine();
            } while (!Char.IsLetter(input));          

            var letter = Char.ToLower(input);
            if (!this.player.GuessedLetters.Contains(letter))
                this.player.GuessedLetters.Add(letter);
            
            return letter;
        }

        List<string[]> SaveScore()
        {
            Console.WriteLine("Saving your score...");
            int guessingTime = (int)(this.EndTime - this.StartTime).TotalSeconds;
            string[] playerScore = {
                this.player.Name, 
                this.EndTime.ToString("dd.MM.yyyy H:mm"), 
                guessingTime.ToString(), 
                this.player.GuessedLetters.Count.ToString(), 
                this.GetGuessedLetters()
            };

            string pathToScores = AppDomain.CurrentDomain.BaseDirectory + @"/data/scores.txt";
            var scoresList = new List<string[]>();

            if (File.Exists(pathToScores)) 
            {
                var scores = File.ReadAllLines(pathToScores);
                scoresList = scores.Select(x => x.Split(" | ")).ToList();
            }
            
            scoresList.Add(playerScore);
            if (scoresList.Count > 1)
                scoresList.Sort((x, y) => int.Parse(x[2]).CompareTo(int.Parse(y[2])));
            if (scoresList.Count > 10)
                scoresList.RemoveRange(10, scoresList.Count-10);
            
            StringBuilder fileContet = new StringBuilder();
            for (int i = 0; i < scoresList.Count; i++)
            {
                fileContet.Append(String.Join(" | ", scoresList[i]));
                if (i != scoresList.Count - 1)
                    fileContet.Append(Environment.NewLine);
            }
        
            File.WriteAllText(pathToScores, fileContet.ToString());
            return scoresList;
        }

        static void DrawLeaderboard(List<string[]> scores)
        {
            var table = new ConsoleTable("name", "date", "guessing time", "guessing_tries", "guessed_word");
            foreach (var x in scores)
            {
                table.AddRow(x[0], x[1], x[2], x[3], x[4]);
            }
            table.Write(Format.Minimal);
        }

        void DisplayResult()
        {
            if (this.player.Winner) {
                Console.WriteLine("Congrats!");
                int guessingTime = (int)(this.EndTime - this.StartTime).TotalSeconds;
                Console.WriteLine($"You guessed the capital after {this.player.GuessedLetters.Count} letters. It took you {guessingTime} seconds.");
                Console.WriteLine();
                var scoreList = SaveScore();
                Console.WriteLine();
                DrawLeaderboard(scoreList);
            }
            else {
                Console.WriteLine("You failed...");
            }
        }
    }
}