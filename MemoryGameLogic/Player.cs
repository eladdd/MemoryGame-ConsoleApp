namespace MemoryGameLogic
{
    public class Player
    {
        private string m_PlayerName;
        private int m_PlayerScore;

        public Player(string i_PlayerName)
        {
            this.PlayerName = i_PlayerName;
            this.PlayerScore = 0;
        }

        public int PlayerScore
        {
            get
            {
                return this.m_PlayerScore;
            }

            set
            {
                this.m_PlayerScore = value;
            }
        }

        public string PlayerName
        {
            get
            {
                return this.m_PlayerName;
            }

            set
            {
                this.m_PlayerName = value;
            }
        }
    }
}
