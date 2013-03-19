using UnityEngine;
using System.Collections;
 
public class HSController : MonoBehaviour
{
    private string secretKey = "blobbo";
    private string addScoreURL = "http://jh47.com/str/addscore.php?";
    private string highscoreURL = "http://jh47.com/str/getscores.php";
	private string killsURL = "http://jh47.com/str/getkills.php";
	private string deathsURL = "http://jh47.com/str/getdeaths.php";
 
    void Start()
    {
        StartCoroutine(GetScores());
		StartCoroutine(GetKills());
		StartCoroutine(GetDeaths());
    }
 
	public string Md5Sum(string strToEncrypt)
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
	
    // remember to use StartCoroutine when calling this function!
    public IEnumerator PostScores(string teamnames, int score, int kills, int deaths)
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
    public IEnumerator GetScores()
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
 
}