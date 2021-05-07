using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Tools.Table
{
    public class TableView : VisualElement
    {
        public readonly VisualElement contentTable;
        public readonly VisualElement contentDetail;

        public readonly VisualElement scrollTable;
        public readonly VisualElement scrollDetail;
        
        public TableView()
        {
            this.AddStyleSheetPath("Table/Styles/TableView");
            var toolbar = new Toolbar { name = "toolbar" };
            Add(toolbar);
            
            var content = new VisualElement { name = "content" };
            Add(content);
            
            scrollTable = new ScrollView { name = "scrollView", verticalScrollerVisibility = ScrollerVisibility.Auto };
            content.Add(scrollTable);
            
            contentTable = new VisualElement { name = "scrollContent" };
            scrollTable.Add(contentTable);
            
            scrollDetail = new ScrollView { name = "detailScrollView", verticalScrollerVisibility = ScrollerVisibility.Auto };
            content.Add(scrollDetail);
            
            contentDetail = new VisualElement { name = "detailContent" };
            scrollDetail.Add(contentDetail);

            scrollDetail.RemoveFromHierarchy();
        }

        public void LoadTable(string tableName)
        {
            
        }
        
    }
}