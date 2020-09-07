using System.Collections.Generic;

public sealed partial class DiceGame
{
	private abstract class GameRulesProvider
	{
		protected DiceGame game { get; private set; }

		public GameRulesProvider(DiceGame game)
		{
			this.game = game;
		}

		public abstract void OnStartGame();
		public abstract int GetDicesCount();
		public abstract bool MoveNextPlayer();
		public abstract Player GetCurrentPlayer();
		public abstract void ScoreCallback(int score);
		public abstract string GetScoreDesc (int score);
		public abstract string GetPlayerDesc (Player player);
		public abstract IEnumerable<Player> GetWinners();
	}
}
