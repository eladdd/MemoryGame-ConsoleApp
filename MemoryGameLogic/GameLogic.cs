using System;

namespace MemoryGameLogic
{
    public class GameLogic
    {
        private readonly Player r_Player1;
        private readonly Player r_Player2;
        private readonly Board r_GameBoard;
        private Card m_SelectedFirstCard;
        private Card m_SelectedSecondCard;
        private AiMoves m_PcAiMoves;

        public GameLogic(string i_Player1, string i_Player2, int i_BoardRows, int i_BoardCols)
        {
            this.r_Player1 = new Player(i_Player1);
            this.r_Player2 = new Player(i_Player2);
            this.r_GameBoard = new Board(i_BoardRows, i_BoardCols);
            this.m_SelectedFirstCard = null;
            this.m_SelectedSecondCard = null;
            this.m_PcAiMoves = null;
        }

        public bool ChooseCard(string i_CardSelect, out eInputError o_InputError)
        {
            int row = int.Parse(i_CardSelect[1].ToString()) - 1;
            int col = i_CardSelect[0] - 'A';
            bool validInput = false;

            if (row >= r_GameBoard.Row || col >= r_GameBoard.Column || row < 0)
            {
                o_InputError = eInputError.OutOfBoardRange;
            }
            else
            {
                if (r_GameBoard.IsCardPicked(row, col))
                {
                    o_InputError = eInputError.CardAlreadyPicked;
                }
                else
                {
                    if (m_SelectedFirstCard == null)
                    {
                        this.m_SelectedFirstCard = r_GameBoard.GetCardFromBoard(row, col);
                    }
                    else
                    {
                        this.m_SelectedSecondCard = r_GameBoard.GetCardFromBoard(row, col);
                    }

                    // add the card for pc data structure if player vs pc
                    this.m_PcAiMoves?.AddExposedCard(i_CardSelect);

                    o_InputError = eInputError.Valid;
                    validInput = true;
                }
            }

            return validInput;
        }

        public bool AreCardsEqual(bool i_FirstPlayerTurn)
        {
            bool equal = false; // if equal return as FALSE UI will re draw the board

            if (!m_SelectedFirstCard.IsEqual(m_SelectedSecondCard))
            {
                m_SelectedFirstCard.ChangeCardSide();
                m_SelectedSecondCard.ChangeCardSide();
            }
            else
            { // update player score
                if (i_FirstPlayerTurn)
                {
                    this.r_Player1.PlayerScore++;
                }
                else
                {
                    this.r_Player2.PlayerScore++;
                }

                this.m_PcAiMoves?.RemoveLastCardsFromLists();
                equal = true;
            }

            this.m_SelectedFirstCard = null;
            this.m_SelectedSecondCard = null;

            return equal;
        }

        public bool IsGameOver()
        {
            return this.r_GameBoard.AreCardsFlipped();
        }

        public char PrintCard(int i_Row, int i_Col)
        {
            return this.r_GameBoard.GetCardFromBoard(i_Row, i_Col).Value;
        }

        public void PlayerDetails(int i_Player, ref string io_Name, ref int io_Score)
        {
            switch (i_Player)
            {
                // 1 - first player , 2 - second player
                case 1:
                    io_Name = r_Player1.PlayerName;
                    io_Score = r_Player1.PlayerScore;
                    break;
                case 2:
                    io_Name = r_Player2.PlayerName;
                    io_Score = r_Player2.PlayerScore;
                    break;
            }
        }

        public void InitAi(int i_TableGameRows, int i_TableGameCols)
        {
            this.m_PcAiMoves = new AiMoves(i_TableGameRows, i_TableGameCols);
        }

        public string AiIndexCard()
        {
            return this.m_PcAiMoves.AiMove(m_SelectedFirstCard, r_GameBoard);
        }

        public int GetPlayerScore(bool i_FirstPlayerTurn)
        {
            return i_FirstPlayerTurn ? r_Player1.PlayerScore : r_Player2.PlayerScore;
        }
    }
}
