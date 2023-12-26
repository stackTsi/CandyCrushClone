using System;
using UnityEngine;

[Serializable]
public class BlankGoal{
    public int numberNeeded;
    public int numberCollected;
    public Sprite goalSprite;
    public string matchValue;
}
public class GoalManager : MonoBehaviour
{
    public BlankGoal[] levelGoals;
    public GameObject goalPrefab;
    public GameObject goalIntroParent;
    public GameObject goalGameParent;

    // Start is called before the first frame update
    void Start()
    {
        SetupIntroGoals();
    }
    void SetupIntroGoals(){
        for (int i = 0; i < levelGoals.Length; i++){
            //Create a new goal Panel at goalIntroParent position
            GameObject goal = Instantiate(goalPrefab,goalIntroParent.transform.position,Quaternion.identity);
            goal.transform.SetParent(goalIntroParent.transform, false);
            //Set the image and text of goal
            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeeded;

            //Create a new goal Panel inside the Game
            GameObject gameGoal = Instantiate(goalPrefab,goalGameParent.transform.position,Quaternion.identity);
            gameGoal.transform.SetParent(goalGameParent.transform, false);
            panel = gameGoal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeeded;
        }   
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
