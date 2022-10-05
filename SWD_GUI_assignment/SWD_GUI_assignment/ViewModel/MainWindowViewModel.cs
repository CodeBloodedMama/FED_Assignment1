using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Animation;
using SWD_GUI_assignment.Model;

namespace SWD_GUI_assignment.ViewModel
{

	public class MainWindowViewModel: INotifyPropertyChanged
	{
        # region Properties
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
        private void InitializeDefaultData()
        {
            _debtors = new ObservableCollection<AccountModel>();
            _debtors.Add(new AccountModel("Rita Binzer", 400));
            _debtors.Add(new AccountModel("Susan Binzer", -200));
            CurrentDebtor = Debtors[0];
        }
        private void AddNewDebtor(AccountModel acc)
        {
            _debtors.Add(acc);
        }

        // Callback for AddDebtorViewModel add event
        public void AddEventCallback(object sender, EventArgs args)
        {
            AddDebtorViewModel.AddEventArgs arguments = args as AddDebtorViewModel.AddEventArgs;
            AccountModel model = arguments.ModelToAdd;
            AddNewDebtor(model);
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

    }
}
