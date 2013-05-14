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
		Debug.Log (track.name);
		audioplayer.Play();
		audioplayer.loop = true;
		audioplayer.volume = 0.8f;
		if (track.name == "vox") audioplayer.volume = 0.6f;
	}

	public void PlayBoom(bool boss)
	{
		Object[] clips = Resources.LoadAll("sounds/explosions");
		AudioClip chosenOne = (!boss) ? (AudioClip) clips[Random.Range (0,clips.Length)] : (AudioClip) Resources.Load ("sounds/explosions/bossexp");
		PlayClipAt(chosenOne,gameObject.transform.position,1);
	}

	public void PlayShot(string shotname)
	{
		AudioClip shot = (AudioClip) Resources.Load("sounds/weapons/" + shotname);
		PlayClipAt(shot,gameObject.transform.position,0.6f);
	}

	AudioSource PlayClipAt(AudioClip clip, Vector3 pos, float vol)
	{
		GameObject tempGO = new GameObject("AudioShot");
		tempGO.transform.position = pos;
		AudioSource aSource = tempGO.AddComponent<AudioSource>();
		aSource.clip = clip;
		aSource.Play();
		aSource.maxDistance = 2000;
		aSource.volume = 0.6f;
		Destroy(tempGO, clip.length);
		return aSource;
	}
}
