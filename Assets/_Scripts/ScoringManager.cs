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

	private int feedbackUpdate = 0;
	private Vector2 feedbackStartPos;

	void Start()
	{
		feedbackStartPos = feedback.transform.position;
		feedback.text = "";
	}

	void UpdateFeedback()
	{
		if (feedbackUpdate != 0)
		{
		feedback.color = Color.Lerp(feedback.color,new Color(1f,1f,1f,0f),Time.deltaTime);
		}
		feedback.transform.position = Vector2.Lerp(feedback.transform.position,new Vector2(feedback.transform.position.x,feedback.transform.position.y +20),Time.deltaTime*5);
		if ((inputManager.ExhaleCurrent < 1.5f)&&(feedbackUpdate == 0))
		{
			feedback.color = new Color(1f,0f,0f,1f);
			feedback.transform.position = feedbackStartPos;
			feedback.text = "Good";
			feedbackUpdate = 1;
		}
		else if ((inputManager.ExhaleCurrent < 2.5)&&(feedbackUpdate == 1)&&(inputManager.ExhaleCurrent > 1.5))
		{
			feedback.color = new Color(0f,0f,1f,1f);
			feedback.transform.position = feedbackStartPos;
			feedback.text = "Great!";
			feedbackUpdate = 2;
		}
		else if ((inputManager.ExhaleCurrent < 3.5)&&(feedbackUpdate == 2)&&(inputManager.ExhaleCurrent > 2.5))
		{
			feedback.color = new Color(1f,0f,1f,1f);
			feedback.transform.position = feedbackStartPos;
			feedback.text = "Awesome!!!";
			feedbackUpdate = 3;
		}
		else if ((inputManager.ExhaleCurrent < 4)&&(feedbackUpdate == 3)&&(inputManager.ExhaleCurrent > 3.5))
		{
			feedback.color = new Color(1f,1f,0f,1f);
			feedback.transform.position = feedbackStartPos;
			feedback.text = "AMAZING!!!";
			feedbackUpdate = 0;
		}
	}
	
	void Update () 
	{
		if (inputManager.ExhaleCurrent >= inputManager.ExhaleTimeMin)
		{
			if (inputManager.ExhaleCurrent < 1.5f)
			{
				IncreaseScore(ScoreType.Good);
				score.text = "Points: " + currentScore;
				//feedback.text = "Good";
				UpdateFeedback();
			}
			else if (inputManager.ExhaleCurrent < 2.5)
			{
				IncreaseScore(ScoreType.Great);
				score.text = "Points: " + currentScore;
				//feedback.text = "Great!";
				UpdateFeedback();
			}
			else if (inputManager.ExhaleCurrent < 3.5)
			{
				IncreaseScore(ScoreType.Awesome);
				score.text = "Points: " + currentScore;
				//feedback.text = "Awesome!!!";
				UpdateFeedback();
			}
			else if (inputManager.ExhaleCurrent < 4)
			{
				IncreaseScore(ScoreType.Amazing);
				score.text = "Points: " + currentScore;
				//feedback.text = "AMAZING!!!";
				UpdateFeedback();
			}
		}
		else
		{
			feedbackUpdate = 0;
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
