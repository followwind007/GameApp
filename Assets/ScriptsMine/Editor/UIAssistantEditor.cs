using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//[InitializeOnLoad]
public class UIAssistantEditor : EditorWindow
{
    //static UIAssistantEditor()
    //{
    //    EditorApplication.hierarchyWindowItemOnGUI += UIAssistantEditor.DrawHierarchyUIDepth;
    //}

    // Begin for window
    static UIAssistantEditor sWinInst = null;
    [@MenuItem("Tools/UIAssistant")]
    public static void ShowWindow()
    {
        // init the window
        if (sWinInst == null)
        {
            sWinInst = EditorWindow.GetWindow(typeof(UIAssistantEditor)) as UIAssistantEditor;
            sWinInst.Initialize();
        }
        sWinInst.Show();
    }

    void OnDestroy()
    {
        //if (sWinInst != null)
        {
            EditorApplication.hierarchyWindowItemOnGUI -= DrawHierarchyUIDepth;
        }
            
    }

    public void Initialize()
    {
        EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchyUIDepth;
    }

    // End for window


    // show ui depth in hierarchy view
    static bool showUIDepth = true;
    // invalid batching depth value
    const int INVALID_BATCHING_DEPTH = -2;
    // invalid batch index
    const int INVALID_BATCH_INDEX = -1;
    // invalid material instance index
    const int INVALID_MATERIAL_INDEX = -1;

    // all ui instructions( canvas renderers) from root ui gameobject, sorted by render depth (from the unity engine)
    List<UIInstruction> uiInstructions = new List<UIInstruction>();
    // visible ui instructions for batching, sorted by batching depth( calculating from uiInstructions )
    List<UIInstruction> visibleUIInstructions = new List<UIInstruction>();
    // batch result from the visibleUIInstructions, by the batching rule
    List<BatchInstruction> outputBatches = new List<BatchInstruction>();

    /// Material instance id list for mapping
    List<int> materialInstances = new List<int>();
    // Get index by material instance id
    int GetMaterialIndex(int matInstId)
    {
        for (int i = 0; i < materialInstances.Count; ++i)
        {
            if (materialInstances[i] == matInstId)
            {
                return i;
            }
        }

        return INVALID_MATERIAL_INDEX;
    }
    // add new material instance and return index
    int AddMaterialInstance(int matInstId)
    {
        for (int i = 0; i < materialInstances.Count; ++i)
        {
            if (materialInstances[i] == matInstId)
            {
                return i;
            }
        }

        int result = materialInstances.Count;
        materialInstances.Add(matInstId);
        return result;
    }

    /// <summary>
    /// UI Assistant: Calculating and displaying batching depth
    /// </summary>
   
    struct UIInstruction
    {
        public int goInstanceID;      /// for mapping gameobject by instance id 

        public int materialInstanceID;
        // for showing different material and mapping
        public int materialIndex;

        public int textureInstanceID;
        public int renderDepth;
        public int absoluteDepth;
        public int batchingDepth;

        public int batchIndex;

        public bool isMask;
        public bool isActive;

        public Rect rect; /// global rect
                            /// 

        public static UIInstruction EmptyInstruction = new UIInstruction(0);

        public UIInstruction(int crID)
        {
            goInstanceID = crID;
            materialInstanceID = 0;
            materialIndex = -1;
            textureInstanceID = 0;

            renderDepth = -1;
            absoluteDepth = -1;
            batchingDepth = INVALID_BATCHING_DEPTH;

            batchIndex = INVALID_BATCH_INDEX;

            isMask = false;
            isActive = true;

            rect = new Rect(0, 0, 0, 0);
        }

        public UIInstruction(UIInstruction sourceUI)
        {
            goInstanceID = sourceUI.goInstanceID;
            materialInstanceID = sourceUI.materialInstanceID;
            materialIndex = sourceUI.materialIndex;
            textureInstanceID = sourceUI.textureInstanceID;

            renderDepth = sourceUI.renderDepth;
            absoluteDepth = sourceUI.absoluteDepth;
            batchingDepth = sourceUI.batchingDepth;

            batchIndex = sourceUI.batchIndex;

            isMask = sourceUI.isMask;
            isActive = sourceUI.isActive;
            rect = new Rect(sourceUI.rect);
        }

        public bool CanRender()
        {
            return isActive && (materialInstanceID != 0); /// vertex > 0 , but can't get
        }

