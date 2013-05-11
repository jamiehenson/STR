using UnityEngine;
using System.Collections;

public class BGMusic : MonoBehaviour
{
	public AudioSource audioplayer, ambientplayer;

	void Start ()
	{
		//PlayRandomTrack();
		PlayAmbientTrack();
	}

	void Update()
	{
		if (!audioplayer.isPlaying) PlayRandomTrack();
	}

	public void PlayRandomTrack()
	{
		Object[] clips = Resources.LoadAll("music/ingame");
		audioplayer.clip = (AudioClip) clips[Random.Range (0,clips.Length)];
		audioplayer.Play();
		audioplayer.loop = true;
		audioplayer.volume = 1;
		audioplayer.pitch = 1 + (Random.Range(0,1));
	}

	public void PlayAmbientTrack()
	{
		Object[] clips = Resources.LoadAll("music/ambient");
		ambientplayer.clip = (AudioClip) clips[Random.Range (0,clips.Length)];
		ambientplayer.Play();
		ambientplayer.loop = true;
		ambientplayer.volume = 0.14f;
	}

	public void PlayBossTrack()
	{
		Object[] clips = Resources.LoadAll("music/boss");
		audioplayer.clip = (AudioClip) clips[Random.Range (0,clips.Length)];
		audioplayer.Play();
		audioplayer.loop = false;
		audioplayer.volume = 1;
	}

	public void PlayTrack(string trackname)
	{
		Object track = Resources.Load("music/ingame/" + trackname);
		audioplayer.clip = (AudioClip) track;
		audioplayer.Play();
		audioplayer.loop = true;
		audioplayer.volume = 1;
	}

	public void PlayShot(string shotname)
	{
		Object[] shot = Resources.LoadAll("sounds/weapons/" + shotname);
		AudioClip chosenOne = (AudioClip) shot[Random.Range(0,shot.Length)];
		AudioSource.PlayClipAtPoint(chosenOne,gameObject.transform.position);
		print ("shot made");
	}
}
