using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle }

public class GameController : MonoBehaviour
{
   [SerializeField] PlayerController playercontroller;
   [SerializeField] BattleSystem battleSystem;
   [SerializeField] Camera worldCamera;

   GameState state;

public void Start(){

    playercontroller.OnEncountered += StartBattle;
    battleSystem.onBattleOver += EndBattle;
}

void StartBattle(){

    state = GameState.Battle;
    battleSystem.gameObject.SetActive(true);
    worldCamera.gameObject.SetActive(false);
    battleSystem.StartBattle();
}

void EndBattle(bool won){
    state = GameState.FreeRoam;
    battleSystem.gameObject.SetActive(false);
    worldCamera.gameObject.SetActive(true);

}

public void Update() {
    
    if(state == GameState.FreeRoam){

        playercontroller.HandleUpdate();
    }
    else if (state == GameState.Battle){

        battleSystem.HandleUpdate();
    }

   }
}
