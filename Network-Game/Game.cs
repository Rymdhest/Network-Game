using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network_Game
{
    internal class Game
    {
        private int[,] boardState;
        private Random rand = new Random();
        public Game ()
        {
            boardState = new int[3,3];
        }

        public Boolean ResolveInput(int input)
        {
            int x = input % 3;
            int y = input / 3;

            boardState[x, y] = 1;

            while (true) {
                int randomX = rand.Next(3);
                int randomY = rand.Next(3);

                if (boardState[randomX, randomY] == 0)
                {
                    boardState[randomX, randomY] = 2;
                    break;
                }
            }

            return true;
        }
        public string getBoardAsString()
        {
            string boardString = "";

            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    boardString += boardState[x, y];
                }
            }
            return boardString;
        }
        public void restart()
        {
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                   boardState[x, y] = 0;
                }
            }
        }
    }
}
