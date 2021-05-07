using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.AnimatorBehaviour
{
    public class SplitPane : VisualElement
    {
        private const string UssClassName = "split-pane";
        private const string ContentContainerClassName = UssClassName + "__content-container";
        private const string HandleDragLineClassName = UssClassName + "__dragline";
        private const string HandleDragLineVerticalClassName = HandleDragLineClassName + "--vertical";
        private const string HandleDragLineHorizontalClassName = HandleDragLineClassName + "--horizontal";
        private const string HandleDragLineAnchorClassName = UssClassName + "__dragline-anchor";
        private const string HandleDragLineAnchorVerticalClassName = HandleDragLineAnchorClassName + "--vertical";
        private const string HandleDragLineAnchorHorizontalClassName = HandleDragLineAnchorClassName + "--horizontal";
        private const string VerticalClassName = UssClassName + "--vertical";
        private const string HorizontalClassName = UssClassName + "--horizontal";

        public enum Orientation
        {
            Horizontal,
            Vertical
        }

        public new class UxmlFactory : UxmlFactory<SplitPane, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlIntAttributeDescription _fixedPaneIndex = new UxmlIntAttributeDescription { name = "fixed-pane-index", defaultValue = 0 };
            private readonly UxmlIntAttributeDescription _fixedPaneInitialSize = new UxmlIntAttributeDescription { name = "fixed-pane-initial-size", defaultValue = 100 };
            private readonly UxmlStringAttributeDescription _orientation = new UxmlStringAttributeDescription { name = "orientation", defaultValue = "horizontal" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var fixedPaneIndex = _fixedPaneIndex.GetValueFromBag(bag, cc);
                var fixedPaneInitialSize = _fixedPaneInitialSize.GetValueFromBag(bag, cc);
                var orientationStr = _orientation.GetValueFromBag(bag, cc);
                var orientation = orientationStr == "horizontal"
                    ? Orientation.Horizontal
                    : Orientation.Vertical;

                ((SplitPane)ve).Init(fixedPaneIndex, fixedPaneInitialSize, orientation);
            }
        }

        public VisualElement Content { get; }

        private VisualElement _leftPane;
        private VisualElement _rightPane;

        private VisualElement _fixedPane;
        private VisualElement _flexedPane;

        private readonly VisualElement _dragLine;
        private readonly VisualElement _dragLineAnchor;
        private float _minDimension;

        private Orientation _orientation;
        private int _fixedPaneIndex;
        private float _fixedPaneInitialDimension;

        private SquareResizer _resizer;

        public SplitPane()
        {
            AddToClassList(UssClassName);

            styleSheets.Add(Resources.Load<StyleSheet>("Common/Styles/SplitPane"));

            Content = new VisualElement {name = "unity-content-container"};
            Content.AddToClassList(ContentContainerClassName);
            hierarchy.Add(Content);

            // Create drag anchor line.
            _dragLineAnchor = new VisualElement {name = "unity-dragline-anchor"};
            _dragLineAnchor.AddToClassList(HandleDragLineAnchorClassName);
            hierarchy.Add(_dragLineAnchor);

            // Create drag
            _dragLine = new VisualElement {name = "unity-dragline"};
            _dragLine.AddToClassList(HandleDragLineClassName);
            _dragLineAnchor.Add(_dragLine);
        }

        public SplitPane(
            int fixedPaneIndex,
            float fixedPaneStartDimension,
            Orientation orientation) : this()
        {
            Init(fixedPaneIndex, fixedPaneStartDimension, orientation);
        }

        public void Init(int fixedPaneIndex, float fixedPaneInitialDimension, Orientation orientation)
        {
            _orientation = orientation;
            _minDimension = 40;
            _fixedPaneIndex = fixedPaneIndex;
            _fixedPaneInitialDimension = fixedPaneInitialDimension;

            if (_orientation == Orientation.Horizontal)
                style.minWidth = _fixedPaneInitialDimension;
            else
                style.minHeight = _fixedPaneInitialDimension;

            Content.RemoveFromClassList(HorizontalClassName);
            Content.RemoveFromClassList(VerticalClassName);
            Content.AddToClassList(_orientation == Orientation.Horizontal
                ? HorizontalClassName
                : VerticalClassName);

            // Create drag anchor line.
            _dragLineAnchor.RemoveFromClassList(HandleDragLineAnchorHorizontalClassName);
            _dragLineAnchor.RemoveFromClassList(HandleDragLineAnchorVerticalClassName);
            _dragLineAnchor.AddToClassList(_orientation == Orientation.Horizontal
                ? HandleDragLineAnchorHorizontalClassName
                : HandleDragLineAnchorVerticalClassName);

            // Create drag
            _dragLine.RemoveFromClassList(HandleDragLineHorizontalClassName);
            _dragLine.RemoveFromClassList(HandleDragLineVerticalClassName);
            _dragLine.AddToClassList(_orientation == Orientation.Horizontal
                ? HandleDragLineHorizontalClassName
                : HandleDragLineVerticalClassName);

            if (_resizer != null)
            {
                _dragLineAnchor.RemoveManipulator(_resizer);
                _resizer = null;
            }

            if (Content.childCount != 2)
                RegisterCallback<GeometryChangedEvent>(OnPostDisplaySetup);
            else
                PostDisplaySetup();
        }

        private void OnPostDisplaySetup(GeometryChangedEvent evt)
        {
            if (Content.childCount != 2)
            {
                Debug.LogError("TwoPaneSplitView needs exactly 2 chilren.");
                return;
            }

            PostDisplaySetup();

            UnregisterCallback<GeometryChangedEvent>(OnPostDisplaySetup);
            RegisterCallback<GeometryChangedEvent>(OnSizeChange);
        }

        private void PostDisplaySetup()
        {
            if (Content.childCount != 2)
            {
                Debug.LogError("TwoPaneSplitView needs exactly 2 children.");
                return;
            }

            _leftPane = Content[0];
            if (_fixedPaneIndex == 0)
            {
                _fixedPane = _leftPane;
                if (_orientation == Orientation.Horizontal)
                    _leftPane.style.width = _fixedPaneInitialDimension;
                else
                    _leftPane.style.height = _fixedPaneInitialDimension;
            }
            else
            {
                _flexedPane = _leftPane;
            }

            _rightPane = Content[1];
            if (_fixedPaneIndex == 1)
            {
                _fixedPane = _rightPane;
                if (_orientation == Orientation.Horizontal)
                    _rightPane.style.width = _fixedPaneInitialDimension;
                else
                    _rightPane.style.height = _fixedPaneInitialDimension;
            }
            else
            {
                _flexedPane = _rightPane;
            }

            _fixedPane.style.flexShrink = 0;
            _flexedPane.style.flexGrow = 1;
            _flexedPane.style.flexShrink = 0;
            _flexedPane.style.flexBasis = 0;

            if (_orientation == Orientation.Horizontal)
            {
                if (_fixedPaneIndex == 0)
                    _dragLineAnchor.style.left = _fixedPaneInitialDimension;
                else
                    _dragLineAnchor.style.left = resolvedStyle.width - _fixedPaneInitialDimension;
            }
            else
            {
                if (_fixedPaneIndex == 0)
                    _dragLineAnchor.style.top = _fixedPaneInitialDimension;
                else
                    _dragLineAnchor.style.top = resolvedStyle.height - _fixedPaneInitialDimension;
            }

            var direction = _fixedPaneIndex == 0 ? 1 : -1;
            
            _resizer = new SquareResizer(this, direction, _minDimension, _orientation);

            _dragLineAnchor.AddManipulator(_resizer);

            UnregisterCallback<GeometryChangedEvent>(OnPostDisplaySetup);
            RegisterCallback<GeometryChangedEvent>(OnSizeChange);
        }

        private void OnSizeChange(GeometryChangedEvent evt)
        {
            var maxLength = resolvedStyle.width;
            var dragLinePos = _dragLineAnchor.resolvedStyle.left;
            var activeElementPos = _fixedPane.resolvedStyle.left;
            if (_orientation == Orientation.Vertical)
            {
                maxLength = resolvedStyle.height;
                dragLinePos = _dragLineAnchor.resolvedStyle.top;
                activeElementPos = _fixedPane.resolvedStyle.top;
            }

            if (_fixedPaneIndex == 0 && dragLinePos > maxLength)
            {
                var delta = maxLength - dragLinePos;
                _resizer.ApplyDelta(delta);
            }
            else if (_fixedPaneIndex == 1)
            {
                if (activeElementPos < 0)
                {
                    var delta = -dragLinePos;
                    _resizer.ApplyDelta(delta);
                }
                else
                {
                    if (_orientation == Orientation.Horizontal)
                        _dragLineAnchor.style.left = activeElementPos;
                    else
                        _dragLineAnchor.style.top = activeElementPos;
                }
            }
        }

        public override VisualElement contentContainer
        {
            get { return Content; }
        }

        private class SquareResizer : MouseManipulator
        {
            private Vector2 _start;
            private bool _active;
            private readonly SplitPane _splitView;
            private readonly VisualElement _pane;
            private readonly int _direction;
            private readonly float _minWidth;
            private readonly Orientation _orientation;

            public SquareResizer(SplitPane splitView, int dir, float minWidth, Orientation orientation)
            {
                _orientation = orientation;
                _minWidth = minWidth;
                _splitView = splitView;
                _pane = splitView._fixedPane;
                _direction = dir;
                activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
                _active = false;
            }

            protected override void RegisterCallbacksOnTarget()
            {
                target.RegisterCallback<MouseDownEvent>(OnMouseDown);
                target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
                target.RegisterCallback<MouseUpEvent>(OnMouseUp);
            }

            protected override void UnregisterCallbacksFromTarget()
            {
                target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
                target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
                target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
            }

            public void ApplyDelta(float delta)
            {
                var oldDimension = _orientation == Orientation.Horizontal
                    ? _pane.resolvedStyle.width
                    : _pane.resolvedStyle.height;
                var newDimension = oldDimension + delta;

                if (newDimension < oldDimension && newDimension < _minWidth)
                    newDimension = _minWidth;

                var maxLength = _orientation == Orientation.Horizontal
                    ? _splitView.resolvedStyle.width
                    : _splitView.resolvedStyle.height;
                if (newDimension > oldDimension && newDimension > maxLength)
                    newDimension = maxLength;

                if (_orientation == Orientation.Horizontal)
                {
                    _pane.style.width = newDimension;
                    if (_splitView._fixedPaneIndex == 0)
                        target.style.left = newDimension;
                    else
                        target.style.left = _splitView.resolvedStyle.width - newDimension;
                }
                else
                {
                    _pane.style.height = newDimension;
                    if (_splitView._fixedPaneIndex == 0)
                        target.style.top = newDimension;
                    else
                        target.style.top = _splitView.resolvedStyle.height - newDimension;
                }
            }

            private void OnMouseDown(MouseDownEvent e)
            {
                if (_active)
                {
                    e.StopImmediatePropagation();
                    return;
                }

                if (CanStartManipulation(e))
                {
                    _start = e.localMousePosition;

                    _active = true;
                    target.CaptureMouse();
                    e.StopPropagation();
                }
            }

            private void OnMouseMove(MouseMoveEvent e)
            {
                if (!_active || !target.HasMouseCapture())
                    return;

                var diff = e.localMousePosition - _start;
                var mouseDiff = _orientation == Orientation.Horizontal ? diff.x : diff.y;

                var delta = _direction * mouseDiff;

                ApplyDelta(delta);

                e.StopPropagation();
            }

            private void OnMouseUp(MouseUpEvent e)
            {
                if (!_active || !target.HasMouseCapture() || !CanStopManipulation(e))
                    return;

                _active = false;
                target.ReleaseMouse();
                e.StopPropagation();
            }
        }
    }
}