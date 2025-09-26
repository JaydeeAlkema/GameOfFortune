using UnityEngine;

public class AudioCue : MonoBehaviour
{
	[SerializeField] private AudioClip AudioClip;
	[SerializeField] private AudioSource AudioSource;

	public void Play()
	{
		if (AudioSource == null || AudioClip == null)
			return;

		AudioSource.PlayOneShot(AudioClip);
	}
}
