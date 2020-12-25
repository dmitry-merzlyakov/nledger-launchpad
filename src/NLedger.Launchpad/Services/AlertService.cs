using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Services
{
    public enum AlertKind
    {
        Success, 
        Info, 
        Warning, 
        Danger, 
        Primary, 
        Secondary, 
        Light,
        Dark
    }

    public class AlertItem
    {
        public AlertItem(AlertKind alertKind, string message, string title = null)
        {
            if (String.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message));

            Kind = alertKind;
            Message = message;
            Title = title;
        }

        public AlertKind Kind { get; }
        public string Message { get; }
        public string Title { get; }
        public bool HasTitle => !String.IsNullOrWhiteSpace(Title);
    }

    public class AlertService
    {
        public async Task TraceResult(Func<Task> action, Func<string> successMessage)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            try
            {
                await action();
                if (successMessage != null)
                    await AddAlert(Services.AlertKind.Success, successMessage());
            }
            catch (Exception ex)
            {
                await AddAlert(Services.AlertKind.Danger, ex.Message, "Error:");
            }
        }

        public async Task AddAlert(AlertKind alertKind, string message, string title = null)
        {
            Queue.Add(new AlertItem(alertKind, message, title));
            await OnItemsChanged.Invoke();
        }

        public async Task CloseAlert(AlertItem item)
        {
            Queue.Remove(item);
            await OnItemsChanged.Invoke();
        }

        public IEnumerable<AlertItem> Items => Queue;
        public event Func<Task> OnItemsChanged;

        private List<AlertItem> Queue { get; } = new List<AlertItem>();
    }
}
