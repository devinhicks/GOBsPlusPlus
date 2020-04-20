using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal
{
    public string name;
    public int identifier;
    public float value;

    public float getDiscontentment(float newValue)
    {
        return newValue * newValue;
    }

    public Goal (string goalName, float goalValue, int goalIdentifier)
    {
        name = goalName;
        value = goalValue;
        identifier = goalIdentifier;
    }

    //public abstract float getChange();
}

public class Action
{
    public string name;

    public List<Goal> targetGoals;

    public Action (string actionName)
    {
        name = actionName;
        targetGoals = new List<Goal>();
    }

    public float getGoalChange(Goal goal)
    {
        foreach (Goal target in targetGoals)
        {
            if (target.name == goal.name)
            {
                return target.value;
            }
        }

        return 0f;
    }

    //public abstract float getDuration();
}

public class Decider : MonoBehaviour
{
    Goal[] goals;
    Action[] actions;
    Action changeOverTime;
    const float TICK_LENGTH = 5.0f;

    // UI stuff
    public Text discontentValue;
    public Text TOTWvalue;
    public Text PVGvalue;
    public Text EATvalue;
    public Text speak;

    void Start()
    {
        // intialize GUI
        speak.text = "> I am not content";

        // initial goals
        goals = new Goal[3];
        goals[0] = new Goal("Eat", 4, 0);
        goals[1] = new Goal("Take Over the World", 3, 1);
        goals[2] = new Goal("Play Video Games", 3, 2);

        // the long part where I define all the actions I can take
        actions = new Action[6];

        actions[0] = new Action("eat some sushi");
        actions[0].targetGoals.Add(new Goal("Eat", -3f, 0));
        actions[0].targetGoals.Add(new Goal("Take Over the World", +1f, 1));
        actions[0].targetGoals.Add(new Goal("Play Video Games", +2f, 2));

        actions[1] = new Action("eat a bag of cheetos");
        actions[1].targetGoals.Add(new Goal("Eat", -2f, 0));
        actions[1].targetGoals.Add(new Goal("Take Over the World", +1f, 1));
        actions[1].targetGoals.Add(new Goal("Play Video Games", +1f, 2));

        actions[2] = new Action("conquer Canada");
        actions[2].targetGoals.Add(new Goal("Eat", +2f, 0));
        actions[2].targetGoals.Add(new Goal("Take Over the World", -1f, 1));
        actions[2].targetGoals.Add(new Goal("Play Video Games",+1f, 2));

        actions[3] = new Action("solve World Hunger");
        actions[3].targetGoals.Add(new Goal("Eat", -1f, 0));
        actions[3].targetGoals.Add(new Goal("Take Over the World", -1f, 1));
        actions[3].targetGoals.Add(new Goal("Play Video Games", +2f, 2));

        actions[4] = new Action("play Animal Crossing: New Horizons");
        actions[4].targetGoals.Add(new Goal("Eat", +2f, 0));
        actions[4].targetGoals.Add(new Goal("Take Over the World", 0f, 1));
        actions[4].targetGoals.Add(new Goal("Play Video Games", -3f, 2));

        actions[5] = new Action("play a game of Risk");
        actions[5].targetGoals.Add(new Goal("Eat", +2f, 0));
        actions[5].targetGoals.Add(new Goal("Take Over the World", -2f, 1));
        actions[5].targetGoals.Add(new Goal("Play Video Games", 0f, 2));

        // lets pass some time
        changeOverTime = new Action("tickTock");
        changeOverTime.targetGoals.Add(new Goal("Eat", +3f, 0));
        changeOverTime.targetGoals.Add(new Goal("Take Over the World", +2f, 1));
        changeOverTime.targetGoals.Add(new Goal("Play Video Games", +2f, 2));

        Debug.Log("Starting clock - one hour will pass every " + TICK_LENGTH
            + " seconds");
        InvokeRepeating("Tick", 0f, TICK_LENGTH);

        Debug.Log("Hit E to do something");
    }

    void Tick()
    {
        foreach (Goal goal in goals)
        {
            goal.value += changeOverTime.getGoalChange(goal);
            goal.value = Mathf.Max(goal.value, 0);
        }

        PrintGoals();
    }

    void PrintGoals()
    {
        // Console
        string goalString = "";

        foreach(Goal goal in goals)
        {
            goalString += goal.name + ": " + goal.value + "; ";
        }
        goalString += "Discontentment: " + CurrentDiscontentment();
        Debug.Log(goalString);

        // GUI
        discontentValue.text = CurrentDiscontentment().ToString();

        foreach (Goal goal in goals)
        {
            if (goal.identifier == 0)
            {
                EATvalue.text = new string('|', (int)goal.value);
            }
            else if (goal.identifier == 1)
            {
                TOTWvalue.text = new string('|', (int)goal.value);
            }
            else if (goal.identifier == 2)
            {
                PVGvalue.text = new string('|', (int)goal.value);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Action bestThingToDo = ChooseAction(actions, goals);
            Debug.Log("I think I'll " + bestThingToDo.name);
            speak.text = "> I think I'll " + bestThingToDo.name;

            foreach (Goal goal in goals)
            {
                goal.value += bestThingToDo.getGoalChange(goal);
                goal.value = Mathf.Max(goal.value, 0);
            }

            PrintGoals();
        }
    }

    public Action ChooseAction(Action[] actions, Goal[] goals)
    {
        // find the action leading to the lowest discontentment
        Action bestAction = null;
        float bestValue = Mathf.Infinity;

        foreach (Action thisAction in actions)
        {
            float thisValue = Discontentment(thisAction, goals);
            if (thisValue < bestValue)
            {
                bestValue = thisValue;
                bestAction = thisAction;
            }
        }

        return bestAction;
    }

    float Discontentment(Action action, Goal[] goals)
    {
        // keep a running total
        float discontentment = 0;

        // loop through each goal
        foreach (Goal goal in goals)
        {
            // calculate the new value after the action
            float newValue = goal.value + action.getGoalChange(goal);

            // calculate the change due to time alone
            //newValue += action.getDuration() * goal.getChange();
            newValue = Mathf.Max(newValue, 0);

            // get the discontentment of this value
            discontentment += goal.getDiscontentment(newValue);
        }

        return discontentment;
    }

    float CurrentDiscontentment()
    {
        float total = 0f;
        foreach(Goal goal in goals)
        {
            total += (goal.value * goal.value);
        }

        return total;
    }
}
