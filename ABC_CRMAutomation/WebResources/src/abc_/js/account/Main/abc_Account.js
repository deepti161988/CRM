var AbcAutomation = AbcAutomation || {};
//Change the below Method name XXXXXForm
AbcAutomation.AccountForm = (function ()
{
	return {
		OnLoad: function (executionContext)
		{
			var formContext = executionContext.getFormContext();
			console.log("Account Form Loaded");
		},
		OnSave: function (executionContext)
		{
			var formContext = executionContext.getFormContext();
		},
		// NEW METHOD (retrieveMultipleRecords example)
		OnChangePhoneNumber: function (executionContext)
		{
			var formContext = executionContext.getFormContext();
			// Get Phone Number field
			var phoneAttr = formContext.getAttribute("telephone1");
			if (phoneAttr)
			{
				var phoneNumber = phoneAttr.getValue();
				if (phoneNumber !== null)
				{
					console.log("New Phone Number: " + phoneNumber);
					// Call retrieveMultipleRecords method
					AbcAutomation.AccountForm.SetAccountDescription();
				}
			}
		},
		SetAccountDescription: function ()
		{
			Xrm.WebApi.retrieveMultipleRecords(
				"account",
				"?$select=name,numberofemployees,address1_city,_parentaccountid_value&$filter=address1_city eq 'Redmond' and numberofemployees lt 3000").then(

			function success(result)
			{
				result.entities.forEach(function (account)
				{
					console.log("Account ID: " + account.accountid);
					console.log("Account Name: " + account.name);
					console.log("numberofemployees: " + account.numberofemployees);
					console.log("address1_city: " + account.address1_city);
					var parentaccountid = account["_parentaccountid_value"]; // Lookup
					var parentaccountid_formatted = account["_parentaccountid_value@OData.Community.Display.V1.FormattedValue"];
					var parentaccountid_lookuplogicalname = account["_parentaccountid_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
					console.log("parentaccountid: " + parentaccountid);
					console.log("parentaccountid_formatted: " + parentaccountid_formatted);
					console.log("parentaccountid_lookuplogicalname: " + parentaccountid_lookuplogicalname);
					// Prepare description value
					var descriptionText = account.address1_city + " - " + account.numberofemployees;
					// Update object
					var data = {
						"description": descriptionText,
						"parentaccountid@odata.bind": "/accounts(4e9ce1cc-af0e-f111-8406-000d3a32ddf9)"
					};
					Xrm.WebApi.updateRecord("account", account.accountid, data).then(

					function successUpdate()
					{
						console.log("Updated description for: " + account.name);
					},

					function (error)
					{
						console.log(error.message);
					});
				});
			},

			function (error)
			{
				console.log(error.message);
			});
		},
		// Triggered when the Parent Account field value changes on the form		
		OnChangeParentAccount: function (executionContext)
		{
			// Get the current form context			
			var formContext = executionContext.getFormContext();
			// Get the Parent Account lookup attribute from the form
			var parentAttr = formContext.getAttribute("parentaccountid");
			// Ensure the attribute exists on the form
			if (parentAttr)
			{
				// Get the selected Parent Account lookup value				
				var parentAccount = parentAttr.getValue();
				// Check if a Parent Account is selected
				if (parentAccount !== null)
				{
					// Log the selected Parent Account name in browser console
					console.log("Parent Account Selected: " + parentAccount[0].name);
					// Call the method that retrieves parent credit limit and updates the current account credit limit based on business logic
					AbcAutomation.AccountForm.UpdateCreditLimitBasedOnParent(executionContext);
				}
			}
		},
		// Function to retrieve Parent Account Credit Limit
		// and update the current Account Credit Limit based on a condition		
		UpdateCreditLimitBasedOnParent: function (executionContext)
		{
			// Get the current form context			
			var formContext = executionContext.getFormContext();
			// Get the Parent Account lookup value
			var parentAccount = formContext.getAttribute("parentaccountid").getValue();
			// Check if Parent Account exists
			if (parentAccount)
			{
				// Extract Parent Account GUID and remove curly braces
				var parentId = parentAccount[0].id.replace(/[{}]/g, "");
				// STEP 1: Retrieve Parent Account Credit Limit using Web API
				Xrm.WebApi.retrieveRecord("account", parentId, "?$select=creditlimit").then(

				function success(result)
				{
					// Get credit limit value from the parent account
					// If null, default to 0					
					var parentCreditLimit = result.creditlimit || 0;
					console.log("Parent Credit Limit:", parentCreditLimit);
					// Variable to store the new credit limit for current account
					var newCreditLimit;
					// STEP 2: Apply business rule based on parent credit limit
					if (parentCreditLimit > 30000)
					{
						// If parent credit limit is greater than 30000
						newCreditLimit = 9999;
					}
					else
					{
						// If parent credit limit is less than or equal to 30000
						newCreditLimit = 5555;
					}
					// Get current Account record ID from the form
					var currentAccountId = formContext.data.entity.getId().replace(/[{}]/g, "");
					// STEP 3: Prepare data object for updating the record
					var data = {
						creditlimit: newCreditLimit
					};
					// Update current account credit limit using Web API
					Xrm.WebApi.updateRecord("account", currentAccountId, data).then(

					function successUpdate()
					{
						console.log("Current Account Credit Limit Updated");
						// Update credit limit field on the form UI
						formContext.getAttribute("creditlimit").setValue(newCreditLimit);
					},
					// Error handling for update operation

					function (error)
					{
						console.log(error.message);
					});
				},
				// Error handling for retrieve operation

				function (error)
				{
					console.log(error.message);
				});
			}
		}
	};
})();