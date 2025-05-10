$(document).ready(function () {

    $('#WaitingToken').on('hidden.bs.modal', function () {
        $(this).find('input[type="text"], input[type="email"], input[type="hidden"], textarea').val('');
        $(this).find('span.text-danger').text('');
    });

    $('#AssignTable').on('hidden.bs.offcanvas', function () {
        $(this).find('input[type="text"], input[type="email"], input[type="hidden"], textarea').val('');
        $(this).find('span.text-danger').text('');
    });


    $('[data-bs-toggle="collapse"]').each(function () {
        var target = $(this).data("bs-target");
        var chevron = $(this).find('img');
        var borderBox = $(this);

        $(target).on('show.bs.collapse', function () {
            chevron.attr('src', '/images/icons/chevron-down.svg');
            borderBox.css('border-color', '#90caf9');
        });

        $(target).on('hide.bs.collapse', function () {
            chevron.attr('src', '/images/icons/chevron-right.svg');
            borderBox.css('border-color', '#e6e5e3');
        });
    });

    initLiveTimers();

});

let selectedSectionId = null;
let selectedTables = [];

$(document).on('click', '.task-card', function () {
    const status = $(this).data('status');
    const tableId = $(this).data('table-id');
    const sectionId = $(this).data('section-id');

    if (status === 'Assigned' || status === 'Occupied') {
        $.ajax({
            url: '/Tables/GetOrderIdByTable',
            type: 'GET',
            data: { tableId: tableId },
            success: function (orderId) {
                window.location.href = '/MenuApp/Index?orderId=' + orderId;
            },
            error: function () {
                toastr.error('Failed to get order ID for this table.');
            }
        });
        return;
    }

    if (!selectedSectionId) {
        selectedSectionId = sectionId;
    }

    if (sectionId !== selectedSectionId) {
        toastr.error('You can only select tables from one section.');
        return;
    }

    const isSelected = $(this).hasClass('bg-selected');

    if (isSelected) {
        $(this).removeClass('bg-selected');
        selectedTables = selectedTables.filter(id => id !== tableId);
    } else {
        $(this).addClass('bg-selected');
        selectedTables.push(tableId);
    }

    const sectionContainer = $(this).closest('.accordion-body');
    sectionContainer.find('.assign-btn').prop('disabled', selectedTables.length === 0);

    if (selectedTables.length > 0) {
        sectionContainer.find('.assign-btn').prop('disabled', false);
    } else {
        selectedSectionId = null;
    }
});


$(document).on("click", ".wait-token-btn", function () {
    var sectionId = $(this).data("section-id");

    $.ajax({
        url: '/Tables/GetSections',
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
        },
        error: function () {
            alert('Failed to load sections.');
        }
    });
});

$(document).on("submit", "#AddWaitingTokenTable", function (e) {
    e.preventDefault();

    if (!$(this).valid()) {
        return false;
    }

    var formData = new FormData(this);

    $.ajax({
        url: '/Tables/AddWaitingToken',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {

            $('#WaitingToken').modal('hide');
            $('#AddWaitingTokenTable')[0].reset();
            toastr.success(response.message);
        },
        error: function (error) {
            toastr.error(error.message);
        }
    });
});

//dropdown in the canvas
$(document).on("click", "#customerAssignbtn", function () {
    var sectionId = $(this).data("section-id");

    $.ajax({
        url: '/Tables/GetSections',
        type: 'GET',
        success: function (sections) {
            var $dropdown = $("#customerSectionId");
            $dropdown.empty();
            $.each(sections, function (i, section) {
                var isSelected = section.id === sectionId;
                $dropdown.append($('<option>', {
                    value: section.id,
                    text: section.name,
                    selected: isSelected
                }));
            });
        },
        error: function () {
            alert('Failed to load sections.');
        }
    });
});

$(document).on("click", '#customerAssignbtn', function () {
    var sectionId = $(this).data('section-id');

    $.ajax({
        url: '/Tables/GetWaitingTokenList',
        type: 'GET',
        data: { sectionId: sectionId },
        success: function (response) {
            $('#waitingListTableModule').html(response);
        },
        error: function (xhr, status, error) {
            console.log("Error fetching the waiting token list");
        }
    });
});

// get data on radio utton
$(document).on('change', '.selectedRadioBtn', function () {
    var tokenId = $(this).data("token-id");
    $.ajax({
        url: '/Tables/GetCustomerDetailsByToken',
        type: 'GET',
        data: { tokenId: tokenId },
        success: function (data) {
            if (data) {
                $('#customerTokenId').val(data.id);
                $('#customerEmail').val(data.email);
                $('#customerName').val(data.name);
                $('#customerMobile').val(data.mobileNo);
                $('#totalPerson').val(data.noOfPerson);
                $('#customerSectionId').val(data.sectionId);
            }
            toastr.success("Customer Details Loaded Successfull");
        },
        error: function () {
            console.log("Failed to load customer details");
        }
    });
});

// get customer by email
$(document).on('blur', '#customerEmail', function () {
    var email = $(this).val();
    if (email) {
        $.ajax({
            url: '/Tables/GetCustomerByEmail', // update with actual controller name
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

$("#assignTableCustomer").on("submit", function (e) {
    e.preventDefault();

    const numberOfPersons = parseInt($('#totalPerson').val());

    const customerData = {
        Name: $('#customerName').val(),
        Email: $('#customerEmail').val(),
        MobileNo: $('#customerMobile').val()
    };

    $.ajax({
        url: '/Tables/AssignTables',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            selectedTables: selectedTables,
            numberOfPersons: numberOfPersons,
            customer: customerData
        }),
        success: function (response) {
            if (response.success) {
                toastr.success('Tables assigned successfully!');
                selectedTables = [];
                selectedSectionId = null;
                $('.bg-selected').removeClass('bg-selected');
                setTimeout(function () {
                    location.reload();
                }, 1500);
            } else {
                toastr.error(response.message);
            }
        },
        error: function () {
            toastr.error("Something went wrong while assigning tables.");
        }
    });
});


//live timer
function initLiveTimers() {
    $('.live-timer').each(function () {
        var $this = $(this);
        var startTimeStr = $this.data('starttime');

        if (!startTimeStr) {
            $this.text('0 min');
            return;
        }
        var startTime = new Date(startTimeStr);
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