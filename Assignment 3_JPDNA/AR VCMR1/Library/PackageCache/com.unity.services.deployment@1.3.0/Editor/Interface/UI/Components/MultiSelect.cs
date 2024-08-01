using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Deployment.Editor.Interface.UI.Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Services.Deployment.Editor.Interface.UI.Components
{
    class MultiSelect : VisualElement
    {
        public Selectable SelectionStart { get; private set; }
        public Selectable SelectionEnd { get; private set; }

        public event Action OnSelectionChanged;

        public MultiSelect()
        {
            RegisterCallback<ClickEvent>(c => OnClick(GetSelectablesQuery(), c));
            RegisterCallback<RightClickedEvent>(OnRightClicked);
        }

        UQueryState<Selectable> GetSelectablesQuery()
        {
            return this.Query<Selectable>().Build();
        }

        public void SetSelection(Selectable target)
        {
            var selectables = GetSelectablesQuery();
            foreach (var selectable in selectables)
            {
                selectable.value = selectable == target;
            }

            SelectionStart = target;
            SelectionEnd = target;
            OnSelectionChanged?.Invoke();
        }

        Selectable GetSelectionEnd(IEnumerable<Selectable> selectables)
        {
            Selectable firstSelectable = null;
            Selectable lastSelectable = null;
            foreach (var selectable in selectables)
            {
                if (selectable.value)
                {
                    if (firstSelectable == null)
                        firstSelectable = selectable;

                    lastSelectable = selectable;
                }
            }

            return lastSelectable == SelectionStart ? firstSelectable : lastSelectable;
        }

        public void AddSelection(Selectable target)
        {
            var selectables = GetSelectablesQuery();
            SweepModeClick(selectables, target);
            SelectionEnd = GetSelectionEnd(selectables);
            OnSelectionChanged?.Invoke();
        }

        public void RemoveSelection(Selectable target)
        {
            var selectables = GetSelectablesQuery();
            MultiModeClick(selectables, target);
            SelectionEnd = GetSelectionEnd(selectables);
            OnSelectionChanged?.Invoke();
        }

        public Selectable GetNextSelectable(Selectable startSelectable, bool selectPrevious, bool includeChildren, bool skipIfSelected)
        {
            var selectables = GetSelectablesQuery().ToList();

            if (startSelectable == null)
            {
                return selectables.First();
            }

            var filteredSelectables = new List<Selectable>(selectables);
            var startAnchor = SelectionStart;
            if (!includeChildren)
            {
                startAnchor = GetSelectableFromTarget(SelectionStart, true) ?? SelectionStart;
                var childrenAndSiblingSelectables = startAnchor.Query<Selectable>().Build().ToList();
                childrenAndSiblingSelectables.Remove(startAnchor);
                var enumerable = selectables.Except(childrenAndSiblingSelectables);
                filteredSelectables = new List<Selectable>(enumerable);
            }

            var sweepIndex = filteredSelectables.IndexOf(startAnchor);
            return GetNextActiveSelectable(filteredSelectables, sweepIndex, selectPrevious, skipIfSelected);
        }

        static Selectable GetNextActiveSelectable(IList<Selectable> selectables, int sweepIndex, bool selectPrevious, bool skipIfSelected)
        {
            Selectable output = null;
            var startIndex = selectPrevious ? sweepIndex - 1 : sweepIndex + 1;
            for (var i = startIndex; i < selectables.Count && i >= 0;)
            {
                if (selectables[i].visible
                && (!skipIfSelected || !selectables[i].value))
                {
                    output = selectables[i];
                    break;
                }

                i = selectPrevious ? i - 1 : i + 1;
            }

            return output;
        }

        public Selectable GetParentItem(Selectable startSelectable)
        {
            var selectables = GetSelectablesQuery().ToList();
            if (startSelectable == null)
            {
                return selectables.First();
            }

            return GetSelectableFromTarget(SelectionStart, true)
                ?? GetNextActiveSelectable(selectables, selectables.IndexOf(SelectionStart), true, false);
        }

        public bool IsItemBeforeTarget(Selectable item, Selectable target)
        {
            var isBefore = false;
            var selectables = GetSelectablesQuery().ToList();
            for (var i = 0; i < selectables.Count && !isBefore; ++i)
            {
                if (selectables[i] == item)
                {
                    isBefore = true;
                }
                else if (selectables[i] == target)
                {
                    break;
                }
            }

            return isBefore;
        }

        void OnClick(UQueryState<Selectable> selectables, ClickEvent c)
        {
            var selectMode = GetSelectModeFromClickEvent(c);
            var target = GetSelectableFromTarget((VisualElement)c.target);
            switch (selectMode)
            {
                case Mode.Single:
                    SingleModeClick(selectables, target);
                    break;
                case Mode.Multi:
                    MultiModeClick(selectables, target);
                    break;
                case Mode.Sweep:
                    SweepModeClick(selectables, target);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            SelectionEnd = target;
            OnSelectionChanged?.Invoke();
        }

        static Mode GetSelectModeFromClickEvent(ClickEvent clickEvent)
        {
            return clickEvent.modifiers switch
            {
                EventModifiers.Control => Mode.Multi,
                EventModifiers.Command => Mode.Multi,
                EventModifiers.Shift => Mode.Sweep,
                _ => Mode.Single
            };
        }

        void SingleModeClick(IEnumerable<Selectable> selectables, Selectable target)
        {
            foreach (var selectable in selectables)
            {
                if (target != selectable)
                {
                    selectable.value = false;
                }
                else
                {
                    selectable.value = true;
                    SelectionStart = selectable;
                }
            }
        }

        void MultiModeClick(IEnumerable<Selectable> selectables, Selectable target)
        {
            var targetSelectable = GetSelectableFromTarget(target);
            if (targetSelectable == null)
                return;

            foreach (var selectable in selectables)
            {
                if (targetSelectable == selectable)
                {
                    if (!targetSelectable.value)
                    {
                        SelectionStart = targetSelectable;
                    }

                    targetSelectable.value = !targetSelectable.value;
                }
            }
        }

        static Selectable GetSelectableFromTarget(VisualElement visualElement, bool ignoreSelf = false)
        {
            while (true)
            {
                if (!ignoreSelf &&  visualElement is Selectable s) return s;

                ignoreSelf = false;
                switch (visualElement.parent)
                {
                    case null:
                        return null;
                    default:
                        visualElement = visualElement.parent;
                        continue;
                }
            }
        }

        void SweepModeClick(IEnumerable<Selectable> selectables, Selectable target)
        {
            var targetSelectable = GetSelectableFromTarget(target);
            if (targetSelectable == null)
                return;

            var selectablesList = selectables.ToList();
            var targetIndex = selectablesList.IndexOf(targetSelectable);
            if (!targetSelectable.value)
            {
                AdjustSweepAnchorOnSweepClick(selectablesList, targetIndex);
            }

            var originIndex = selectablesList.IndexOf(SelectionStart);
            var topIndex = Mathf.Min(originIndex, targetIndex);
            var bottomIndex = Mathf.Max(originIndex, targetIndex);
            for (var i = 0; i < selectablesList.Count; ++i)
            {
                selectablesList[i].value = i >= topIndex && i <= bottomIndex;
            }
        }

        void AdjustSweepAnchorOnSweepClick(List<Selectable> selectablesList, int targetIndex)
        {
            var firstSelectedIndex = selectablesList.First(x => x.value);
            var lastSelectedIndex = selectablesList.Last(x => x.value);

            SelectionStart = targetIndex < selectablesList.IndexOf(firstSelectedIndex)
                ? lastSelectedIndex
                : firstSelectedIndex;
        }

        void OnRightClicked(RightClickedEvent viewBase)
        {
            var target = viewBase.target as VisualElement;
            var selectable = target.Q<Selectable>();
            if (!selectable.value)
            {
                SetSelection(selectable);
            }
        }

        enum Mode
        {
            Single,
            Multi,
            Sweep
        }

        new class UxmlFactory : UxmlFactory<MultiSelect> {}
    }
}
