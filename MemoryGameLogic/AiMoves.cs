using System;
using System.Collections.Generic;


namespace MemoryGameLogic
{
    public class AiMoves
    {
        // this class will do AI moves, pc will pick first card RANDOM
        // on the second pick pc will search if there is equal card on his list (m_ExposedCards)
        // if there is, he will pick it, if not so again random pick
        // m_AvailableCardsIndexes - all the indexes of available cards to pick (used in random move)
        // m_ExposedCards - all the indexes of cards that were exposed by players  (like history memory)
        private readonly List<string> r_AvailableCardsIndexes;
        private readonly List<string> r_ExposedCards;
        private readonly Random r_Rnd;

        public AiMoves(int i_Rows, int i_Cols)
        {
            this.r_AvailableCardsIndexes = new List<string>();
            this.r_ExposedCards = new List<string>();
            this.initIndexList(i_Rows, i_Cols);
            r_Rnd = new Random();
        }

        private void initIndexList(int i_Rows, int i_Cols)
        {
            for (int i = 1; i <= i_Rows; i++)
            {
                for (int j = 0; j < i_Cols; j++)
                {
                    char col = (char)(j + 'A');
                    this.r_AvailableCardsIndexes.Add(col.ToString() + i);
                }
            }
        }

        public void AddExposedCard(string i_Index)
        {
            // to avoid duplicate
            if (r_ExposedCards.Contains(i_Index))
            {
                // delete and add again on last index (adjust to RemoveLastCardsFromLists /aiMove that use the last index as card picked)
                r_ExposedCards.RemoveAt(r_ExposedCards.IndexOf(i_Index));
                r_ExposedCards.Add(i_Index);

                return;
            }

            r_ExposedCards.Add(i_Index);
        }

        public void RemoveLastCardsFromLists()
        { // if player/pc picked cards and they are equal than we cant choose them again/use them for ai pick
            if (r_ExposedCards.Count >= 2)
            { // remove the last 2 cards picked in r_exposedCards, search them to remove from r_AvailableCardsIndexes
                r_AvailableCardsIndexes.RemoveAt(r_AvailableCardsIndexes.IndexOf(r_ExposedCards[r_ExposedCards.Count - 1]));
                r_ExposedCards.RemoveAt(r_ExposedCards.Count - 1);
                r_AvailableCardsIndexes.RemoveAt(r_AvailableCardsIndexes.IndexOf(r_ExposedCards[r_ExposedCards.Count - 1]));
                r_ExposedCards.RemoveAt(r_ExposedCards.Count - 1);
            }
        }

        public string AiMove(Card i_FirstPick, Board i_GameBoard)
        {
            string aiPick = string.Empty;

            if (i_FirstPick == null)
            {
                // random pick
                aiPick = r_AvailableCardsIndexes[randomIndex()];
            }
            else
            {
                // ai move
                bool matchCards = false; // index of match card
                string temp = r_ExposedCards[r_ExposedCards.Count - 1]; // remove the first card from list for we wont get it as match
                r_ExposedCards.RemoveAt(r_ExposedCards.Count - 1);

                foreach (string index in r_ExposedCards)
                {
                    int row = int.Parse(index[1].ToString()) - 1;
                    int col = index[0] - 'A';
                    if (i_GameBoard.GetCardFromBoard(row, col).IsEqual(i_FirstPick))
                    {
                        matchCards = true;
                        aiPick = index;
                        break;
                    }
                }

                if (!matchCards)
                { // do random pick, remove the first card from list for we wont get it as match
                    string temp2 = r_AvailableCardsIndexes[r_AvailableCardsIndexes.IndexOf(temp)];
                    r_AvailableCardsIndexes.RemoveAt(r_AvailableCardsIndexes.IndexOf(temp));
                    aiPick = r_AvailableCardsIndexes[randomIndex()];
                    r_AvailableCardsIndexes.Add(temp2);
                }

                r_ExposedCards.Add(temp);
            }

            return aiPick;
        }

        private int randomIndex()
        {
            return r_Rnd.Next(r_AvailableCardsIndexes.Count - 1);
        }
    }
}
