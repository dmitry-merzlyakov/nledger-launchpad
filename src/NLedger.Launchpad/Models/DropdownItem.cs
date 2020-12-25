using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Models
{
    public class DropdownItem
    {
        public static readonly string DividerTitle = "-";

        public static readonly DropdownItem Divider = new DropdownItem(DividerTitle, null, null);

        public DropdownItem(string title, string imageClass, Func<Task> handler)
        {
            if (String.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException(title);
            
            if (title == DividerTitle)
            {
                if (handler != null)
                    throw new ArgumentException("Divider cannot have a handler");
            }
            else
            {
                if (handler == null)
                    throw new ArgumentException(nameof(handler));
            }

            Title = title;
            ImageClass = imageClass;
            Handler = handler;
        }

        public string Title { get; }
        public string ImageClass { get; }
        public Func<Task> Handler { get; }

        public bool IsDivider => Title == DividerTitle;
        public bool HasImage => !String.IsNullOrEmpty(ImageClass);
    }
}
