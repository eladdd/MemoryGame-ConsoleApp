using System;

namespace MemoryGameLogic
{
    public class Board
    {
        private readonly int r_BoardRows;
        private readonly int r_BoardCols;
        private Card[,] m_GameTable;

        public Board(int i_BoardRows, int i_BoardCols)
        {
            this.r_BoardRows = i_BoardRows;
            this.r_BoardCols = i_BoardCols;
            this.initGameTable();
        }

        public int Row
        {
            get
            {
                return this.r_BoardRows;
            }
        }

        public int Column
        {
            get
            {
                return this.r_BoardCols;
            }
        }

        private void initGameTable()
        {
            this.m_GameTable = new Card[r_BoardRows, r_BoardCols];
            initCards();
        }

        private void initCards()
        {
            // we create array of alpha beta pairs according to the size
            // to avoid insert twice the same card index from  random we swap the chosen card to the last position in the array
            // when we swap the positions we reduce the "cardsSize" which the random use to avoid choose the index again
            int cardsSize = r_BoardCols * r_BoardRows;
            Random rnd = new Random();
            char[] alphaBeta = new char[cardsSize];

            for (int i = 0; i < (cardsSize / 2); i++)
            {
                alphaBeta[i * 2] = (char)(i + 'A');
                alphaBeta[(i * 2) + 1] = (char)(i + 'A');
            }

            for (int i = 0; i < r_BoardRows; i++)
            {
                for (int j = 0; j < r_BoardCols; j++)
                {
                    int index = rnd.Next(cardsSize);
                    m_GameTable[i, j] = new Card(alphaBeta[index]);
                    swap(ref alphaBeta[index], ref alphaBeta[cardsSize - 1]);
                    cardsSize--;
                }
            }
        }

        private void swap<T>(ref T io_Left, ref T io_Right)
        {
            T temp = io_Left;
            io_Left = io_Right;
            io_Right = temp;
        }

        public bool IsCardPicked(int i_Row, int i_Col)
        { // check if the card in the row, col has been picked already before, if not then we expose the card
            bool cardPicked = true;

            if (m_GameTable[i_Row, i_Col].IsCardHidden)
            {
                m_GameTable[i_Row, i_Col].ChangeCardSide();
                cardPicked = false;
            }

            return cardPicked;
        }

        public Card GetCardFromBoard(int i_Row, int i_Col)
        {
            return this.m_GameTable[i_Row, i_Col];
        }

        public bool AreCardsFlipped()
        {
            bool cardFlipped = true;

            for (int i = 0; (i < this.r_BoardRows) && cardFlipped; i++)
            {
                for (int j = 0; (j < this.r_BoardCols) && cardFlipped; j++)
                {
                    if (this.m_GameTable[i, j].IsCardHidden)
                    {
                        cardFlipped = false;
                    }
                }
            }

            return cardFlipped;
        }
    }
}
