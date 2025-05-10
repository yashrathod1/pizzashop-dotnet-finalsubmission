
$(document).ready(function () {

    $('#WaitingToken').on('hidden.bs.modal', function () {
        $(this).find('input[type="text"], input[type="email"], input[type="hidden"], textarea').val('');
        $(this).find('span.text-danger').text('');
    });

    $('#assignTableModal').on('hidden.bs.modal', function () {
        
        $(this).find('input[type="text"], input[type="email"], input[type="hidden"], textarea').val('');
        $('#assignSectionDropdown').val('');
        $('#assignTableDropdownButton').text('Select Tables');
        $('#sectionValidation').addClass('d-none').text('Required');
        $('#tableValidation').addClass('d-none').text('Required');
    });

    loadWaitingList(null);

    $(".nav-link").on("click", function () {
        const sectionId = $(this).data("id");
        loadWaitingList(sectionId);
    });

});

function loadWaitingList(sectionId) {
    const containerId = sectionId ? `#waitingListContainer-${sectionId}` : "#waitingListContainer";

    $.ajax({
        url: '/WaitingList/GetWaitingListBySection',
        type: 'GET',
        data: { sectionId: sectionId },
        success: function (html) {
            $(containerId).html(html);
            initLiveTimers();
        },
        error: function () {
            $(containerId).html('<p class="text-danger">Failed to load waiting list.</p>');
        }
    });
}

// get customer by email
$(document).on('blur', '#customerEmail', function () {
    var email = $(this).val();
    if (email) {
        $.ajax({
            url: '/WaitingList/GetCustomerByEmail',
            type: 'GET',
            data: { email: email },
            success: function (data) {
                if (data) {
                    $('#customerName').val(data.name);
                    $('#customerMobile').val(data.mobileNo);
                    $('#customerTokenId').val(data.id);
                    toastr.success("Customer Details Loaded Successfull");
                } else {
                    $('#customerName, #customerMobile, #totalPerson').val('');

                }
            }
        });
    }
});

$(document).on("submit", "#AddWaitingTokenTable", function (e) {
    e.preventDefault();

    if (!$(this).valid()) {
        return false;
    }

    var formData = new FormData(this);

    $.ajax({
        url: '/WaitingList/CreateWaitingToken',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.success) {
                $('#WaitingToken').modal('hide');
                $('#AddWaitingTokenTable')[0].reset();
                toastr.success(response.message);
                setTimeout(function () {
                    window.location.reload();
                }, 500);
            } else {
                toastr.error(response.message);
            }

        },
        error: function (error) {
            toastr.error(error.message);
        }
    });
});

// dropdown in add waiting token the modal

$(document).on("click", ".wait-token-btn", function () {

    var sectionId = $(".nav-tabs .nav-link.active").data("id");

    $.ajax({
        url: '/WaitingList/GetSections',
        type: 'GET',
        success: function (sections) {
            var $dropdown = $("#WaititngTokensectionId");
            $dropdown.empty();
            $.each(sections, function (i, section) {
                var isSelected = section.id === sectionId;
                $dropdown.append($('<option>', {
                    value: section.id,
                    text: section.name,
                    selected: isSelected
                }));
            });

            $("#selectedSectionId").val(sectionId)
        },
        error: function () {
            alert('Failed to load sections.');
        }
    });
});

// get detail on click pencil

