using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BlankGoal
{
    public int numberNeeded;
    public int numberCollected;
    public Sprite goalSprite;
    public string matchValue;
}
public class GoalManager : MonoBehaviour
{
    public BlankGoal[] levelGoals;
    public List<GoalPanel> currentGoals = new();
    public GameObject goalPrefab;
    public GameObject goalIntroParent;
    public GameObject goalGameParent;

    // Start is called before the first frame update
    void Start()
    {
        SetupGoals();
    }
    void SetupGoals()
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            //Create a new goal Panel at goalIntroParent position
            GameObject goal = Instantiate(goalPrefab, goalIntroParent.transform.position, Quaternion.identity);
            goal.transform.SetParent(goalIntroParent.transform, false);
            //Set the image and text of goal
            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeeded;

            //Create a new goal Panel inside the Game
            GameObject gameGoal = Instantiate(goalPrefab, goalGameParent.transform.position, Quaternion.identity);
            gameGoal.transform.SetParent(goalGameParent.transform, false);
            panel = gameGoal.GetComponent<GoalPanel>();
            currentGoals.Add(panel);
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeeded;
        }
    }

    //in game actual goals
    public void UpdateGoals()
    {
        int goalsCompleted = 0;
        for (int i = 0; i < levelGoals.Length; i++)
        {
            currentGoals[i].thisText.text = "" + levelGoals[i].numberCollected + "/" + levelGoals[i].numberNeeded;
            //if the collected number is greater than the needed number
            if (levelGoals[i].numberCollected >= levelGoals[i].numberNeeded)
            {
                goalsCompleted++;
                currentGoals[i].thisText.text = "" + levelGoals[i].numberNeeded + "/" + levelGoals[i].numberNeeded;
            }
        }
        if (goalsCompleted >= levelGoals.Length)
        {
            Debug.Log("Congrats mate, you win!");
        }
    }

    public void CompareGoal(string goalToCompare){
        for (int i = 0; i <levelGoals.Length; i++){
            if(goalToCompare == levelGoals[i].matchValue){
                levelGoals[i].numberCollected++;
            } 
        }
    }
}
