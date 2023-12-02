using System;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace Alchemy.Editor.Elements
{
    public sealed class MethodButton : VisualElement
    {
        const string ButtonLabelText = "Invoke";

        public MethodButton(object target, MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();

            // Create parameterless button
            if (parameters.Length == 0)
            {
                button = new Button((Action)methodInfo.CreateDelegate(typeof(Action), target))
                {
                    text = methodInfo.Name
                };
                Add(button);
                return;
            }

            var parameterObjects = new object[parameters.Length];

            var box = new HelpBox();
            Add(box);

            foldout = new Foldout()
            {
                text = methodInfo.Name,
                value = false,
                style = {
                    flexGrow = 1f
                }
            };

            button = new Button(() => methodInfo.Invoke(target, parameterObjects))
            {
                text = ButtonLabelText,
                style = {
                    position = Position.Absolute,
                    right = 1f,
                    top = 1.5f,
                    width = 100f
                }
            };

            box.Add(new VisualElement() { style = { width = 12f } });
            box.Add(foldout);
            box.Add(button);

            for (int i = 0; i < parameters.Length; i++)
            {
                var index = i;
                var parameter = parameters[index];
                parameterObjects[index] = Activator.CreateInstance(parameter.ParameterType);
                var element = new GenericField(parameterObjects[index], parameter.ParameterType, ObjectNames.NicifyVariableName(parameter.Name), 0);
                element.OnValueChanged += x => parameterObjects[index] = x;
                element.style.paddingRight = 4f;
                foldout.Add(element);
            }
        }

        readonly Foldout foldout;
        readonly Button button;
    }
}