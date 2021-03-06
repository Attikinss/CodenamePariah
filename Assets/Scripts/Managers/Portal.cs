using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WhiteWillow;

public class Portal : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The entities that can activate this script.")]
    private GameObject[] m_TargetEntities;

    private void OnTriggerEnter(Collider other)
    {
        foreach (var target in m_TargetEntities)
        {
            if (other.gameObject == target)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = true; // Making sure cursor is visible.
                FMODAudioEvent.StopAllSounds();
                SceneManager.LoadScene("MainMenu");
                return;
            }
        }

        if (other.TryGetComponent(out Agent agent))
        {
            if (agent.Possessed)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = true; // Making sure cursor is visible.
                FMODAudioEvent.StopAllSounds();
                SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
                //SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            }
        }
    }
}
