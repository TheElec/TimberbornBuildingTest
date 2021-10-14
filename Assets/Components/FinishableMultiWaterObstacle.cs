using Timberborn.ConstructibleSystem;
using Timberborn.WaterBuildings;
using UnityEngine;

public class FinishableMultiWaterObstacle : MonoBehaviour, IFinishedStateListener
{
    public float height;
    private WaterObstacle _waterObstacle;

    private void Awake()
    {
        /*MethodInfo getComponent = typeof(FinishableMultiWaterObstacle).GetMethod("GetComponent");
        MethodInfo getWaterObstacle = getComponent.MakeGenericMethod()*/
        this._waterObstacle = this.GetComponent<WaterObstacle>();
    }

    public void OnEnterFinishedState() => this._waterObstacle.AddMultipleToWaterService(height);

    public void OnExitFinishedState() => this._waterObstacle.RemoveFromWaterService();
}
