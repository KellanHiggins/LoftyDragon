using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoringManager : MonoBehaviour {

	public enum ScoreType
	{
		NoScoring = 0,
		Good,
		Great,
		Awesome,
		Amazing
	}

	[SerializeField]
	private InputManager inputManager;

	public int currentScore;

	public ScoreType currentlyScoringType;

	public int GoodScoreNum = 100;
	public int GreatScoreNum = 150;
	public int AwesomeScoreNum = 225;
	public int AmazingScoreNum = 350;
	[SerializeField]
	private Text score;
	[SerializeField]
	private Text feedback;

	void Start()
	{
		feedback.text = "";
	}

	void Update () 
	{
		if (inputManager.ExhaleCurrent >= inputManager.ExhaleTimeMin)
		{
			if (inputManager.ExhaleCurrent < 1.5f)
			{
				IncreaseScore(ScoreType.Good);
				score.text = "Points: " + currentScore;
				feedback.text = "Good";
			}
			else if (inputManager.ExhaleCurrent < 2.5)
			{
				IncreaseScore(ScoreType.Great);
				score.text = "Points: " + currentScore;
				feedback.text = "Great!";
			}
			else if (inputManager.ExhaleCurrent < 3.5)
			{
				IncreaseScore(ScoreType.Awesome);
				score.text = "Points: " + currentScore;
				feedback.text = "Awesome!!!";
			}
			else if (inputManager.ExhaleCurrent < 4)
			{
				IncreaseScore(ScoreType.Amazing);
				score.text = "Points: " + currentScore;
				feedback.text = "AMAZING!!!";
			}
		}
		else
		{
			feedback.text = "";
		}
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

			case ScoreType.Amazing:
				thisRoundScoreNumber = AmazingScoreNum;
				break;
			
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
