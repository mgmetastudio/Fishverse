using UnityEngine;
using Mirror;
using System.Collections;

public class FishEntity : NetworkBehaviour
{

    [SyncVar] public int fishUniqueId;

    private static FishScriptable[] _fishScriptables;
    private FishScriptable _scriptable;
    public FishAIController controller;
    public GameObject FishCaughtMessage;
    public GameObject FishModel;

    private void Awake()
    {
        if (_fishScriptables == null)
        {
            _fishScriptables = Resources.LoadAll<FishScriptable>("Fish");
        }
    }

    private void Start()
    {
        for (int i = 0; i < _fishScriptables.Length; i++)
        {
            if (_fishScriptables[i].uniqueId == fishUniqueId)
            {
                _scriptable = _fishScriptables[i];
                break;
            }
        }
        if (_scriptable == null)
        {
            throw new UnityException("_scriptable == null");
        }

        FishModel = Instantiate(_scriptable.modelPrefab, transform);

        if (isServer)
        {
            controller = gameObject.AddComponent<FishAIController>();
            controller.Setup(_scriptable);
        }

        StartCoroutine(BiteLoop());
    }

#pragma warning disable IDE0051
    private void HookedChanged(FishingFloat oldValue, FishingFloat newValue)
    {
#pragma warning restore IDE0051
        if (oldValue != null)
        {
            oldValue.GetComponent<NetworkTransform>().clientAuthority = false;
            oldValue.transform.SetParent(null);
            oldValue._collider.enabled = true;
            oldValue._interactor.enabled = false;
            oldValue._rb.isKinematic = true;
            oldValue._rb.useGravity = false;
        }
        if (newValue != null)
        {
            newValue.GetComponent<NetworkTransform>().clientAuthority = false;
            newValue.transform.SetParent(transform);
            newValue._collider.enabled = false;
            newValue._interactor.enabled = false;
            newValue._rb.isKinematic = true;
            newValue._rb.useGravity = false;
        }
    }

    public Rigidbody rb;
    [SyncVar(hook = "HookedChanged")] private FishingFloat _hookedTo;
    private void Bite(FishingFloat _targetFloat)
    {
        if (_hookedTo == null)
        {
            _targetFloat.GetComponent<NetworkTransform>().clientAuthority = false;
            _targetFloat._collider.enabled = false;
            _targetFloat._interactor.enabled = false;
            _targetFloat._rb.isKinematic = true;
            _targetFloat._rb.useGravity = false;
            _targetFloat.transform.SetParent(transform);
            Vector3 NewFloatPosition = new Vector3(transform.position.x, transform.position.y + 0.0f, transform.position.z);
            _targetFloat.transform.position = NewFloatPosition;
            _hookedTo = _targetFloat;
            _hookedTo.fish = this;
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            controller.stamina = 1f;
            controller.doNotUpdateTarget = true;
            controller.fearfulness = 1f;
            _hookedTo._owner.GetComponent<PlayerFishing>().SpawnedFloatSimulation.GetComponent<FloatSimulation>().SimulateBite();
        }
    }

    private IEnumerator BiteLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (_hookedTo == null && controller != null)
            {
                if (controller.target != null && Random.Range(.0f, 1f) > .4f)
                {
                    if (controller.target.gameObject.TryGetComponent<FishingFloat>(out FishingFloat fishingFloat))
                        Bite(fishingFloat);

                    // Bite(controller.target.gameObject.GetComponent<FishingFloat>());
                }
            }
        }
    }

    private void Update()
    {
        if (isServer)
        {
            if (_hookedTo == null && rb != null)
            {
                Destroy(rb);
                rb = null;
                controller.doNotUpdateTarget = false;
                controller.fearfulness = .0f;
            }

            if (_hookedTo != null)
            {
                Vector3 NewFishModelPosition = new Vector3(_hookedTo.Hook.transform.position.x, _hookedTo.Hook.transform.position.y - 0.15f, _hookedTo.Hook.transform.position.z);
                FishModel.transform.position = NewFishModelPosition;
                if (controller.stamina < 0.7f)
                {
                    FishModel.transform.LookAt(_hookedTo._owner._rodEndPoint.position);
                    /*Quaternion NewFishModelRotation = new Quaternion(FishModel.transform.rotation.x, 180, 180, 0);
                    FishModel.transform.rotation = Quaternion.Lerp(FishModel.transform.rotation, NewFishModelRotation, 5f);*/
                }
                if (controller.stamina > 0.7f)
                {
                    FishModel.transform.rotation = new Quaternion(0, 0, 0, 0);
                }
                if (Vector3.Distance(transform.position, _hookedTo._owner._rodEndPoint.position) < 2.3f)
                {
                    var anim = _hookedTo._owner.GetComponent<PlayerAnimator>();
                    Inventory inv = _hookedTo._owner.GetComponent<Inventory>();

                    FishAIController ai = this.GetComponent<FishAIController>();

                    Debug.Log("Fish with ID " + this.GetComponent<FishAIController>()._scriptable.uniqueId + " caught!");

                    anim.FishCatch();
                    inv.HoldCaughtFish(ai._scriptable.uniqueId);
                    inv.AddFishItem(ai._scriptable.uniqueId, ai._scriptable.FishName, ai._scriptable.FishLength, "Weight: " + ai._scriptable.FishWeight, ai._scriptable.FishRetailValue, ai._scriptable.FishSprite);
                    RpcHoldCaughtFish(ai._scriptable.uniqueId);
                    Instantiate(FishCaughtMessage).GetComponent<FishCaughtMessage>().Message.text = "<color=orange>" + inv.PlayerName + "</color>" + " caught a " + "<color=green>" + ai._scriptable.FishWeight + "</color>" + " " + "<color=green>" + ai._scriptable.FishName + "</color>";
                    _hookedTo._owner.GetComponent<PlayerFishing>().DestroyFloatSimulation();
                    NetworkServer.Destroy(gameObject);
                }
            }
        }
    }

    [ClientRpc]
    public void RpcHoldCaughtFish(int uniqueId)
    {
        var inv = _hookedTo._owner.GetComponent<Inventory>();

        inv.HoldCaughtFish(uniqueId);
        _hookedTo._owner.GetComponent<Animator>().Play(inv.FishHolderAnimationName);

        GameObject SpawnedInventoryFish;

        SpawnedInventoryFish = Instantiate(inv.InventoryFishPrefab);
        SpawnedInventoryFish.transform.SetParent(inv.Content);

        var fish = SpawnedInventoryFish.GetComponent<InventoryFish>();
        fish.FishName.text = _scriptable.FishName;
        fish.FishLength.text = _scriptable.FishLength;
        fish.FishWeight.text = "Weight: " + _scriptable.FishWeight;
        fish.FishRetailValue.text = _scriptable.FishRetailValue;
        fish.FishImage.sprite = _scriptable.FishSprite;

        SpawnedInventoryFish = null;

        inv.CheckForItems();
    }

    private void OnGUI()
    {
        if (isServer)
        {
            if (_hookedTo != null)
            {
                GUI.Label(new Rect(1820, 0, 100, 100), "Server Fish Stamina: " + controller.stamina + "\n hooked!");
            }
        }
    }
}
