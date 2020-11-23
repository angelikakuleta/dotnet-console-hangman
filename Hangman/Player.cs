using System.Collections.Generic;

namespace Hangman
{
    class Player
    {
        public string Name { get; private set; }
        int lifePoints;
        public int LifePoints { 
            get => lifePoints; 
            set => lifePoints = (value >= 0 ? value : 0); 
        }
        HashSet<char> guessedLetters = new HashSet<char>();
        public List<char> GuessedLetters { get; set; }

        bool winner;
        
        public bool Winner { get; private set; }

        public Player(string name, int lifes)
        {
            this.Name = name;
            this.LifePoints = lifes;
            this.Winner = false;
        }
    }
}