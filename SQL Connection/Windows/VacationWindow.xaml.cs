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
    /// Interaction logic for VacationWindow.xaml
    /// </summary>
    public partial class VacationWindow : MetroWindow
    {
        int empID = 0;
        Vacations tempVac = null;

        //добавляем отпуск сотруднику
        public VacationWindow(int employeeID)
        {
            try
            {
                if (employeeID == 0)
                    throw new Exception("Не выбран сотрудник, которому добавляем отпуск!");
                else
                {
                    empID = employeeID;
                    InitializeComponent();
                }
            }
            catch(Exception ex)
            {
                ShowMessageBox(ex.Message, "Ошибка");
            }
        }

        //редактирование отпуска
        public VacationWindow(Vacations vac)
        {
            tempVac = vac;
            empID = vac.EmpID;

            InitializeComponent();

            DPBegin.SelectedDate = tempVac.BeginDate;
            DPEnd.SelectedDate = tempVac.EndDate;
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

        private void ButOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DPBegin.SelectedDate == null || DPEnd.SelectedDate == null)
                    throw new Exception("Необходимо задать обе даты!");
                DateTime dateBegin = (DateTime)DPBegin.SelectedDate, dateEnd = (DateTime)DPEnd.SelectedDate;

                using (StaffDataBaseEntities entities = new StaffDataBaseEntities())
                {
                    entities.InsertVacation(dateBegin.ToShortDateString(), dateEnd.ToShortDateString(), empID);
                }
            }
            catch(Exception ex)
            {
                ShowMessageBox(ex.Message, "Ошибка");
            }
        }
    }
}
