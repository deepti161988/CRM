var AbcAutomation = AbcAutomation || {};

AbcAutomation.StateForm = (function () {

    // =======================
    // Private Helper Functions
    // =======================

    function isEmpty(value) {
        return value === null || value === "";
    }

    function getAttributeValue(formContext, fieldName) {
        var attribute = formContext.getAttribute(fieldName);
        return attribute ? attribute.getValue() : null;
    }

    // =======================
    // Public Methods
    // =======================

    return {

        // =======================
        // OnLoad Event
        // =======================
        OnLoad: function (executionContext) {

            var formContext = executionContext.getFormContext();

            var mandatoryFields = [
                "abc_statename",
                "abc_statecode",
                "abc_statetype",
                "abc_population"
            ];

            var missingFields = [];

            mandatoryFields.forEach(function (field) {
                var value = getAttributeValue(formContext, field);

                if (isEmpty(value)) {
                    missingFields.push(field);
                }
            });

            if (missingFields.length > 0) {
                formContext.ui.setFormNotification(
                    "Mandatory fields missing: " + missingFields.join(", "),
                    "ERROR",
                    "MissingFields"
                );
            }
        },

        // =======================
        // OnSave Event
        // =======================
        OnSave: function (executionContext) {

            var formContext = executionContext.getFormContext();
            var eventArgs = executionContext.getEventArgs();

            var formationDate = getAttributeValue(formContext, "abc_formationdate");

            if (isEmpty(formationDate)) {

                formContext.ui.setFormNotification(
                    "Formation Date is required!",
                    "ERROR",
                    "FormationDateError"
                );

                eventArgs.preventDefault();
            } else {
                formContext.ui.clearFormNotification("FormationDateError");
            }
        },

        // =======================
        // OnChange - Capital City
        // =======================
        OnChangeCapitalCity: function (executionContext) {

            var formContext = executionContext.getFormContext();
            var value = getAttributeValue(formContext, "abc_capitalcity");

            Xrm.Navigation.openAlertDialog({
                confirmButtonLabel: "OK",
                text: "Capital City changed! Current value: " + (value ? value : "No Value"),
                title: "Field Changed"
            });
        },

        // =======================
        // OnChange - Population
        // =======================
        OnChangePopulation: function (executionContext) {

            var formContext = executionContext.getFormContext();
            var value = getAttributeValue(formContext, "abc_population");

            Xrm.Navigation.openAlertDialog({
                confirmButtonLabel: "OK",
                text: "Population changed! Current value: " + (value !== null ? value : "No Value"),
                title: "Field Changed"
            });
        }

    };

})();