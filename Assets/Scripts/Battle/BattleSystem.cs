using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;

    public event Action<bool> onBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;

    public void StartBattle() {
        
        StartCoroutine(SetUpBattle());
    }

    public IEnumerator SetUpBattle(){

        playerUnit.SetUp();
        playerHud.SetData(playerUnit.Pokemon);

        enemyUnit.SetUp();
        enemyHud.SetData(enemyUnit.Pokemon);

        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

       yield return dialogBox.TypeDialog($"¡Un {enemyUnit.Pokemon.Base.Name} salvaje junto a Abascal ha aparecido!");
        yield return new WaitForSeconds(1f);

        PlayerAction();
    }

    void PlayerAction(){
        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("Escoge una acción"));
        dialogBox.EnableActionSelector(true);
    }

    void PlayerMove(){

        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator PerformPlayerMove(){

        state = BattleState.Busy;

        var move = playerUnit.Pokemon.Moves[currentMove];
        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} usa {move.Base.Name}");
        yield return new WaitForSeconds(1f);

       var damageDetails = enemyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
        yield return enemyHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if(damageDetails.Fainted){
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} se ha debilitado!");
            yield return new WaitForSeconds(2f);
            onBattleOver(true);
       
        }else{
            StartCoroutine(EnemyMove());
        }
    }   

    IEnumerator EnemyMove(){

        state = BattleState.EnemyMove;
        var move = enemyUnit.Pokemon.GetRandomMove();

        yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} usa {move.Base.Name}");
        yield return new WaitForSeconds(1f);

      var damageDetails = playerUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
       yield return playerHud.UpdateHP();
       yield return ShowDamageDetails(damageDetails);

        if(damageDetails.Fainted){
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} se ha debilitado!");
            yield return new WaitForSeconds(2f);
            onBattleOver(false);
        
        }else{
            PlayerAction();
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails){
        if(damageDetails.Critical > 1f){
            yield return dialogBox.TypeDialog("¡Un ataque critico!");
        }
        if(damageDetails.Type > 1f){
            yield return dialogBox.TypeDialog("¡Es super efectivo!");

        }else if(damageDetails.Type < 1f){
            yield return dialogBox.TypeDialog("¡No es efectivo!");
        }
    }
    

    public void HandleUpdate() {
        
        if(state == BattleState.PlayerAction){
            HandleActionSelection();
        }else if (state == BattleState.PlayerMove){
            HandleMoveSelection();
        }
    }

    void HandleActionSelection(){

        if(Input.GetKeyDown(KeyCode.DownArrow)){

            if(currentAction < 1){
                ++currentAction;
            }
        }else if(Input.GetKeyDown(KeyCode.UpArrow)){

            if(currentAction > 0){
                --currentAction;
            }
        }

        dialogBox.UpdateActionSelection(currentAction);

        if(Input.GetKeyDown(KeyCode.Z)){

            if(currentAction == 0){

                PlayerMove();
            }else if (currentAction == 1){

            }
    }
}

void HandleMoveSelection(){

    if(Input.GetKeyDown(KeyCode.RightArrow)){
        if(currentMove < playerUnit.Pokemon.Moves.Count - 1){
            ++currentMove;
        }
    }
    else if(Input.GetKeyDown(KeyCode.LeftArrow)){
        if(currentMove > 0){
            --currentMove;
        }
    }
    else if(Input.GetKeyDown(KeyCode.DownArrow)){
        if(currentMove < playerUnit.Pokemon.Moves.Count - 2){
            currentMove += 2;
        }
    }
    else if(Input.GetKeyDown(KeyCode.UpArrow)){
        if(currentMove > 1){
            currentMove -= 2;
        }
    }

    dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

    if(Input.GetKeyDown(KeyCode.Z)){
        dialogBox.EnableMoveSelector(false);
        dialogBox.EnableDialogText(true);
        StartCoroutine(PerformPlayerMove());
    }
}

}
