using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//Software Technology DiplomaEngineering
//4th Semester @Aarhus Universitet
//Front End Development
//Code and program developed by :
//    Szymanek, Marcin
//           and
//    Lennert, Elisabeth
//Debt Book program using MVVM desingn pattern

namespace SWD_GUI_assignment.Model
{
    // Each debtor has an account. Balance list shows all transactions (debts taken or paid)
    // AccountModel models this data
    public class AccountModel
    {
        ObservableCollection<Tuple<DateTime,double>> _balanceList = new ObservableCollection<Tuple<DateTime, double>>();

        public AccountModel(string name, double balance = 0)
        {
            Name = name; 
            ChangeBalance(balance);
        }

        public ObservableCollection<Tuple<DateTime, double>> BalanceList
        {
            get { return _balanceList; }
            set { }
        }

        public String Information
        {
            get
            {
                return (Name +"\t "+ Balance.ToString());
            }
        }

        public String Name { get; set; }
        
        public double Balance { get; set; }

        public void ChangeBalance(double balance)
        {
            DateTime currentDateTime = new DateTime();
            currentDateTime = DateTime.Now;
            Tuple<DateTime, double> balanceEntry = new Tuple<DateTime, double>(currentDateTime, balance);

            _balanceList.Add(balanceEntry);

            Balance += balance;
        }
    }

} 
