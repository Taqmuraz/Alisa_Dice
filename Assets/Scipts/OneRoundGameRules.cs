using System.Collections.Generic;
using System.Linq;

public sealed partial class DiceGame
{
	private sealed class OneRoundGameRules : GameRulesProvider
	{
		IEnumerator<Player> playerIterator;

		public OneRoundGameRules(DiceGame game) : base(game)
		{
			playerIterator = game.players.GetEnumerator();
		}

		public override int GetDicesCount()
		{
			return 2;
		}

		public override bool MoveNextPlayer()
		{
			bool move = playerIterator.MoveNext();
			if (!move) OnEndGame();
			return move;
		}

		public override void ScoreCallback(int score)
		{
			GetCurrentPlayer().scores += score;
		}

		public override Player GetCurrentPlayer()
		{
			return playerIterator.Current;
		}

		public override void OnStartGame()
		{
			game.ShowThrowWindow();
		}

		private void OnEndGame()
		{
			game.ShowWinnerWindow();
		}

		public override string GetScoreDesc(int score)
		{
			return string.Empty;
		}

		public override string GetPlayerDesc(Player player)
		{
			return player.ToString();
		}

		public override IEnumerable<Player> GetWinners()
		{
			int max = game.players.Max(p => p.scores);
			var winners = game.players.Where(p => p.scores == max);
			return winners;
		}
	}
}
