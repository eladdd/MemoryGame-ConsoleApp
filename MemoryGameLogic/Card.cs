namespace MemoryGameLogic
{
    public class Card
    {
        private char m_Value;
        private bool m_CardHidden;

        public Card(char i_Value)
        {
            this.Value = i_Value;
            this.IsCardHidden = true;
        }

        public char Value
        {
            get
            {
                return this.IsCardHidden ? ' ' : this.m_Value;
            }

            private set
            {
                this.m_Value = value;
            }
        }

        public bool IsCardHidden
        {
            get
            {
                return this.m_CardHidden;
            }

            private set
            {
                this.m_CardHidden = value;
            }
        }

        public void ChangeCardSide()
        {
            this.IsCardHidden = !this.IsCardHidden;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public bool IsEqual(Card i_SelectedCard)
        {
            return this.m_Value == i_SelectedCard.m_Value;
        }
    }
}
