using System;
using System.Text;
using MemoryGameLogic;


namespace MemoryGameUI
{
    public class GameUi
    {
        private const string k_Quit = "Q";
        private GameLogic m_GameLogic;
        private string m_PlayerName = "default";
        private string m_SecondPlayerName = "PC";
        private int m_TableGameRows = 0;
        private int m_TableGameCols = 0;
        private bool m_FirstPlayerTurn = true;
        private bool m_IsQuitKeyPressed = false;
        private bool m_RestartGame = true;
        private bool m_IsSinglePlayer = true;

        public void Run()
        {
            getParticipantInfo();
            while (m_RestartGame)
            {
                getGameBoardSize();

                m_GameLogic = new GameLogic(m_PlayerName, m_SecondPlayerName, m_TableGameRows, m_TableGameCols);
                drawGameTable();

                if (m_IsSinglePlayer)
                {
                    m_GameLogic.InitAi(m_TableGameRows, m_TableGameCols);
                }

                while (!m_GameLogic.IsGameOver())
                {
                    if (m_IsSinglePlayer && !m_FirstPlayerTurn)
                    {
                        pcTurn();
                    }
                    else
                    {
                        playerTurn();
                    }

                    if (m_IsQuitKeyPressed)
                    {
                        break;
                    }

                    if (!m_GameLogic.AreCardsEqual(m_FirstPlayerTurn))
                    {
                        // wait 2 sec if cards are not equal
                        Console.WriteLine(":( , Not a match");
                        System.Threading.Thread.Sleep(2000);
                        drawGameTable();
                        m_FirstPlayerTurn = !m_FirstPlayerTurn;
                    }
                    else
                    {
                        Console.WriteLine(":) , Match!");
                    }
                }

                printStats();
            }
        }

