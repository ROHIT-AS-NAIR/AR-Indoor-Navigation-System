using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class CanvasButtonScript : MonoBehaviour
{

    public GameObject markerPrefab, roomButtonPrefab;
    public GameObject actionBar;
    public GameObject searchPanel, mapPanel, errorPanel; //panel
    private GameObject hambergerButton, mapButton, searchButton, backButton, clearButton; //button
    private GameObject searchInputField, appName; //InputFields + text
    private Text appNameText;
    public Text dbtext;

    private GameObject searchHelpText, searchList, viewPort, scrollbar, searchContent, roomDataPanel, roomDataDialog;
    private GameObject roomNameTitle, roomMapImage, roomDesData, roomNavigateButton;
    private GameObject mapImage, rightButton, leftButton, navline, userDot;
    private GameObject errorDialog, errorHeadText;


    private GameObject showingFloor;
    private List<GameObject> searchShowList;

    private enum Page
    {
        Main,
        Search,
        Map
    }
    private Page page = Page.Main;

    //private CanvasResolutionScript canvasResolutionScript;
    private MapControlScript mapControl;
    private BuildingData building;
    private StateDisplayController stateDisplay;
    private ToastMessageScript toastMessageScript;

    private bool canQuitApp = true;
    private bool isErrorCantReadFile = false;

    // Use this for initialization
    void Start()
    {
        //StartCanvas();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (page)
            {
                case Page.Search: OnCloseSearch(); break;
                case Page.Map: OnCloseMap(); break;
                case Page.Main:
                    if (canQuitApp)
                    {
                        Application.Quit();
                    }
                    else
                    {
                        canQuitApp = true;
                        //showToast
                    }
                    break;
                default: OnCloseSearch(); break;
            }
        }
    }

    public void StartCanvas()
    {
        Screen.fullScreen = false;

        toastMessageScript = gameObject.GetComponent<ToastMessageScript>();
        stateDisplay = gameObject.GetComponent<StateDisplayController>();


        hambergerButton = actionBar.gameObject.transform.Find("HambergerButton").gameObject;
        mapButton = actionBar.gameObject.transform.Find("MapButton").gameObject;
        searchButton = actionBar.gameObject.transform.Find("SearchButton").gameObject;
        appName = actionBar.gameObject.transform.Find("AppName").gameObject;
        appNameText = appName.GetComponent<Text>();
        dbtext = gameObject.transform.Find("DebugText").GetComponent<Text>();

        backButton = actionBar.gameObject.transform.Find("BackButton").gameObject;
        searchInputField = actionBar.gameObject.transform.Find("SearchInputField").gameObject;
        clearButton = actionBar.gameObject.transform.Find("ClearSearchButton").gameObject;

        /* search */
        searchHelpText = searchPanel.transform.Find("HelpText").gameObject;
        searchList = searchPanel.transform.Find("Scroll View").gameObject;
        viewPort = searchList.gameObject.transform.Find("Viewport").gameObject;
        scrollbar = searchList.gameObject.transform.Find("Scrollbar Vertical").gameObject;
        searchContent = viewPort.gameObject.transform.Find("Content").gameObject;
        roomDataPanel = searchPanel.transform.Find("RoomDataPanel").gameObject;
        roomDataDialog = roomDataPanel.transform.Find("RoomDataDialog").gameObject;
        roomNameTitle = roomDataDialog.transform.Find("RoomNameTitle").gameObject;
        roomMapImage = roomDataDialog.transform.Find("RoomMapImage").gameObject;
        roomDesData = roomDataDialog.transform.Find("DescriptionScrollView").GetChild(0).GetChild(0).Find("RoomData").gameObject;
        roomNavigateButton = roomDataDialog.transform.Find("ButtonViewport").GetChild(0).Find("NavButtonText").gameObject;

        /* map */
        mapImage = mapPanel.transform.Find("MapScrollViewArea").gameObject;
        rightButton = mapPanel.transform.Find("RightButton").gameObject;
        leftButton = mapPanel.transform.Find("LeftButton").gameObject;
        mapControl = mapImage.transform.Find("Mask/MapImage").gameObject.GetComponent<MapControlScript>();

        /* error */
        errorDialog = errorPanel.transform.Find("ErrorDialog").gameObject;
        errorHeadText = errorDialog.transform.Find("HeadText").gameObject;

        backButton.SetActive(false);
        searchInputField.SetActive(false);
        clearButton.SetActive(false);
    }

    /* start canvas in readable data mode */
    public void StartNormalStateAppCanvas()
    {
        try
        {
            building = GameObject.FindWithTag("Building").GetComponent<BuildingData>();
            Debug.Log(building.name);
            showingFloor = building.floorList[0];
            searchShowList = new List<GameObject>();
            stateDisplay.AddSound(SoundManager.SoundType.InitApp, 0);
            stateDisplay.PlaySoundQueue();
            isErrorCantReadFile = false;
            stateDisplay.ShowToastMessage("ส่องกล้องไปยังจุดต่างๆ เช่น ป้ายบอกทาง เลขห้อง เพื่อเริ่มต้นระบุตำแหน่งของคุณ", 5);
        }
        catch (System.Exception e)
        {
            dbtext.text = Random.Range(10, 99) + ": startnormalstate Error " + e.Message + "\n" + e.StackTrace;
        }

    }


    public void OnBackButton()
    {
        switch (page)
        {
            case Page.Search: OnCloseSearch(); break;
            case Page.Map: OnCloseMap(); break;
            default: OnCloseSearch(); break;
        }
    }

    public void DummyOnclickFunction()
    {
        dbtext.text = Random.Range(10, 99) + ": Dummy";
        Debug.Log("Dummy");
    }

    #region Open New Page

    public void OnOpenSearch()
    {
        page = Page.Search;
        mapPanel.SetActive(false);
        searchPanel.SetActive(true);
        hambergerButton.SetActive(false);
        mapButton.SetActive(false);
        searchButton.SetActive(false);
        appName.SetActive(false);
        roomDataPanel.SetActive(false);

        backButton.SetActive(true);
        searchInputField.SetActive(true);
        clearButton.SetActive(true);

        OnTyping();
    }

    public void OnCloseSearch()
    {
        page = Page.Main;
        mapPanel.SetActive(false);
        searchPanel.SetActive(false);
        hambergerButton.SetActive(true);
        mapButton.SetActive(true);
        searchButton.SetActive(true);
        appName.SetActive(true);
        roomDataPanel.SetActive(false);

        backButton.SetActive(false);
        searchInputField.SetActive(false);
        clearButton.SetActive(false);
    }

    public void OnOpenMap()
    {
        page = Page.Map;
        mapPanel.SetActive(true);
        searchPanel.SetActive(false);
        hambergerButton.SetActive(false);
        mapButton.SetActive(false);
        searchButton.SetActive(false);
        appName.SetActive(true);
        roomDataPanel.SetActive(false);

        backButton.SetActive(true);
        searchInputField.SetActive(false);
        clearButton.SetActive(false);

        if (MainController.instance.beginPoint != null && !isErrorCantReadFile)
        {
            showingFloor = MainController.instance.beginPoint.GetComponent<NodeData>().GetParentObjectData().GetParentFloorObject();
        }
        mapControl.UpdateMap(showingFloor);
    }

    public void OnCloseMap()
    {
        page = Page.Main;
        mapPanel.SetActive(false);
        searchPanel.SetActive(false);
        hambergerButton.SetActive(true);
        mapButton.SetActive(true);
        searchButton.SetActive(true);
        appName.SetActive(true);
        roomDataPanel.SetActive(false);

        backButton.SetActive(false);
        searchInputField.SetActive(false);
        clearButton.SetActive(false);
    }

    public void OnOpenRoomDialoge(GameObject roomObj, bool isDestination)
    /* show room dialoge box data with map */
    {
        NodeData nddt = roomObj.GetComponent<NodeData>();
        roomDataPanel.SetActive(true);
        roomNameTitle.GetComponent<Text>().text = "ข้อมูลห้อง: " + nddt.GetParentObjectData().roomName;
        Material floorMaterial = nddt.GetParentObjectData().GetParentFloorObject().transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().materials[0];
        roomMapImage.GetComponent<Image>().material = floorMaterial;
        roomDesData.GetComponent<Text>().text = nddt.GetParentObjectData().roomDescription;
        if (!isDestination)
        {
            try
            {

roomNavigateButton.GetComponent<Text>().text = "Navigate.";
            roomNavigateButton.GetComponent<Text>().color = new Color32(64, 0, 192, 255);
            roomNavigateButton.GetComponent<Button>().onClick.RemoveAllListeners();
            roomNavigateButton.GetComponent<Button>().onClick.AddListener(delegate { SelectToNavigate(roomObj); });
            if (MainController.instance.beginPoint != null)
            {
                if (nddt.GetParentObjectData().roomName ==
                    MainController.instance.beginPoint.GetComponent<NodeData>().GetParentObjectData().roomName)
                {
                    roomNavigateButton.GetComponent<Text>().text = "Here is Current Room";
                    roomNavigateButton.GetComponent<Text>().color = new Color32(150, 130, 180, 255);
                    roomNavigateButton.GetComponent<Button>().onClick.RemoveAllListeners();
                    roomNavigateButton.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        stateDisplay.ShowToastMessage("จุดล่าสุดของคุณคือจุดที่คุณเลือกอยู่ กรุณาเลือกปลายทางที่อื่น", 1);
                    });
                }
            }

            }
            catch (System.Exception e)
            {
                dbtext.text = Random.Range(10, 99) + ": OnOpenRoomDialoge Error " + e.Message+ "\n" + e.StackTrace;
            }
            
        }
        else
        {
            roomNavigateButton.GetComponent<Text>().text = "Cancle Navigate";
            roomNavigateButton.GetComponent<Text>().color = new Color32(64, 0, 192, 255);
            roomNavigateButton.GetComponent<Button>().onClick.RemoveAllListeners();
            roomNavigateButton.GetComponent<Button>().onClick.AddListener(ClearPoint);
        }

        //mark point and navigate draw line to destination
        GameObject destDot = roomMapImage.transform.Find("MarkDot/DestMarker").gameObject;
        destDot.GetComponent<RectTransform>().anchoredPosition = new Vector2(
            nddt.position.x * (roomMapImage.GetComponent<RectTransform>().sizeDelta.x / 1000),
            nddt.position.z * (roomMapImage.GetComponent<RectTransform>().sizeDelta.y / 1000)
        );

    }

    public void OnCloseRoomDialog()
    {
        roomDataPanel.SetActive(false);
    }

    private void SelectToNavigate(GameObject roomObj)
    {
        MainController.instance.SetDestinationPoint(roomObj);
        OnCloseRoomDialog();
        OnCloseSearch();
    }

    private void ClearPoint()
    {
        MainController.instance.ClearDestinationPoint();
        OnCloseRoomDialog();
        OnCloseSearch();
    }

    #endregion

    #region Search Action

    /* create room list from word typed, and send to show room button from list */
    public void OnTyping()
    {
        string typingWord = searchInputField.GetComponent<InputField>().text;
        typingWord = typingWord.ToLower();
        searchShowList.Clear();
        List<GameObject> searchindescription = new List<GameObject>();

        string fkrid = "";
        if (MainController.instance.beginPoint != null)
        {
            fkrid = MainController.instance.beginPoint.GetComponent<NodeData>().fkRoomID;
        }

        foreach (GameObject floor in building.floorList)
        {
            foreach (GameObject nodet in floor.GetComponent<FloorData>().GetNodesList())
            {
                NodeData nodeData = nodet.GetComponent<NodeData>();
                if (typingWord == ""
                    && !IsDuplicateShowingRoom(searchShowList, nodeData.GetParentObjectData().roomName)
                    && (nodeData.GetParentObjectData().showInSearch || nodeData.fkRoomID == fkrid))
                {
                    searchShowList.Add(nodet);
                }
                else if (nodeData.GetParentObjectData().roomName.ToLower().Contains(typingWord)
                    && !IsDuplicateShowingRoom(searchShowList, nodeData.GetParentObjectData().roomName)
                    && (nodeData.GetParentObjectData().showInSearch || nodeData.fkRoomID == fkrid))
                {
                    searchShowList.Add(nodet);
                }
                else if (nodeData.GetParentObjectData().roomDescription.ToLower().Contains(typingWord)
                    && !IsDuplicateShowingRoom(searchShowList, nodeData.GetParentObjectData().roomName)
                    && (nodeData.GetParentObjectData().showInSearch || nodeData.fkRoomID == fkrid))
                {
                    searchindescription.Add(nodet);
                }
            }
        }
        foreach (GameObject searchdes in searchindescription)
        {
            searchShowList.Add(searchdes);
        }
        ShowAllRoomOf(searchShowList);
    }

    // detect too much loop here
    private bool IsDuplicateShowingRoom(List<GameObject> searchLst, string findingMarkerName) /* false if already has marker of that room */
    {
        foreach (GameObject mk in searchLst)
        {
            if (mk.GetComponent<NodeData>().GetParentObjectData().roomName == findingMarkerName)
            {
                return true;
            }
        }
        return false;
    }

    private void ShowAllRoomOf(List<GameObject> searchMarkerList)
    {
        //if system have begin point, need to color it, set to false
        bool beginColored = !(MainController.instance.beginPoint != null);
        bool destColored = !(MainController.instance.destinationPoint != null);

        //destroy all list
        foreach (Transform ch in searchContent.transform)
        {
            Destroy(ch.gameObject);
        }
        foreach (GameObject nodeob in searchMarkerList)
        {
            GameObject roomButton = Instantiate(roomButtonPrefab);
            roomButton.GetComponent<RoomButtonScript>().room = nodeob;
            NodeData nodedata = nodeob.GetComponent<NodeData>();
            roomButton.transform.SetParent(searchContent.transform, false);
            Text RoomNameContent = roomButton.transform.GetChild(0).gameObject.GetComponent<Text>();
            RoomNameContent.text = nodedata.GetParentObjectData().roomName;
            Text RoomDescriptionContent = roomButton.transform.GetChild(1).gameObject.GetComponent<Text>();
            RoomDescriptionContent.text = nodedata.GetParentObjectData().roomDescription;
            if (!beginColored)
            {
                if (MainController.instance.beginPoint.GetComponent<NodeData>().GetParentObjectData().roomName
                    == nodedata.GetParentObjectData().roomName)
                {
                    roomButton.GetComponent<Image>().color = new Color32(0, 0, 0, 255);
                    RoomNameContent.color = Color.white;
                    RoomDescriptionContent.color = Color.white;
                    RoomNameContent.text = " ⦿ ต้นทาง: " + nodedata.GetParentObjectData().roomName;
                    beginColored = true;
                }
            }
            if (!destColored)
            {
                if (MainController.instance.destinationPoint.GetComponent<NodeData>().GetParentObjectData().roomName
                    == nodedata.GetParentObjectData().roomName)
                {
                    roomButton.GetComponent<RoomButtonScript>().isDestination = true;
                    roomButton.GetComponent<Image>().color = new Color32(0, 0, 0, 255);
                    RoomNameContent.color = Color.white;
                    RoomDescriptionContent.color = Color.white;
                    //roomButton.GetComponent<Image>().color = new Color32(126, 60, 255, 255); // purple
                    RoomNameContent.text = " ⦿ ปลายทาง: " + nodedata.GetParentObjectData().roomName;
                    destColored = true;
                }
            }
        }
    }

    public void ClearSearch() {
        searchInputField.GetComponent<InputField>().text = "";
    }
    
    #endregion

    #region Map Action

    public void OnShiftMap(bool isForward)
    {
        GameObject floorObject;
        if (isForward)
        {
            floorObject = building.GetNextFloor(showingFloor.GetComponent<FloorData>().floorName);
        }
        else
        {
            floorObject = building.GetPreviousFloor(showingFloor.GetComponent<FloorData>().floorName);
        }

        mapControl.UpdateMap(floorObject);
        showingFloor = floorObject;
    }

    #endregion

    #region Error Message

    public void ShowErrorCantReadFile(JsonReader.ReadState readState)
    {
        isErrorCantReadFile = true;
        errorDialog.SetActive(true);
        errorPanel.SetActive(true);
        searchButton.GetComponent<Button>().enabled = false;
        searchButton.transform.GetChild(0).gameObject.GetComponent<Image>().color = Color.gray;
        mapButton.GetComponent<Button>().enabled = false;
        mapButton.transform.GetChild(0).gameObject.GetComponent<Image>().color = Color.gray;
        string errortext = "Error in Json File";
        switch (readState)
        {
            case JsonReader.ReadState.BuildingError:
                errortext = "Error in Building Json File";
                break;
            case JsonReader.ReadState.FloorError:
                errortext = "Error in Floor Json File";
                break;
            case JsonReader.ReadState.RoomError:
                errortext = "Error in Room Json File";
                break;
            case JsonReader.ReadState.NodeError:
                errortext = "Error in Node Json File";
                break;
            case JsonReader.ReadState.MarkerError:
                errortext = "Error in Marker Json File";
                break;
            case JsonReader.ReadState.ConnectError:
                errortext = "Error in Connect Json File";
                break;
        }
        errorHeadText.GetComponent<Text>().text = errortext;
    }

    public void CloseErrorPanel()
    {
        errorDialog.SetActive(false);
        errorPanel.SetActive(false);
    }

    #endregion
}
