using System.Collections.Generic;
using System.Linq;
using CelticEgyptianRatscrewKata.GameSetup;
using CelticEgyptianRatscrewKata.SnapRules;
using NSubstitute.Exceptions;
using NSubstitute.Routing.Handlers;

namespace CelticEgyptianRatscrewKata.Game
{
    /// <summary>
    /// Controls a game of Celtic Egyptian Ratscrew.
    /// </summary>
    public class GameController : IGameController
    {
        private readonly ISnapValidator _snapValidator;
        private readonly IDealer _dealer;
        private readonly IShuffler _shuffler;
        private readonly IList<IPlayer> _players;
        private IPlayer _currentPlayer;
        private readonly IGameState _gameState;

        public GameController(IGameState gameState, ISnapValidator snapValidator, IDealer dealer, IShuffler shuffler)
        {
            _players = new List<IPlayer>();
            _gameState = gameState;
            _snapValidator = snapValidator;
            _dealer = dealer;
            _shuffler = shuffler;
        }

        public IEnumerable<IPlayer> Players
        {
            get { return _players; }
        }

        public int StackSize
        {
            get { return _gameState.Stack.Count(); }
        }

        public Card TopOfStack
        {
            get { return _gameState.Stack.FirstOrDefault(); }
        }

        public int NumberOfCards(IPlayer player)
        {
            return _gameState.NumberOfCards(player.Name);
        }

        public bool AddPlayer(IPlayer player)
        {
            if (Players.Any(x => x.Name == player.Name)) return false;

            // The first player to be added goes first;
            if (_currentPlayer == null)
                _currentPlayer = player;

            _players.Add(player);
            _gameState.AddPlayer(player.Name, Cards.Empty());
            return true;
        }

        public PlayOutcome PlayCard(IPlayer player)
        {
            if (_currentPlayer != player)
            {
                player.IsPenalised = true;
                return new PlayOutcome() {CardPlayed = null, Outcome = PlayCardOutcome.OutOfTurn};
            }

            _currentPlayer = NextPlayer(_currentPlayer);

            if (_gameState.HasCards(player.Name))
            {
                var cardPlayed = _gameState.PlayCard(player.Name);
                return new PlayOutcome() { CardPlayed = cardPlayed, Outcome = PlayCardOutcome.Valid };
            }

            return new PlayOutcome() { CardPlayed = null, Outcome = PlayCardOutcome.HadNoCards };
        }

        public IPlayer NextPlayer(IPlayer player)
        {
            var index = _players.IndexOf(player);
            var nextIndex = (index == _players.Count - 1) ? 0 : index + 1;
            return _players[nextIndex];
        }


        public SnapOutcome AttemptSnap(IPlayer player)
        {
            if (player.IsPenalised)
                return SnapOutcome.IgnoredAsAlreadyPenalised;

            if (_snapValidator.CanSnap(_gameState.Stack))
            {
                _gameState.WinStack(player.Name);
                // After good snap, all players are back in the game!
                ReinstateAllPlayers();

                return SnapOutcome.Valid;
            }
            else
            {
                // penalise the player for an invalid snap
                player.IsPenalised = true;

                // If everyone's penalised, forgive them all
                if (_players.All(x => x.IsPenalised))
                {
                    ReinstateAllPlayers();
                    return SnapOutcome.InvalidButEveryoneReinstated;
                }
                return SnapOutcome.Invalid;
            }
        }

        private void ReinstateAllPlayers()
        {
            foreach (var p in _players) p.IsPenalised = false;
        }


        /// <summary>
        /// Starts a game with the currently added players
        /// </summary>
        public void StartGame(Cards deck)
        {
            _gameState.Clear();

            var shuffledDeck = _shuffler.Shuffle(deck);
            var decks = _dealer.Deal(_players.Count, shuffledDeck);
            for (var i = 0; i < decks.Count; i++)
            {
                _gameState.AddPlayer(_players[i].Name, decks[i]);
            }
        }

        public bool TryGetWinner(out IPlayer winner)
        {
            var playersWithCards = _players.Where(p => _gameState.HasCards(p.Name)).ToList();

            if (!_gameState.Stack.Any() && playersWithCards.Count() == 1)
            {
                winner = playersWithCards.Single();
                return true;
            }

            winner = null;
            return false;
        }
    }
}
