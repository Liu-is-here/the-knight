using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class TransitionPoint : MonoBehaviour
{
    public enum TransitionType
    {
        SameScene, DifferentScene
    }
    [Header("Transition Info")]
    public string sceneName; // name of different scene
    public TransitionType transitionType;
    public TransitionDestination.DestinationTag destinationTag;

    private bool canTrans;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canTrans)
        {
            // Debug.Log("Transition to " + destinationTag);
            SceneController.Instance.TransitionToDestination(this);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            canTrans = true;
        }
    }
    void onTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            canTrans = false;
        }
    }
}
