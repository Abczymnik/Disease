using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GAgent : MonoBehaviour
{
    public List<GAction> actions = new List<GAction>();
    public Dictionary<SubGoal, int> goals = new Dictionary<SubGoal, int>();
    //public GInventory inventory = new GInventory();
    public WorldStates beliefs = new WorldStates();

    GPlanner planner;
    Queue<GAction> actionQueue;
    public GAction currentAction;
    SubGoal currentGoal;
    private Coroutine decisionMakerCoroutine;

    //Refresh ferquency for coroutine yield
    private Dictionary<string, float> refreshRateDict = new Dictionary<string, float>()
    {
        {"Search", 1f},
        {"Lost", 0.5f},
        {"Static movement", 1f},
        {"Dynamic movement", 0f},
        {"Attack", 0.1f}
    };

    public void Start()
    {
        GAction[] acts = GetComponents<GAction>();
        foreach (GAction a in acts)
        {
            actions.Add(a);
        }

        decisionMakerCoroutine = StartCoroutine(DecisionMaker());
    }

    void CompleteAction()
    {
        currentAction.running = false;
        currentAction.PostPerform();
    }

    IEnumerator DecisionMaker()
    {
        while(true)
        {
            if(currentAction != null && currentAction.running)
            {
                if (currentAction.Success())
                {
                    currentAction.Agent.ResetPath();
                    CompleteAction();
                    continue;
                }

                if (!currentAction.Func()) 
                {
                    actionQueue = null;
                    currentAction.running = false;
                }

                yield return new WaitForSeconds(GetRefreshRate(currentAction.ActionType));

                continue;
            }

            if (planner == null || actionQueue == null) { FindNewPlan(); }

            if (actionQueue != null && actionQueue.Count == 0) { CurrGoalAchieved(); }

            if (actionQueue != null && actionQueue.Count > 0) { TrySelectNextAction(); }

            yield return null;

        }
    }

    private float GetRefreshRate(string actionType)
    {
        float refreshRate;
        bool hasValue = refreshRateDict.TryGetValue(actionType, out refreshRate);
        if (hasValue) return refreshRate;
        return 0f;
    }

    private void FindNewPlan()
    {
        planner = new GPlanner();

        var sortedGoals = from entry in goals orderby entry.Value descending select entry;

        foreach (KeyValuePair<SubGoal, int> sg in sortedGoals)
        {
            actionQueue = planner.Plan(actions, sg.Key.sgoals, beliefs);
            if (actionQueue != null)
            {
                currentGoal = sg.Key;
                break;
            }
        }
    }

    private void TrySelectNextAction()
    {
        currentAction = actionQueue.Dequeue();

        if (currentAction.PrePerform()) { currentAction.running = true; }

        else { actionQueue = null; }
    }

    private void CurrGoalAchieved()
    {
        if (currentGoal.remove) { goals.Remove(currentGoal); }
        planner = null;
    }

    protected void PlanStop()
    {
        StopCoroutine(decisionMakerCoroutine);
        decisionMakerCoroutine = null;
    }
}
