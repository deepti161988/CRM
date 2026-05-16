using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//==============================Adding below Library==========
using SharedMetadata;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
//===========================
//using Microsoft.Xrm.Tooling.Connector;
//Svc Client
using Microsoft.PowerPlatform.Dataverse.Client;
using OfficeOpenXml;
using System.IO;
using Microsoft.Xrm.Sdk.Query;

namespace ABC_ConsoleApp
{
    class ProgramPOC
    {
        public static string dev_connectionString = @"
                                        AuthType=OAuth;
                                        Url=https://org843d3fcc.crm.dynamics.com;
                                        AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;
                                        RedirectUri=http://localhost;
                                        LoginPrompt=Auto;";
        static void Main1(string[] args)
        {
            ServiceClient devservice = new ServiceClient(dev_connectionString);
            if (devservice.IsReady)
            {
                Console.WriteLine("Dev CRM Connected successfully!");
            }
            else
            {
                Console.WriteLine("Dev CRM Connected successfully!");
                return;
            }

            Console.WriteLine("Dev CRM Connected successfully!");


            Console.WriteLine("\n===== MENU =====");
            Console.WriteLine("1. Create Deep Insert Way");
            Console.WriteLine("2. Create Employee");
            Console.WriteLine("3. Exit");
            Console.Write("Choose an option: ");

            int choice = Console.Read();

            switch (choice)
            {
                case 1:
                    CreateDeepInsert(devservice);
                    break;

                case 2:
                    CreateEmployee(devservice);
                    break;
                case 3:
                    Console.WriteLine("Exiting...");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }

        }

        public static void CreateDeepInsert(ServiceClient devservice)
        { 
          Account acc = new Account();
acc.Name = "ABC Technologies";
acc.Telephone1 = "9999999999";
acc.WebSiteURL = "https://abc.com";

// =========================
// 🔹 CHILD CONTACT 1
// =========================
Contact contact1 = new Contact();
contact1.FirstName = "Amit";
contact1.LastName = "Sharma";
contact1.EMailAddress1 = "amit@abc.com";

// Link to parent account (VERY IMPORTANT)
contact1.ParentCustomerId = new EntityReference("account", Guid.Empty); // placeholder

// =========================
// 🔹 CHILD CONTACT 2
// =========================
Contact contact2 = new Contact();
contact2.FirstName = "Ravi";
contact2.LastName = "Kumar";
contact2.EMailAddress1 = "ravi@abc.com";
contact2.ParentCustomerId = new EntityReference("account", Guid.Empty);

// =========================
// 🔹 ADD CHILDREN TO ACCOUNT
// =========================
 

// =========================
// 🔹 CREATE (DEEP INSERT HAPPENS HERE)
// =========================
Guid accountId = devservice.Create(acc);

Console.WriteLine("Account + Contacts created successfully!");
Console.WriteLine("Account ID: " + accountId);
        }

        public static void CreateEmployee(ServiceClient devservice)
        {
            try
            {
                abc_Employee emp = new abc_Employee();

                // =========================
                // 🔹 BASIC FIELDS
                // =========================
                emp.abc_EmployeeName = "Amit Sharma";
                emp.abc_Address = "Pune";
                emp.abc_City = "Pune";
                emp.abc_email = "amit@test.com";
                // =========================
                // 🔹 NUMERIC
                // =========================
                emp.abc_ContactNo = 987654321;
                emp.abc_experience = 5;
                emp.abc_Salary = 50000;
                emp.abc_GrossSalary = 60000;
                // =========================
                // 🔹 BOOLEAN
                // =========================
                emp.abc_Gender = true;
                emp.abc_isEmployeeInMetroCity = true;
                // =========================
                // 🔹 DATE
                // =========================
                emp.abc_EmpDob = new DateTime(1999, 5, 21);
                emp.abc_JoiningDate = new DateTime(2025, 7, 25);

                // =========================
                // 🔹 OPTION SET (IMPORTANT)
                // =========================
                // Preferred (if early-bound enums exist)
                //emp.abc_Hobby = abc_employee_abc_hobby.Cricket;     // change value

                // =========================
                // 🔹 LOOKUP (STATE)
                // =========================
                Guid stateId = new Guid("");
                if (stateId != Guid.Empty)
                {
                    emp.abc_StateName = new EntityReference("abc_state", stateId);
                }

                // =========================
                // 🔹 LOOKUP (DISTRICT)
                // =========================
                Guid districtId = new Guid("");
                if (districtId != Guid.Empty)
                {
                    emp.abc_DistrictName = new EntityReference("abc_district", districtId);
                }

                // =========================
                // 🔹 IMAGE FIELD
                // =========================
                string imgPath = @"C:\Temp\profile.jpg";

                //if (File.Exists(imgPath))
                //{
                //    emp.abc_profileimage = File.ReadAllBytes(imgPath);
                //}

                // =========================
                // 🔹 CREATE RECORD
                // =========================
                Guid empId = devservice.Create(emp);

                Console.WriteLine("Employee Created Successfully!");
                Console.WriteLine("Employee ID: " + empId);
            }
            catch (Exception e )
            { }


        }
    }
}

           