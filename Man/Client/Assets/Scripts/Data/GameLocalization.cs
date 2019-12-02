using UnityEngine;
using UnityEngine.UI;

class GameLocalization : MonoBehaviour
{
    public GameStringType type;

    void Start()
    {
        updateText();
    }

    public void updateText()
    {
        Text text = GetComponent<Text>();
        text.text = GameStringData.instance.getString( type );
    }

}
