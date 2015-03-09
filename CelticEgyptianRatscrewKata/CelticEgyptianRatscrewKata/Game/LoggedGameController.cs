using System;
using System.Collections.Generic;

namespace CelticEgyptianRatscrewKata.Game
{
    public class LoggedGameController : IGameController
    {
        private readonly IGameController _gameController;
        private readonly ILog _log;

        public LoggedGameController(IGameController gameController, ILog log)
        {
            _gameController = gameController;
            _log = log;
        }

        public bool AddPlayer(IPlayer player)
        {
            return _gameController.AddPlayer(player);
        }

        public PlayOutcome PlayCard(IPlayer player)
        {
            string snapLogMessage;
            var outcome = _gameController.PlayCard(player);
            switch (outcome.Outcome)
            {
                case PlayCardOutcome.Valid:
                    snapLogMessage = string.Format("{0} has played the {1}", player.Name, outcome.CardPlayed);
                    break;
                case PlayCardOutcome.OutOfTurn:
                    snapLogMessage = string.Format("{0} played out of turn - penalised and {1} pushed to bottom of stack", player.Name, outcome.CardPlayed);
                    break;
                case PlayCardOutcome.HadNoCards:
                    snapLogMessage = string.Format("{0} had no cards to play", player.Name);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _log.Log(snapLogMessage);
            LogGameState();
            return outcome;
        }

        public SnapOutcome AttemptSnap(IPlayer player)
        {
            string snapLogMessage;
            var outcome = _gameController.AttemptSnap(player);
            switch (outcome)
            {
                case SnapOutcome.Valid:
                    snapLogMessage = string.Format("{0} won the stack", player.Name);
                    break;
                case SnapOutcome.Invalid:
                    snapLogMessage = string.Format("{0} is penalised for a wrong snap", player.Name);
                    break;
                case SnapOutcome.IgnoredAsAlreadyPenalised:
                    snapLogMessage = string.Format("{0} was already penalised", player.Name);
                    break;
                case SnapOutcome.InvalidButEveryoneReinstated:
                    snapLogMessage = string.Format("Everyone penalised - so you're all back in the game");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _log.Log(snapLogMessage);
            LogGameState();
            return outcome;
        }

        public void StartGame(Cards deck)
        {
            _gameController.StartGame(deck);
            LogGameState();
        }

        public bool TryGetWinner(out IPlayer winner)
        {
            var hasWinner = _gameController.TryGetWinner(out winner);
            if (hasWinner)
            {
                _log.Log(string.Format("{0} won the game!", winner.Name));
            }
            return hasWinner;
        }

        public IEnumerable<IPlayer> Players
        {
            get { return _gameController.Players; }
        }

        public int StackSize
        {
            get { return _gameController.StackSize; }
        }

        public Card TopOfStack
        {
            get { return _gameController.TopOfStack; }
        }

        public int NumberOfCards(IPlayer player)
        {
            return _gameController.NumberOfCards(player);
        }

        private void LogGameState()
        {
            _log.Log(string.Format("Stack ({0}): {1} ", _gameController.StackSize, _gameController.StackSize > 0 ? _gameController.TopOfStack.ToString() : ""));
            foreach (var player in _gameController.Players)
            {
                _log.Log(string.Format("{0}: {1} cards", player.Name, _gameController.NumberOfCards(player)));
            }
        }
    }
}