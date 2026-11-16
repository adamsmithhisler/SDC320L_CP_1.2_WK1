
// Author: Adam Smithhisler
// Course: SDC320L
// Date: November 15, 2026
// Purpose: Employee Management System - Week 1
// Demonstrates inheritance, composition, and console interaction.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

// BASE CLASS: Contains information common to all employees.
public class Employee
{
    public int EmployeeId { get; set; }
    public string FullName { get; set; } = "";
    public string Department { get; set; } = "";
    public DateTime HireDate { get; set; }

    // COMPOSITION: An employee HAS a ContactInformation object.
    public ContactInformation Contact { get; set; } = new();

    public virtual string EmployeeType => "Employee";
}

// COMPOSITION CLASS: Stores contact details for an employee.
public class ContactInformation
{
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
}

// INHERITANCE: AdministrativeEmployee IS an Employee.
public class AdministrativeEmployee : Employee
{
    public string JobTitle { get; set; } = "";
    public string OfficeLocation { get; set; } = "";
    public decimal AnnualSalary { get; set; }

    public override string EmployeeType => "Administrative";
}

// INHERITANCE: ShopFloorEmployee IS an Employee.
public class ShopFloorEmployee : Employee
{
    public string WorkShift { get; set; } = "";
    public string ProductionArea { get; set; } = "";
    public decimal HourlyRate { get; set; }

    public override string EmployeeType => "Shop Floor";
}

// INHERITANCE: TemporaryEmployee IS an Employee.
public class TemporaryEmployee : Employee
{
    public string StaffingAgency { get; set; } = "";
    public DateTime ContractStart { get; set; }
    public DateTime ContractEnd { get; set; }

    public override string EmployeeType => "Temporary";
}

internal static class Program
{
    private static readonly List<Employee> Employees = new()
    {
        new AdministrativeEmployee
        {
            EmployeeId = 1001,
            FullName = "Sarah Miller",
            Department = "Production Planning",
            HireDate = new DateTime(2020, 4, 14),
            Contact = new ContactInformation
            {
                Phone = "402-555-0101",
                Email = "sarah@example.com"
            },
            JobTitle = "Production Planner",
            OfficeLocation = "Office A-201",
            AnnualSalary = 73500m
        },
        new ShopFloorEmployee
        {
            EmployeeId = 1002,
            FullName = "James Wilson",
            Department = "Fabrication",
            HireDate = new DateTime(2022, 7, 19),
            Contact = new ContactInformation
            {
                Phone = "402-555-0102",
                Email = "james@example.com"
            },
            WorkShift = "First Shift",
            ProductionArea = "Laser Cutting",
            HourlyRate = 28.50m
        },
        new TemporaryEmployee
        {
            EmployeeId = 1003,
            FullName = "Michael Davis",
            Department = "Assembly",
            HireDate = new DateTime(2026, 10, 1),
            Contact = new ContactInformation
            {
                Phone = "402-555-0103",
                Email = "michael@example.com"
            },
            StaffingAgency = "Premier Staffing",
            ContractStart = new DateTime(2026, 10, 1),
            ContractEnd = new DateTime(2027, 1, 31)
        }
    };

    private static int nextEmployeeId = 1004;

    static void Main()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n=====================================");
            Console.WriteLine("      EMPLOYEE MANAGEMENT SYSTEM");
            Console.WriteLine(" SDC320L | WEEK 1 | ADAM SMITHHISLER");
            Console.WriteLine("=====================================");
            Console.WriteLine("Welcome! Select an option below.");
            Console.WriteLine();
            Console.WriteLine("1. Add a new employee");
            Console.WriteLine("2. Remove an employee");
            Console.WriteLine("3. Update employee information");
            Console.WriteLine("4. Display all employees");
            Console.WriteLine("5. Filter employees by type");
            Console.WriteLine("6. View employee details by ID");
            Console.WriteLine("0. Exit application");

