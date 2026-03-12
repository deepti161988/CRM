var AbcAutomation = AbcAutomation || {};
//Change the below Method name XXXXXForm
AbcAutomation.AccountForm = (function ()
{
	return {
		OnLoad: function (executionContext)
		{
			var formContext = executionContext.getFormContext();
			Xrm.Navigation.openAlertDialog(
			{
				confirmButtonLabel: "OK",
				text: "Welcome to onload event",
				title: "Form Onload"
			});
		},
		OnSave: function (executionContext)
		{
			var formContext = executionContext.getFormContext();
			Xrm.Navigation.openAlertDialog(
			{
				confirmButtonLabel: "OK",
				text: "Welcome to onsave event",
				title: "Form OnSave"
			});
		},
		OnChangeFax: function (executionContext)
		{
			var formContext = executionContext.getFormContext();
			Xrm.Navigation.openAlertDialog(
			{
				confirmButtonLabel: "OK",
				text: "Welcome to field onchange event",
				title: "Field Onchange"
			});
		},
		OnChangeParentAccount: function (executionContext)
		{
			var formContext = executionContext.getFormContext();
			// Xrm.Navigation.openAlertDialog(
			// {
			// 	confirmButtonLabel: "OK",
			// 	text: "Welcome to field onchange event",
			// 	title: "Field Onchange"
			// });
		}
	};
})();