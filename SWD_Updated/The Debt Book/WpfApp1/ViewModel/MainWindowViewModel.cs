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
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Animation;
using SWD_GUI_assignment.Model;
using Microsoft.Win32;

namespace SWD_GUI_assignment.ViewModel
{

	public class MainWindowViewModel: INotifyPropertyChanged
	{
        # region Properties
        private ObservableCollection<AccountModel> _debtors = new ObservableCollection<AccountModel>();
        private string? _currentSaveFilepath = null; 

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
        private void InitializeDefaultData()
        {
            _debtors = new ObservableCollection<AccountModel>();
            _debtors.Add(new AccountModel("Rita Binzer", 400));
            _debtors.Add(new AccountModel("Susan Binzer", -200));
            CurrentDebtor = Debtors[0];
        }
    
        // Callback for AddDebtorViewModel add event
        public void AddEventCallback(object sender, EventArgs args)
        {
            AddDebtorViewModel.AddEventArgs arguments = args as AddDebtorViewModel.AddEventArgs;
            AccountModel model = arguments.ModelToAdd;
            _debtors.Add(model);
        }
        
        public MainWindowViewModel()
        {
            InitializeDefaultData();
        }


        // On Property Changed
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //Commands

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

    }
}
