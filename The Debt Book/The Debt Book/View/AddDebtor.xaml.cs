using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SWD_GUI_assignment
{
    /// <summary>
    /// Interaction logic for AddDebtor.xaml
    /// </summary>
    public partial class AddDebtor : Window
    {
        public AddDebtor()
        {
            InitializeComponent();
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
