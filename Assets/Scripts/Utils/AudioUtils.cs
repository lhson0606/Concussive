using UnityEngine;

public class AudioUtils
{
    public static void PlayAudioClipAtPoint(AudioClip clip, Vector3 position, float volume = 1.0f)
    {
        GameObject tempAudioObject = new GameObject("TempAudio");
        tempAudioObject.transform.position = position;
        AudioSource audioSource = tempAudioObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        Object.Destroy(tempAudioObject, clip.length);
    }
}
