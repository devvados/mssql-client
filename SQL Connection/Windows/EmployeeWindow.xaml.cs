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
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace SQL_Connection
{
    /// <summary>
    /// Interaction logic for EmployeeWindow.xaml
    /// </summary>
    public partial class EmployeeWindow : MetroWindow
    {
        StaffDataBaseEntities entities = new StaffDataBaseEntities();
        Emps tempEmp;
        int depID = 0;
        string depName;

        public EmployeeWindow(int departamentID)
        {
            InitializeComponent();

            depID = departamentID;

            if (depID != 0)
            {
                //получим название указанного отдела
                var query = from dep in entities.Deps
                            where dep.DepartamentID == depID
                            select dep.DepartamentName;
                depName = query.ToList().First().ToString();
            }
        }

        public EmployeeWindow(Emps employee)
        {
            tempEmp = employee;

            InitializeComponent();

            TBName.Text = tempEmp.Name.ToString();
            TBSurname.Text = tempEmp.Surname.ToString();
            TBPatronymic.Text = tempEmp.Patronymic.ToString();
            depID = tempEmp.DepID;
        }

        private void CBPosition_Loaded(object sender, RoutedEventArgs e)
        {
            //заполним список должностей
            var positions = entities.Positions;
            var q = from pos in positions
                    orderby pos.PositionID
                    select pos.PositionName;

            CBPosition.ItemsSource = q.ToList();

            if (tempEmp != null)
            {
                CBPosition.SelectedIndex = tempEmp.PosID - 1;
            }
            else
                CBPosition.SelectedValue = null;
        }

        private void CBOffice_Loaded(object sender, RoutedEventArgs e)
        {
            //заполним список управлений
            var offices = entities.Offices;
            var q = from of in offices
                    orderby of.OfficeID
                    select of;
            CBOffice.ItemsSource = q.ToList();

            if (depID != 0)
            {
                var query = from dep in entities.Deps
                            where dep.DepartamentID == depID
                            select dep.OfID;
                CBOffice.SelectedIndex = query.First() - 1;
            }
            else
            {
                CBOffice.SelectedValue = null;
            }
            CBDepartament.IsEnabled = false;
        }

        private void CBDepartament_Loaded(object sender, RoutedEventArgs e)
        {
            if(CBOffice.SelectedValue != null)
            {
                //заполним список отделов
                var deps = entities.Deps;
                var q = from dep in deps
                        where dep.OfID == CBOffice.SelectedIndex + 1
                        orderby dep.DepartamentID 
                        select dep;
                CBDepartament.ItemsSource = q.ToList();

                if (depID != 0)
                {
                    //CBDepartament.IsEnabled = true;

                    foreach(var dep in entities.Deps)
                    { }

                    var query = from dep in entities.Deps
                                where dep.DepartamentID == depID
                                orderby dep.DepartamentID
                                select dep;
                    CBDepartament.SelectedValue = query.ToList().First() as Deps;
                }
                else
                {
                    CBDepartament.SelectedValue = null;
                }
            }
        }

        private void CBOffice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CBDepartament.IsEnabled = true;

            //заполним список отделов
            var deps = entities.Deps;
 
            var q = from dep in deps
                    where dep.OfID == CBOffice.SelectedIndex + 1
                    orderby dep.DepartamentID
                    select dep;
            CBDepartament.ItemsSource = q.ToList();
        }

        private void ButOK_Click(object sender, RoutedEventArgs e)
        {
            using (StaffDataBaseEntities newEntity = new StaffDataBaseEntities())
            {
                int DepID = CBDepartament.SelectedIndex + 1, PosID = CBPosition.SelectedIndex + 1;
                try
                {
                    newEntity.InsertEmployee(TBName.Text.ToString(), TBSurname.Text.ToString(), TBPatronymic.Text.ToString(), PosID, DepID);
                }
                catch (Exception ex)
                {
                    ShowMessageBox(ex.Message, "Ошибка");
                }
            }
        }

        //MahApps MessageBox
        public async void ShowMessageBox(string text, string title)
        {
            MetroDialogSettings ms = new MetroDialogSettings()
            {
                ColorScheme = MetroDialogColorScheme.Theme,
                AnimateShow = true,
                AnimateHide = true
            };
            await this.ShowMessageAsync(title, text, MessageDialogStyle.Affirmative, ms);
        }
    }
}
