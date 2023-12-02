using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Assertions;
using Alchemy.Editor.Internal;

namespace Alchemy.Editor.Elements
{
    /// <summary>
    /// Visual Element that draws an IList
    /// </summary>
    public sealed class ListField : VisualElement
    {
        sealed class Item : VisualElement
        {
            public int index;
        }

        const string ItemClassName = "unity-list-view__item";

        public ListField(IList target, string label, int depth)
        {
            Assert.IsNotNull(target);
            list = target;

            listView = GUIHelper.CreateDefaultListView(label);
            listView.makeItem = () => new Item();
            listView.bindItem = (element, index) =>
            {
                ((Item)element).index = index;

                var value = list[index];
                var listType = list.GetType();
                var valueType = value != null ? value.GetType() : listType.IsGenericType ? listType.GenericTypeArguments[0] : typeof(object);
                var fieldElement = new GenericField(value, valueType, label, depth);
                element.Add(fieldElement);
                var labelElement = fieldElement.Q<Label>();
                if (labelElement != null) labelElement.text = "Element " + index;

                fieldElement.OnValueChanged += x =>
                {
                    list[((Item)element).index] = x;
                    NotifyOnValueChanged();
                };
            };
            listView.unbindItem = (element, index) =>
            {
                element.Clear();
            };
            listView.itemsSource = list;
            listView.itemIndexChanged += (prevIndex, index) =>
            {
                var item = listView.Query<VisualElement>(className: ItemClassName).AtIndex(index).Q<Item>();
                item.index = index;
                NotifyOnValueChanged();
            };
            listView.itemsAdded += indexes => NotifyOnValueChanged();

            listView.Q<Label>().style.unityFontStyleAndWeight = FontStyle.Bold;
            Add(listView);
        }

        readonly IList list;
        readonly ListView listView;

        public event Action<object> OnValueChanged;

        void NotifyOnValueChanged()
        {
            OnValueChanged?.Invoke(list);
        }
    }
}