        public bool Equals(UIInstruction otherUI)
        {
            bool result = (goInstanceID == otherUI.goInstanceID) && (materialInstanceID == otherUI.materialInstanceID) && (materialIndex == otherUI.materialIndex) && (textureInstanceID == otherUI.textureInstanceID)
                            && (renderDepth == otherUI.renderDepth) && (absoluteDepth == otherUI.absoluteDepth) && (batchingDepth == otherUI.batchingDepth)
                            && (batchIndex == otherUI.batchIndex) && (isMask == otherUI.isMask) && (isActive && otherUI.isActive) && (rect.Equals(otherUI.rect));

            return result;
        }


        public bool CanBatchWith(UIInstruction otherUI)
        {
            // ignore the same one
            if (Equals(otherUI))
            {
                return true;
            }

            if (isMask || otherUI.isMask)
            {
                return false;
            }

            return (materialInstanceID == otherUI.materialInstanceID) && (textureInstanceID == otherUI.textureInstanceID);
        }

        public void GetGlobalRect(RectTransform rectTrans)
        {
            if (rectTrans == null)
            {
                return;
            }

            Vector3[] corners = new Vector3[4];
            rectTrans.GetWorldCorners(corners);
            rect.xMin = 99999.0f;
            rect.yMin = 99999.0f;
            rect.xMax = -99999.0f;
            rect.yMax = -99999.0f;
            for (int j = 0; j < 4; ++j)
            {
                //Debug.Info(string.Format("Corner {0}: {1}, {2}, {3}", curRenderer.name, corners[j].x, corners[j].y, corners[j].z));
                rect.xMin = Mathf.Min(rect.xMin, corners[j].x);
                rect.yMin = Mathf.Min(rect.yMin, corners[j].y);
                rect.xMax = Mathf.Max(rect.xMax, corners[j].x);
                rect.yMax = Mathf.Max(rect.yMax, corners[j].y);
            }
        }

        public static int Compare(UIInstruction ui1, UIInstruction ui2)
        {
            if (ui1.batchingDepth != ui2.batchingDepth)
                return ui1.batchingDepth.CompareTo(ui2.batchingDepth);
            if (ui1.materialInstanceID != ui2.materialInstanceID)
                return ui1.materialInstanceID.CompareTo(ui2.materialInstanceID);
            if (ui1.textureInstanceID != ui2.textureInstanceID)
                return ui1.textureInstanceID.CompareTo(ui2.textureInstanceID);

            return ui1.renderDepth.CompareTo(ui2.renderDepth);
        }
    }

    struct BatchInstruction
    {
        public int beginBatchIndex;
        public int batchSize;
        public bool isValidBatch;
    }


    void PrintUIInstructions(List<UIInstruction> uilist)
    {
        for (int i = 0; i < uilist.Count; ++i)
        {
            Debug.Log(string.Format("BatchIndex: {0}, Depth: {1}, MaterialID: {2}, TextureID: {3}, RenderDepth: {4}", uilist[i].batchIndex, uilist[i].batchingDepth, uilist[i].materialInstanceID, uilist[i].textureInstanceID, uilist[i].renderDepth));
        }
    }

    void ClearUIInstructions()
    {
        uiInstructions.Clear();
        materialInstances.Clear();
    }

    void UpdateOrderAndSyncData(GameObject uiRootGo)
    {
        ClearUIInstructions();

        bool includeInactive = true; //// include inactive ?
        CanvasRenderer[] canvasRenderers = uiRootGo.GetComponentsInChildren<CanvasRenderer>(includeInactive);
        if (canvasRenderers == null)
        {
            return;
        }

        int crCount = canvasRenderers.Length;
        // update batch order
        for (int i = 0; i < crCount; ++i)
        {
            CanvasRenderer curRenderer = canvasRenderers[i];
            UIInstruction newUI = new UIInstruction(curRenderer.gameObject.GetInstanceID());

            if (curRenderer.GetMaterial() != null)
            {
                newUI.materialInstanceID = curRenderer.GetMaterial().GetInstanceID();
                newUI.materialIndex = AddMaterialInstance(newUI.materialInstanceID);
                if (curRenderer.GetMaterial().mainTexture != null)
                {
                    newUI.textureInstanceID = curRenderer.GetMaterial().mainTexture.GetInstanceID();
                }
            }

            newUI.absoluteDepth = curRenderer.absoluteDepth;
            newUI.renderDepth = curRenderer.relativeDepth;
            newUI.isMask = curRenderer.hasRectClipping;
            newUI.isActive = curRenderer.gameObject.activeSelf;

            /// get global rect
            RectTransform rectTrans = curRenderer.GetComponent<RectTransform>();
            newUI.GetGlobalRect(rectTrans);

            uiInstructions.Add(newUI);
        }

        // sort by render depth
        uiInstructions = uiInstructions.OrderBy(a => a.renderDepth).ToList();
        //PrintUIInstructions(uiInstructions);

    }

