using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ReactiveUI;
using System;

namespace Explorer
{
    public class ViewLocator : IDataTemplate
    {
        public Control? Build(object data)
        {
            var name = data.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type != null)
            {
                if (!type.IsAssignableTo(typeof(Control)))
                    return null;

                return Activator.CreateInstance(type) as Control;
            }
            else
            {
                return new TextBlock { Text = "Not Found: " + name };
            }
        }

        public bool Match(object data)
        {
            return data is ReactiveObject;
        }
    }
}