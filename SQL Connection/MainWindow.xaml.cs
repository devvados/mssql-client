using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace SQL_Connection
{
    public partial class MainWindow : MetroWindow
    {
        public static StaffDataBaseEntities dataEntities = new StaffDataBaseEntities();
        public static bool? admin = null;
 
        public MainWindow()
        {
            AuthorizationWindow wnd = new AuthorizationWindow();  
            //событие для определения режима пользователя        
            wnd.LogIn += value => admin = value;
            wnd.ShowDialog();

            if (admin != null)
            {
                InitializeComponent();

                DGEmployees.ContextMenu = EmployeesCM();
                DGVacations.ContextMenu = VacationsCM();
            }
            else
            {
                //пользователь не выбрал режим входа
                this.Close();
            }
        }

        /// <summary>
        /// Отображение режима авторизованного пользователя
        /// </summary>
        private void ShowUserType()
        {
            if (admin == true)
            {
                ShowMessageBox("Вы вошли как Администратор.\nРазрешено просматривать, вносить изменения в базу данных (добавлять/удалять/редактировать записи)", "Информация");
                BAddEmployee.IsEnabled = true;
            }
            else if (admin == false) ShowMessageBox("Вы вошли как Гость.\nРазрешено только просматривать записи базы данных", "Информация");

            string type = (admin == true) ? "Администратор" : "Гость";
            LabUserType.Content = "Вы вошли как: " + type;
        }

        /// <summary>
        /// Отображение режима пользователя при запуске
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ShowUserType();
            FillDataGrid(DGOffices, null, null);
        }

        /// <summary>
        /// Окошко с сообщениями
        /// </summary>
        /// <param name="text"></param>
        /// <param name="title"></param>
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

        #region Заполнение Combobox и DataGrid данными из БД 

        /// <summary>
        /// Заполнение Управлений и Профессий
        /// </summary>
        /// <param name="cb"> Заполняемый Combobox </param>
        public void FillComboBox(ComboBox cb)
        {
            if (Equals(cb, CBPositions))
            {
                //заполним список должностей
                var positions = dataEntities.Positions;
                var q = from pos in positions
                         orderby pos.PositionName
                         select pos.PositionName;

                List<string> queryList = q.ToList();
                queryList.Insert(0, "--- Не выбрано ---");
                CBPositions.ItemsSource = queryList;
                CBPositions.SelectionChanged += CBPositions_SelectionChanged;
            }
        }

        /// <summary>
        /// Заполним Combobox как только загрузится
        /// </summary>
        /// <param name="sender"> Combobox </param>
        /// <param name="e"></param>
        private void CBPositions_Loaded(object sender, RoutedEventArgs e)
        {
            FillComboBox(CBPositions);
        }

        /// <summary>
        /// Заполнение DataGrid
        /// </summary>
        /// <param name="dg"> Заполняемый DataGrid </param>
        /// <param name="columnHeaders"> Заголовки для столбцов </param>
        /// <param name="foreignKey"> Внешний ключ для таблиц </param>
        /// <param name="position"> Профессия </param>
        private void FillDataGrid(DataGrid dg, dynamic foreignKey, string position)
        {
            //заполним датагрид управлений
            if(Equals(dg, DGOffices))
            {
                var of = dataEntities.Offices;
                var ofQuery = from office in of
                              select office;
                DGOffices.ItemsSource = ofQuery.ToList();
                DGOffices.Visibility = Visibility.Visible;
                Dispatcher.BeginInvoke((Action)(() => TCTables.SelectedIndex = 0));
            }
            ///заполним датагрид отделов
            if (Equals(dg, DGDepartaments))
            {
                int officeID = Convert.ToInt32(foreignKey);
                var deps = dataEntities.Deps;
                var depsQuery = from dep in deps
                                where dep.Offices.OfficeID == officeID
                                orderby dep.DepartamentID
                                select dep;
                DGDepartaments.ItemsSource = depsQuery.ToList();
                DGDepartaments.Visibility = Visibility.Visible;
                Dispatcher.BeginInvoke((Action)(() => TCTables.SelectedIndex = 1));
            }
            //заполним датагрид сотрудников
            if (Equals(dg, DGEmployees))
            {
                var emps = dataEntities.Emps;

                //в структурированном режиме упр -> отделы и тд
                if (foreignKey != null)
                {
                    int depID = Convert.ToInt32(foreignKey);
                    
                    if (position == "--- Не выбрано ---")
                    {
                        var empsQuery = from emp in emps
                                        where emp.DepID == depID
                                        orderby emp.Surname
                                        select emp;
                        DGEmployees.ItemsSource = empsQuery.ToList();
                    }
                    else
                    {
                        var empsQuery = from emp in emps
                                        where emp.DepID == depID && emp.Positions.PositionName == position
                                        orderby emp.Surname
                                        select emp;
                        DGEmployees.ItemsSource = empsQuery.ToList();
                    }
                    Dispatcher.BeginInvoke((Action)(() => TCTables.SelectedIndex = 2));
                    DGEmployees.Visibility = Visibility.Visible;
                }
                //в режиме "показать всех" (только сотрудники)
                else
                {
                    if (position == "--- Не выбрано ---")
                    {
                        var empsQuery = from emp in emps
                                        orderby emp.Surname
                                        select emp;
                        DGEmployees.ItemsSource = empsQuery.ToList();
                    }
                    else
                    {
                        var empsQuery = from emp in emps
                                        where emp.Positions.PositionName == position
                                        orderby emp.Surname
                                        select emp;
                        DGEmployees.ItemsSource = empsQuery.ToList();
                    }
                    Dispatcher.BeginInvoke((Action)(() => TCTables.SelectedIndex = 2));
                    DGEmployees.Visibility = Visibility.Visible;
                }
            }
            //заполним датагрид отпусков
            else if (Equals(dg, DGVacations))
            {
                int empID = Convert.ToInt32(foreignKey);
                var vacs = dataEntities.Vacations;

                var vacsQuery = from vac in vacs
                                where vac.EmpID == empID
                                orderby vac.BeginDate
                                select vac;

                DGVacations.ItemsSource = vacsQuery.ToList();
                Dispatcher.BeginInvoke((Action)(() => TCTables.SelectedIndex = 3));
                DGVacations.Visibility = Visibility.Visible;
            }
        }
        
        /// <summary>
        /// Скрыть DataGrid, когда понадобится
        /// </summary>
        /// <param name="dg"> DataGrid </param>
        private void ClearAndHideDG(DataGrid dg)
        {
            dg.ItemsSource = null;
            dg.Visibility = Visibility.Hidden;
        }

        #endregion

        #region Кнопки меню

        /// <summary>
        /// Выход из программыы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MIExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// О программе
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MIAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow wnd = new AboutWindow();
            wnd.ShowDialog();
        }

        #endregion

        #region Обработчики событий (выбор элементов в таблицах) 

        /// <summary>
        /// Смена элементов TabControl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TCTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tabControl = sender as TabControl;
            if (tabControl != null)
            {
                int tempTC = tabControl.SelectedIndex;
                if (tempTC == 0)
                {
                    CBPositions.IsEnabled = false;
                }
                if (tempTC == 1)
                {
                    CBPositions.IsEnabled = false;
                }
                if (tempTC == 2)
                {
                    CBPositions.IsEnabled = true;
                }
                if(tempTC == 3)
                {
                    CBPositions.IsEnabled = false;
                }
            }
        }

        #endregion

        #region Контекстные меню для DataGrid'ов

        /// <summary>
        /// Получаем позицию в таблице по клику мыши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DG_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGrid dgTable = sender as DataGrid;
            DependencyObject dep = (DependencyObject)e.OriginalSource;
            while((dep != null) && !(dep is DataGridCell))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }
            if (dep == null && admin == true)
            {
                if (dgTable != null) EnableMenuItems(dgTable.ContextMenu, true, false, false);
            }
            else if(dep == null && admin != true)
            {
                if (dgTable != null) EnableMenuItems(dgTable.ContextMenu, false, false, false);
            }

            if(dep is DataGridCell)
            {
                if(admin == true)
                    EnableMenuItems(dgTable.ContextMenu, false, true, true);
                else
                {
                    EnableMenuItems(dgTable.ContextMenu, false, false, false);
                }

                DataGridCell cell = dep as DataGridCell;
                cell.Focus();

                dgTable.SelectedItem = cell.DataContext;

                if (dgTable.SelectedItem != null)
                {
                    if (Equals(dgTable, DGDepartaments))
                    {
                        DGDepartaments.ContextMenu.Tag = dgTable;
                        DGDepartaments.ContextMenu.IsOpen = true;
                        
                    }
                    else if (Equals(dgTable, DGEmployees))
                    {
                        DGEmployees.ContextMenu.Tag = dgTable;
                        DGEmployees.ContextMenu.IsOpen = true;
                    }
                    else if (Equals(dgTable, DGVacations))
                    {
                        DGVacations.ContextMenu.Tag = dgTable;
                        DGVacations.ContextMenu.IsOpen = true;
                    }
                }
            }
        }

        /// <summary>
        /// Работают определенные пункты контекстного меню
        /// </summary>
        /// <param name="cm"> Контекстное меню </param>
        /// <param name="m1"> работает/нет </param>
        /// <param name="m2"> работает/нет </param>
        /// <param name="m3"> работает/нет </param>
        private void EnableMenuItems(ItemsControl cm, bool m1, bool m2, bool m3)
        {
            var menuItem1 = cm.Items[0] as MenuItem;
            if (menuItem1 != null) menuItem1.IsEnabled = m1;
            var menuItem2 = cm.Items[1] as MenuItem;
            if (menuItem2 != null) menuItem2.IsEnabled = m2;
            var menuItem3 = cm.Items[2] as MenuItem;
            if (menuItem3 != null) menuItem3.IsEnabled = m3;
        }

        /// <summary>
        /// Контекстное меню для таблицы сотрудников
        /// </summary>
        /// <returns></returns>
        private ContextMenu EmployeesCM()
        {
            ContextMenu menu = new ContextMenu();
            MenuItem editEmp = new MenuItem { Header = "Редактировать" };
            MenuItem addEmp = new MenuItem { Header = "Добавить" };
            MenuItem deleteEmp = new MenuItem { Header = "Удалить" };

            editEmp.Click += EditClick;
            addEmp.Click += AddClick;
            deleteEmp.Click += DeleteClick;

            menu.Items.Add(addEmp);
            menu.Items.Add(editEmp);
            menu.Items.Add(deleteEmp);

            return menu;
        }

        /// <summary>
        /// Контекстное меню для таблицы отпусков
        /// </summary>
        /// <returns></returns>
        private ContextMenu VacationsCM()
        {
            ContextMenu menu = new ContextMenu();
            MenuItem editVac = new MenuItem { Header = "Редактировать" };
            MenuItem addVac = new MenuItem { Header = "Добавить" };
            MenuItem deleteVac = new MenuItem { Header = "Удалить" };

            editVac.Click += EditClick;
            addVac.Click += AddClick;
            deleteVac.Click += DeleteClick;

            menu.Items.Add(addVac);
            menu.Items.Add(editVac);
            menu.Items.Add(deleteVac);

            return menu;
        }

        #endregion

        #region Операции над элементами в таблицах

        /// <summary>
        /// Удаление позиции в таблице
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            if (DGEmployees.SelectedItem != null)
            {
                if (DGVacations.SelectedItem != null)
                {
                    int vacID = (DGVacations.SelectedItem as Vacations).VacationID;
                    dataEntities.DeleteVacation(vacID);
                }
                else
                {
                    int empID = (DGEmployees.SelectedItem as Emps).EmployeeID;
                    dataEntities.DeleteEmployee(empID);
                }
            }   
        }

        /// <summary>
        /// Добавление позиции в таблицу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddClick(object sender, RoutedEventArgs e)
        {
            string getId = "";
            DataGrid dgTable = null;

            if (TCTables.SelectedIndex == 2)
            {
                dgTable = DGEmployees;

                //получим отдел, в который добавить сотрудника
                Deps selectedDepartament = DGDepartaments.SelectedItem as Deps;
                EmployeeWindow wnd;

                //добавим в указанный отдел или в общий список
                if(selectedDepartament != null)
                {
                    wnd = new EmployeeWindow(selectedDepartament.DepartamentID);
                }
                else
                {
                    wnd = new EmployeeWindow(0);
                }
                wnd.ShowDialog();
            }
            else if (TCTables.SelectedIndex == 3)
            {
                dgTable = DGVacations;

                //получаем сотрудника, которому добавить отпуск
                Emps selectedEmployee = DGEmployees.SelectedItem as Emps;

                if (selectedEmployee != null)
                {
                    var wnd = new VacationWindow(selectedEmployee.EmployeeID);
                    wnd.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Редактирование позиции в таблице
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditClick(object sender, RoutedEventArgs e)
        {
            if(TCTables.SelectedIndex == 2)
            {
                Emps emp = DGEmployees.SelectedItem as Emps;
                var wnd = new EmployeeWindow(emp);
                wnd.ShowDialog();
            }
            else if(TCTables.SelectedIndex == 3)
            {
                Vacations vac = DGVacations.SelectedItem as Vacations;
                var wnd = new VacationWindow(vac);
                wnd.ShowDialog();
            }          
        }

        #endregion

        #region Отображение данных в таблицах

        /// <summary>
        /// Смена элемента Combobox с должностями
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CBPositions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                Deps selectedDep = DGDepartaments.SelectedItem as Deps;
                var positionName = comboBox.SelectedItem.ToString();

                if (selectedDep != null)
                {
                    FillDataGrid(DGEmployees, selectedDep.DepartamentID, positionName);
                }
                else
                {
                    FillDataGrid(DGEmployees, null, positionName);
                }
            }
            //ClearAndHideDG(DGDepartaments);
            ClearAndHideDG(DGVacations);
        }

        /// <summary>
        /// Выбор управления для отображения отделов в нем
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DGOffices_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid != null)
            {
                Offices officeItem = (dataGrid.SelectedItem) as Offices;

                if (officeItem != null)
                {
                    FillDataGrid(DGDepartaments, officeItem.OfficeID, null);
                }
            }
        }

        /// <summary>
        /// Выбор отдела для отображения сотрудников
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DGDepartaments_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid != null)
            {
                Deps depItem = dataGrid.SelectedItem as Deps;

                if (depItem != null)
                {
                    var depID = depItem.DepartamentID;
                    var position = CBPositions.SelectedItem.ToString();

                    FillDataGrid(DGEmployees, depID, position);
                }
            }
        }

        /// <summary>
        /// Выбор сотрудника для отображения его отпусков
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DGEmployees_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid != null)
            {
                Emps empItem = dataGrid.SelectedItem as Emps;

                if (empItem != null)
                {
                    var empID = empItem.EmployeeID;
                    var position = CBPositions.SelectedItem.ToString();

                    FillDataGrid(DGVacations, empID, position);

                }
            }
        }

        /// <summary>
        /// Показать всех сотрудников
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BShowAll_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => TCTables.SelectedIndex = 2));
            FillDataGrid(DGEmployees, null, CBPositions.SelectedItem.ToString());

            DGOffices.SelectedIndex = -1;
            ClearAndHideDG(DGDepartaments);
            ClearAndHideDG(DGVacations);
            //DGDepartaments.SelectedIndex = -1;
            //DGVacations.SelectedIndex = -1;
        }
        #endregion

        #region Навигация по вкладкам TabControl

        private void GoToFirstTab(object sender, ExecutedRoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => TCTables.SelectedIndex = 0));
        }

        private void GoToSecondTab(object sender, ExecutedRoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => TCTables.SelectedIndex = 1));
        }

        private void GoToThirdTab(object sender, ExecutedRoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => TCTables.SelectedIndex = 2));
        }

        private void GoToFourthTab(object sender, ExecutedRoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => TCTables.SelectedIndex = 3));
        }

        #endregion

        /// <summary>
        /// Авторизация пользователя для работы с БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BConnect_Click(object sender, RoutedEventArgs e)
        {
            AuthorizationWindow wnd = new AuthorizationWindow();
            //событие для определения режима пользователя        
            wnd.LogIn += value => admin = value;
            wnd.ShowDialog();
            ShowUserType();

            FillDataGrid(DGOffices, null, null);
            ClearAndHideDG(DGDepartaments);
            ClearAndHideDG(DGEmployees);
            ClearAndHideDG(DGVacations);
        }

        /// <summary>
        /// Обновление таблиц после изменения данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BUpdateLayout_Click(object sender, RoutedEventArgs e)
        {
            using (StaffDataBaseEntities tempEntity = new StaffDataBaseEntities())
            {
                Offices tempOffice = null;
                Deps tempDep = null;
                Emps tempEmp = null;
                Vacations tempVac = null;

                //обновим управления
                if (DGOffices.ItemsSource != null)
                {
                    if (DGOffices.SelectedItem != null)
                        tempOffice = DGOffices.SelectedItem as Offices;

                    DGOffices.ItemsSource = null;
                    FillDataGrid(DGOffices, null, null);

                    //обновим отделы
                    if(DGDepartaments.ItemsSource != null)
                    {
                        //проверяем, выбрано ли управление
                        if (tempOffice != null)
                        {
                            if (DGDepartaments.SelectedItem != null)
                                tempDep = DGDepartaments.SelectedItem as Deps;

                            DGDepartaments.ItemsSource = null;
                            FillDataGrid(DGDepartaments, tempOffice.OfficeID, null);
                            DGDepartaments.SelectedIndex = tempDep != null ? tempDep.DepartamentID - 1 : -1;

                            //обновим сотрудников
                            if (DGEmployees.ItemsSource != null)
                            {
                                //проверяем, выбран ли отдел
                                if(tempDep != null)
                                {
                                    if (DGEmployees.SelectedItem != null)
                                        tempEmp = DGEmployees.SelectedItem as Emps;

                                    DGEmployees.ItemsSource = null;
                                    FillDataGrid(DGEmployees, tempDep.DepartamentID, CBPositions.SelectedItem.ToString());
                                    DGEmployees.SelectedIndex = tempEmp != null ? tempEmp.EmployeeID - 1 : -1;

                                    //обновим отпуски
                                    if (DGVacations.ItemsSource != null)
                                    {
                                        if(tempEmp != null)
                                        {
                                            if (DGVacations.SelectedItem != null)
                                                tempVac = DGVacations.SelectedItem as Vacations;

                                            DGVacations.ItemsSource = null;
                                            FillDataGrid(DGVacations, tempEmp.EmployeeID, null);
                                            DGVacations.SelectedIndex = tempVac != null ? tempVac.VacationID - 1 : -1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if(DGEmployees.ItemsSource != null)
                        {
                            var qEmpAll = from emp in tempEntity.Emps
                                          orderby emp.Surname
                                          select emp;
                            DGEmployees.ItemsSource = null;
                            FillDataGrid(DGEmployees, null, CBPositions.SelectedItem.ToString());
                        }
                    }
                }
            }
        }

        private void ShowHelp(object sender, ExecutedRoutedEventArgs e)
        {
            var wnd = new HelpWindow();
            wnd.ShowDialog();
        }

        private void BBackup_Click(object sender, RoutedEventArgs e)
        {
            dataEntities.BackupDataBase();
        }

        private void BRestore_Click(object sender, RoutedEventArgs e)
        {
            dataEntities.RestoreDataBase();
        }
    }
}
