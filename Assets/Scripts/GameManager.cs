using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private const int BET_INCREMENT = 10;

    [Header("UI Elements")]
    [SerializeField] private Button _betButton;
    [SerializeField] private Toggle _betOnGreenButton;
    [SerializeField] private Toggle _betOnRedButton;
    [SerializeField] private Button _increseBetButton;
    [SerializeField] private Button _decreseBetButton;
    [SerializeField] private TextMeshProUGUI _currentAmountText;
    [SerializeField] private Animator _currentAmountAnimator;
    [SerializeField] private TextMeshProUGUI _currentBetText;
    [SerializeField] private LayoutElement _chipsLayoutElement;
    [Tooltip("In UI units")]
    [SerializeField] private float _chipSize = 5;

    [Header("3D Elements")]
    [SerializeField] private Vector3 _cubeSpawnPoint;
    [SerializeField] private Transform _cube;
    [SerializeField] private Material _cubeMaterial;

    [SerializeField] private Animator FloorAnimator;
    [SerializeField] private Animator LightAnimator;

    private readonly System.Random _random = new System.Random();

    private bool _currentCubeIsGreen;
    private bool _currentBetIsGreen;
    private int _currentBet;
    private int _currentAmount;

    private void Awake()
    {
        if (_increseBetButton != null)
            _increseBetButton.onClick.AddListener(IncreseBet);
        if (_decreseBetButton != null)
            _decreseBetButton.onClick.AddListener(DescreseBet);
        if (_betButton != null)
            _betButton.onClick.AddListener(FinishBet);
        if (_betOnGreenButton != null)
            _betOnGreenButton.onValueChanged.AddListener(BetOnGreen);
        if (_betOnRedButton != null)
            _betOnRedButton.onValueChanged.AddListener(BetOnRed);
    }

    private void Start()
    {
        StartRound();
    }

    private void StartRound()
    {
        StopAllCoroutines();
        StartCoroutine(StartRoundCo());
    }

    private IEnumerator StartRoundCo()
    {
        if (_currentAmount == 0)
            SetCurrentAmount(100);
        IncreseBet();
        SetLight(false);

        yield return new WaitForSeconds(1);

        _currentCubeIsGreen = _random.Next(0, 2) == 0;

        SetCubeColor();
        SpawnCube();
        SetButtonsInteractable(true);
        ResetToggle();
    }

    private IEnumerator FinishRoundCo()
    {
        SetButtonsInteractable(false);
        SetLight(true);

        yield return new WaitForSeconds(2);

        if (_currentBetIsGreen == _currentCubeIsGreen)
        {
            Debug.Log("Win");
            SetCurrentAmount(_currentAmount + _currentBet);
            if (_currentAmountAnimator != null)
                _currentAmountAnimator.SetTrigger("Win");
        }
        else
        {
            Debug.Log("Lose");
            SetCurrentAmount(_currentAmount - _currentBet);
            if (_currentAmountAnimator != null)
                _currentAmountAnimator.SetTrigger("Lose");
        }
        SetCurrentBet(0);

        yield return new WaitForSeconds(2);

        if (FloorAnimator != null)
            FloorAnimator.SetTrigger("Flip");

        yield return new WaitForSeconds(2);

        StartRound();
    }

    private void SetCubeColor()
    {
        if (_cubeMaterial != null)
            _cubeMaterial.color = _currentCubeIsGreen ? Color.green : Color.red;
    }

    private void SpawnCube()
    {
        if (_cube != null)
            _cube.position = _cubeSpawnPoint;
    }

    [ContextMenu("Increse Bet")]
    private void IncreseBet()
    {
        if (_currentBet <= _currentAmount - BET_INCREMENT)
            SetCurrentBet(_currentBet + BET_INCREMENT);
    }

    [ContextMenu("Descrese Bet")]
    private void DescreseBet()
    {
        if (_currentBet > BET_INCREMENT)
            SetCurrentBet(_currentBet - BET_INCREMENT);
    }

    private void BetOnRed(bool value)
    {
        if (value)
            BetOnRed();
        SetBetButtonInteractable(value);
    }

    [ContextMenu("Bet On Red")]
    private void BetOnRed()
    {
        _currentBetIsGreen = false;
    }


    private void BetOnGreen(bool value)
    {
        if (value)
            BetOnGreen();
        SetBetButtonInteractable(value);
    }

    [ContextMenu("Bet On Green")]
    private void BetOnGreen()
    {
        _currentBetIsGreen = true;
    }

    [ContextMenu("Finish Bet")]
    private void FinishBet()
    {
        StopAllCoroutines();
        StartCoroutine(FinishRoundCo());
    }

    private void SetBetButtonInteractable(bool value)
    {
        if (_betButton != null)
            _betButton.interactable = value;
    }

    private void SetCurrentBet(int value)
    {
        _currentBet = value;
        if (_currentBetText != null)
            _currentBetText.text = $"Bet: {value}";
    }

    private void SetCurrentAmount(int value)
    {
        _currentAmount = value;
        if (_currentAmountText != null)
            _currentAmountText.text = $"Chips: {value}";
        if (_chipsLayoutElement != null)
            _chipsLayoutElement.minHeight = value / 2;
    }

    private void SetLight(bool value)
    {
        if (LightAnimator != null)
            LightAnimator.SetBool("On", value);
    }

    private void SetButtonsInteractable(bool value)
    {
        if (_increseBetButton != null)
            _increseBetButton.interactable = value;
        if (_decreseBetButton != null)
            _decreseBetButton.interactable = value;
        if (_betOnGreenButton != null)
            _betOnGreenButton.interactable = value;
        if (_betOnRedButton != null)
            _betOnRedButton.interactable = value;
        if (_betButton != null)
            _betButton.interactable = value;
    }

    private void ResetToggle()
    {
        if (_betOnGreenButton != null)
            _betOnGreenButton.isOn = false;
        if (_betOnRedButton != null)
            _betOnRedButton.isOn = false;
    }
}
