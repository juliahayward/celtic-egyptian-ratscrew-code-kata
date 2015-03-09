namespace CelticEgyptianRatscrewKata.Game
{
    public class Player : IPlayer
    {
        public Player(string playerId)
        {
            Name = playerId;
            IsPenalised = false;
        }

        public string Name { get; private set; }

        public bool IsPenalised { get; set; }
    }
}