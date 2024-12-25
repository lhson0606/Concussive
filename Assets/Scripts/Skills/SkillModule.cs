using System.Collections.Generic;
using UnityEngine;

public class SkillModule : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> skillPrefabs = new List<GameObject>();

    private List<BaseSkill> skills = new List<BaseSkill>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var skillPrefab in skillPrefabs)
        {
            var skill = Instantiate(skillPrefab, transform).GetComponent<BaseSkill>();
            if (skill == null)
            {
                Debug.LogError("Skill prefab does not have BaseSkill component");
                continue;
            }
            skills.Add(skill);
            skill.SetOwner(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UseRandomSkill()
    {
        if (skills.Count == 0)
        {
            return;
        }
        int randomIndex = Random.Range(0, skills.Count);
        skills[randomIndex].Use();
    }

    public bool UseRandomSkillWithProbability(float probability)
    {
        if (skills.Count == 0)
        {
            return false;
        }
        if (Random.value < probability)
        {
            int randomIndex = Random.Range(0, skills.Count);
            return skills[randomIndex].Use();
        }

        return false;
    }

    public void UseAllSkill()
    {
        foreach (var skill in skills)
        {
            skill.Use();
        }
    }

    public void UseSkill(int index)
    {
        if (index < 0 || index >= skills.Count)
        {
            return;
        }
        skills[index].Use();
    }
}
