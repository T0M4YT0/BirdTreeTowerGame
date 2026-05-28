using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectUIElement : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private Selectable elementToSelect;

    [Header("Visualization")]
    [SerializeField] private bool showVisualization;
    [SerializeField] private Color navigationColour = Color.cyan;

    private void OnDrawGizmos()
    {
        if (!showVisualization)
            return;

        if (elementToSelect == null)
            return;

        Gizmos.color = navigationColour;
        Gizmos.DrawLine(gameObject.transform.position, elementToSelect.gameObject.transform.position);
    }

    private void Reset()
    {
        eventSystem = FindObjectOfType<EventSystem>();

        if (eventSystem == null)
            Debug.Log("Did not find an Event System in your Scene", context:this);
    }

    public void JumpToElement()
    {
        if (eventSystem == null)
            Debug.Log("This item has no event system referenced yet", context: this);

        if (elementToSelect == null)
            Debug.Log("This should jump where", context: this);

        eventSystem.SetSelectedGameObject(elementToSelect.gameObject);                       
    }

}