    int ProcessBatching(int begin, int end, List<UIInstruction> depthSortedList, int batchIndex, ref BatchInstruction outBatch)
    {
        outBatch.beginBatchIndex = begin;

        int i = begin;
        for (; (i < end) && (depthSortedList[begin].CanBatchWith(depthSortedList[i])); i++)
        {
            // record the batch index
            UIInstruction ui = depthSortedList[i];
            ui.batchIndex = batchIndex;
            depthSortedList[i] = ui;
        }

        outBatch.isValidBatch = true;
        outBatch.batchSize = i - begin;

        if (outBatch.batchSize == 0)
        {
            UnityEngine.Debug.LogError("Error Batch, begin :" + begin);
        }


        return outBatch.batchSize;
    }

    void ClearBatches()
    {
        visibleUIInstructions.Clear();
        outputBatches.Clear();
        outputBatches.Capacity = uiInstructions.Count;
    }

    void UpdateBatching()
    {
        if (uiInstructions.Count == 0)
        {
            return;
        }

        ClearBatches();

        /// sort for batching
        List<UIInstruction> outUIList = new List<UIInstruction>(uiInstructions.Count);
        for (int i = 0; i < uiInstructions.Count; ++i)
        {
            UIInstruction curUI = uiInstructions[i];
            UIInstruction newOutUI = new UIInstruction(curUI);
            outUIList.Add(newOutUI);
            if (!curUI.CanRender())
            {
                newOutUI.batchingDepth = -1;
                outUIList[i] = newOutUI;
                continue;
            }

            int highestDepth = 0;
            for (int j = 0; j < i; ++j)
            {
                if (outUIList[j].batchingDepth == -1)
                {
                    continue;
                }

                Rect lowerRect = uiInstructions[j].rect;
                if (!curUI.rect.Overlaps(lowerRect))
                {
                    continue; // Don't intersect any lower ui, skip over it
                }

                if (curUI.CanBatchWith(outUIList[j]))
                {
                    highestDepth = Mathf.Max(outUIList[j].batchingDepth, highestDepth);
                }
                else
                {
                    highestDepth = Mathf.Max(outUIList[j].batchingDepth + 1, highestDepth);
                }
            }

            newOutUI.batchingDepth = highestDepth;
            outUIList[i] = newOutUI;
        }

        /// sort out ui list
        outUIList.Sort(UIInstruction.Compare);

        for (int i = 0; i < outUIList.Count; ++i)
        {
            if (outUIList[i].batchingDepth == -1)
            {
                continue;
            }

            visibleUIInstructions.Add(outUIList[i]);
        }

        /// Testing output ui instructions
        //PrintUIInstructions(visibleUIInstructions);

        int beginIndex = 0;
        int endIndex = visibleUIInstructions.Count;
        while (beginIndex < endIndex)
        {
            BatchInstruction newBatch = new BatchInstruction();
            beginIndex += ProcessBatching(beginIndex, endIndex, visibleUIInstructions, outputBatches.Count, ref newBatch);
            outputBatches.Add(newBatch);
        }

        /// Testing final visible ui instructions with batch index
        PrintUIInstructions(visibleUIInstructions);

        if (outputBatches.Count > 20)
        {
            Debug.Log("<color=red>Batch Count is more then 20: </color>" + outputBatches.Count);
        }
        else
        {
            Debug.Log("<color=green>Batch Count is less then 20: </color>" + outputBatches.Count);
        }

        if (showUIDepth)
        {   // force repainting the hierarchy view when batching updates
            EditorApplication.RepaintHierarchyWindow();
        }
          
    }

    int GetBatchingDepth(int uiGoInstanceId)
    {
        for(int i = 0; i < visibleUIInstructions.Count; ++i)
        {
            if (visibleUIInstructions[i].goInstanceID == uiGoInstanceId)
            {
                return visibleUIInstructions[i].batchingDepth;
            }
        }

        return INVALID_BATCHING_DEPTH;        
    }

    int GetBatchIndex(int uiGoInstanceId)
    {
        for (int i = 0; i < visibleUIInstructions.Count; ++i)
        {
            if (visibleUIInstructions[i].goInstanceID == uiGoInstanceId)
            {
                return visibleUIInstructions[i].batchIndex;
            }
        }

        return INVALID_BATCH_INDEX;     
    }

    UIInstruction GetBatchingUIInstruction(int uiGoInstanceId)
    {
        for (int i = 0; i < visibleUIInstructions.Count; ++i)
        {
            if (visibleUIInstructions[i].goInstanceID == uiGoInstanceId)
            {
                return visibleUIInstructions[i];
            }
        }

        return UIInstruction.EmptyInstruction;
    }


