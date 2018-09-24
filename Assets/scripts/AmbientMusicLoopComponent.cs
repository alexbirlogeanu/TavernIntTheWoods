using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
/*NOT USED*/
public class AmbientMusicLoopComponent : MonoBehaviour {
    public List<AudioClip> AudioClips = new List<AudioClip>();

    //private:
    private int Index = 0;
    private AudioSource Audio = null;
	// Use this for initialization
	void Start () {
        Audio = GetComponent<AudioSource>();
        Assert.IsNotNull(Audio, "AmbietMusicLoopComponent.Start: No AudioSource component found!");
        Assert.IsTrue(AudioClips.Count > 0, "AmbietMusicLoopComponent.Start: No audio clips set");

        if (Audio != null && AudioClips.Count > 0)
        {
            StartCoroutine(PlaySounds());
        }
	}
	
	private IEnumerator PlaySounds()
    {
        while(true)
        {
            Audio.clip = AudioClips[Index];
            Audio.Play();
            yield return new WaitForSeconds(AudioClips[Index].length);
            Index = (Index + 1) % AudioClips.Count;
        }
    }
}
