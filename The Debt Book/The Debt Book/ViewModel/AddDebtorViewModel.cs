using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JetBrains.Annotations;
using SWD_GUI_assignment.Model;

namespace SWD_GUI_assignment.ViewModel
{
    public class AddDebtorViewModel: INotifyPropertyChanged
    {
        private ObservableCollection<AccountModel> _accs;

        public AddDebtorViewModel(ref ObservableCollection<AccountModel> accs, EventHandler callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("EventHandler callback");
            }
            _addDebtorClicked += callback;
            _accs = accs;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        AccountModel _accountModel=new AccountModel(null);

        public String Name
        {
            get => _accountModel.Name;
            set
            {
                if (value != _accountModel.Name)
                {
                    _accountModel.Name = value;
                    OnPropertyChanged();
                }
            }
        }
        public double Balance
        {
            get => _accountModel.Balance;
            set
            {
                if (value != _accountModel.Balance)
                {
                    _accountModel.ChangeBalance(value);
                    OnPropertyChanged();
                }
            }
        }

        public class AddEventArgs : EventArgs
        {
            private AccountModel _modelToAdd;

            public AccountModel ModelToAdd
            {
                get => _modelToAdd;
                set => _modelToAdd = value;
            }
        }

        private event EventHandler _addDebtorClicked ;

        public void AddDebtor_OnClick()
        {
            if (_accountModel != null)
            {
                AddEventArgs args = new AddEventArgs();
                args.ModelToAdd = _accountModel;
                _addDebtorClicked.Invoke(this, args);
            }
        }

        public void CloseApplication_OnClick()
        {
            //this.Close;
        }


        private ICommand _AddDebtorCommand;

        public ICommand AddDebtorCommand
        {
        
            get
            {
                return _AddDebtorCommand ?? (_AddDebtorCommand = new RelayCommand(AddDebtor_OnClick));
            }
        
        }

        private ICommand _CloseCommand;

        public ICommand CloseCommand
        {
            get
            {
                return _CloseCommand ?? (_CloseCommand = new RelayCommand(CloseApplication_OnClick));
            }
        }


    }

} 


