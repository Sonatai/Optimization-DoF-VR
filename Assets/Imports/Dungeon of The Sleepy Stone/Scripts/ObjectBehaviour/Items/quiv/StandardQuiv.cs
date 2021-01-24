using UnityEngine;
using UnityEngine.SceneManagement;

public class StandardQuiv : Triggerable
{
    public override void Trigger(HandController hand)
    {
        if (SceneManager.GetActiveScene().name == "Level1")
        {
            var _level1MainPuzzle = GameObject.FindGameObjectWithTag("EventHandler").GetComponent<Level1_MainPuzzle>();

            _level1MainPuzzle.quiverFound = true;
            _level1MainPuzzle.CheckOpenExit();
        }

        GameObject.Find("Controller (left)").transform.Find("RadialMenu").GetComponent<RadialMenu>().ActivateButton(1);
        GameObject.Find("Controller (right)").transform.Find("RadialMenu").GetComponent<RadialMenu>().ActivateButton(1);
        
        hand.ExitObject(gameObject);
        
        Destroy(gameObject);
    }
}