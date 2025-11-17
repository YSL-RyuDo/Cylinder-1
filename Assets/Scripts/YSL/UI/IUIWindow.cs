using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIWindow
{
    bool IsOpen { get; }
    void Open();
    void Close();
}
