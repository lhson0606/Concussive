using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagHandler
{
    private const string COLLECT_TAG = "collect";
    private const string FLAG_TAG = "flag";
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
                default:
                    Debug.LogWarning("Co tag chua duoc xu li" + tag);
                    break;
            }
        }
    }


}