        private void getParticipantInfo()
        {
            // get player name and game type
            int numberOfPlayers;

            Console.Write("Please type your name: ");
            m_PlayerName = getPlayerName();
            string openMsg = string.Format(
@"Choose game type(1 or 2):
1. Player vs Pc
2. Player vs Player");

            Console.WriteLine(openMsg);
            bool checkInput = int.TryParse(Console.ReadLine(), out numberOfPlayers);

            while (!checkInput || (numberOfPlayers != 1 && numberOfPlayers != 2))
            {
                Console.Write("Invalid input, Try again: ");
                checkInput = int.TryParse(Console.ReadLine(), out numberOfPlayers);
            }

            if (numberOfPlayers == 2)
            {
                m_IsSinglePlayer = false;
                Console.Write("Please type second player name: ");
                m_SecondPlayerName = getPlayerName();
            }
        }

        private string getPlayerName()
        {
            string playerName = Console.ReadLine();
            while (string.IsNullOrEmpty(playerName))
            {
                Console.Write("Invalid name, Try again: ");
                playerName = Console.ReadLine();
            }

            return playerName;
        }

        private void getGameBoardSize()
        { // get game board size and check the input (muse be in the range of 4x4 - 6x6 and even)
            const int k_MinRows = 4;
            const int k_MaxRows = 6;
            const int k_MinCols = 4;
            const int k_MaxCols = 6;

            string settingMsg = string.Format(
                @"Please choose board game size between 4x4 to 6x6:
(must be number an even multiple, number of tiles):");

            Console.WriteLine(settingMsg);
            while (true)
            {
                Console.Write("Please enter number of rows: ");
                string rows = Console.ReadLine();
                while (!int.TryParse(rows, out m_TableGameRows) || (m_TableGameRows < k_MinRows || m_TableGameRows > k_MaxRows))
                {
                    Console.Write("Invalid input, try again: ");
                    rows = Console.ReadLine();
                }

                Console.Write("Please enter number of cols: ");
                string cols = Console.ReadLine();
                while (!int.TryParse(cols, out m_TableGameCols) || (m_TableGameCols < k_MinCols || m_TableGameCols > k_MaxCols))
                {
                    Console.Write("Invalid input, try again: ");
                    cols = Console.ReadLine();
                }

                if ((m_TableGameCols * m_TableGameRows) % 2 != 0)
                {
                    Console.WriteLine("The board can not have an odd number of tiles.");
                }
                else
                {
                    break;
                }
            }
        }

        private void drawGameTable()
        {
            Console.Clear();
            StringBuilder drawTable = new StringBuilder();
            drawTable.Append("  ");
            for (int i = 0; i < m_TableGameCols; i++)
            {
                drawTable.Append("   " + Convert.ToChar(i + 65));
            }

            drawTable.AppendLine();

            for (int i = 0; i < m_TableGameRows; i++)
            {
                drawTable.Append("   ");
                drawTable.Append('=', (m_TableGameCols * 4) + 1).AppendLine();
                drawTable.Append((i + 1) + " ");

                for (int j = 0; j < m_TableGameCols; j++)
                {
                    drawTable.Append(" | ");
                    drawTable.Append(m_GameLogic.PrintCard(i, j));
                }

                drawTable.Append(" | ").AppendLine();
            }

            drawTable.Append("   ");
            drawTable.Append('=', (m_TableGameCols * 4) + 1).AppendLine();
            Console.Write(drawTable);
        }

        private void playerTurn()
        {
            pickCard();
            drawGameTable();
            if (!m_IsQuitKeyPressed)
            {
                pickCard();
                drawGameTable();
            }
        }

        private void pcTurn()
        {
            eInputError inputError;

            Console.WriteLine("PC is picking cards...");
            m_GameLogic.ChooseCard(m_GameLogic.AiIndexCard(), out inputError);
            System.Threading.Thread.Sleep(1000); // to show the player the msg that pc is picking cards..
            drawGameTable();
            System.Threading.Thread.Sleep(500); // to allow the player see what pc picked in first turn
            m_GameLogic.ChooseCard(m_GameLogic.AiIndexCard(), out inputError);
            drawGameTable();
        }

        private void pickCard()
        {
            eInputError inputError;
            string playerTurn = m_FirstPlayerTurn ? $"First player: {m_PlayerName}," :
                                                    $"Second player: {m_SecondPlayerName},";
            Console.Write("{0} please choose card (for example A2), type Q to quit game : ", playerTurn);
            while (true)
            {
                string playerPick = playerPickValidation();

                if (!m_IsQuitKeyPressed && !m_GameLogic.ChooseCard(playerPick, out inputError))
                {
                    switch (inputError)
                    {
                        case eInputError.OutOfBoardRange:
                            Console.WriteLine("Wrong input, {0} out of board range", playerPick);
                            break;
                        case eInputError.CardAlreadyPicked:
                            Console.WriteLine("Oops, card in location {0} already picked", playerPick);
                            break;
                        default:
                            Console.WriteLine("Something wrong");
                            break;
                    }

                    Console.Write("Try again: ");
                    continue;
                }

                break;
            }
        }

        private string playerPickValidation()
        {
            string playerPick = string.Empty;
            int userInputLength = 0;

            while (true)
            {
                playerPick = Console.ReadLine();
                userInputLength = playerPick?.Length ?? 0; // check if its null

                while ((playerPick != k_Quit) && (userInputLength != 2))
                {
                    Console.Write("Invalid input, try again: ");
                    playerPick = Console.ReadLine();
                    userInputLength = playerPick?.Length ?? 0;
                }

                if (userInputLength == 1 && playerPick == k_Quit)
                {
                    m_IsQuitKeyPressed = true;
                    printStats();
                    m_RestartGame = false;
                    break;
                }
                else
                { // in case length is 2
                    if (char.IsUpper(playerPick?[0] ?? ',') && char.IsDigit(playerPick?[1] ?? ','))
                    {  // to avoid playerPick is null, so "," wont pass this condition
                        break;
                    }
                    else
                    {
                        Console.Write("Invalid input, try again: ");
                    }
                }
            }

            return playerPick;
        }

        private void printStats()
        {
            string firstPlayerName = string.Empty;
            string secondPlayerName = string.Empty;
            int firstPlayerScore = 0;
            int secondPlayerScore = 0;

            m_GameLogic.PlayerDetails(1, ref firstPlayerName, ref firstPlayerScore);
            m_GameLogic.PlayerDetails(2, ref secondPlayerName, ref secondPlayerScore);

            string msg = string.Format(
@"Game End, Statistics:
First Player {0} Score: {1}
Second Player {2} Score: {3}",
                            firstPlayerName,
                            firstPlayerScore,
                            secondPlayerName,
                            secondPlayerScore);
            Console.WriteLine(msg);

            // winner announcement (in case QuitKey not pressed)
            if (!m_IsQuitKeyPressed)
            {
                Console.WriteLine();
                if (firstPlayerScore > secondPlayerScore)
                {
                    Console.WriteLine("First Player {0} is the winner!", firstPlayerName);
                }
                else if (firstPlayerScore < secondPlayerScore)
                {
                    Console.WriteLine("Second Player {0} is the winner!", secondPlayerName);
                }
                else
                {
                    // firstPlayerScore == SecondPlayerScore
                    Console.WriteLine("DRAW! there is no winner");
                }

                playAgain();
            }
        }

        private void playAgain()
        {
            int PlayAgain;
            string playAgainMsg = string.Format(
                @"
Play Again?(1 or 2):
1. Yes
2. No");

            Console.WriteLine(playAgainMsg);
            bool checkInput = int.TryParse(Console.ReadLine(), out PlayAgain);

            while (!checkInput && PlayAgain != 1 && PlayAgain != 2)
            {
                Console.Write("Invalid input, Try again: ");
                checkInput = int.TryParse(Console.ReadLine(), out PlayAgain);
            }

            if (PlayAgain == 1)
            {
                restartGame();
            }
            else
            { // == 2 (due to the the while loop condition)
                m_RestartGame = false;
                Console.WriteLine("Thanks for playing, Bye Bye");
            }
        }

        private void restartGame()
        {
            m_RestartGame = true;
            m_FirstPlayerTurn = true;
            m_IsQuitKeyPressed = false;
            Console.Clear();
        }
    }
}
