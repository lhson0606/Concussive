using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine; 
using UnityEngine.SceneManagement;
public class TagHandler
{
    private const string SPEAKER_TAG = "speaker";
    private const string ENDSCENE_TAG = "endScene";
    // private const string DROP_TAG = "drop";
    private TMP_Text speakerName;
    private Animator portraitAnimator;

    private GameObject currentGameObject;

    public void HandleTags(List<string> currentTags)
    {
        currentGameObject = DialogueManager.GetInstance().GetCurrentGameObject();

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
                // case DROP_TAG:

                //     Vector3 itemSpawnPos;
                //     //Find Prefab by tag value
                //     GameObject prefab = Resources.Load<GameObject>("Prefabs/" + tagValue);

                //     if(prefab == null)
                //     {
                //         Debug.LogError("Prefab not found: " + tagValue);
                //         break;
                //     }
                //     itemSpawnPos = new Vector3(currentGameObject.transform.position.x, 
                //                                 currentGameObject.transform.position.y - 0.5f, 
                //                                 currentGameObject.transform.position.z);   

                //     GameObject spawnedItem = Object.Instantiate(prefab, itemSpawnPos, Quaternion.identity);
                //     GameItem gameItem = spawnedItem.GetComponent<GameItem>();

                //     gameItem.DropItem(itemSpawnPos); 

                //     break;
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
