document.addEventListener("DOMContentLoaded", () => {
    //alert("patients.js is running!")
    const apiUrl = "/api/PatientsApi";

    const tableBody = document.getElementById("patientsTableBody");
    const totalPatients = document.getElementById("totalPatients");
    const searchInput = document.getElementById("searchInput");
    const statusMessage = document.getElementById("statusMessage");
    const resultCount = document.getElementById("resultCount");
    const refreshButton = document.getElementById("refreshButton");
    const addPatientButton = document.getElementById("addPatientButton");

    const detailsBody = document.getElementById("patientDetailsBody");
    const detailsTitle = document.getElementById("patientModalTitle");

    const patientFormModal = document.getElementById("patientFormModal");
    const patientForm = document.getElementById("patientForm");
    const patientFormTitle = document.getElementById("patientFormTitle");
    const savePatientButton = document.getElementById("savePatientButton");
    const formError = document.getElementById("formError");

    let patients = [];

    // Safely escape values before inserting them into HTML.
    function escapeHtml(value) {
        return String(value ?? "").replace(/[&<>"']/g, character => ({
            "&": "&amp;",
            "<": "&lt;",
            ">": "&gt;",
            '"': "&quot;",
            "'": "&#39;"
        })[character]);
    }

    function formatDate(value) {
        if (!value) return "—";

        const date = new Date(value);

        if (Number.isNaN(date.getTime())) return "—";

        return date.toLocaleDateString("en-IN", {
            day: "2-digit",
            month: "short",
            year: "numeric"
        });
    }

    function showStatus(message, type = "info") {
        if (!statusMessage) return;

        statusMessage.textContent = message;
        statusMessage.className = `alert alert-${type}`;
        statusMessage.hidden = false;
    }

    function showFormError(message) {
        formError.textContent = message;
        formError.hidden = false;
    }

    function clearFormError() {
        formError.textContent = "";
        formError.hidden = true;
    }

    function getField(id) {
        return document.getElementById(id);
    }

    function resetPatientForm() {
        patientForm.reset();
        getField("patientId").value = "";
        clearFormError();
    }

    function openAddPatientForm() {
        resetPatientForm();

        patientFormTitle.textContent = "Add Patient";
        savePatientButton.textContent = "Save Patient";

        bootstrap.Modal
            .getOrCreateInstance(patientFormModal)
            .show();
    }

    function openEditPatientForm(patient) {
        resetPatientForm();

        getField("patientId").value = patient.patientId;
        getField("patientName").value = patient.patientName ?? "";

        getField("dateOfBirth").value = patient.dateOfBirth
            ? String(patient.dateOfBirth).substring(0, 10)
            : "";

        getField("gender").value = patient.gender ?? "";
        getField("phone").value = patient.phone ?? "";
        getField("email").value = patient.email ?? "";
        getField("address").value = patient.address ?? "";
        getField("bloodGroup").value = patient.bloodGroup ?? "";
        getField("emergencyContact").value =
            patient.emergencyContact ?? "";
        getField("medicalHistory").value =
            patient.medicalHistory ?? "";

        patientFormTitle.textContent = "Edit Patient";
        savePatientButton.textContent = "Update Patient";

        bootstrap.Modal
            .getOrCreateInstance(patientFormModal)
            .show();
    }

    function renderPatients() {
        const searchTerm = searchInput.value.trim().toLowerCase();

        const filteredPatients = patients.filter(patient => {
            const searchableText = [
                patient.patientId,
                patient.patientName,
                patient.phone,
                patient.address,
                patient.gender,
                patient.bloodGroup
            ].join(" ").toLowerCase();

            return searchableText.includes(searchTerm);
        });

        totalPatients.textContent = patients.length;

        resultCount.textContent =
            `Showing ${filteredPatients.length} of ${patients.length} patients`;

        if (filteredPatients.length === 0) {
            tableBody.innerHTML = `
                <tr>
                    <td colspan="8" class="text-center py-4 text-secondary">
                        No patient records found.
                    </td>
                </tr>`;
            return;
        }

        tableBody.innerHTML = filteredPatients.map(patient => `
            <tr>
                <td>${escapeHtml(patient.patientId)}</td>

                <td class="fw-semibold">
                    ${escapeHtml(patient.patientName)}
                </td>

                <td>${escapeHtml(formatDate(patient.dateOfBirth))}</td>
                <td>${escapeHtml(patient.gender || "—")}</td>
                <td>${escapeHtml(patient.phone || "—")}</td>

                <td>
                    <span class="badge text-bg-info">
                        ${escapeHtml(patient.bloodGroup || "—")}
                    </span>
                </td>

                <td>${escapeHtml(patient.address || "—")}</td>

                <td>
                    <div class="d-flex flex-wrap gap-1">
                        <button type="button"
                                class="btn btn-sm btn-outline-primary details-button"
                                data-patient-id="${escapeHtml(patient.patientId)}">
                            View
                        </button>

                        <button type="button"
                                class="btn btn-sm btn-outline-success edit-button"
                                data-patient-id="${escapeHtml(patient.patientId)}">
                            Edit
                        </button>

                        <button type="button"
                                class="btn btn-sm btn-outline-danger delete-button"
                                data-patient-id="${escapeHtml(patient.patientId)}">
                            Delete
                        </button>
                    </div>
                </td>
            </tr>
        `).join("");
    }

    // LOAD PATIENTS
    async function loadPatients() {
        //alert("Calling Patients API: " + apiUrl);

        refreshButton.disabled = true;
        refreshButton.textContent = "Loading...";

        tableBody.innerHTML = `
            <tr>
                <td colspan="8" class="text-center py-4">
                    Loading patient records...
                </td>
            </tr>`;

        showStatus("Loading patient records...", "info");

        try {
            const response = await fetch(apiUrl, {
                method: "GET",
                credentials: "same-origin",
                headers: {
                    "Accept": "application/json"
                }
            });

            // Diagnostic alert: displays the API HTTP status.
            //alert("Patients API HTTP status: " + response.status);

            console.log("Patients API URL:", response.url);
            console.log("Patients API status:", response.status);
            console.log(
                "Patients API content type:",
                response.headers.get("content-type")
            );

            if (!response.ok) {
                const errorText = await response.text();

                throw new Error(
                    `HTTP ${response.status} ${response.statusText}: ` +
                    errorText.substring(0, 500)
                );
            }

            const contentType =
                response.headers.get("content-type") || "";

            if (!contentType.includes("application/json")) {
                const responseText = await response.text();

                throw new Error(
                    "Expected JSON but received " + contentType +
                    ". Response: " + responseText.substring(0, 300)
                );
            }

            const data = await response.json();

            if (!Array.isArray(data)) {
                throw new Error(
                    "The API response is not a patient array."
                );
            }

            patients = data;
            renderPatients();

            showStatus(
                `Successfully loaded ${patients.length} patient record(s).`,
                "success"
            );

        } catch (error) {
            console.error("Unable to load patients:", error);

            patients = [];
            totalPatients.textContent = "—";
            resultCount.textContent = "";

            tableBody.innerHTML = `
                <tr>
                    <td colspan="8"
                        class="text-center text-danger py-4">
                        Unable to load patient records.
                    </td>
                </tr>`;

            showStatus(
                "Could not load patients: " + error.message,
                "danger"
            );

        } finally {
            refreshButton.disabled = false;
            refreshButton.textContent = "Refresh Records";
        }
    
    }

    // VIEW PATIENT DETAILS
    function showPatientDetails(patient) {
        detailsTitle.textContent =
            `Patient Details — ${patient.patientName || "Patient"}`;

        const fields = [
            ["Patient ID", patient.patientId],
            ["Patient Name", patient.patientName],
            ["Date of Birth", formatDate(patient.dateOfBirth)],
            ["Gender", patient.gender],
            ["Phone", patient.phone],
            ["Email", patient.email],
            ["Address", patient.address],
            ["Blood Group", patient.bloodGroup],
            ["Emergency Contact", patient.emergencyContact],
            ["Medical History", patient.medicalHistory]
        ];

        detailsBody.innerHTML = `
            <div class="row g-3">
                ${fields.map(([label, value]) => `
                    <div class="col-md-6">
                        <div class="border rounded p-3 h-100">
                            <div class="text-secondary small mb-1">
                                ${escapeHtml(label)}
                            </div>
                            <div class="fw-semibold text-break">
                                ${escapeHtml(value || "Not provided")}
                            </div>
                        </div>
                    </div>
                `).join("")}
            </div>`;

        bootstrap.Modal
            .getOrCreateInstance(document.getElementById("patientModal"))
            .show();
    }

    // CREATE PATIENT JSON
    function getPatientFormData() {
        return {
            patientName: getField("patientName").value.trim(),
            dateOfBirth: getField("dateOfBirth").value,
            gender: getField("gender").value,
            phone: getField("phone").value.trim(),
            email: getField("email").value.trim() || null,
            address: getField("address").value.trim() || null,
            bloodGroup: getField("bloodGroup").value || null,
            emergencyContact:
                getField("emergencyContact").value.trim() || null,
            medicalHistory:
                getField("medicalHistory").value.trim() || null
        };
    }

    // READ SERVER ERROR
    async function readErrorMessage(response) {
        const text = await response.text();

        if (!text) {
            return `Request failed with HTTP ${response.status}.`;
        }

        try {
            const data = JSON.parse(text);

            if (data.errors) {
                return Object.values(data.errors).flat().join(" ");
            }

            return data.title || data.message || text;
        } catch {
            return text;
        }
    }

    // ADD AND EDIT PATIENT
    async function savePatient(event) {
        event.preventDefault();
        clearFormError();

        if (!patientForm.reportValidity()) return;

        const patientId = getField("patientId").value;
        const isEditing = patientId !== "";
        const patientData = getPatientFormData();

        const originalButtonText = savePatientButton.textContent;

        savePatientButton.disabled = true;
        savePatientButton.textContent =
            isEditing ? "Updating..." : "Saving...";

        try {
            const url = isEditing
                ? `${apiUrl}/${encodeURIComponent(patientId)}`
                : apiUrl;

            const response = await fetch(url, {
                method: isEditing ? "PUT" : "POST",
                credentials: "same-origin",
                headers: {
                    "Content-Type": "application/json",
                    "Accept": "application/json"
                },
                body: JSON.stringify({
                    ...patientData,
                    ...(isEditing
                        ? { patientId: Number(patientId) }
                        : {})
                })
            });

            if (!response.ok) {
                throw new Error(await readErrorMessage(response));
            }

            bootstrap.Modal
                .getOrCreateInstance(patientFormModal)
                .hide();

            showStatus(
                isEditing
                    ? "Patient updated successfully."
                    : "Patient added successfully.",
                "success"
            );

            await loadPatients();
        } catch (error) {
            console.error("Unable to save patient:", error);
            showFormError(error.message || "Unable to save patient.");
        } finally {
            savePatientButton.disabled = false;
            savePatientButton.textContent = originalButtonText;
        }
    }

    // DELETE PATIENT
    async function deletePatient(patient) {
        const confirmed = window.confirm(
            `Delete patient "${patient.patientName}" (ID: ${patient.patientId})?\n\nThis action may not be reversible.`
        );

        if (!confirmed) return;

        try {
            const response = await fetch(
                `${apiUrl}/${encodeURIComponent(patient.patientId)}`,
                {
                    method: "DELETE",
                    credentials: "same-origin",
                    headers: {
                        "Accept": "application/json"
                    }
                }
            );

            if (!response.ok) {
                throw new Error(await readErrorMessage(response));
            }

            showStatus("Patient deleted successfully.", "success");
            await loadPatients();
        } catch (error) {
            console.error("Unable to delete patient:", error);

            showStatus(
                error.message || "Unable to delete patient.",
                "danger"
            );
        }
    }

    // EVENT HANDLERS
    searchInput.addEventListener("input", renderPatients);
    refreshButton.addEventListener("click", loadPatients);
    addPatientButton.addEventListener("click", openAddPatientForm);
    patientForm.addEventListener("submit", savePatient);

    tableBody.addEventListener("click", event => {
        const button = event.target.closest("button[data-patient-id]");

        if (!button) return;

        const patient = patients.find(
            item => String(item.patientId) === button.dataset.patientId
        );

        if (!patient) {
            showStatus(
                "Patient record not found. Refresh and try again.",
                "warning"
            );
            return;
        }

        if (button.classList.contains("details-button")) {
            showPatientDetails(patient);
        } else if (button.classList.contains("edit-button")) {
            openEditPatientForm(patient);
        } else if (button.classList.contains("delete-button")) {
            deletePatient(patient);
        }
    });

    patientFormModal.addEventListener(
        "hidden.bs.modal",
        resetPatientForm
    );

    loadPatients();
});