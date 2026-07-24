using UnityEngine;
using VContainer;

public class InputConnector : MonoBehaviour
{
    [Inject] private InputManager _inputManager;
    public InputManager InputManager { get => _inputManager; }
}
