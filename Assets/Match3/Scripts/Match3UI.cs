using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
        match3.OnLose += Match3_OnLose;

    }
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Match3_OnWin(this,EventArgs.Empty);
        }else if (Input.GetKeyDown(KeyCode.D))
        {
            Match3_OnLose(this,EventArgs.Empty);
        }
    }
#endif

    void Restart()
    {
        match3.Restart();
    }
    
    private void Match3_OnWin(object sender, EventArgs e) {
        winLoseTransform.gameObject.SetActive(true);
        winLoseTransform.Find("text").GetComponent<TextMeshProUGUI>().text = "<color=#1ACC23>YOU WIN!</color>";
        winLoseTransform.Find("Restart").GetComponent<Button>().onClick.AddListener(Restart);
    }

    private void Match3_OnLose(object sender, EventArgs e)
    {
        winLoseTransform.gameObject.SetActive(true);
        winLoseTransform.Find("text").GetComponent<TextMeshProUGUI>().text = "<color=#FF0000>YOU LOSE</color>";
        winLoseTransform.Find("Restart").GetComponent<Button>().onClick.AddListener(Restart);
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
