using UnityEngine;
using System.Collections;

public class BGMusic : MonoBehaviour
{
	public AudioSource audioplayer;

	void Start ()
	{
		PlayRandomTrack();
	}

	public void PlayRandomTrack()
	{
		Object[] clips = Resources.LoadAll("music/ingame");
		audioplayer.clip = (AudioClip) clips[Random.Range (0,clips.Length)];
		audioplayer.Play();
		audioplayer.loop = false;
		audioplayer.volume = 1;
		audioplayer.pitch = 1 + (Random.Range(0,1));
		if (!audioplayer.isPlaying) PlayRandomTrack();
	}

	public void PlayBossTrack()
	{
		Object[] clips = Resources.LoadAll("music/boss");
		audioplayer.clip = (AudioClip) clips[Random.Range (0,clips.Length)];
		audioplayer.Play();
		audioplayer.loop = false;
		audioplayer.volume = 1;
		if (!audioplayer.isPlaying) PlayRandomTrack();
	}

	public void PlayTrack(string trackname)
	{
		Object track = Resources.Load("music/ingame/" + trackname);
		audioplayer.clip = (AudioClip) track;
		audioplayer.Play();
		audioplayer.loop = true;
		audioplayer.volume = 1;
		audioplayer.pitch = 1 + (Random.Range(0,1));
	}

}
