using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTransitionTrigger : MonoBehaviour
{
    public enum Direction { Next, Previous }
    public Direction transitionDirection;
    public int nextMapIndex;
    public GameObject hintUI; 

    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (transitionDirection == Direction.Next)
            {
                if (MapConditionManager.Instance == null || MapConditionManager.Instance.CanEnter(nextMapIndex))
                {
                    MapManager.Instance.ShowMap(nextMapIndex);
                }
                else
                {
                    if (hintUI != null)
                        hintUI.SetActive(true);
                }
            }
            else
            {
                // 如果是向回切图就不判断条件，直接切
                MapManager.Instance.ShowMap(nextMapIndex);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && hintUI != null)
        {
            hintUI.SetActive(false);
        }
    }
}
