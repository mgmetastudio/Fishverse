using UnityEngine;
using UnityEngine.UI;

namespace LibsEngine.States.Implements
{
    public class HidenInputStateMachineMonobeh : MonoBehaviour, IStateables<bool>
    {
        [SerializeField] protected ChangeInputFieldHiden _hidenInputStates;

        [SerializeField] private Button _hidenButton;

        [SerializeField] private bool _isInitInStart;
        [SerializeField] private bool _initState = true;

        [SerializeField] private ImageChangeSprites<bool> _imageChange;

        private void Awake()
        {
            _hidenButton?.onClick.AddListener(() => this.SetInvertedState());
        }

        private void Start()
        {
            if (_isInitInStart)
                Init();
        }

        private void Init()
        {
            SetState(_initState, true);
        }

        public bool GetState()
        {
            return _hidenInputStates.GetState();
        }

        public void SetState(bool setState, bool isForcibly = false)
        {
            _imageChange.SetState(setState, isForcibly);
            _hidenInputStates.SetState(setState, isForcibly);
        }
    }
}