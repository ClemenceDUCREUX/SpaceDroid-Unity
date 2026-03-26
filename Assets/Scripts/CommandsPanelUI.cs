using UnityEngine;

public class CommandsPanelUI : MonoBehaviour{
    // panneau des commandes
    [SerializeField] private GameObject commandsPanel;

    private void Start(){
        // cache le panneau au début
        if (commandsPanel != null) commandsPanel.SetActive(false);
    }

    // ouvre le panneau
    public void Open() => commandsPanel.SetActive(true);

    // ferme le panneau
    public void Close() => commandsPanel.SetActive(false);
}
