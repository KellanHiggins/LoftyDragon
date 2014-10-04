using UnityEngine;
using System.Collections;

public class ScoringManager : MonoBehaviour {

	public enum ScoreType
	{
		NoScoring = 0,
		Good,
		Great,
		Awesome,
		Amazing
	}

	public int currentScore;

	public ScoreType currentlyScoringType;

	public int GoodScoreNum = 100;
	public int GreatScoreNum = 150;
	public int AwesomeScoreNum = 225;
	public int AmazingScoreNum = 350;

	void Update () 
	{
		if(currentlyScoringType != ScoreType.NoScoring)
		{
			// What the score is this round.
			int thisRoundScoreNumber = 0;

			// checks to see what type of scoring will be going on
			switch(currentlyScoringType)
			{
			case ScoreType.NoScoring:
				thisRoundScoreNumber = 0;
				break;
			case ScoreType.Good:
				thisRoundScoreNumber = GoodScoreNum;
				break;

			case ScoreType.Great:
				thisRoundScoreNumber = GreatScoreNum;
				break;

			case ScoreType.Awesome:
				thisRoundScoreNumber = AwesomeScoreNum;
				break;

			case ScoreType:
				thisRoundScoreNumber = AmazingScoreNum;
			
			default:
				thisRoundScoreNumber = 0;
				break;
			}

			currentScore += Mathf.RoundToInt(GoodScoreNum * Time.deltaTime);
		}
	}

	public void IncreaseScore(ScoreType scoreType)
	{
		currentlyScoringType = scoreType;
	}

	public void StopScoring()
	{
		currentlyScoringType = ScoreType.NoScoring;
	}
}
