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
    class Program
    {
        static void Main(string[] args)
        {
            string dev_connectionString = @"
                                        AuthType=OAuth;
                                        Url=https://org843d3fcc.crm.dynamics.com;
                                        AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;
                                        RedirectUri=http://localhost;
                                        LoginPrompt=Auto;";
            string sit_connectionString = @"
                                        AuthType=OAuth;
                                        Url=https://orgc3bb4d32.crm.dynamics.com;
                                        AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;
                                        RedirectUri=http://localhost;
                                        LoginPrompt=Auto;";
            ServiceClient devservice = new ServiceClient(dev_connectionString);
            ServiceClient sitservice = new ServiceClient(sit_connectionString);
            if (devservice.IsReady)
            {
                Console.WriteLine("Dev CRM Connected successfully!");
                while (true)
                {
                    Console.WriteLine("\n===== MENU =====");
                    Console.WriteLine("1. Create Contact");
                    Console.WriteLine("2. Create Account from Excel");
                    Console.WriteLine("3. Sync Accounts (DEV CRM → SIT CRM)");
                    Console.WriteLine("4. Create Employee");
                    Console.WriteLine("5. Exit");
                    Console.Write("Choose an option: ");
                    string choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            CreateContactOption(devservice);
                            break;

                        case "2":
                            string filePath = @"C:\Deepti\CRM\CRMIntegration\Source\SAPAccounts.xlsx";
                            CreateAccountsFromExcel(devservice, filePath);
                            break;

                        case "3":
                            SyncAccounts(devservice, sitservice);
                            break;

                        case "4":
                            CreateEmployee(devservice);
                            break;

                        case "5":
                            Console.WriteLine("Exiting...");
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Try again.");
                            break;
                    }
                }
            }
        }

            
        public static void CreateContactOption(ServiceClient service)
        { 
            Console.WriteLine("createcontacts");
            string filePath = @"C:\Deepti\CRM\CRMIntegration\Source\SAPAccounts.xlsx";
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Excel file not found at: " + filePath);
                Console.ReadLine();
                return;
            }
            ExcelPackage.License.SetNonCommercialPersonal("Deepti");
            if (!service.IsReady)
            {
                Console.WriteLine("CRM connection failed: " + service.LastError);
                return;
            }
            Console.WriteLine("For Contact Creation , CRM Connected successfully!");
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var sheet = package.Workbook.Worksheets[0];
                int rowCount = sheet.Dimension.Rows;
                // 👉 Start from row 2 (skip header)
                for (int row = 2; row <= rowCount; row++)
                {
                    string contactName = sheet.Cells[row, 5].Text;   // Column 5
                    string contactEmail = sheet.Cells[row, 6].Text;  // Column 6
                                                                     // 🔹 Skip empty rows
                    if (string.IsNullOrWhiteSpace(contactName) &&
                        string.IsNullOrWhiteSpace(contactEmail))
                    {
                        continue;
                    }
                    try
                    {
                        // ===========================
                        // 🔹 1. Check if exists by email
                        // ===========================
                        QueryExpression query = new QueryExpression("contact")
                        {
                            ColumnSet = new ColumnSet("contactid"),
                            Criteria =
                    {
                        Conditions =
                        {
                            new ConditionExpression("emailaddress1", ConditionOperator.Equal, contactEmail)
                        }
                    }
                        };

                        var result = service.RetrieveMultiple(query);

                        if (result.Entities.Count > 0)
                        {
                            Console.WriteLine($"Skipped (already exists): {contactEmail}");
                            continue;
                        }

                        // ===========================
                        // 🔹 2. Create new contact
                        // ===========================
                        Contact newContact = new Contact
                        {
                            FirstName = contactName,
                            ["emailaddress1"] = contactEmail
                        };

                        Guid contactId = service.Create(newContact);

                        Console.WriteLine($"Created Contact: {contactName} | Email: {contactEmail} | ID: {contactId}");


                    }
                        
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error at row {row}: {ex.Message}");
                    }

                }
            }
            Console.WriteLine("Contact processing completed!");
    }

        public static void CreateAccountsFromExcel(ServiceClient service, string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Excel file not found at: " + filePath);
                Console.ReadLine();
                return;
            }

            ExcelPackage.License.SetNonCommercialPersonal("Bikash Patel");
            var file = new FileInfo(filePath);

            using (var package = new ExcelPackage(file))
            {
                var sheet = package.Workbook.Worksheets[0];
                int rowCount = sheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    string name = sheet.Cells[row, 1].Text;
                    string phone = sheet.Cells[row, 2].Text;
                    string website = sheet.Cells[row, 3].Text;
                    string address = sheet.Cells[row, 4].Text;
                    string contactname = sheet.Cells[row, 5].Text;
                    string contactemail = sheet.Cells[row, 6].Text;

                    // 🔹 Skip empty rows
                    if (string.IsNullOrWhiteSpace(name))
                        continue;

                    try
                    {
                        // ===========================
                        // 🔹 1. Duplicate Check (by phone)
                        // ===========================
                        if (!string.IsNullOrWhiteSpace(phone))
                        {
                            QueryExpression query = new QueryExpression("account")
                            {
                                ColumnSet = new ColumnSet("accountid"),
                                Criteria =
                        {
                            Conditions =
                            {
                                new ConditionExpression("telephone1", ConditionOperator.Equal, phone)
                            }
                        }
                            };

                            var existing = service.RetrieveMultiple(query);

                            if (existing.Entities.Count > 0)
                            {
                                Console.WriteLine($"Skipped (already exists with same phone): {name}");
                                continue; // 👉 skip and move to next row
                            }
                        }

                        // ===========================
                        // 🔹 2. Get/Create Contact
                        // ===========================
                        EntityReference contactRef = null;

                        if (!string.IsNullOrWhiteSpace(contactemail))
                        {
                            contactRef = GetOrCreateContact(service, contactname, contactemail);
                        }

                        // ===========================
                        // 🔹 3. Create Account
                        // ===========================
                        Account acc = new Account
                        {
                            Name = name,
                            WebSiteURL = website,
                            Telephone1 = phone,
                            Address1_City = address
                        };

                        // 🔹 4. Link Contact
                        if (contactRef != null)
                        {
                            acc.PrimaryContactId = contactRef;
                        }

                        Guid id = service.Create(acc);

                        Console.WriteLine($"Created: {name} | ID: {id}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error at row {row}: {ex.Message}");
                    }
                }
            }
        }
                
        static void SyncAccounts(ServiceClient devservice, ServiceClient sitservice)
        {
            if (!sitservice.IsReady)
            {
                Console.WriteLine("SIT CRM connection failed: " + sitservice.LastError);
                return;
            }

            Console.WriteLine("Fetching accounts from DEV...");

            QueryExpression query = new QueryExpression("account")
            {
                ColumnSet = new ColumnSet("name", "telephone1", "websiteurl", "address1_line1")
            };

            var results = devservice.RetrieveMultiple(query);

            Console.WriteLine($"Total records fetched: {results.Entities.Count}");

            foreach (var entity in results.Entities)
            {
                try
                {
                    Account sourceAcc = entity.ToEntity<Account>();

                    // 🔹 Optional: Check duplicate in SIT (by name)
                    QueryExpression checkQuery = new QueryExpression("account")
                    {
                        ColumnSet = new ColumnSet("accountid"),
                        Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression("name", ConditionOperator.Equal, sourceAcc.Name)
                    }
                }
                    };

                    var existing = sitservice.RetrieveMultiple(checkQuery);

                    if (existing.Entities.Count > 0)
                    {
                        Console.WriteLine($"Skipped (already exists): {sourceAcc.Name}");
                        continue;
                    }

                    // 🔹 Create in SIT
                    Account targetAcc = new Account();
                    targetAcc.Name = sourceAcc.Name;

                    if (sourceAcc.Contains("telephone1"))
                        targetAcc["telephone1"] = sourceAcc["telephone1"];

                    if (sourceAcc.Contains("websiteurl"))
                        targetAcc["websiteurl"] = sourceAcc["websiteurl"];

                    if (sourceAcc.Contains("address1_line1"))
                        targetAcc["address1_line1"] = sourceAcc["address1_line1"];

                    sitservice.Create(targetAcc);

                    Console.WriteLine($"Synced: {sourceAcc.Name}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error syncing record: {ex.Message}");
                }
            }

            Console.WriteLine("Sync completed!");
        }

        public static EntityReference GetOrCreateContact(ServiceClient service, string contactName, string email)
        {
            // 🔹 1. Try to find existing contact by email (best unique field)
            QueryExpression query = new QueryExpression("contact")
            {
                ColumnSet = new ColumnSet("contactid", "fullname"),
                Criteria =
        {
            Conditions =
            {
                new ConditionExpression("emailaddress1", ConditionOperator.Equal, email)
            }
        }
            };

            var result = service.RetrieveMultiple(query);

            // 🔹 2. If found → return existing
            if (result.Entities.Count > 0)
            {
                var existingContact = result.Entities[0];
                return new EntityReference("contact", existingContact.Id);
            }

            // 🔹 3. If NOT found → create new contact
            Contact newContact = new Contact();

            // Simple name handling (you can improve later)
            newContact.FirstName = contactName;
            newContact["emailaddress1"] = email;

            Guid contactId = service.Create(newContact);

            // 🔹 4. Return newly created contact reference
            return new EntityReference("contact", contactId);
        }
        /// <summary>
        /// This function will create employee record
        /// </summary>
        /// <param name="service">This is a service object of ServiceClient</param>
        public static void CreateEmployee(ServiceClient service)
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
                // 🔹 SINGLE CHOICE OPTION SET (IMPORTANT)
                // =========================
                // Preferred (if early-bound enums exist)    

                //emp.abc_JobTitle = abc_employee_abc_jobtitle.Analyst;
                
                emp["abc_jobtitle"] = new OptionSetValue(745780004);
                // =========================
                // 🔹 MULTI CHOICE OPTION SET (IMPORTANT)
                // =========================
                emp["abc_hobby"] = new OptionSetValueCollection
                {
                    new OptionSetValue((int)abc_employee_abc_hobby.travelling),
                    new OptionSetValue((int)abc_employee_abc_hobby.Reading),
                    new OptionSetValue((int)abc_employee_abc_hobby.cricket)

                };

                // =========================
                // 🔹 LOOKUP (STATE)
                // =========================

                Guid stateguid = new Guid("7ab8fd93-2316-f111-8341-000d3a3717bf");
                

                if (stateguid != Guid.Empty)
                {
                    emp.abc_StateName = new EntityReference(abc_State.EntityLogicalName, stateguid);
                }

                // =========================
                // 🔹 LOOKUP (DISTRICT)
                // =========================
                //Guid districtId = GetLookupRecord(service, "abc_district", "abc_name", "Pune");

                //if (districtId != Guid.Empty)
                //{
                  //  emp.abc_DistrictName = new EntityReference("abc_district", districtId);
                //}

               

                // =========================
                // 🔹 IMAGE FIELD
                // =========================
                //string imgPath = @"C:\Temp\profile.jpg";

                //if (File.Exists(imgPath))
                //{
                //    emp.abc_profileimage = File.ReadAllBytes(imgPath);
                //}

                // =========================
                // 🔹 CREATE RECORD
                // =========================
                Guid empId = service.Create(emp);

                Console.WriteLine("Employee Created Successfully!");
                Console.WriteLine("Employee ID: " + empId);

                // =========================
                // 🔹 FILE UPLOAD (CV)
                // =========================
                // UploadEmployeeFile(service, empId, @"C:\Temp\resume.pdf");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public static Guid GetLookupRecord(ServiceClient service, string entityName, string fieldName, string value)
        {
            try
            {
                QueryExpression query = new QueryExpression(entityName)
                {
                    ColumnSet = new ColumnSet(entityName + "id")
                };

                query.Criteria.AddCondition(fieldName, ConditionOperator.Equal, value);

                var result = service.RetrieveMultiple(query);

                if (result.Entities.Count > 0)
                {
                    return result.Entities[0].Id;
                }

                Console.WriteLine($"Lookup not found: {value} in {entityName}");
                return Guid.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lookup error: " + ex.Message);
                return Guid.Empty;
            }
        }
    }
}