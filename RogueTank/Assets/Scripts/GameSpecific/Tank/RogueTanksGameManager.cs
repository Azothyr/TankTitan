using UnityEngine;
using UnityEngine.Events;

namespace GameSpecific.Tank
{
    public class RogueTanksGameManager : MonoBehaviour
    {
        [Header("Game Manager")]
        [SerializeField] private GameAction onStartAction;
        [SerializeField] private UnityEvent onStart;
        private void Start()
        {
            onStartAction.RaiseAction();
        }
    }
}
