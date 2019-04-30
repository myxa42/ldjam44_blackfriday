
using UnityEngine;

public sealed class RestartGame : MonoBehaviour
{
    public void OnRestartButtonClicked()
    {
        Global.Instance.ResetPlayer();
        Global.Instance.LoadScene("Park");
    }
}