            Console.Write("\nEnter your selection: ");
            string? choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1": AddEmployee(); break;
                case "2": RemoveEmployee(); break;
                case "3": UpdateEmployee(); break;
                case "4": DisplayEmployees(Employees); break;
                case "5": FilterEmployees(); break;
                case "6": ViewEmployee(); break;
                case "0":
                    if (Confirm("Are you sure you want to exit?"))
                        running = false;
                    break;
                default:
                    Console.WriteLine("Invalid menu selection.");
                    break;
            }

            if (choice == null)
                running = false;
        }

        Console.WriteLine("Thank you for using the Employee Management System.");
    }

    // Creates an employee based on the selected category.
    static void AddEmployee()
    {
        string? type = SelectType();
        if (type == null) return;

        // INHERITANCE: A derived employee is assigned to
        // an Employee base-class reference.
        Employee employee = type switch
        {
            "Administrative" => new AdministrativeEmployee(),
            "Shop Floor" => new ShopFloorEmployee(),
            _ => new TemporaryEmployee()
        };

        employee.FullName = Required("Full name");
        employee.Department = Required("Department");
        employee.HireDate = ReadDate("Hire date (YYYY-MM-DD)");

        // COMPOSITION: Create the employee's contact object.
        employee.Contact = new ContactInformation
        {
            Phone = Required("Phone number"),
            Email = ReadEmail("Email address")
        };

        switch (employee)
        {
            case AdministrativeEmployee admin:
                admin.JobTitle = Required("Job title");
                admin.OfficeLocation = Required("Office location");
                admin.AnnualSalary = ReadMoney("Annual salary");
                break;

            case ShopFloorEmployee shop:
                shop.WorkShift = Required("Work shift");
                shop.ProductionArea = Required("Production area");
                shop.HourlyRate = ReadMoney("Hourly rate");
                break;

            case TemporaryEmployee temp:
                temp.StaffingAgency = Required("Staffing agency");
                temp.ContractStart = ReadDate("Contract start (YYYY-MM-DD)");
                temp.ContractEnd = ReadContractEnd(temp.ContractStart);
                break;
        }

        Console.WriteLine("\nReview the new employee:");
        DisplayDetails(employee);

        if (Confirm("Save this employee?"))
        {
            employee.EmployeeId = nextEmployeeId++;
            Employees.Add(employee);
            Console.WriteLine($"Employee added. ID: {employee.EmployeeId}");
        }
        else
        {
            Console.WriteLine("Addition canceled.");
        }
    }

    // Removes a record after confirming the employee ID.
    static void RemoveEmployee()
    {
        Employee? employee = FindEmployee();
        if (employee == null) return;

        DisplayDetails(employee);

        if (Confirm("Remove this employee?"))
        {
            Employees.Remove(employee);
            Console.WriteLine("Employee removed successfully.");
        }
        else
        {
            Console.WriteLine("Removal canceled.");
        }
    }

    // Allows the user to update common or category-specific fields.
    static void UpdateEmployee()
    {
        Employee? employee = FindEmployee();
        if (employee == null) return;

        DisplayDetails(employee);

        Console.WriteLine("\nSelect the field to update:");
        Console.WriteLine("1. Full name");
        Console.WriteLine("2. Department");
        Console.WriteLine("3. Hire date");
        Console.WriteLine("4. Phone number");
        Console.WriteLine("5. Email address");

        if (employee is AdministrativeEmployee)
        {
            Console.WriteLine("6. Job title");
            Console.WriteLine("7. Office location");
            Console.WriteLine("8. Annual salary");
        }
        else if (employee is ShopFloorEmployee)
        {
            Console.WriteLine("6. Work shift");
            Console.WriteLine("7. Production area");
            Console.WriteLine("8. Hourly rate");
        }
        else
        {
            Console.WriteLine("6. Staffing agency");
            Console.WriteLine("7. Contract start");
            Console.WriteLine("8. Contract end");
        }

        Console.WriteLine("0. Cancel");
        Console.Write("Selection: ");
        string? field = Console.ReadLine()?.Trim();

        // Prepare the change without modifying the record yet.
        Action? applyChange = field switch
        {
            "1" => TextChange(v => employee.FullName = v, "Full name"),
            "2" => TextChange(v => employee.Department = v, "Department"),
            "3" => DateChange(v => employee.HireDate = v, "Hire date"),
            "4" => TextChange(v => employee.Contact.Phone = v, "Phone"),
            "5" => EmailChange(v => employee.Contact.Email = v, "Email"),
            "6" or "7" or "8" => CategoryChange(employee, field),
            _ => null
        };

        if (applyChange == null)
        {
            Console.WriteLine("Update canceled or invalid selection.");
            return;
        }

        if (Confirm("Save this change?"))
        {
            applyChange();
            Console.WriteLine("Employee updated successfully.");
            DisplayDetails(employee);
        }
        else
        {
            Console.WriteLine("Update canceled.");
        }
    }

    static Action? CategoryChange(Employee employee, string field)
    {
        if (employee is AdministrativeEmployee admin)
        {
            return field switch
            {
                "6" => TextChange(v => admin.JobTitle = v, "Job title"),
                "7" => TextChange(v => admin.OfficeLocation = v, "Office location"),
                "8" => MoneyChange(v => admin.AnnualSalary = v, "Annual salary"),
                _ => null
            };
        }

        if (employee is ShopFloorEmployee shop)
        {
            return field switch
            {
                "6" => TextChange(v => shop.WorkShift = v, "Work shift"),
                "7" => TextChange(v => shop.ProductionArea = v, "Production area"),
                "8" => MoneyChange(v => shop.HourlyRate = v, "Hourly rate"),
                _ => null
            };
        }

        if (employee is TemporaryEmployee temp)
        {
            switch (field)
            {
                case "6":
                    return TextChange(v => temp.StaffingAgency = v, "Staffing agency");
                case "7":
                    DateTime start = ReadDate("Contract start (YYYY-MM-DD)");
                    if (start > temp.ContractEnd)
                    {
                        Console.WriteLine("Start cannot be after contract end.");
                        return null;
                    }
                    return () => temp.ContractStart = start;
                case "8":
                    DateTime end = ReadContractEnd(temp.ContractStart);
                    return () => temp.ContractEnd = end;
            }
        }

        return null;
    }

    static Action TextChange(Action<string> setter, string label)
    {
        string value = Required(label);
        return () => setter(value);
    }

    static Action EmailChange(Action<string> setter, string label)
    {
        string value = ReadEmail(label);
        return () => setter(value);
    }

    static Action DateChange(Action<DateTime> setter, string label)
    {
        DateTime value = ReadDate(label + " (YYYY-MM-DD)");
        return () => setter(value);
    }

    static Action MoneyChange(Action<decimal> setter, string label)
    {
        decimal value = ReadMoney(label);
        return () => setter(value);
    }

    // Displays all records in a formatted table.
    static void DisplayEmployees(IEnumerable<Employee> employees)
    {
        var results = employees.OrderBy(e => e.EmployeeId).ToList();

        Console.WriteLine("\nEMPLOYEE DIRECTORY");
        Console.WriteLine($"{"ID",-8} {"NAME",-22} {"TYPE",-17} {"DEPARTMENT",-22}");
        Console.WriteLine(new string('-', 72));

        if (results.Count == 0)
        {
            Console.WriteLine("No employee records match the selected criteria.");
            return;
        }

        foreach (Employee employee in results)
        {
            Console.WriteLine(
                $"{employee.EmployeeId,-8} " +
                $"{employee.FullName,-22} " +
                $"{employee.EmployeeType,-17} " +
                $"{employee.Department,-22}");
        }
    }

    // Filters records by one of the three supported types.
    static void FilterEmployees()
    {
        string? type = SelectType();
        if (type == null) return;

        DisplayEmployees(
            Employees.Where(e => e.EmployeeType == type));
    }

    static void ViewEmployee()
    {
        Employee? employee = FindEmployee();
        if (employee != null)
            DisplayDetails(employee);
    }

    static Employee? FindEmployee()
    {
        Console.Write("\nEmployee ID (0 to cancel): ");
        string? input = Console.ReadLine();

        if (input == "0") return null;

        if (!int.TryParse(input, out int id))
        {
            Console.WriteLine("Invalid employee ID.");
            return null;
        }

        Employee? employee = Employees.FirstOrDefault(e => e.EmployeeId == id);

        if (employee == null)
            Console.WriteLine("No employee found with that ID.");

        return employee;
    }

    static void DisplayDetails(Employee employee)
    {
        Console.WriteLine("\nEMPLOYEE DETAILS");
        Console.WriteLine($"ID: {(employee.EmployeeId == 0 ? "Pending" : employee.EmployeeId)}");
        Console.WriteLine($"Name: {employee.FullName}");
        Console.WriteLine($"Type: {employee.EmployeeType}");
        Console.WriteLine($"Department: {employee.Department}");
        Console.WriteLine($"Hire date: {employee.HireDate:yyyy-MM-dd}");
        Console.WriteLine($"Phone: {employee.Contact.Phone}");
        Console.WriteLine($"Email: {employee.Contact.Email}");

        switch (employee)
        {
            case AdministrativeEmployee admin:
                Console.WriteLine($"Job title: {admin.JobTitle}");
                Console.WriteLine($"Office: {admin.OfficeLocation}");
                Console.WriteLine($"Annual salary: {admin.AnnualSalary:C}");
                break;

            case ShopFloorEmployee shop:
                Console.WriteLine($"Shift: {shop.WorkShift}");
                Console.WriteLine($"Production area: {shop.ProductionArea}");
                Console.WriteLine($"Hourly rate: {shop.HourlyRate:C}");
                break;

            case TemporaryEmployee temp:
                Console.WriteLine($"Staffing agency: {temp.StaffingAgency}");
                Console.WriteLine($"Contract start: {temp.ContractStart:yyyy-MM-dd}");
                Console.WriteLine($"Contract end: {temp.ContractEnd:yyyy-MM-dd}");
                break;
        }
    }

    static string? SelectType()
    {
        Console.WriteLine("\nEMPLOYEE TYPES");
        Console.WriteLine("1. Administrative");
        Console.WriteLine("2. Shop Floor");
        Console.WriteLine("3. Temporary");
        Console.WriteLine("0. Back");
        Console.Write("Selection: ");

        return Console.ReadLine()?.Trim() switch
        {
            "1" => "Administrative",
            "2" => "Shop Floor",
            "3" => "Temporary",
            _ => null
        };
    }

    // Input validation helpers.
    static string Required(string label)
    {
        while (true)
        {
            Console.Write($"{label}: ");
            string? value = Console.ReadLine()?.Trim();

            if (!string.IsNullOrWhiteSpace(value))
                return value;

            Console.WriteLine("A value is required.");
        }
    }

    static string ReadEmail(string label)
    {
        while (true)
        {
            string value = Required(label);

            if (value.Contains('@') && value.Contains('.'))
                return value;

            Console.WriteLine("Enter a valid email address.");
        }
    }

    static DateTime ReadDate(string label)
    {
        while (true)
        {
            Console.Write($"{label}: ");
            string? value = Console.ReadLine();

            if (DateTime.TryParseExact(
                value,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime date))
                return date;

            Console.WriteLine("Use YYYY-MM-DD format.");
        }
    }

    static DateTime ReadContractEnd(DateTime start)
    {
        while (true)
        {
            DateTime end = ReadDate("Contract end (YYYY-MM-DD)");

            if (end >= start) return end;

            Console.WriteLine("Contract end cannot be before contract start.");
        }
    }

    static decimal ReadMoney(string label)
    {
        while (true)
        {
            Console.Write($"{label}: ");
            string? value = Console.ReadLine();

            if (decimal.TryParse(value, out decimal amount) && amount > 0)
                return amount;

            Console.WriteLine("Enter a positive numeric amount.");
        }
    }

    static bool Confirm(string message)
    {
        while (true)
        {
            Console.Write($"{message} (Y/N): ");
            string? input = Console.ReadLine()?.Trim().ToUpperInvariant();

            if (input == "Y" || input == "YES") return true;
            if (input == "N" || input == "NO") return false;

            Console.WriteLine("Please enter Y or N.");
        }
    }
}
