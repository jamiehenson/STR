using UnityEngine;
using System.Collections;
using System.Linq;

public class EndGame : MonoBehaviour {
	
	private string heading, subheading, teamscores, indvscores, twitstring;
	public static int[] scores;
	public static string[] names;

    const string PLAYER_PREFS_TWITTER_USER_ID           = "STR_Game";
    const string PLAYER_PREFS_TWITTER_USER_SCREEN_NAME  = "STR";
    const string PLAYER_PREFS_TWITTER_USER_TOKEN        = "1396863306-Wxc36ezyhrlRNy95kJFz9czx5JtT7t6rrOwVog1";
    const string PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET = "ByxPbk2364GvJ7WMnTmgizNvcmkTCigvWGvKd0XIo";

    Twitter.RequestTokenResponse m_RequestTokenResponse;
    Twitter.AccessTokenResponse m_AccessTokenResponse;
	
	void Start()
	{
		heading = GenerateEndHeader();
		subheading = GenerateEndSubHeader();
		teamscores = GenerateTeamScore();
		indvscores = GenerateIndvScores();
		twitstring = GenerateTwitString();

		GameObject header = GameObject.Find ("Header");
		header.guiText.text = heading;
		GameObject subheader = GameObject.Find ("Subheader");
		subheader.guiText.text = subheading;
		GameObject finalscore = GameObject.Find ("Team");
		finalscore.guiText.text = "TEAM SCORE... \n" + teamscores;
		GameObject youscore = GameObject.Find ("Indv");
		youscore.guiText.text = "INDIVIDUAL SCORES... \n" + indvscores;

		//StartCoroutine(HSController.PostScores,name,endIndividualScore,kills,deaths);

		LoadTwitterUserInfo();
	}
	
	string GenerateEndHeader() {
		ArrayList endHeader = new ArrayList();
		endHeader.Add("Game Over!");
		endHeader.Add("Unlucky!");
		endHeader.Add("Strike out!");
		endHeader.Add("You're toast!");
		endHeader.Add("Chin up, mate.");
		string winner = (string) endHeader[(int) Random.Range(0,endHeader.Count)];
		return winner.ToUpper();
	}

	string GenerateTwitString()
	{
		string allname = "";
		foreach (string name in names) allname = allname + ", " + name;
		return allname + " got " + GenerateTeamScore() + " in #STR";
	}

	string GenerateTeamScore()
	{
		int sum = 0;
		foreach (int i in scores) sum += i;
		return "  " + sum.ToString();
	}

	string GenerateIndvScores()
	{
		string ss = "\n";
		int i = 0;
		int top = scores.Max();
		int bottom = scores.Min();
		foreach (string s in names)
		{
			string winner = (scores[i] == top) ? " - TOP GUN!" : (scores[i] == bottom) ? " - SHAMEFUL." : "";
			ss = ss + "  " + s.ToUpper() + ": " + scores[i] + winner + "\n";
			i++;
		}
		return ss;
	}
	
	string GenerateEndSubHeader() {
		ArrayList endSub = new ArrayList();
		endSub.Add("He died as he lived.");
		endSub.Add("Another one bites the dust.");
		endSub.Add("A solid effort.");
		string winner = (string) endSub[(int) Random.Range(0,endSub.Count)];
		return winner.ToUpper();
	}

	void LoadTwitterUserInfo()
    {
        m_AccessTokenResponse = new Twitter.AccessTokenResponse();

        m_AccessTokenResponse.UserId        = (PLAYER_PREFS_TWITTER_USER_ID);
        m_AccessTokenResponse.ScreenName    = (PLAYER_PREFS_TWITTER_USER_SCREEN_NAME);
        m_AccessTokenResponse.Token         = (PLAYER_PREFS_TWITTER_USER_TOKEN);
        m_AccessTokenResponse.TokenSecret   = (PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET);

        if (!string.IsNullOrEmpty(m_AccessTokenResponse.Token) &&
            !string.IsNullOrEmpty(m_AccessTokenResponse.ScreenName) &&
            !string.IsNullOrEmpty(m_AccessTokenResponse.Token) &&
            !string.IsNullOrEmpty(m_AccessTokenResponse.TokenSecret))
        {
            string log = "We've got the details, sir.";
            log += "\n    UserId : " + m_AccessTokenResponse.UserId;
            log += "\n    ScreenName : " + m_AccessTokenResponse.ScreenName;
            log += "\n    Token : " + m_AccessTokenResponse.Token;
            log += "\n    TokenSecret : " + m_AccessTokenResponse.TokenSecret;
            print(log);
        }
    }

    void OnPostTweet(bool success)
    {
        print("Tweet status: " + (success ? "SUCCESS" : "FAILURE"));
    }
	
	void OnGUI() {
		
		Font deco = (Font) Resources.Load ("Belgrad");
		Texture2D twit = (Texture2D) Resources.Load ("end/twit");
		
		GUIStyle endBigStyle = new GUIStyle();
    	endBigStyle.font = deco;
		endBigStyle.normal.textColor = Color.white;
		endBigStyle.fontSize = 120;
		endBigStyle.alignment = TextAnchor.MiddleCenter;
		
		GUIStyle endSubHeading = new GUIStyle();
    	endSubHeading.font = deco;
		endSubHeading.normal.textColor = Color.white;
		endSubHeading.alignment = TextAnchor.MiddleCenter;
		endSubHeading.fontStyle = FontStyle.Italic;
		endSubHeading.fontSize = 30;

		GUIStyle endSubHeading2 = new GUIStyle();
    	endSubHeading2.font = deco;
		endSubHeading2.normal.textColor = Color.white;
		endSubHeading2.alignment = TextAnchor.MiddleLeft;
		endSubHeading2.fontStyle = FontStyle.Italic;
		endSubHeading2.fontSize = 30;
		
		GUIStyle endSmallStyle = new GUIStyle();
    	endSmallStyle.font = deco;
		endSmallStyle.normal.textColor = Color.white;
		
		GUIStyle rightButton = new GUIStyle();
    	rightButton.font = deco;
		rightButton.normal.textColor = Color.white;
		rightButton.alignment = TextAnchor.MiddleRight;

		GUI.Label (new Rect(Screen.width*0.8f + 80, Screen.height*0.33f,100,30),"TELL THE \n WORLD!",endSubHeading2);

		if (GUI.Button (new Rect(Screen.width*0.8f,Screen.height*0.3f,64,64),twit))
		{
			StartCoroutine(Twitter.API.PostTweet(twitstring, "eEIcGZ2AY6StgnPj793yQ", "mEWCEdNR4eNiKAl3fBQmOipijjuY6N7zh4V2AKSaYQ", m_AccessTokenResponse,
                           new Twitter.PostTweetCallback(this.OnPostTweet)));
		}
		
		if (GUI.Button(new Rect(100,(Screen.height/1.15f), 200, 50), "PLAY AGAIN")) {
			HudOn.gameOver = false;
			Application.LoadLevel("plane");
		}
		if (GUI.Button(new Rect(Screen.width-300,(Screen.height/1.15f), 200, 50), "BACK TO MENU")) {
			HudOn.gameOver = false;
			Misc.CleanStatics();
			Application.LoadLevel("menu");
		}
	}

}
