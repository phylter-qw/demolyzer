using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuantumBitDesigns.Mvvm;

namespace Demolyzer.ViewModel
{
    public class ProgressPanelViewModel : ViewModelBase
    {
        private string _description;
        private int _progressMax;
        private int _progress;

        public ProgressPanelViewModel()
        {
        }

        public string Description
        {
            get
            {
                return this._description;
            }
            set
            {
                this._description = value;
                OnPropertyChanged(() => Description);
            }
        }

        public int ProgressMax
        {
            get
            {
                return this._progressMax;
            }
            set
            {
                this._progressMax = value;
                OnPropertyChanged(() => ProgressMax);
            }
        }

        public int Progress
        {
            get
            {
                return this._progress;
            }
            set
            {
                this._progress = value;
                OnPropertyChanged(() => Progress);
            }
        }
    }
}
