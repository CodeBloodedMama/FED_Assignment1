using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.NetworkInformation;

using System.Text.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using SWD_GUI_assignment.Model;
using Microsoft.Win32;

namespace SWD_GUI_assignment.ViewModel
{

	public class MainWindowViewModel: INotifyPropertyChanged
	{
        // On Property Changed
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

       
        private void InitializeDefaultData()
        {
            _debtors = new ObservableCollection<AccountModel>();
            _debtors.Add(new AccountModel("John Doe", 400));
            _debtors.Add(new AccountModel("Stefan Kowalczyk", -200));
            CurrentDebtor = Debtors[0];
        }

        public MainWindowViewModel()
        {
            InitializeDefaultData();
        }

        #region Properties & fields

        private string? _currentSaveFilepath = null;
        private Brush _backgroundBrush = null;

        public Brush BackgroundBrush
        {
            get => _backgroundBrush;
            set
            {
                if (_backgroundBrush != value)
                {
                    _backgroundBrush = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ObservableCollection<AccountModel> _debtors = new ObservableCollection<AccountModel>();
        public ObservableCollection<AccountModel> Debtors
        {
            get
            {
                return _debtors;
            }
        }

        private AccountModel _currentDebtor = null;
        public AccountModel CurrentDebtor
        {
            get { return _currentDebtor; }
            set
            {
                if (_currentDebtor != value)
                {
                    _currentDebtor = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        #region Commands
        // Technically making new views via ViewModel does NOT adhere to MVVM architecture
        // Prefered solution would be converting app to PrismApplication and
        // using dialogService Dependency Injection
        // Not implemented due to too much refactoring for too little gain.
        private ICommand _openAddDebtorCommand;

        public ICommand OpenAddDebtorCommand
        {
            get
            {
                return _openAddDebtorCommand ?? (_openAddDebtorCommand = new RelayCommand(()=> {
                    var vm = new AddDebtorViewModel(ref _debtors, AddEventCallback);
                    
                    var addDebtorWin = new AddDebtor()
                        {
                            DataContext = vm
                        };

                    addDebtorWin.Show();
                }));
            }
        }

        private ICommand _openOverviewCommand;
        public ICommand OpenOverviewCommand
        {
            get
            {
                return _openOverviewCommand ?? (_openOverviewCommand = new RelayCommand(()=>
                {
                    var vm = new DebtorOverviewViewModel(ref _currentDebtor);

                    DebtorOverview overViewWin = new DebtorOverview(CurrentDebtor)
                        {
                            DataContext = vm 
                        };
                    overViewWin.Show();
                   
                }));
            }
        }

        private ICommand _commandExitApp;

        public ICommand CommandExitApp
        {
            get
            {
                return _commandExitApp ?? (_commandExitApp = new RelayCommand(() =>
                {
                    OnExit();
                }));
            }
        }
        // Saving and loading functionality
        private ICommand _commandSave;

        public ICommand CommandSave
        {
            get
            {
                return _commandSave ?? (_commandSave = new RelayCommand(() =>
                {
                    if (_currentSaveFilepath == null)
                    {
                        OnSaveAs();
                    }
                    else
                    {
                        OnSave();
                    }
                }));
            }
        }

        private ICommand _commandSaveAs;

        public ICommand CommandSaveAs
        {
            get
            {
                return _commandSaveAs ?? (_commandSaveAs = new RelayCommand(() =>
                {
                    OnSaveAs();
                }));
            }
        }

        private ICommand _commandLoad;

        public ICommand CommandLoad
        {
            get
            {
                return _commandLoad ?? (_commandLoad = new RelayCommand(() => 
                {
                    OnLoad();
                }));
            }
        }

        private ICommand _commandChangeBackgroundColor;

        public ICommand CommandChangeBackgroundColor
        {
            get
            {
                return _commandChangeBackgroundColor ??
                       (_commandChangeBackgroundColor = new RelayCommand<object>((object brush) =>
                       {
                           OnChangeColor(brush);
                       }));
            }
        }
        #endregion

        #region Callbacks 

        // Callback for AddDebtor dialog add event
        public void AddEventCallback(object sender, EventArgs args)
        {
            AddDebtorViewModel.AddEventArgs arguments = args as AddDebtorViewModel.AddEventArgs;
            AccountModel model = arguments.ModelToAdd;
            _debtors.Add(model);
        }
        private void OnChangeColor(object brush)
        {
            BackgroundBrush = brush as Brush;
        }
        private void OnSave()
        {
            string debtBookToSave = new string("");
            debtBookToSave += JsonSerializer.Serialize(_debtors);
            File.WriteAllText(_currentSaveFilepath, debtBookToSave);
        }

        private void OnSaveAs()
        {

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JSON bro|*.json";
            sfd.Title = "Save yer DebtBook";
            sfd.ShowDialog();
            
            if (sfd.FileName != "")
            {
                string debtBookToSave = new string("");
                debtBookToSave += JsonSerializer.Serialize(_debtors);
                File.WriteAllText(sfd.FileName, debtBookToSave);
                _currentSaveFilepath = Path.GetFullPath(sfd.FileName);
            }
        }

        private void OnLoad()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                string serializedDebtbook = File.ReadAllText(ofd.FileName);
                Debtors.Clear();
                List<AccountModel> tempList = new List<AccountModel>();
                tempList = JsonSerializer.Deserialize<List<AccountModel>>(serializedDebtbook);
                foreach (AccountModel m in tempList)
                {
                    Debtors.Add(m);
                }

                _currentSaveFilepath = Path.GetFullPath(ofd.FileName);

            }
        }

        private void OnExit()
        {
            System.Windows.Application.Current.Shutdown();
        }
#endregion

    }
}
