using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using GoG.Client.Services;

namespace GoG.Client.ViewModels.Games
{
    /// <summary>
    /// This adapts the games list paging in the online go client to controls that support data virtualization
    /// via the ISupportIncrementalLoading interface.  Controls load it automatically when bound to it; you
    /// should not attempt to modify it manually.  To reload, just create a new instance.
    /// </summary>
    public class IncrementalObservableGameViewModelCollection : ObservableCollection<GameViewModel>, ISupportIncrementalLoading
    {
        #region Data
        private readonly IGoGService _onlineGoService;
        private Models.OnlineGoService.MyGames.GamesPager _gamesPager;
        private int _pageNo;
        private bool _busy;
        #endregion Data

        #region Ctor

        public IncrementalObservableGameViewModelCollection(IGoGService onlineGoService)
        {
            _onlineGoService = onlineGoService;
        }

        #endregion Ctor

        #region Public Methods

        #endregion Public Methods

        #region Properties

        #region TotalCount
        private int _totalCount;
        public int TotalCount
        {
            get { return _totalCount; }
            set
            {
                _totalCount = value; 
                OnPropertyChanged(new PropertyChangedEventArgs("TotalCount")); 
            }
        }
        #endregion TotalCount

        #endregion Properties

        #region Private Helpers

        private async Task<bool> LoadPage(CancellationToken ct)
        {
            // Loads page '_pageNo' from the server and adds those records to our collection.
            // The server also includes a 'count,' which is the total # of records on the server.
            // We save that in TotalCount for data binding.

            var newPager = await _onlineGoService.GetGamesPagerAsync(_pageNo);
            if (ct.IsCancellationRequested || newPager == null)
                return false;
            TotalCount = newPager.count;
            _gamesPager = newPager;
            foreach (var game in _gamesPager.results)
            {
                var vm = new GameViewModel(game, _onlineGoService);
                vm.LoadDetailsAsync();
                Add(vm);
            }
            return true;
        }

        private async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c, uint count)
        {
            // OG server gives back pages according to an index you pass in the url.  So we
            // must make repeated calls to the server, incrementing the server page # until 
            // we've gotten 'count' records, or there none left, whichever comes first.

            try
            {
                int loaded = 0;
                // Watch cancellation token.
                while (count > loaded || c.IsCancellationRequested)
                {
                    _pageNo++;
                    var success = await LoadPage(c);
                    if (!success)
                    {
                        _pageNo--;
                        break;
                    }
                    loaded += _gamesPager.results.Count;
                }

                return new LoadMoreItemsResult {Count = (uint) loaded};
            }
            finally
            {
                _busy = false;
            }
        }

        #endregion Private Helpers

        #region ISupportIncrementalLoading Members

        public bool HasMoreItems
        {
            get { return _gamesPager == null || _gamesPager.next != null; }
        }

        public Windows.Foundation.IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            if (_busy)
                throw new InvalidOperationException("Only one operation at a time please.");
            _busy = true;

            return AsyncInfo.Run(c => LoadMoreItemsAsync(c, count));
        }

        #endregion
    }
}