    public GameObject selectGameobject = null;
    public GameObject previousRootUI = null;
    public Dictionary<GameObject, bool> preUIChildrenDict = new Dictionary<GameObject, bool>();

    void ForceActiveAllChildren(GameObject rootUI)
    {
        if (rootUI == null)
        {
            return;
        }

        if (previousRootUI != null)
        {
            //bool toRevert = EditorUtility.DisplayDialog("Revert Previous Selected UI", "Previous selected ui is existed. Revert the previous root UI?", "Yes!", "No!");
            //if (toRevert)
            {
                RevertPreviousRootUI();
            }
        }

        previousRootUI = rootUI;

        Transform[] children = rootUI.GetComponentsInChildren<Transform>(true);
        int childrenCount = children.Length;
        for (int i = 0; i < childrenCount; ++i)
        {
            Transform child = children[i];
            if (!preUIChildrenDict.ContainsKey(child.gameObject))
                preUIChildrenDict.Add(child.gameObject, child.gameObject.activeSelf);
            child.gameObject.SetActive(true);
        }

    }

    void RevertActiveChildren(GameObject rootUI)
    {
        if (previousRootUI != rootUI)
        {
            Debug.LogError("Root UI No Match!, previous UI is:" + previousRootUI.name);
            return;
        }

        bool active = true;
        Transform[] children = rootUI.GetComponentsInChildren<Transform>(true);
        int childrenCount = children.Length;
        for (int i = 0; i < childrenCount; ++i)
        {
            Transform child = children[i];
            if (preUIChildrenDict.TryGetValue(child.gameObject, out active))
            {
                child.gameObject.SetActive(active);
            }
        }

        preUIChildrenDict.Clear();
        previousRootUI = null;
    }

    void RevertPreviousRootUI()
    {
        if (previousRootUI == null)
        {
            return;
        }

        RevertActiveChildren(previousRootUI);
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Find UIRoot by default(None), or select custom UI");
        selectGameobject = EditorGUILayout.ObjectField(selectGameobject, typeof(GameObject), true) as GameObject;

        if (GUILayout.Button("Force Activate"))
        {
            GameObject uiRoot = selectGameobject;
            if (uiRoot == null)
            {
                uiRoot = GameObject.Find("UIRoot");
                if (uiRoot == null)
                {
                    Debug.LogError("[UI Assistant Editor] UIRoot not found, please select root UI!");
                    return;
                }
            }

            ForceActiveAllChildren(uiRoot);
        }

        if (GUILayout.Button("Revert Activate"))
        {
            RevertPreviousRootUI();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //bool show = EditorGUILayout.Toggle("ShowDepth", showUIDepth);

        bool show = EditorGUILayout.Toggle(new GUIContent("ShowDepth", "Format: BatchIndex / Depth / MaterialIndex"), showUIDepth);
        if (GUILayout.Button("UpdateBatching"))
        {
            GameObject uiRoot = selectGameobject;
            if (uiRoot == null)
            {
                uiRoot = GameObject.Find("UIRoot_YL");
                if (uiRoot == null)
                {
                    Debug.LogError("[UI Assistant Editor] UIRoot_YL not found, please select root UI!");
                    return;
                }
            }

            UpdateOrderAndSyncData(uiRoot);
            UpdateBatching();
        }
    
        if (showUIDepth != show)
        {
            ShowHierarchyUIDepth(show);
        }

    }

    // force repainting hierarchy window
    void ShowHierarchyUIDepth(bool enabled)
    {
        showUIDepth = enabled;
        EditorApplication.RepaintHierarchyWindow();
    }

    GUIStyle mySytle = new GUIStyle();        
    public void DrawHierarchyUIDepth(int instanceID, Rect selectionRect)
    {
        if (!showUIDepth)
        {
            return;
        }
        mySytle.richText = true;

        UIInstruction ui = GetBatchingUIInstruction(instanceID);
        if (ui.batchingDepth != INVALID_BATCHING_DEPTH)
        {
            // place the depth to the left of the list item:
            Rect r = new Rect(selectionRect);
            r.x -= 40.0f;
            r.width = 40.0f;

            string msg = "";
            if ((outputBatches.Count > 20) && (selectGameobject != null))
            {
                msg = string.Format("<color=red>{0}/{1}/{2}</color>", ui.batchIndex, ui.batchingDepth, ui.materialIndex);
            }
            else
            {
                msg = string.Format("<color=yellow>{0}/{1}/</color><color=cyan>{2}</color>", ui.batchIndex, ui.batchingDepth, ui.materialIndex);
            }
                 
            GUI.Label(r, msg, mySytle);
        }

        mySytle.richText = false;

    }

}

