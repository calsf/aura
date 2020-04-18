using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StoppableMovementBehaviour : MonoBehaviour
{
    public abstract void StopMoving();
    public abstract void ResumeMoving();
}
