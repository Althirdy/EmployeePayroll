(function ($) {
    const apiBaseUrl = "http://localhost:5237/api";
    const enableDebugLogs = false;
    let employeesCache = [];
    let editingEmployeeId = null;
    let computingEmployeeId = null;

    const $statusMessage = $("#statusMessage");
    const $tableRegion = $("#tableRegion");
    const $refreshButton = $("#refreshButton");
    const $employeeForm = $("#employeeForm");
    const $submitButton = $("#submitButton");
    const $formMessage = $("#formMessage");
    const $resetButton = $("#resetButton");
    const $formTitle = $("#formTitle");
    const $employeeNumberDisplay = $("#employeeNumberDisplay");
    const $editModeHint = $("#editModeHint");
    const $computeSection = $("#computeSection");
    const $computeForm = $("#computeForm");
    const $computeEmployeeDisplay = $("#computeEmployeeDisplay");
    const $computeMessage = $("#computeMessage");
    const $computeSubmitButton = $("#computeSubmitButton");
    const $computeResetButton = $("#computeResetButton");
    const $computeResult = $("#computeResult");

    function debugLog(message, payload) {
        if (!enableDebugLogs || typeof console === "undefined") {
            return;
        }

        if (typeof payload === "undefined") {
            console.log("[employee-ui]", message);
            return;
        }

        console.log("[employee-ui]", message, payload);
    }

    function setFormMessage(message, stateClass) {
        $formMessage
            .text(message)
            .removeClass("is-error is-success");

        if (stateClass) {
            $formMessage.addClass(stateClass);
        }
    }

    function clearFormMessage() {
        $formMessage
            .text("")
            .removeClass("is-error is-success");
    }

    function setComputeMessage(message, stateClass) {
        $computeMessage
            .text(message)
            .removeClass("is-error is-success");

        if (stateClass) {
            $computeMessage.addClass(stateClass);
        }
    }

    function clearComputeMessage() {
        $computeMessage
            .text("")
            .removeClass("is-error is-success");
    }

    function setStatus(message, stateClass) {
        $statusMessage
            .text(message)
            .removeClass("is-error is-empty is-success");

        if (stateClass) {
            $statusMessage.addClass(stateClass);
        }
    }

    function setEditingMode(employee) {
        editingEmployeeId = Number(employee.id);
        $formTitle.text("Update Employee");
        $submitButton.text("Update Employee");
        $resetButton.text("Cancel");
        $employeeNumberDisplay.text("Employee Number: " + (employee.employeeNumber || ""));
        $editModeHint.text("Only Daily Rate and Working Days can be edited in update mode.");
        setIdentityFieldsEditable(false);
        debugLog("Entered edit mode", { employeeId: editingEmployeeId });
    }

    function setCreateMode() {
        editingEmployeeId = null;
        $formTitle.text("Create Employee");
        $submitButton.text("Create Employee");
        $resetButton.text("Clear");
        $employeeNumberDisplay.text("");
        $editModeHint.text("");
        setIdentityFieldsEditable(true);
        debugLog("Entered create mode");
    }

    function setIdentityFieldsEditable(isEditable) {
        $("#lastName").prop("readonly", !isEditable);
        $("#firstName").prop("readonly", !isEditable);
        $("#middleName").prop("readonly", !isEditable);
        $("#dateOfBirth").prop("readonly", !isEditable);
    }

    function escapeHtml(value) {
        return $("<div>").text(value ?? "").html();
    }

    function normalizeName(value) {
        const trimmedValue = (value || "").trim();
        if (!trimmedValue) {
            return "";
        }

        return trimmedValue.charAt(0).toUpperCase() + trimmedValue.slice(1).toLowerCase();
    }

    function formatDailyRate(value) {
        const amount = Number(value);

        if (Number.isNaN(amount)) {
            return escapeHtml(value);
        }

        return amount.toLocaleString("en-PH", {
            style: "currency",
            currency: "PHP"
        });
    }

    function formatCurrency(value) {
        const amount = Number(value);

        if (Number.isNaN(amount)) {
            return escapeHtml(value);
        }

        return amount.toLocaleString("en-PH", {
            style: "currency",
            currency: "PHP"
        });
    }

    function renderEmptyState() {
        employeesCache = [];
        $tableRegion.html(
            '<div class="empty-state">' +
                "<strong>No employees found</strong>" +
                "<span>The API returned an empty list.</span>" +
            "</div>"
        );
        setStatus("No employees were returned by the API.", "is-empty");
    }

    function renderEmployees(employees) {
        employeesCache = Array.isArray(employees) ? employees : [];

        if (!Array.isArray(employees) || employees.length === 0) {
            renderEmptyState();
            return;
        }

        const rows = employees.map(function (employee) {
            return (
                "<tr>" +
                    "<td>" + escapeHtml(employee.id) + "</td>" +
                    "<td>" + escapeHtml((employee.employeeNumber || "").toUpperCase()) + "</td>" +
                    "<td>" + escapeHtml(employee.employeeName) + "</td>" +
                    "<td>" + escapeHtml(employee.dateOfBirth) + "</td>" +
                    "<td>" + formatDailyRate(employee.dailyRate) + "</td>" +
                    "<td>" + escapeHtml(employee.workingDays) + "</td>" +
                    '<td><div class="table-actions">' +
                        '<button class="secondary-button table-action-button js-edit-employee" type="button" data-id="' + escapeHtml(employee.id) + '">Update</button>' +
                        '<button class="secondary-button table-action-button js-compute-employee" type="button" data-id="' + escapeHtml(employee.id) + '">Compute</button>' +
                        '<button class="danger-button table-action-button js-delete-employee" type="button" data-id="' + escapeHtml(employee.id) + '">Delete</button>' +
                    "</div></td>" +
                "</tr>"
            );
        }).join("");

        $tableRegion.html(
            '<table class="employee-table">' +
                "<thead>" +
                    "<tr>" +
                        "<th>Id</th>" +
                        "<th>Employee Number</th>" +
                        "<th>Employee Name</th>" +
                        "<th>Date of Birth</th>" +
                        "<th>Daily Rate</th>" +
                        "<th>Working Days</th>" +
                        "<th>Action</th>" +
                    "</tr>" +
                "</thead>" +
                "<tbody>" + rows + "</tbody>" +
            "</table>"
        );

        setStatus("Employees loaded successfully.", "is-success");
    }

    function renderRequestError(jqXhr, textStatus) {
        const responseText = jqXhr.responseText ? jqXhr.responseText.trim() : "";
        const message = responseText || jqXhr.statusText || textStatus || "Unknown error";

        $tableRegion.html(
            '<div class="empty-state">' +
                "<strong>Unable to load employees</strong>" +
                "<span>Check that the API is running on http://localhost:5237 and try again.</span>" +
            "</div>"
        );

        setStatus("Request failed: " + message, "is-error");
    }

    function getFormPayload() {
        return {
            lastName: normalizeName($("#lastName").val()),
            firstName: normalizeName($("#firstName").val()),
            middleName: normalizeName($("#middleName").val()) || null,
            dateOfBirth: ($("#dateOfBirth").val() || "").trim(),
            dailyRate: Number($("#dailyRate").val()),
            workingDays: ($("#workingDays").val() || "").trim()
        };
    }

    function getUpdatePayload() {
        return {
            dailyRate: Number($("#dailyRate").val()),
            workingDays: ($("#workingDays").val() || "").trim()
        };
    }

    function validateForm(payload) {
        if (!payload.lastName) {
            return "Last name is required.";
        }

        if (!payload.firstName) {
            return "First name is required.";
        }

        if (!payload.dateOfBirth) {
            return "Date of birth is required.";
        }

        if (!Number.isFinite(payload.dailyRate) || payload.dailyRate <= 0) {
            return "Daily rate must be greater than 0.";
        }

        if (!payload.workingDays) {
            return "Working days is required.";
        }

        return "";
    }

    function validateUpdatePayload(payload) {
        if (!Number.isFinite(payload.dailyRate) || payload.dailyRate <= 0) {
            return "Daily rate must be greater than 0.";
        }

        if (!payload.workingDays) {
            return "Working days is required.";
        }

        return "";
    }

    function getComputePayload() {
        return {
            startDate: ($("#computeStartDate").val() || "").trim(),
            endDate: ($("#computeEndDate").val() || "").trim()
        };
    }

    function validateComputePayload(payload) {
        if (computingEmployeeId === null) {
            return "Select an employee first before computing take-home pay.";
        }

        if (!payload.startDate) {
            return "Start date is required.";
        }

        if (!payload.endDate) {
            return "End date is required.";
        }

        if (payload.endDate < payload.startDate) {
            return "End date must be greater than or equal to start date.";
        }

        return "";
    }

    function populateForm(employee) {
        $("#lastName").val(employee.lastName || "");
        $("#firstName").val(employee.firstName || "");
        $("#middleName").val(employee.middleName || "");
        $("#dateOfBirth").val(employee.dateOfBirthValue || "");
        $("#dailyRate").val(employee.dailyRate);
        $("#workingDays").val(employee.workingDays || "");
    }

    function clearEmployeeFormFields() {
        $("#lastName").val("");
        $("#firstName").val("");
        $("#middleName").val("");
        $("#dateOfBirth").val("");
        $("#dailyRate").val("");
        $("#workingDays").val("");
    }

    function resetFormState() {
        clearEmployeeFormFields();
        clearFormMessage();
        setCreateMode();
    }

    function handleEmployeeFormReset() {
        debugLog("Clear/Cancel clicked", { isEditing: editingEmployeeId !== null });
        resetFormState();
    }

    function clearComputeResult() {
        $computeResult
            .removeClass("is-visible")
            .html("");
    }

    function resetComputeState() {
        computingEmployeeId = null;
        $computeForm[0].reset();
        clearComputeMessage();
        clearComputeResult();
        $computeEmployeeDisplay.text("Select an employee from the table to compute take-home pay.");
    }

    function clearComputeFormInputs() {
        $("#computeStartDate").val("");
        $("#computeEndDate").val("");
    }

    function handleComputeFormReset() {
        resetComputeState();
    }

    function renderComputeResult(result) {
        $computeResult
            .addClass("is-visible")
            .html(
                "<h3>Take-Home Pay Result</h3>" +
                "<p><strong>Employee Number:</strong> " + escapeHtml(result.employeeNumber) + "</p>" +
                "<p><strong>Start Date:</strong> " + escapeHtml(result.startingDate) + "</p>" +
                "<p><strong>End Date:</strong> " + escapeHtml(result.endDate) + "</p>" +
                "<p><strong>Take-Home Pay:</strong> " + formatCurrency(result.takeHomePay) + "</p>"
            );
    }

    function loadEmployees() {
        setStatus("Loading employees...", "");
        $refreshButton.prop("disabled", true);

        $.ajax({
            url: apiBaseUrl + "/employees",
            method: "GET",
            dataType: "json"
        })
            .done(renderEmployees)
            .fail(renderRequestError)
            .always(function () {
                $refreshButton.prop("disabled", false);
            });
    }

    function submitEmployeeForm(event) {
        event.preventDefault();
        clearFormMessage();

        const isEditing = editingEmployeeId !== null;
        const payload = isEditing ? getUpdatePayload() : getFormPayload();
        const validationMessage = isEditing ? validateUpdatePayload(payload) : validateForm(payload);

        if (validationMessage) {
            setFormMessage(validationMessage, "is-error");
            return;
        }

        $submitButton.prop("disabled", true);
        $resetButton.prop("disabled", true);

        $.ajax({
            url: isEditing ? apiBaseUrl + "/employees/" + editingEmployeeId : apiBaseUrl + "/employees",
            method: isEditing ? "PUT" : "POST",
            contentType: "application/json",
            data: JSON.stringify(payload)
        })
            .done(function () {
                const successMessage = isEditing ? "Employee updated successfully." : "Employee created successfully.";
                debugLog("Employee submit succeeded", { isEditing: isEditing, employeeId: editingEmployeeId });
                resetFormState();
                setFormMessage(successMessage, "is-success");
                setStatus(successMessage + " Reloading list...", "is-success");
                loadEmployees();
            })
            .fail(function (jqXhr, textStatus) {
                const responseText = jqXhr.responseText ? jqXhr.responseText.trim() : "";
                const fallback = isEditing ? "Unable to update employee." : "Unable to create employee.";
                const message = responseText || jqXhr.statusText || textStatus || fallback;
                setFormMessage(message, "is-error");
            })
            .always(function () {
                $submitButton.prop("disabled", false);
                $resetButton.prop("disabled", false);
            });
    }

    function submitComputeForm(event) {
        event.preventDefault();
        clearComputeMessage();
        clearComputeResult();

        const payload = getComputePayload();
        const validationMessage = validateComputePayload(payload);

        if (validationMessage) {
            setComputeMessage(validationMessage, "is-error");
            return;
        }

        $computeSubmitButton.prop("disabled", true);
        $computeResetButton.prop("disabled", true);

        $.ajax({
            url: apiBaseUrl + "/employees/" + computingEmployeeId + "/compute",
            method: "POST",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(payload)
        })
            .done(function (result) {
                renderComputeResult(result);
                setComputeMessage("Take-home pay computed successfully.", "is-success");
                setStatus("Take-home pay computed successfully.", "is-success");
            })
            .fail(function (jqXhr, textStatus) {
                const responseText = jqXhr.responseText ? jqXhr.responseText.trim() : "";
                const message = responseText || jqXhr.statusText || textStatus || "Unable to compute take-home pay.";
                setComputeMessage(message, "is-error");
            })
            .always(function () {
                $computeSubmitButton.prop("disabled", false);
                $computeResetButton.prop("disabled", false);
            });
    }

    function startEditingEmployee(employeeId) {
        const employee = employeesCache.find(function (item) {
            return Number(item.id) === Number(employeeId);
        });

        if (!employee) {
            setStatus("Employee data could not be found for update.", "is-error");
            return;
        }

        populateForm(employee);
        clearFormMessage();
        setEditingMode(employee);
        window.scrollTo({ top: 0, behavior: "smooth" });
    }

    function startComputingEmployee(employeeId) {
        const employee = employeesCache.find(function (item) {
            return Number(item.id) === Number(employeeId);
        });

        if (!employee) {
            setStatus("Employee data could not be found for computation.", "is-error");
            return;
        }

        resetFormState();
        computingEmployeeId = Number(employee.id);
        clearComputeMessage();
        clearComputeResult();
        clearComputeFormInputs();
        $computeEmployeeDisplay.text("Selected Employee: " + (employee.employeeNumber || "") + " - " + (employee.employeeName || ""));
        $computeSection[0].scrollIntoView({ behavior: "smooth", block: "start" });
    }

    function deleteEmployee(employeeId) {
        const employee = employeesCache.find(function (item) {
            return Number(item.id) === Number(employeeId);
        });
        const employeeLabel = employee ? employee.employeeName : "this employee";

        if (!window.confirm("Delete " + employeeLabel + "?")) {
            return;
        }

        setStatus("Deleting employee...", "");

        $.ajax({
            url: apiBaseUrl + "/employees/" + employeeId,
            method: "DELETE"
        })
            .done(function () {
                if (editingEmployeeId === Number(employeeId)) {
                    resetFormState();
                }

                if (computingEmployeeId === Number(employeeId)) {
                    resetComputeState();
                }

                setStatus("Employee deleted successfully.", "is-success");
                loadEmployees();
            })
            .fail(function (jqXhr, textStatus) {
                const responseText = jqXhr.responseText ? jqXhr.responseText.trim() : "";
                const message = responseText || jqXhr.statusText || textStatus || "Unable to delete employee.";
                setStatus(message, "is-error");
            });
    }

    $(function () {
        setCreateMode();
        resetComputeState();
        $refreshButton.on("click", loadEmployees);
        $employeeForm.on("submit", submitEmployeeForm);
        $resetButton.on("click", handleEmployeeFormReset);
        $computeForm.on("submit", submitComputeForm);
        $computeResetButton.on("click", handleComputeFormReset);
        $tableRegion.on("click", ".js-edit-employee", function () {
            startEditingEmployee($(this).data("id"));
        });
        $tableRegion.on("click", ".js-compute-employee", function () {
            startComputingEmployee($(this).data("id"));
        });
        $tableRegion.on("click", ".js-delete-employee", function () {
            deleteEmployee($(this).data("id"));
        });
        loadEmployees();
    });
})(jQuery);
