using Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreView : MonoBehaviour
{
    private TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        text.SetText(SingletonGameData.Instance.playerScore.score.ToString()); 
    }
}
