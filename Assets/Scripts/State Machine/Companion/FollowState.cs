﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowState : ICompanionState {

    private readonly Companion companion;
    private readonly StatePatternCompanion stateMachine;


    public FollowState(StatePatternCompanion statePatternCompanion)
    {
        companion = statePatternCompanion.companion;
        stateMachine = statePatternCompanion;
    }

    public void UpdateState()
    {
        CheckPosition();
    }

    public void OnTriggerEnter(Collider other)
    {
    
    }

    public void ToFollowState()
    {
        Debug.Log("Can't transition to same state");
    }

    public void FromFollowState()
    {
        Debug.Log("Can't transition from same state");
    }

    public void ToCombatState()
    {
        stateMachine.currentState = stateMachine.combatState;
        stateMachine.currentState.FromFollowState();
    }

    public void FromCombatState()
    {
        companion.speed = 5;
    }

    void CheckPosition()
    {
        companion.GetCandidatePositions();
        if (AStar.GetDistance(companion.grid.NodeFromWorldPoint(companion.transform.position), companion.grid.NodeFromWorldPoint(companion.player.transform.position)) > companion.maximumDistance)
        {
            int x = Random.Range(0, companion.candidatePositions.Count - 1);
            Vector3 targetPos = companion.candidatePositions[x];
            companion.RequestNewPath(targetPos);
        }
    }

}
