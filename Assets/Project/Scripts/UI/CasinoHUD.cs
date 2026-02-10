using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Necessário para ler o teclado direto
using TMPro;
using MoreMountains.Feedbacks; // Para o Camera Shake

public class CasinoHUD : MonoBehaviour
{
    private enum CasinoState { Hidden, Entering, WaitingForSpin, Spinning, ResultReveal, WaitingForDecision, Exiting }

    [Header("Sistemas")]
    [SerializeField] private CasinoManager casinoManager;
    [SerializeField] private PlayerUpgrades playerUpgrades;
    [SerializeField] private LevelSystem levelSystem;

    [Header("UI - Estrutura")]
    [SerializeField] private CanvasGroup blackPanel; // O fundo preto (CanvasGroup para controlar Alpha)
    [SerializeField] private RectTransform slotMachineContainer; // A máquina inteira que vai cair

    [Header("UI - Elementos da Máquina")]
    [SerializeField] private Animator reelsAnimator; // A animação dos olhos/ícones girando
    [SerializeField] private Image resultIconImage;  // O ícone que aparece no final
    [SerializeField] private GameObject resultTextPanel; // O painel com Nome/Descrição (começa escondido)
    [SerializeField] private TextMeshProUGUI resultNameText;
    [SerializeField] private TextMeshProUGUI resultDescText;

    [Header("Efeitos (Juice)")]
    [SerializeField] private ParticleSystem winParticles; // Confetes quando ganha
    [SerializeField] private MMF_Player cameraShakeFeedback; // Treme a tela quando a máquina bate no chão

    // Estado interno
    private CasinoState currentState = CasinoState.Hidden;
    private UpgradeBaseSO draftedCard;
    private float hiddenYPosition = 1200f; // Posição Y fora da tela (em cima)
    private float centerYPosition = 0f;    // Posição Y no centro

    private void Start()
    {
        // Setup Inicial: Esconde tudo
        blackPanel.alpha = 0;
        blackPanel.gameObject.SetActive(false);

        // Coloca a máquina lá em cima, fora da tela
        slotMachineContainer.anchoredPosition = new Vector2(0, hiddenYPosition);

        resultTextPanel.SetActive(false);
    }

    private void Update()
    {
        // 1. Gatilho para abrir (Só se estiver escondido e tiver fichas)
        if (currentState == CasinoState.Hidden)
        {
            if (Input.GetKeyDown(KeyCode.Tab) && levelSystem.HasPendingUpgrades())
            {
                StartCoroutine(Sequence_Intro());
            }
        }

        // 2. Gatilho para Girar (Input J)
        if (currentState == CasinoState.WaitingForSpin)
        {
            if (Keyboard.current.jKey.wasPressedThisFrame || Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                StartCoroutine(Sequence_Spin());
            }
        }

        // 3. Gatilho para Decidir (J = Aceitar, K = Recusar)
        if (currentState == CasinoState.WaitingForDecision)
        {
            if (Keyboard.current.jKey.wasPressedThisFrame || Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                OnConfirmDecision();
            }
            else if (Keyboard.current.kKey.wasPressedThisFrame || Gamepad.current.buttonEast.wasPressedThisFrame)
            {
                OnDiscardDecision();
            }
        }
    }

