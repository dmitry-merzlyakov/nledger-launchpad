using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Models
{
    public class SampleViewModel
    {
        public SampleViewModel(SampleItem sampleItem, Action selectedChanged)
        {
            if (sampleItem == null)
                throw new ArgumentNullException(nameof(sampleItem));

            SampleItem = sampleItem;
            SelectedChanged = selectedChanged;
        }

        public SampleItem SampleItem { get; }

        public bool Selected 
        { 
            get { return _Selected; }
            set
            {
                _Selected = value;
                SelectedChanged?.Invoke();
            }
        }
        public Action SelectedChanged { get; }

        public string Title => SampleItem.Title;

        private bool _Selected;
    }
}
