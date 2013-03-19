using UnityEngine;
using System.Collections;
 
public class HSController : MonoBehaviour
{
    private string secretKey = "blobbo"; // Edit this value and make sure it's the same as the one stored on the server
    public string addScoreURL = "http://jh47.com/str/addscore.php?"; //be sure to add a ? to your url
    public string highscoreURL = "http://jh47.com/str/getscores.php";
 
    void Start()
    {
        StartCoroutine(GetScores());
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
    IEnumerator PostScores(string teamnames, int score)
    {
        //This connects to a server side php script that will add the name and score to a MySQL DB.
        // Supply it with a string representing the players name and the players score.
        string hash = Md5Sum(name + score + secretKey);
 
        string post_url = addScoreURL + "teamnames=" + WWW.EscapeURL(teamnames) + "&score=" + score + "&hash=" + hash;
 
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
    IEnumerator GetScores()
    {
        gameObject.guiText.text = "LOADING SCORES...";
        WWW hs_get = new WWW(highscoreURL);
        yield return hs_get;
 
        if (hs_get.error != null)
        {
            print("There was an error, due to an error: " + hs_get.error);
        }
        else
        {
            gameObject.guiText.text = hs_get.text.ToUpper(); // this is a GUIText that will display the scores in game.
        }
    }
 
}