    // --- FASE 1: ENTRADA DRAMÁTICA ---
    private IEnumerator Sequence_Intro()
    {
        currentState = CasinoState.Entering;
        Time.timeScale = 0; // Pausa o jogo

        // A. Fundo Preto aparece suave (Fade In)
        blackPanel.gameObject.SetActive(true);
        LeanTween.alphaCanvas(blackPanel, 0.8f, 0.3f).setIgnoreTimeScale(true);

        // B. Máquina Cai do Céu (Brusco!)
        // easeOutBounce dá aquele efeito de bater e quicar
        LeanTween.moveY(slotMachineContainer, centerYPosition, 0.6f)
            .setEase(LeanTweenType.easeOutBounce)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                // C. Treme a tela quando bate (se tiver Feel configurado)
                if (cameraShakeFeedback) cameraShakeFeedback.PlayFeedbacks();
            });

        yield return new WaitForSecondsRealtime(0.6f);

        currentState = CasinoState.WaitingForSpin;
        // Aqui podes piscar um texto "APERTE J PARA GIRAR"
    }

    // --- FASE 2: O GIRO E O REVEAL ---
    private IEnumerator Sequence_Spin()
    {
        currentState = CasinoState.Spinning;

        // A. Começa animação visual
        if (reelsAnimator) reelsAnimator.SetTrigger("Spin");

        // B. Sorteia a carta no backend
        List<UpgradeBaseSO> rolled = casinoManager.RollOptions(1);
        if (rolled.Count > 0) draftedCard = rolled[0];
        else { CloseCasino(); yield break; } // Erro de deck vazio

        // C. Tensão (Tempo girando)
        float spinTime = 1.5f; // Ajusta conforme tua animação
        yield return new WaitForSecondsRealtime(spinTime);

        // D. Para Animação e Mostra Ícone
        if (reelsAnimator) reelsAnimator.SetTrigger("Stop");

        // Troca o sprite da roleta pelo ícone da habilidade
        resultIconImage.sprite = draftedCard.icon;

        // Efeito de explosão de partículas
        if (winParticles) winParticles.Play();
        if (cameraShakeFeedback) cameraShakeFeedback.PlayFeedbacks(); // Treme de novo no reveal

        currentState = CasinoState.ResultReveal;

        // E. Pequeno delay para ler o ícone antes de mostrar o texto
        yield return new WaitForSecondsRealtime(0.5f);

        // F. Mostra os textos (Painel de baixo)
        resultNameText.text = draftedCard.upgradeName;
        resultDescText.text = draftedCard.description;
        resultTextPanel.SetActive(true);

        // Animaçãozinha de entrada do texto (Scale Up)
        resultTextPanel.transform.localScale = Vector3.zero;
        LeanTween.scale(resultTextPanel, Vector3.one, 0.3f)
            .setEase(LeanTweenType.easeOutBack)
            .setIgnoreTimeScale(true);

        currentState = CasinoState.WaitingForDecision;
    }

    // --- FASE 3: DECISÃO ---
    private void OnConfirmDecision()
    {
        bool success = playerUpgrades.TryEquipUpgrade(draftedCard);

        if (success)
        {
            levelSystem.ConsumeUpgradeToken();
            casinoManager.ConfirmChoice(draftedCard);
            StartCoroutine(Sequence_Outro());
        }
        else
        {
            // TODO: Se inventário cheio, tremer a UI ou tocar som de erro
            Debug.Log("Inventário Cheio - Lógica de troca pendente");
            StartCoroutine(Sequence_Outro()); // Fecha forçado por enquanto
        }
    }

    private void OnDiscardDecision()
    {
        casinoManager.ReturnToDeck(draftedCard);
        levelSystem.ConsumeUpgradeToken(); // Gastou a ficha
        StartCoroutine(Sequence_Outro());
    }

    // --- FASE 4: SAÍDA ---
    private IEnumerator Sequence_Outro()
    {
        currentState = CasinoState.Exiting;

        // A. Esconde texto
        resultTextPanel.SetActive(false);

        // B. Máquina sobe rápido (Reverse Drop)
        LeanTween.moveY(slotMachineContainer, hiddenYPosition, 0.4f)
            .setEase(LeanTweenType.easeInBack)
            .setIgnoreTimeScale(true);

        // C. Fade out do fundo
        LeanTween.alphaCanvas(blackPanel, 0f, 0.4f)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                CloseCasino();
            });

        yield return null;
    }

    private void CloseCasino()
    {
        blackPanel.gameObject.SetActive(false);
        currentState = CasinoState.Hidden;
        Time.timeScale = 1f; // VOLTA O JOGO
    }
}