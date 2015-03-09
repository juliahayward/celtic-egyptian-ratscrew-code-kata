using CelticEgyptianRatscrewKata.Game;

namespace ConsoleBasedGame
{
    internal class PlayerInfo : IPlayer
    {
        public string Name { get; private set; }
        public char PlayCardKey { get; private set; }
        public char SnapKey { get; private set; }
        public bool IsPenalised { get; set; }
        
        public PlayerInfo(string name, char playCardKey, char snapKey)
        {
            SnapKey = snapKey;
            Name = name;
            PlayCardKey = playCardKey;
        }

        public override string ToString()
        {
            return string.Format("{0} - Play '{1}' - Snap '{2}'", Name, PlayCardKey, SnapKey);
        }

        #region Equality Members

        protected bool Equals(PlayerInfo other)
        {
            return string.Equals(Name, other.Name) && PlayCardKey == other.PlayCardKey && SnapKey == other.SnapKey;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PlayerInfo)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ PlayCardKey.GetHashCode();
                hashCode = (hashCode * 397) ^ SnapKey.GetHashCode();
                return hashCode;
            }
        }
        #endregion
    }
}