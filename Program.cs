using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public class Employee
{
    public int Id { get; set; }                  // Column 1
    public string FirstName { get; set; }        // Column 2
    public string LastName { get; set; }         // Column 3
    public string Department { get; set; }       // Column 4
    public string JobTitle { get; set; }         // Column 5
    public decimal MonthlySalary { get; set; }   // Column 6
    public double TaxRate { get; set; }          // Column 7 (e.g., 0.12 = 12%)
    public DateTime HireDate { get; set; }       // Column 8
    public bool IsActive { get; set; }           // Column 9
    public string Email { get; set; }            // Column 10

    public decimal NetSalary => MonthlySalary - (MonthlySalary * (decimal)TaxRate);
}

class Program
{
    // Starts EMPTY (0 pre-defined records)
    static List<Employee> employees = new List<Employee>();
    static int nextId = 1001;

    static void Main(string[] args)
    {
        bool running = true;
        while (running)
        {
            Console.Clear();
            Console.WriteLine("=== EPHRS - MAIN + BRANCH MERGED ===");
            Console.WriteLine("1. Create (Add Employee)");
            Console.WriteLine("2. Read (View All Employees)");
            Console.WriteLine("3. Search / Filter");
            Console.WriteLine("4. Update Employee");
            Console.WriteLine("5. Delete Employee");
            Console.WriteLine("6. Exit");
            Console.Write("\nSelect an option: ");
            string choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1": CreateEmployee(); break;
                case "2": ReadEmployees(); break;
                case "3": SearchEmployees(); break;
                case "4": UpdateEmployee(); break;
                case "5": DeleteEmployee(); break;
                case "6": running = false; break;
                default: Console.WriteLine("Invalid choice."); break;
            }

            if (running)
            {
                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
            }
        }
    }

    // ---------- CREATE ----------
    static void CreateEmployee()
    {
        Console.WriteLine("--- Add New Employee ---");
        Console.Write("First Name: ");
        string first = Console.ReadLine();
        Console.Write("Last Name: ");
        string last = Console.ReadLine();
        Console.Write("Department (IT, HR, Sales, Finance...): ");
        string dept = Console.ReadLine();
        Console.Write("Job Title: ");
        string title = Console.ReadLine();

        if (!decimal.TryParse(Prompt("Monthly Salary: "), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal salary) || salary < 0)
        {
            Console.WriteLine("\nInvalid salary. Employee not added.");
            return;
        }
        if (!double.TryParse(Prompt("Tax Rate (e.g., 0.12 for 12%): "), NumberStyles.Float, CultureInfo.InvariantCulture, out double tax) || tax < 0 || tax > 1)
        {
            Console.WriteLine("\nInvalid tax rate (must be 0 to 1). Employee not added.");
            return;
        }
        if (!DateTime.TryParse(Prompt("Hire Date (yyyy-MM-dd): "), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime hire))
        {
            Console.WriteLine("\nInvalid date. Employee not added.");
            return;
        }
        string activeInput = Prompt("Is Active? (y/n): ").Trim().ToLower();
        bool isActive = activeInput == "y" || activeInput == "yes" || activeInput == "true";
        Console.Write("Email: ");
        string email = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(last))
        {
            Console.WriteLine("\nFirst and last name are required. Employee not added.");
            return;
        }

        employees.Add(new Employee
        {
            Id = nextId++,
            FirstName = first.Trim(),
            LastName = last.Trim(),
            Department = dept.Trim(),
            JobTitle = title.Trim(),
            MonthlySalary = salary,
            TaxRate = tax,
            HireDate = hire,
            IsActive = isActive,
            Email = email.Trim()
        });

        Console.WriteLine("\nEmployee added successfully!");
    }

    // ---------- READ ----------
    static void ReadEmployees()
    {
        Console.WriteLine("--- Employee Records ---");
        DisplayEmployees(employees.OrderBy(e => e.Id));
    }

    // ---------- SEARCH / FILTER ----------
    static void SearchEmployees()
    {
        Console.WriteLine("--- Search / Filter ---");
        Console.WriteLine("1. By name (first or last)");
        Console.WriteLine("2. By department");
        Console.WriteLine("3. By minimum monthly salary");
        Console.WriteLine("4. By status (active / inactive)");
        Console.Write("\nChoose filter: ");
        string opt = Console.ReadLine();

        IEnumerable<Employee> results = Enumerable.Empty<Employee>();
        switch (opt)
        {
            case "1":
                string name = Prompt("Enter name keyword: ").Trim();
                results = employees.Where(e =>
                    e.FirstName.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                    e.LastName.Contains(name, StringComparison.OrdinalIgnoreCase));
                break;
            case "2":
                string dept = Prompt("Enter department: ").Trim();
                results = employees.Where(e =>
                    e.Department.Equals(dept, StringComparison.OrdinalIgnoreCase));
                break;
            case "3":
                if (!decimal.TryParse(Prompt("Minimum salary: "), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal min))
                {
                    Console.WriteLine("\nInvalid amount.");
                    return;
                }
                results = employees.Where(e => e.MonthlySalary >= min).OrderByDescending(e => e.MonthlySalary);
                break;
            case "4":
                string s = Prompt("Active? (y/n): ").Trim().ToLower();
                bool active = s == "y" || s == "yes";
                results = employees.Where(e => e.IsActive == active);
                break;
            default:
                Console.WriteLine("\nInvalid filter.");
                return;
        }

        Console.WriteLine();
        DisplayEmployees(results);
    }

    // ---------- UPDATE ----------
    static void UpdateEmployee()
    {
        ReadEmployees();
        if (!employees.Any()) return;

        if (!int.TryParse(Prompt("\nEnter the ID of the employee to update: "), out int id))
        {
            Console.WriteLine("\nInvalid ID format.");
            return;
        }

        Employee emp = employees.FirstOrDefault(e => e.Id == id);
        if (emp == null)
        {
            Console.WriteLine("\nEmployee not found.");
            return;
        }

        Console.WriteLine("(Press Enter to keep the current value)");

        string v = Prompt($"First Name ({emp.FirstName}): ");
        if (!string.IsNullOrWhiteSpace(v)) emp.FirstName = v.Trim();

        v = Prompt($"Last Name ({emp.LastName}): ");
        if (!string.IsNullOrWhiteSpace(v)) emp.LastName = v.Trim();

        v = Prompt($"Department ({emp.Department}): ");
        if (!string.IsNullOrWhiteSpace(v)) emp.Department = v.Trim();

        v = Prompt($"Job Title ({emp.JobTitle}): ");
        if (!string.IsNullOrWhiteSpace(v)) emp.JobTitle = v.Trim();

        v = Prompt($"Monthly Salary ({emp.MonthlySalary:N2}): ");
        if (decimal.TryParse(v, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal sal) && sal >= 0)
            emp.MonthlySalary = sal;

        v = Prompt($"Tax Rate ({emp.TaxRate}): ");
        if (double.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out double tax) && tax >= 0 && tax <= 1)
            emp.TaxRate = tax;

        v = Prompt($"Hire Date ({emp.HireDate:yyyy-MM-dd}): ");
        if (DateTime.TryParse(v, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime hd))
            emp.HireDate = hd;

        v = Prompt($"Is Active? y/n ({(emp.IsActive ? "y" : "n")}): ").Trim().ToLower();
        if (v == "y" || v == "yes") emp.IsActive = true;
        else if (v == "n" || v == "no") emp.IsActive = false;

        v = Prompt($"Email ({emp.Email}): ");
        if (!string.IsNullOrWhiteSpace(v)) emp.Email = v.Trim();

        Console.WriteLine("\nEmployee updated successfully!");
    }

    // ---------- DELETE ----------
    static void DeleteEmployee()
    {
        ReadEmployees();
        if (!employees.Any()) return;

        if (!int.TryParse(Prompt("\nEnter the ID of the employee to delete: "), out int id))
        {
            Console.WriteLine("\nInvalid ID format.");
            return;
        }

        int removed = employees.RemoveAll(e => e.Id == id);
        Console.WriteLine(removed > 0 ? "\nEmployee deleted successfully." : "\nEmployee not found.");
    }

    // ---------- HELPERS ----------
    static string Prompt(string text)
    {
        Console.Write(text);
        return Console.ReadLine() ?? "";
    }

    static void DisplayEmployees(IEnumerable<Employee> list)
    {
        var items = list.ToList();
        if (items.Count == 0)
        {
            Console.WriteLine("No employees found.");
            return;
        }

        foreach (var e in items)
        {
            Console.WriteLine($"ID: {e.Id}");
            Console.WriteLine($"  Name       : {e.FirstName} {e.LastName}");
            Console.WriteLine($"  Department : {e.Department}");
            Console.WriteLine($"  Job Title  : {e.JobTitle}");
            Console.WriteLine($"  Salary     : {e.MonthlySalary:N2}   Tax: {e.TaxRate:P0}   Net: {e.NetSalary:N2}");
            Console.WriteLine($"  Hire Date  : {e.HireDate:yyyy-MM-dd}");
            Console.WriteLine($"  Status     : {(e.IsActive ? "Active" : "On Leave/Inactive")}");
            Console.WriteLine($"  Email      : {e.Email}");
            Console.WriteLine(new string('-', 50));
        }
        Console.WriteLine($"Total: {items.Count} record(s)");
    }
}