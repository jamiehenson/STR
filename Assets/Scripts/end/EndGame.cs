using UnityEngine;
using System.Collections;
using System.Linq;

public class EndGame : MonoBehaviour {
	
	private string heading, subheading, teamscores, indvscores, twitstring;
	public static int[] scores;
	public static string[] names;

	// Website stuff
	private string secretKey = "blobbo";
    //private string addScoreURL = "http://jh47.com/str/addscore.php?";
    //private string highscoreURL = "http://jh47.com/str/getscores.php";
	//private string killsURL = "http://jh47.com/str/getkills.php";
	///private string deathsURL = "http://jh47.com/str/getdeaths.php";

	// Twitter stuff
    const string PLAYER_PREFS_TWITTER_USER_ID           = "STR_Game";
    const string PLAYER_PREFS_TWITTER_USER_SCREEN_NAME  = "STR";
    const string PLAYER_PREFS_TWITTER_USER_TOKEN        = "1396863306-Wxc36ezyhrlRNy95kJFz9czx5JtT7t6rrOwVog1";
    const string PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET = "ByxPbk2364GvJ7WMnTmgizNvcmkTCigvWGvKd0XIo";

    Twitter.RequestTokenResponse m_RequestTokenResponse;
    Twitter.AccessTokenResponse m_AccessTokenResponse;
	
