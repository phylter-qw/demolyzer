using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Demolyzer.Model;
using QuantumBitDesigns.Charting;
using QuantumBitDesigns.Mvvm;

namespace Demolyzer.ViewModel
{
    public class PlayerResultsViewModel : ViewModelBase
    {
        public PlayerResultsViewModel(Dictionary<MatchType, Dictionary<string, List<ParticipantInfo>>> playerMatchResults)
        {
            Dictionary<string, List<ParticipantInfo>> results4on4 = playerMatchResults[MatchType.Match4on4];
            this._playerResults4on4MapAll = new SortableObservableCollection<ParticipantInfoViewModel>();
            foreach (var kvp in results4on4)
            {
                this._playerResults4on4MapAll.Add(new ParticipantInfoViewModel(ParticipantInfo.Average(kvp.Value.ToArray())));
            }
        }

        //private SortableObservableCollection<ParticipantInfoViewModel> _playerResults1on1MapAll;
        //private SortableObservableCollection<ParticipantInfoViewModel> _playerResults1on1Mapdm2;
        //private SortableObservableCollection<ParticipantInfoViewModel> _playerResults1on1Mapdm4;
        //private SortableObservableCollection<ParticipantInfoViewModel> _playerResults1on1Mapdm6;
        //private SortableObservableCollection<ParticipantInfoViewModel> _playerResults1on1Mapztndm3;
        //private SortableObservableCollection<ParticipantInfoViewModel> _playerResults1on1Mapaerowalk;

        //private SortableObservableCollection<ParticipantInfoViewModel> _playerResults2on2MapAll;
        //private SortableObservableCollection<ParticipantInfoViewModel> _playerResults1on1Mapdm2;
        //private SortableObservableCollection<ParticipantInfoViewModel> _playerResults1on1Mapdm4;
        //private SortableObservableCollection<ParticipantInfoViewModel> _playerResults1on1Mapdm6;
        //private SortableObservableCollection<ParticipantInfoViewModel> _playerResults1on1Mapztndm3;
        //private SortableObservableCollection<ParticipantInfoViewModel> _playerResults1on1Mapaerowalk;

        private SortableObservableCollection<ParticipantInfoViewModel> _playerResults4on4MapAll;

        public SortableObservableCollection<ParticipantInfoViewModel> PlayerResults4on4MapAll
        {
            get
            {
                return this._playerResults4on4MapAll;
            }
        }
    }
}
