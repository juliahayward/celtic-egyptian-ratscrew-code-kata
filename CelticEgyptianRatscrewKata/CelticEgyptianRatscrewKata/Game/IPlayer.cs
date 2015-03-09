namespace CelticEgyptianRatscrewKata.Game
{
    /// <summary>
    /// Represents a player of the game.
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        /// The name of the player, <em>must</em> be unique.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// If the payer has made an invalid move, he becomes penalised temporarily - can't make snaps in this state
        /// </summary>
        bool IsPenalised { get; set; }
    }
}