using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Match3UI : MonoBehaviour {

    [SerializeField] private Match3 match3;
    
    private TextMeshProUGUI scoreText;
    private Transform winLoseTransform;

    private void Awake() {
        scoreText = transform.Find("scoreText").GetComponent<TextMeshProUGUI>();
        winLoseTransform = transform.Find("winLose");
        winLoseTransform.gameObject.SetActive(false);

        match3.OnLevelSet += Match3_OnLevelSet;
        match3.OnScoreChanged += Match3_OnScoreChanged;
        match3.OnWin += Match3_OnWin;
    }

    private void Match3_OnWin(object sender, System.EventArgs e) {
        winLoseTransform.gameObject.SetActive(true);
        winLoseTransform.Find("text").GetComponent<TextMeshProUGUI>().text = "<color=#1ACC23>YOU WIN!</color>";
    }

    private void Match3_OnOutOfMoves(object sender, System.EventArgs e) {
        winLoseTransform.gameObject.SetActive(true);
        winLoseTransform.Find("text").GetComponent<TextMeshProUGUI>().text = "<color=#CC411A>YOU LOSE!</color>";
    }

    private void Match3_OnScoreChanged(object sender, System.EventArgs e) {
        UpdateText();
    }
    

    private void Match3_OnLevelSet(object sender, System.EventArgs e) {
        UpdateText();
    }

    private void UpdateText() {
        scoreText.text = match3.GetScore().ToString();
    }


}
