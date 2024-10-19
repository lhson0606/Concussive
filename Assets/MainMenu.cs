using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PlayGame()
	{
		SceneManager.LoadSceneAsync(1);
	}
    public void MuteHandler(bool mute)
	{
		if(mute)
		{
			AudioListener.volume = 0;
		}	
		else
		{
			AudioListener.volume = 1;
		}
	}
}
