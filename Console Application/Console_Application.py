#!/usr/bin/env python

import pyodbc
import os
import sys

#для очистки консоли
clear = lambda: os.system('cls')
pause = lambda: os.system('pause')

#подключаемся к базе данных
conn = pyodbc.connect(r'DRIVER={SQL Server};SERVER=DESKTOP-KFOGMK4\SQLEXPRESS;DATABASE=StaffDataBase;')
cursor = conn.cursor()

#показать отпуски сорудника
def show_employee_vacations(id):
    vacations = []
    sql = "{call dbo.GetEmployeeVacations(?)}"
    params = id
    rows = cursor.execute(sql, params).fetchall()
    for row in rows:
        vacations.append("С " + str(row[0]) + " по " + str(row[1]))
        #row = cursor.fetchone()
    if len(vacations) > 0:
       return "\n".join(vacations)
    else:
       return "" 
    
#показать всех сотрудников
def show_all_employees():
    rows = cursor.execute("{call dbo.GetAllEmployees}").fetchall()
    for row in rows:
        vacs = show_employee_vacations(int(row[0]))
        print("ID: " + str(row[0]) + "\n" + "Имя: " + str(row[1]) + "\n" + "Фамилия: " + str(row[2]) + "\n" + "Отчество: " + str(row[3]) + "\n" + "Количество отпусков: " + str(row[4]) + "\n" + vacs + "\n"
              "Должность: " + str(row[5]) + "\n" + "Отдел: " + str(row[6]) + "\n") 
        print("-------------------------")
        #row = cursor.fetchone()
    operation = int(input("Введите 0, чтобы вернуться в главное меню: "))
    options[operation]()

#показать сотрудника по номеру
def show_employee_by_id(id):
    sql = "{ call dbo.GetEmployeeByID (?) }"
    params = id
    rows = cursor.execute(sql, params) 
    row = cursor.fetchone()
    while row:
        print("ID: " + str(row[0]) + "\n" + "Имя: " + str(row[1]) + "\n" + "Фамилия: " + str(row[2]) + "\n" + "Отчество: " + str(row[3]) + "\n" + "Количество отпусков: " + str(row[4]) + "\n" + "Должность: " + str(row[5]) + "\n" + "Отдел: " + str(row[6]) + "\n")  
        print("-------------------------")
        row = cursor.fetchone()  
    op = input("Введите 0, чтобы вернуться в главное меню: ")
    try: 
        operation = int(op)
        options[operation]()
    except ValueError:
        print("Введите число!")
        pause()
        main()

#показать сотрудника по имени
def show_employee_by_name(surname, name, patronymic):
    sql = "{call dbo.GetEmployeeByName(?,?,?) }"
    params = (surname, name, patronymic)
    rows = cursor.execute(sql, params)
    row = cursor.fetchone()
    while row:
        print("ID: " + str(row[0]) + "\n" + 
              "Имя: " + str(row[1]) + "\n" + 
              "Фамилия: " + str(row[2]) + "\n" + 
              "Отчество: " + str(row[3]) + "\n" + 
              "Количество отпусков: " + str(row[4]) + "\n" + 
              "Должность: " + str(row[5]) + "\n" + 
              "Отдел: " + str(row[6]) + "\n")
        print("-------------------------")
        row = cursor.fetchone()    
    op = input("Введите 0, чтобы вернуться в главное меню: ")
    try: 
        operation = int(op)
        options[operation]()
    except ValueError:
        print("Введите число!")
        pause()
        main()

#добавление сотрудника
def add_employee(name, surname, patronymic, pos, dep):
    sql = "{call dbo.InsertEmployee(?,?,?,?,?)}"
    params = (name,surname,patronymic,pos,dep)
    cursor.execute(sql, params)
    op = input("Введите 0, чтобы вернуться в главное меню: ")
    try: 
        operation = int(op)
        options[operation]()
    except ValueError:
        print("Введите число!")
        pause()
        main()

#удаление сотрудника
def delete_employee(id):
    sql = "{call dbo.DeleteEmployee(?)}"
    params = id
    cursor.execute(sql, params)
    operation = int(input("Введите 0, чтобы вернуться в главное меню: "))
    options[operation]()

#показать все отделы
def show_all_deps():
    sql = "{call dbo.GetAllDepartaments}"
    rows = cursor.execute(sql)
    row = cursor.fetchone()
    while row:
        print("ID: " + str(row[0]) + "\n" + 
                "Название: " + str(row[1]) + "\n" + 
                "Количество сотрудников: " + str(row[2]))
        print("-------------------------")
        row = cursor.fetchone()    
    op = input("Введите 0, чтобы вернуться в главное меню: ")
    try: 
        operation = int(op)
        options[operation]()
    except ValueError:
        print("Введите число!")
        pause()
        main()
    operation = int(input("Введите 0, чтобы вернуться в главное меню: "))
    options[operation]()

def main():
    clear()
    print("1. Показать всех сотрудников")
    print("2. Показать сотрудника по имени")
    print("3. Показать сотрудника по ID")
    print("4. Добавить сотрудника")
    print("5. Удалить сотрудника")
    print("6. Показать все отделы")
    print("----------------------------------------")
    print("Введите 100, чтобы выйти из программы")
    print("----------------------------------------")
    
    #обрабатываем выбранные операции
    op = input("Введите число, соответствующее операции: ")
    try: 
        operation = int(op)
    except ValueError:
        print("Введите число!")
        pause()
        main()

    if operation == 1: 
        clear()
        show_all_employees()
    elif operation == 2:
        clear()
        s = str(input("Введите фамилию: "))
        n = str(input("Введие имя: "))
        p = str(input("Введите отчество: "))
        show_employee_by_name(s,n,p)
    elif operation == 3:
        clear()
        a = int(input("Введите код сотрудника: "))
        show_employee_by_id(a)
    elif operation == 4:
        clear()       
        s = str(input("Введите фамилию: "))
        n = str(input("Введие имя: "))
        p = str(input("Введите отчество: ")) 
        pos = int(input("Введите код должности: "))
        dep = int(input("Введите код отдела: "))       
        yes = input("Введите 1, чтобы продолжить или 0, чтобы отменить: ")
        try:
            operation = int(yes)
            if operation == 1:
                add_employee(n,s,p,pos,dep)
            elif operation == 0:
                main()
            elif operation != 1 or operation != 0:
                raise ValueError()
        except ValueError:
            print("Введите предложенное число!")
            pause()
            main()
    elif operation == 5:
        clear()
        a = input("Введите код удаляемого сотрудника: ")
        try:
            emp_code = int(a)
            yes = input("Введите 1, чтобы продолжить или 0, чтобы отменить: ")
            operation = int(yes)
            if operation == 1:
                 delete_employee(emp_code)
            elif operation == 0:
                main()
            elif operation != 1 or operation != 0:
                raise ValueError()
        except ValueError:
            print("Введите предложенное число!")
            pause()
            main()
    elif operation == 6:
        clear()
        show_all_deps()
    elif operation == 100:
        sys.exit()
        
         

options = { 0 : main,
            1 : show_all_employees,
}

if __name__ == "__main__":
    main()