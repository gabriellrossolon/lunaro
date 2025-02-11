using UnityEngine;

public class AutoSaver : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5)) // Correção aqui
        {
            ES3AutoSaveMgr.Current.Save();
        }
    }
}
