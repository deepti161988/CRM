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
		}
	};
})();