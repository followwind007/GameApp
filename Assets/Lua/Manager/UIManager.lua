UIMgr = {}

function UIMgr:Init()
    local rootPrefab = Resources.Load("UICanvas")
    self.root = GameObject.Instantiate(rootPrefab)
    GameObject.DontDestroyOnLoad(self.root)

    self.rootTrans = self.root:GetComponent("Canvas", "RectTransform")


end

function UIMgr:Show()

end

function UIMgr:Hide()

end

function UIMgr:Close()

end

UIMgr:Init()