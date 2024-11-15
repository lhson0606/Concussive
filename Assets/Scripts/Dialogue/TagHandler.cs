using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine; 
using UnityEngine.SceneManagement;
public class TagHandler
{
    private const string SPEAKER_TAG = "speaker";
    private const string ENDSCENE_TAG = "endScene";
    private TMP_Text speakerName;
    private Animator portraitAnimator;

    public void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            if (splitTag.Length > 2)
            {
                Debug.LogError("Wrong tag" + tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = "";
            if(splitTag.Length == 2)
            {
                tagValue = splitTag[1].Trim();
            }
            switch (tagKey)
            {
                case SPEAKER_TAG:
                    speakerName.text = tagValue;
                    portraitAnimator.Play(tagValue);
                    if(tagValue == "default")
                    {
                        DialogueManager.GetInstance().SetNameTextBox(false);
                    }
                    else
                    {
                        DialogueManager.GetInstance().SetNameTextBox(true);
                    }
                    break;
                case ENDSCENE_TAG:
                    SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
                    break;
                default:
                    Debug.LogWarning("Non-definded tag: " + tag);
                    break;
            }
        }
    }

    public void SetSpeakerName(TMP_Text speaker)
    {
        speakerName = speaker;
    }

    public void SetPortraitAnimator(Animator animator)
    {
        portraitAnimator = animator;
    }

}
