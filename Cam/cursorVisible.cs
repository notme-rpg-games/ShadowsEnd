using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class cursorVisible : MonoBehaviour
{
    void Start()
    {
        #if UNITY_EDITOR
            Cursor.SetCursor(PlayerSettings.defaultCursor, Vector2.zero, CursorMode.ForceSoftware);
        #endif
    }
}