$(document).on("click", ".edit-waiting-token", function () {
    var tokenId = $(this).data("id");

    $.ajax({
        url: '/WaitingList/GetSections',
        type: 'GET',
        success: function (sections) {
            var $dropdown = $("#eWaititngTokensectionId");
            $dropdown.empty();
            $.each(sections, function (i, section) {
                $dropdown.append($('<option>', {
                    value: section.id,
                    text: section.name,
                }));
            });
        },
        error: function () {
            alert('Failed to load sections.');
        }
    });

    $.ajax({
        url: '/WaitingList/GetTokenById',
        type: 'GET',
        data: { id: tokenId },
        success: function (data) {
            if (data) {
                $('#tokenId').val(data.id);
                $('#ecustomerName').val(data.name);
                $('#ecustomerEmail').val(data.email);
                $('#ecustomerMobile').val(data.phoneNumber);
                $('#enop').val(data.noOfPerson);
                $('#eWaititngTokensectionId').val(data.sectionId);
            } else {
                toastr.error("Token not found.");
            }
            $('#editWaitingToken').modal('show');
        },
        error: function () {
            toastr.error("Error fetching token.");
        }
    });
});


// edit waiting token

$(document).on("submit", "#EditWaitingTokenTable", function (e) {
    e.preventDefault();

    if (!$(this).valid()) {
        return;
    }

    var sectionId = $(".nav-tabs .nav-link.active").data("id");

    formData = {
        Id: $("#tokenId").val(),
        Name: $("#ecustomerName").val(),
        Email: $("#ecustomerEmail").val(),
        MobileNo: $("#ecustomerMobile").val(),
        SectionId: $("#eWaititngTokensectionId").val(),
        NoOfPerson: $("#enop").val()
    };

    $.ajax({
        url: "/WaitingList/EditWaitingToken",
        type: 'POST',
        data: formData,
        success: function (response) {
            if (response.success) {
                toastr.success("WaitingToken edited successfully");
                $("#editWaitingToken").modal("hide");
                $("#EditWaitingTokenTable")[0].reset();
                // loadWaitingList(sectionId);
                setTimeout(function () {
                    window.location.reload();
                }, 500);

            } else {
                toastr.error(response.message);
            }
        },
        error: function () {
            toastr.error("Error updating modifier group.");
        }
    });
});


// delete the waitingtoken

$(document).on('click', '.token-delete-btn', function () {
    var id = $(this).data('id');
    $('#deleteWaitingTokenModal').modal('show');
    $('#confirmDeleteBtnToken').data('id', id);
});

$('#confirmDeleteBtnToken').on('click', function () {
    var id = $(this).data('id');
    var sectionId = $(".nav-tabs .nav-link.active").data("id");

    $.ajax({
        url: '/WaitingList/SoftDeleteToken',
        type: 'POST',
        data: { id: id },
        success: function (response) {
            if (response.success) {
                toastr.success(response.message);
                $('#deleteWaitingTokenModal').modal('hide');
                // loadWaitingList(sectionId);
                setTimeout(function () {
                    window.location.reload();
                }, 500);
            } else {
                toastr.error(response.message);
            }
        },
        error: function () {
            alert('An error occurred while attempting to delete the waiting token.');
        }
    });

});

//live timer

function initLiveTimers() {
    $('.live-timer-token').each(function () {
        var $this = $(this);
        var startTimeStrr = $this.data('starttime');

        // Ensure startTime is a valid date
        var startTime = new Date(startTimeStrr);

        var updateTimer = function () {
            var now = new Date();
            var diff = now - startTime;
            var totalSeconds = Math.floor(diff / 1000);

            var days = Math.floor(totalSeconds / (3600 * 24));
            var hours = Math.floor((totalSeconds % (3600 * 24)) / 3600);
            var minutes = Math.floor((totalSeconds % 3600) / 60);
            var seconds = totalSeconds % 60;


            $this.text(days + " days " + hours + " hours " + minutes + " min " + seconds + " sec");
        };
        updateTimer();
        setInterval(updateTimer, 1000);
    });
}


// table and section assign

