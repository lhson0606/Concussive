using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TagHandler
{
    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";

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
            string tagValue = splitTag[1].Trim();

            switch (tagKey)
            {
                case SPEAKER_TAG:
                    speakerName.text = tagValue;
                    portraitAnimator.Play(tagValue);
                    
                    break;
                default:
                    Debug.LogWarning("Co tag chua duoc xu li" + tag);
                    break;
            }
        }
    }

    public void SetSpeakerName(TMP_Text speaker){
        speakerName = speaker;
    }

    public void SetPortraitAnimator(Animator animator){
        portraitAnimator = animator;
    }

}
