using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public string startSceneName = "PrototypeMap";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PlayGame()
	{
		try
        {
            SceneManager.LoadScene(startSceneName);
        }
        catch
        {
            Debug.LogError(@"MainMenu: Scene not found: " + startSceneName);
        }
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
