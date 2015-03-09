using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CelticEgyptianRatscrewKata.Game
{
    public enum SnapOutcome
    {
        Valid,
        Invalid,
        InvalidButEveryoneReinstated,
        IgnoredAsAlreadyPenalised
    }

    public enum PlayCardOutcome
    {
        Valid,
        HadNoCards,
        OutOfTurn
    }

    public class PlayOutcome
    {
        public PlayCardOutcome Outcome { get; set; }
        public Card CardPlayed { get; set; }
    }
}
