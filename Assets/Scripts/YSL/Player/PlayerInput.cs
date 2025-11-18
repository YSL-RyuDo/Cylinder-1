using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Vector2 MoveAxis { get; private set; }
    public bool InteractPressed { get; private set; }

    void Update()
    {
        MoveAxis = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        if (MoveAxis.sqrMagnitude > 1)
            MoveAxis.Normalize();

        InteractPressed = Input.GetKeyDown(KeyCode.F);
    }
}