$(document).on('click', '.table-assign', function () {
    $('#assignTableModal').modal('show');
    const sectionId = $(".nav-tabs .nav-link.active").data("id");
    const customerId = $(this).data('customer-id');
    const numberOfPersons = $(this).data('number-of-persons');

    $('#customerIdInput').val(customerId);
    $('#numberOfPersonsInput').val(numberOfPersons);

    $.ajax({
        url: '/WaitingList/GetAvailableSections',
        type: 'GET',
        success: function (sections) {
            const $sectionDropdown = $('#assignSectionDropdown');
            $sectionDropdown.empty().append('<option value="">Section*</option>');

            let hasPreselected = false;
            $.each(sections, function (i, section) {
                const isSelected = section.id === sectionId;
                if (isSelected) hasPreselected = true;
                $sectionDropdown.append($('<option>', {
                    value: section.id,
                    text: section.name,
                    selected: isSelected
                }));
            });

            // Clear and reset table dropdown UI
            $('#assignTableDropdown').empty();
            $('#assignTableDropdownButton').text('Select Tables').prop('disabled', true);

            $("#selectedSectionId").val(sectionId);

            if (hasPreselected) {
                loadAvailableTables(sectionId);
            }

        },
        error: function () {
            alert('Failed to load sections.');
        }
    });
});

$('#assignSectionDropdown').on('change', function () {
    const sectionId = $(this).val();
    validateInputs();
    loadAvailableTables(sectionId);
});

function loadAvailableTables(sectionId) {
    if (!sectionId) {
        $('#assignTableDropdownButton').text('Select Tables').prop('disabled', true);
        $('#assignTableDropdown').empty();
        return;
    }

    $.ajax({
        url: `/WaitingList/GetAvailableTables?sectionId=${sectionId}`,
        type: 'GET',
        success: function (tables) {
            const $tableDropdown = $('#assignTableDropdown');
            $tableDropdown.empty();

            $.each(tables, function (i, table) {
                const item = `
                    <li>
                        <label class="dropdown-item">
                            <input type="checkbox" class="table-checkbox" value="${table.id}"> ${table.name}
                        </label>
                    </li>`;
                $tableDropdown.append(item);
            });

            $('#assignTableDropdownButton')
                .text('Select Tables')
                .prop('disabled', false);
        }
    });
}

// Update table button label and validate on checkbox change
$(document).on('change', '.table-checkbox', function () {
    const selected = $('.table-checkbox:checked').map(function () {
        return $(this).closest('label').text().trim();
    }).get();

    const text = selected.length > 0 ? selected.join(', ') : 'Select Tables';
    $('#assignTableDropdownButton').text(text);

    validateInputs();
});

// Validation logic
function validateInputs() {
    const sectionId = $('#assignSectionDropdown').val();
    const selectedTables = $('.table-checkbox:checked');

    if (!sectionId) {
        $('#sectionValidation').removeClass('d-none');
    } else {
        $('#sectionValidation').addClass('d-none');

        if (selectedTables.length === 0) {
            $('#tableValidation').removeClass('d-none');
        } else {
            $('#tableValidation').addClass('d-none');
        }
    }

    return sectionId && selectedTables.length > 0;
}

// Final submission handler
$(document).on('click', '#assignTableButton', function () {
    const isValid = validateInputs();
    if (!isValid) return;

    const sectionId = $('#assignSectionDropdown').val();
    const selectedTables = $('.table-checkbox:checked').map(function () {
        return parseInt($(this).val());
    }).get();
    const numberOfPersons = parseInt($('#numberOfPersonsInput').val());
    const customerId = parseInt($('#customerIdInput').val());

    const data = {
        SectionId: sectionId,
        SelectedTables: selectedTables,
        NumberOfPersons: numberOfPersons,
        CustomerId: customerId
    };

    $.ajax({
        url: '/WaitingList/AssignTables',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            if (response.success) {
                toastr.success(response.message);
                $('#assignTableModal').modal('hide');
                console.log("order id is", response.orderId)

                if (response.orderId) {
                    window.location.href = '/MenuApp/Index?orderId=' + response.orderId;
                }
            } else {
                toastr.error(response.message);
            }

        },
        error: function () {
            toastr.error("An error occurred while assigning tables.");
        }
    });
});
