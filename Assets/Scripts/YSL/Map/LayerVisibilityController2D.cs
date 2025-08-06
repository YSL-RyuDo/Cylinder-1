using UnityEngine;

public class LayerVisibilityController2D : MonoBehaviour
{
    public PlayerLayerController playerLayerController;
    public GameObject[] layers;

    void Update()
    {
        if (playerLayerController == null || layers == null || layers.Length == 0)
            return;

        int current = playerLayerController.currentLayerIndex;

        int start, end;

        if (current <= 1)
        {
            start = 0;
            end = 3;
        }
        else if (current >= layers.Length - 3)
        {
            start = layers.Length - 4;
            end = layers.Length - 1;
        }
        else
        {
            start = current - 1;
            end = current + 2;
        }

        start = Mathf.Clamp(start, 0, layers.Length - 1);
        end = Mathf.Clamp(end, 0, layers.Length - 1);

        for (int i = 0; i < layers.Length; i++)
        {
            layers[i].SetActive(i >= start && i <= end);
        }
    }
}
