using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class AudioCode : MonoBehaviour
{
	//rotate
	public AudioSource audioSource;
	//hook
	public AudioSource audioSource2;
	//motion
	public AudioSource audioSource3;
	//strela
	public AudioSource audioSource4;


	void Start()
	{

		audioSource.volume = 0;
		audioSource2.volume = 0;
		audioSource3.volume = 0;
		audioSource4.volume = 0;

	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKey(KeyCode.W))
		{
			if (!audioSource2.isPlaying)
			{
				audioSource2.Play();
				audioSource2.volume += Time.deltaTime * 0.5f;
			}
		}

		if (Input.GetKey(KeyCode.S))
		{
			if (!audioSource2.isPlaying)
			{
				audioSource2.Play();
				audioSource2.volume += Time.deltaTime * 0.5f;
			}
		}
		if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
		{
			audioSource2.volume -= Time.deltaTime * 1f;
			if (audioSource2.volume == 0)
				audioSource2.Pause();
		}
		if (Input.GetKey(KeyCode.A))
		{
			if (!audioSource.isPlaying)
			{
				audioSource.Play();
				audioSource.volume += Time.deltaTime * 0.5f;
			}
		}
		if (Input.GetKey(KeyCode.D))
		{

			if (!audioSource.isPlaying)
			{
				audioSource.Play();
				audioSource.volume += Time.deltaTime * 0.5f;
			}
		}
		if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
		{
			audioSource.volume -= Time.deltaTime * 0.5f;
			if (audioSource.volume == 0)
				audioSource.Pause();
		}
		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
		{
			if (!audioSource3.isPlaying)
			{
				audioSource3.volume += Time.deltaTime * 2f;
				audioSource3.Play();
				
			}
		}
		if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
		{
			audioSource3.volume -= Time.deltaTime * 1f;
			if (audioSource3.volume == 0)
				audioSource3.Pause();
		}

		if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.C))
		{
			if (!audioSource4.isPlaying)
			{
				audioSource4.Play();
				audioSource4.volume += Time.deltaTime * 1.5f;
			}
		}
		if (!Input.GetKey(KeyCode.Z) && !Input.GetKey(KeyCode.C))
		{

			//audioSource5.volume = audioSource4.volume;
			audioSource4.volume -= Time.deltaTime * 1f;
			if (audioSource4.volume == 0)
				audioSource4.Pause();
		}
	}
}
