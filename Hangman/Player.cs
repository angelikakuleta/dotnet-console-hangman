using System;
using System.Collections.Generic;

namespace Hangman
{
    class Player
    {
        public string Name { get; private set; }

        private int lifePoints;
        public int LifePoints { 
            get => lifePoints; 
            set => lifePoints = (value >= 0 ? value : 0); 
        }
        public HashSet<char> GuessedLetters { get; set; } = new HashSet<char>();
        
        public bool Winner { get; private set; } = false;

        public Player(string name, int lifes)
        {
            this.Name = name;
            this.LifePoints = lifes;
        }

        public void CheckIfHasWon(string capital)
        {
            if (this.LifePoints > 0)
                this.Winner = true;
            foreach (char c in capital.ToLower())
            {
               if (Char.IsLetter(c) && this.GuessedLetters.Contains(c) == false)
                   this.Winner = false;
                   break;
            }
        }
    }
}