using Microsoft.AspNetCore.Components;
using NLedger.Launchpad.Models;
using NLedger.Launchpad.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Services
{
    public class ModelService
    {
        public AppViewModel Model
        {
            get { return _Model ?? throw new InvalidOperationException("Not initialized yet"); }
        }

        public async Task Initialize(LocalStorageRepository localStorageRepository)
        {
            if (localStorageRepository == null)
                throw new ArgumentNullException(nameof(localStorageRepository));

            if (_Model != null)
                throw new InvalidOperationException("Already initialized");

            _Model = new AppViewModel(localStorageRepository);
            await _Model.Initialize();
        }

        private AppViewModel _Model = null;
    }
}
