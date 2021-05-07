using UnityEngine;
using EnhancedUI.EnhancedScroller;
using LuaInterface;
using GameApp.ScenePlayable;
using UnityEngine.UI;

namespace Pangu.GUIs
{
    [RequireComponent(typeof(EnhancedScroller))]
    public class ScrollRectDataSource : MonoBehaviour, IEnhancedScrollerDelegate
    {
        #region Delegate
        public delegate EnhancedScrollerCellView GetCellViewDelegate(EnhancedScroller scroller, int dataIndex, int cellIndex);
        /// <summary>
        /// 自定义获取cell view的方法，如果设置了cellPath，不会使用此方法
        /// </summary>
        public GetCellViewDelegate getCellView;

        public delegate float GetCellViewSizeDelegate(EnhancedScroller scroller, int dataIndex);
        /// <summary>
        /// 运行时获取cell宽或者高的代理
        /// </summary>
        public GetCellViewSizeDelegate getCellViewSize;

        public delegate int GetNumberOfCellsDelegate(EnhancedScroller scroller);
        /// <summary>
        /// 获取cell个数
        /// </summary>
        public GetNumberOfCellsDelegate getNumberOfCells;

        /// <summary>
        /// 获取cell view会调用此方法来设置该cell view
        /// </summary>
        public System.Action<EnhancedScroller, EnhancedScrollerCellView> setCellView;
        public System.Action<EnhancedScroller, EnhancedScrollerGridCellView> setGridCellView;
        #endregion

        #region List Layout
        public bool loadCellFromPrefab = true;

        [PathRef(typeof(GameObject))]
        public string cellPath;

        public EnhancedScrollerCellView cellView;
        #endregion

        #region Grid Layout
        /// <summary>
        /// 使用Grid排版
        /// </summary>
        public bool useGridLayout;

        public int lineGridCellCount = 1;

        public bool loadGridCellFromPrefab = true;

        [PathRef(typeof(GameObject))]
        public string gridCellPath;

        public EnhancedScrollerGridCellView gridCellView;
        #endregion

        private EnhancedScroller _scroller;

        public void Init()
        {
            _scroller.Delegate = this;
            _scroller.ReloadData();
        }

        #region IEnhancedScrollerDelegate
        [NoToLua]
        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            EnhancedScrollerCellView cell = null;
            if (cellView != null)
                cell = _scroller.GetCellView(cellView);
            else if (getCellView != null)
                cell = getCellView(scroller, dataIndex, cellIndex);

            if (cell != null && setCellView != null)
            {
                cell.dataIndex = dataIndex;
                cell.cellIndex = cellIndex;
                if (!useGridLayout && setCellView != null)
                    setCellView(scroller, cell);   
                else if (useGridLayout && setGridCellView != null)
                    SetGridCellView(scroller, cell);  
            }
            if (cell != null) cell.gameObject.SetActive(true);
            return cell;
        }

        [NoToLua]
        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return getCellViewSize != null ? getCellViewSize(scroller, dataIndex) : 0f;
        }

        [NoToLua]
        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            if (getNumberOfCells == null) return 0;
            var cellCount = getNumberOfCells(scroller);
            if (useGridLayout)
            {
                cellCount = Mathf.CeilToInt((float)cellCount / lineGridCellCount);
            }
            return cellCount;
        }
        #endregion

        private void Awake()
        {
            _scroller = GetComponent<EnhancedScroller>();
            if (cellView == null && !string.IsNullOrEmpty(cellPath))
            {
                var go = PlayableLoader.LoadAssetAtPath<GameObject>(cellPath);
                if (go)
                {
                    cellView = go.GetComponent<EnhancedScrollerCellView>();
                }
            }

            if (useGridLayout && gridCellView == null && !string.IsNullOrEmpty(gridCellPath))
            {
                var go = PlayableLoader.LoadAssetAtPath<GameObject>(gridCellPath);
                if (go)
                {
                    gridCellView = go.GetComponent<EnhancedScrollerGridCellView>();
                }
            }
        }

        #region Grid Support
        private void SetGridCellView(EnhancedScroller scroller, EnhancedScrollerCellView cell)
        {
            var layout = cell.GetComponentInChildren<LayoutGroup>();
            if (gridCellView != null)
            {
                var childCount = layout.transform.childCount;
                for (var i = 0; i < lineGridCellCount - childCount; i++)
                {
                    var gridCell = Instantiate(gridCellView.gameObject);
                    layout.transform.AddChild(gridCell);
                }
            }
            else
            {
                Debug.LogError("null grid cell, make sure grid cell or it's path is set");
            }
            
            var lineCount = GetNumberOfCells(scroller);
            var colCount = lineGridCellCount;
            if (cell.dataIndex >= lineCount - 1)
            {
                colCount = getNumberOfCells(scroller) % lineGridCellCount;
                for (var i = colCount; i < lineGridCellCount; i++)
                {
                    layout.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            for (var i = 0; i < colCount; i++)
            {
                var gCellView = layout.transform.GetChild(i).GetComponent<EnhancedScrollerGridCellView>();
                if (setGridCellView == null) continue;
                gCellView.rowIndex = cell.dataIndex;
                gCellView.columnIndex = i;
                gCellView.dataIndex = gCellView.rowIndex * lineGridCellCount + i;
                setGridCellView(scroller, gCellView);
                gCellView.gameObject.SetActive(true);
            }
        }
        #endregion

    }
}