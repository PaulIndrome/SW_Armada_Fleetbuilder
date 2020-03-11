using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using DG.Tweening;

public class DeckFileList : MonoBehaviour
{

    [Header("Scene references")]
    [SerializeField] private Canvas deckFileListCanvas;
    [SerializeField] private RectTransform contentTransform;
    [SerializeField] private TMP_InputField deckFileInputField;
    [SerializeField] private DeckContentControl deckContentControl;
    [SerializeField] private CanvasGroup inputNeededCanvasGroup;


    [Header("Asset references")]
    [SerializeField] private DeckFileEntry deckFileEntryPrefab;

    [Header("Set via script")]
    [SerializeField] private DeckFileEntry currentSelectedEntry;

    [Header("Constants")]
    [SerializeField] private float inputBlinkDuration = 1f;
    [SerializeField] private int inputBlinkRepeats = 4;
    [SerializeField] public static string DECKFILEPATH;
    
    public Dictionary<int, DeckFileEntry> deckFileEntries;
    public string[] fullFilePaths;



    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        DeckFileList.DECKFILEPATH = $"{Application.persistentDataPath}/LocalDecks/";

        if(!deckFileEntryPrefab){
            this.enabled = false;
            deckFileListCanvas.enabled = false;
            gameObject.SetActive(false);
        }

        if(deckFileEntries == null){
            deckFileEntries = new Dictionary<int, DeckFileEntry>();
        }

        
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        LoadDeckFileList();
        DeckFileEntry.OnDeckFileSelected += DeckFileSelected;
    }

    void LoadDeckFileList(){
        fullFilePaths = System.IO.Directory.GetFiles(DECKFILEPATH);
        for(int i = 0; i < fullFilePaths.Length; i++){
            Debug.Log(Path.GetFileName(fullFilePaths[i]));
            LoadDeckFile(fullFilePaths[i]);
        }
    }

    private void LoadDeckFile(string fullFilePath){
        SerializableDeck sDeck = JsonConvert.DeserializeObject<SerializableDeck>(File.ReadAllText(fullFilePath));
        if(sDeck == null) return;

        DeckFileEntry newListEntry;
        string fileName = Path.GetFileName(fullFilePath);
        if(!deckFileEntries.TryGetValue(fileName.GetHashCode(), out newListEntry)){
            newListEntry = Instantiate(deckFileEntryPrefab, Vector3.zero, Quaternion.identity, contentTransform);
            deckFileEntries.Add(fileName.GetHashCode(), newListEntry);
        }
        
        newListEntry.name = $"{newListEntry.transform.GetSiblingIndex().ToString("000")}_{sDeck.deckName}";
        newListEntry.SetupDeckFileEntry(sDeck, fileName);
    }

    public void DeckFileSelected(DeckFileEntry entry, SerializableDeck sDeck){
        if(entry == null || sDeck == null){
            currentSelectedEntry = null;
            deckFileInputField.text = "";
            return;
        }
        currentSelectedEntry = entry;
        deckFileInputField.text = entry.DeckNameText;
    }

    public void LoadDeckFile(){
        if(currentSelectedEntry == null) return;
        Debug.Log("Attempting to load deck " + currentSelectedEntry.FileName);
        deckContentControl.LoadDeckFromFile(currentSelectedEntry.SerializableDeck);
        ModalWindowHandler.ShowModalWindow(null, "Deck file loaded", $"Deck \"{deckContentControl.CurrentDeck.DeckName}\" loaded", ModalResult.Ok);
        deckFileListCanvas.enabled = false;
    }

    public void NewDeck(){
        if(deckFileInputField.text.Length < 1){
            inputNeededCanvasGroup.alpha = 1;
            StartCoroutine(FlashCanvasGroup(inputNeededCanvasGroup, inputBlinkRepeats, inputBlinkDuration));
            return;
        }
        ModalWindowHandler.ShowModalWindow(ModalNewDeck, "Choose a faction", "Which faction are you building a deck for?", ModalResult.Empire, ModalResult.Rebellion, ModalResult.Cancel);
    }

    public void ModalNewDeck(ModalResult result){
        Debug.Log("Modal new Deck result: " + result);
        switch(result){
            case ModalResult.Empire:
                deckContentControl.NewDeck(deckFileInputField.text, 400, Faction.Empire);
                break;
            case ModalResult.Rebellion:
                deckContentControl.NewDeck(deckFileInputField.text, 400, Faction.Rebellion);
                break;
            case ModalResult.Cancel:
                break;
        }
        Debug.Log("About to set gameobject inactive");
        deckFileListCanvas.enabled = false;
    }


    public void SaveDeckFile(){
        string path = DeserializeCardsFromJSON.SerializeDeck(deckContentControl.CurrentDeck, deckFileInputField.text.Trim());
        LoadDeckFile(path);
    }

    public void DeleteDeckFile(){
        if(currentSelectedEntry == null) return;
        ModalWindowHandler.ShowModalWindow(ModalDeleteDeckFile, "Delete deck file", $"Are you sure you want to delete \"{currentSelectedEntry.DeckNameText}\"?", ModalResult.Yes, ModalResult.No);
    }

    private void ModalDeleteDeckFile(ModalResult result){
        switch(result){
            case ModalResult.Yes:
                currentSelectedEntry.gameObject.SetActive(false);
                
                Debug.Log(DECKFILEPATH + currentSelectedEntry.FileName);
                File.Delete(DECKFILEPATH + currentSelectedEntry.FileName);

                deckFileEntries.Remove(currentSelectedEntry.FileName.GetHashCode());
                Destroy(currentSelectedEntry.gameObject);
                currentSelectedEntry = null;
                break;
            case ModalResult.No:
                break;
        }
    }

    IEnumerator FlashCanvasGroup(CanvasGroup group, int repeats = 4, float duration = 1f){
        // float startTime = Time.time;

        float startAlpha = group.alpha;

        group.alpha = 0;

        float blinkTime = duration / (repeats * 2);
        float blinkSpeed = 1 / blinkTime;

        for(float t = 0; t < duration; t += Time.deltaTime){
            group.alpha = 1 - Mathf.PingPong(t * blinkSpeed, 1);
            yield return null;
        }
        // Debug.Log(group.alpha);

        // Debug.Log("Duration: " + (Time.time - startTime));

        group.alpha = startAlpha;
        yield return null;
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        DeckFileSelected(null, null);
        inputNeededCanvasGroup.alpha = 0;
        DeckFileEntry.OnDeckFileSelected += DeckFileSelected;
    }

    [ContextMenu("Debug DECKFILEPATH")]
    public void DebugDECKFILEPATH(){
        Debug.Log(DeserializeCardsFromJSON.DECKFILEPATH);
        Debug.Log(Application.persistentDataPath);
    }
}
