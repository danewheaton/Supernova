using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonkeyIs { STARTING, JUMPING, HOLDING_VINE }

public class MonkeyStates : MonoBehaviour
{
    public static MonkeyIs currentState = MonkeyIs.STARTING;
}