	void Start()
	{
		Screen.showCursor = true;

		heading = GenerateEndHeader();
		subheading = GenerateEndSubHeader();
		teamscores = GenerateTeamScore();
		indvscores = GenerateIndvScores();
		twitstring = GenerateTwitString();

		//StartCoroutine(GetScores());
		//StartCoroutine(GetKills());
		//StartCoroutine(GetDeaths());

		GameObject header = GameObject.Find ("Header");
		header.guiText.text = heading;
		GameObject subheader = GameObject.Find ("Subheader");
		subheader.guiText.text = subheading;
		GameObject finalscore = GameObject.Find ("Team");
		finalscore.guiText.text = "TEAM SCORE... \n  " + teamscores;
		GameObject youscore = GameObject.Find ("Indv");
		youscore.guiText.text = "INDIVIDUAL SCORES... \n  " + indvscores;

		// Post to website
		//for (int i = 0; i < names.Length; i++) StartCoroutine(PostScores(names[i],scores[i],0,0));

		// Sort twitter out
		LoadTwitterUserInfo();

		// Post to twitter
		StartCoroutine(Twitter.API.PostTweet(twitstring, "eEIcGZ2AY6StgnPj793yQ", "mEWCEdNR4eNiKAl3fBQmOipijjuY6N7zh4V2AKSaYQ", m_AccessTokenResponse,
                           new Twitter.PostTweetCallback(this.OnPostTweet)));
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

	string GenerateGood() {
		ArrayList player = new ArrayList();
            player.Add("Top gun!");
            player.Add("Winner!");
            player.Add("Like a boss!");
            player.Add("Good work!");
            player.Add("Owned.");
            player.Add("How it's done.");
            player.Add("Head honcho");
            player.Add("Big Daddy");
            player.Add("Stone cold killa");
            player.Add("Tactical victory");
            player.Add("The crowd goes wild!");
            player.Add("Prime");
		string winner = (string) player[(int) Random.Range(0,player.Count)];
		return winner.ToUpper();
	}

	string GenerateBad() {
		ArrayList player = new ArrayList();
            player.Add("Shameful");
            player.Add("What was that?");
            player.Add("Boooooo.");
            player.Add("A puppy cried.");
            player.Add("Once more, with enthusiasm.");
            player.Add("Cadet");
            player.Add("Nub.");
            player.Add("Rookie error.");
		string winner = (string) player[(int) Random.Range(0,player.Count)];
		return winner.ToUpper();
	}

	string GenerateTwitString()
	{
		string allname = "";
		foreach (string name in names) allname = allname + ", " + name;
		string twit = (allname + " got " + GenerateTeamScore() + " in a game of #STR!").Remove(0,1);
		return twit.Substring(0, Mathf.Min(140, twit.Length));
	}

	string GenerateTeamScore()
	{
		int sum = 0;
		foreach (int i in scores) sum += i;
		return sum.ToString();
	}

	string GenerateIndvScores()
	{
		string ss = "\n";
		int i = 0;
		int top = scores.Max();
		int bottom = scores.Min();
		foreach (string s in names)
		{
			string winner = (scores[i] == top) ? " - " + GenerateGood() : (scores[i] == bottom) ? " - " + GenerateBad() : "";
			ss = ss + s.ToUpper() + ": " + scores[i] + winner + "\n";
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

		GUI.Label (new Rect(Screen.width*0.6f + 80, Screen.height*0.4f+15,100,30),"SCORE SENT \nTO TWITTER\n" +
			"AT @STR_GAME",endSubHeading2);

		if (GUI.Button (new Rect(Screen.width*0.6f,Screen.height*0.4f,64,64),twit))
		{
			Application.OpenURL ("https://twitter.com/STR_Game");
		}

		/*
		if (GUI.Button(new Rect(100,(Screen.height/1.15f), 200, 50), "PLAY AGAIN")) {
			HudOn.gameOver = false;
			Application.LoadLevel("plane");
		}
		 */

		if (GUI.Button(new Rect(Screen.width-300,(Screen.height/1.15f), 200, 50), "EXIT THE GAME")) {
			HudOn.gameOver = false;
			Misc.CleanStatics();
			Application.Quit();
		}
	}

	private string Md5Sum(string strToEncrypt)
	{
		System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
		byte[] bytes = ue.GetBytes(strToEncrypt);
	 
		// encrypt bytes
		System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
		byte[] hashBytes = md5.ComputeHash(bytes);
	 
		// Convert the encrypted bytes back to a string (base 16)
		string hashString = "";
	 
		for (int i = 0; i < hashBytes.Length; i++)
		{
			hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
		}
 
		return hashString.PadLeft(32, '0');
	}

	/*
    private IEnumerator PostScores(string teamnames, int score, int kills, int deaths)
    {
        //This connects to a server side php script that will add the name and score to a MySQL DB.
        // Supply it with a string representing the players name and the players score.
        string hash = Md5Sum(name + score + secretKey);
 
        string post_url = addScoreURL + "teamnames=" + WWW.EscapeURL(teamnames) + "&score=" + score + "&kills=" + kills + "&deaths=" + deaths + "&hash=" + hash;
 
        // Post the URL to the site and create a download object to get the result.
        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done
 
        if (hs_post.error != null)
        {
            print("There was an error posting the high score: " + hs_post.error);
        }
    }
 
    // Get the scores from the MySQL DB to display in a GUIText.
    // remember to use StartCoroutine when calling this function!
    private IEnumerator GetScores()
    {
        GameObject scoreBox = GameObject.Find ("Scores");
		scoreBox.guiText.text = "LOADING SCORES...";
        WWW hs_get = new WWW(highscoreURL);
        yield return hs_get;
 
        if (hs_get.error != null)
        {
            print("SCORES. There was an error, due to an error: " + hs_get.error);
        }
        else
        {
            scoreBox.guiText.text = hs_get.text.ToUpper(); // this is a GUIText that will display the scores in game.
        }
    }

	IEnumerator GetKills()
    {
        GameObject killBox = GameObject.Find ("Kills");
		killBox.guiText.text = "LOADING KILLS...";
        WWW hs_get = new WWW(killsURL);
        yield return hs_get;
 
        if (hs_get.error != null)
        {
            print("KILLS. There was an error, due to an error: " + hs_get.error);
        }
        else
        {
            killBox.guiText.text = hs_get.text.ToUpper(); // this is a GUIText that will display the scores in game.
        }
    }
	
	IEnumerator GetDeaths()
    {
        GameObject deathsBox = GameObject.Find ("Deaths");
		deathsBox.guiText.text = "LOADING DEATHS...";
        WWW hs_get = new WWW(deathsURL);
        yield return hs_get;
 
        if (hs_get.error != null)
        {
            print("DEATHS. There was an error, due to an error: " + hs_get.error);
        }
        else
        {
            deathsBox.guiText.text = hs_get.text.ToUpper(); // this is a GUIText that will display the scores in game.
        }
    }
    */
}
