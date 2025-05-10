$(document).ready(function () {
    loadSections();
    loadTables(1);
});


$(document).on("click", "#selectAllCheckbox", function () {
    $(".row-checkbox").prop("checked", this.checked);
});


$(document).on("click", ".row-checkbox", function () {
    if (!$(this).prop("checked")) {
        $("#selectAllCheckbox").prop("checked", false);
    } else {
        if ($(".row-checkbox:checked").length === $(".row-checkbox").length) {
            $("#selectAllCheckbox").prop("checked", true);
        }
    }
});


function loadSections() {
    $.ajax({
        url: "/TableAndSection/GetSections",
        type: "GET",
        success: function (response) {
            $("#sectionsContainer").html(response);

            $(".section-item").first().trigger("click");
        },
        error: function () {
            toastr.error("Error fetching Sections groups.");
        }
    });
}
$(document).on("click", ".section-item", function () {
    let sectionId = $(this).data("id");
    $(".section-item").removeClass("selected");
    $(this).addClass("selected");
    loadTables(sectionId, pageNumber = 1, pageSize = 5);
});

$("#searchTables").on("keyup", function () {
    clearTimeout(this.delay);
    this.delay = setTimeout(function () {
        searchTables();
    }, 300);
});

function searchTables() {
    let sectionId = $(".section-item.selected").data("id");
    let searchTerm = $("#searchTables").val().trim();
    loadTables(sectionId, 1, 5, searchTerm);
}

function loadTables(sectionId, pageNumber = 1, pageSize = 5, searchTerm = "") {
    $.ajax({
        url: "/TableAndSection/GetTableBySection",
        type: "GET",
        data: { sectionId: sectionId, pageNumber: pageNumber, pageSize: pageSize, searchTerm: searchTerm },
        success: function (response) {
            $("#TableListContainer").html(response);
            $("#tablesPerPage").val(pageSize);
            $(".pagination-link").removeClass("active");
            $(".pagination-link[data-page='" + pageNumber + "']").addClass("active");
        },

        error: function () {
            toastr.error("Error fetching tables.");
        }
    });
}

// Handle pagination button clicks
$(document).on("click", ".tablepagination-link", function (e) {
    e.preventDefault();
    let sectionId = $(".section-item.selected").data("id");
    let pageNumber = $(this).data("page");
    let pageSize = $("#tablesPerPage").val();
    loadTables(sectionId, pageNumber, pageSize);
});

// Handle items per page dropdown
$(document).on("change", "#tablesPerPage", function () {
    let sectionId = $(".section-item.selected").data("id");
    loadTables(sectionId, 1, $(this).val());
});


// @* addsection *@

$("#addSectionForm").submit(function (e) {
    e.preventDefault();

    if (!$(this).valid()) {
        return;
    }
    
    let sectionData = {
        name: $("#sectionname").val(),
        description: $("#sectiondescription").val()
    };

    $.ajax({
        url: "/TableAndSection/AddSection",
        type: "POST",
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify(sectionData),
        success: function (response) {
            if (response.success) {
                toastr.success(response.message);
                $("#addSectionModal").modal("hide");
                loadSections();
                $("#addSectionForm")[0].reset();    
            } else {
                toastr.error(response.message);
            }
        },
        error: function () {
            toastr.error("Error adding Section");
        }
    });
});



// @* editsection  *@

$(document).on("click", ".edit-section", function () {
    let sectionId = $(this).data("id");

    $.ajax({
        url: '/TableAndSection/GetSectionById',
        type: 'GET',
        data: { id: sectionId },
        success: function (section) {
            $("#SectionId1").val(section.id);
            $("#Name").val(section.name);
            $("#Description").val(section.description);
            $("#editSectionModal").modal('show');
        }
    });
});


$("#editSectionForm").submit(function (e) {
    e.preventDefault();

    if (!$(this).valid()) {
        return;
    }

    let sectionData = {
        Id: $("#SectionId1").val(),
        Name: $("#Name").val(),
        Description: $("#Description").val()
    };

    $.ajax({
        url: "/TableAndSection/UpdateSection",
        type: "PUT",
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify(sectionData),
        success: function (response) {
            if (response.success) {
                toastr.success(response.message);
                $("#editSectionModal").modal("hide");
                loadSections();
            } else {
                toastr.error(response.message);
            }
        },
        error: function (xhr, status, error) {
            toastr.error(" Error Response:", xhr.responseText);
        }
    });
});




// @* deletesection *@


$(document).on("click", ".delete-section", function () {
    let sectionId = $(this).data("id");
    $("#confirmDeleteBtnSection").data("section-id", sectionId);
    $("#deleteSectionModal").modal("show");
});

$("#confirmDeleteBtnSection").click(function () {
    let sectionId = $(this).data("section-id");
    deleteSection(sectionId);
    $("#deleteSectionModal").modal("hide");
});

function deleteSection(sectionId) {

    $.ajax({
        url: "/TableAndSection/DeleteSections",
        type: "POST",
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify({ id: sectionId }),
        success: function (response) {
            if (response.success) {
                toastr.success("Section deleted successfully");
                loadSections();
            } else {
                toastr.error("Failed to delete Section");
            }
        },
        error: function () {
            toastr.error("Error deleting Section");
        }
    });
}


// addtable into section

$(document).ready(function () {
    $("#addTableForm").submit(function (e) {
        e.preventDefault();

        
        let isValid = $(this).validate().form();
        if (!isValid) {
            return;
        }

        var formData = new FormData(this);

        formData.append("Name", $("input[name='Table.Name']").val()); 
        formData.append("SectionId", $("select[name='Table.SectionId']").val());
        formData.append("Capacity", $("input[name='Table.Capacity']").val());
        formData.append("Status", $("select[name='Table.Status']").val());

        $.ajax({
            url: '/TableAndSection/AddTable',
            type: 'POST',
            data: formData,
            processData: false, 
            contentType: false, 
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    $("#addTableModal").modal("hide");
                    $("#addTableForm")[0].reset();

                    let currentSectionId = $(".section-item.selected").data("id");
                    let pageSize = $("#tablesPerPage").val();
                    let pageNumber = $(".tablepagination-link.active").data("page");

                    loadTables(currentSectionId, pageNumber, pageSize);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function () {
                toastr.error("Something went wrong");
            }
        });
    });
});



// getdata for edit

$(document).on("click", ".edit-table", function () {
    let tableId = $(this).data("id");

    $.ajax({
        url: '/TableAndSection/GetTableById',
        type: 'GET',
        data: { id: tableId },
        success: function (table) {
            $("#Name1").val(table.name);
            $("#SectionId2").val(table.sectionId);
            $("#Capacity1").val(table.capacity);
            $("#Status1").val(table.status);
            $("#tableId1").val(table.id);

            if ($("#editTableModal").length > 0) {
                $("#editTableModal").modal('show');
            } else {
                toastr.error("Modal not found in the DOM!");
            }
        }
    });
});

$("#editTableForm").submit(function (e) {
    e.preventDefault();

    if (!$(this).valid()) {
        return;
    }

    let tableData = {
        Id: $("#tableId1").val(),
        Name: $("#Name1").val(),
        SectionId: $("#SectionId2").val(),
        Capacity: $("#Capacity1").val(),
        Status: $("#Status1").val()
    };

    $.ajax({
        url: '/TableAndSection/EditTable',
        type: 'POST',
        contentType: "application/json",
        data: JSON.stringify(tableData),
        success: function (response) {
            if(response.success){
                toastr.success(response.message);
                $("#editTableModal").modal('hide');
                let currentSectionId = $(".section-item.selected").data("id");
                let pageSize = $("#tablesPerPage").val();
                let pageNumber = $(".tablepagination-link.active").data("page");
    
                loadTables(currentSectionId, pageNumber, pageSize);

            }else{
                toastr.error(response.message);
            }
        },
        error: function () {
            toastr.error("Failed to update table.");
        }
    });
});


$(document).on("click", ".delete-table", function () {
    var tableId = $(this).data("id");
    $("#confirmDeleteBtnTable").data("id", tableId);
    $("#deleteTableModal").modal("show");
});

$("#confirmDeleteBtnTable").click(function () {
    var tableId = $(this).data("id");
    deleteTable(tableId);
    $("#deleteTableModal").modal("hide");
});

function deleteTable(tableId) {
    $.ajax({
        url: "/TableAndSection/DeleteTable",
        type: "POST",
        contentType: "application/json",
        dataType: "json",
        data: JSON.stringify({ id: tableId }),
        success: function (response) {
            if (response.success) {
                toastr.success("Table deleted successfully");
                let currentSectionId = $(".section-item.selected").data("id");
                let pageSize = $("#tablesPerPage").val();
                let pageNumber = $(".tablepagination-link.active").data("page");
    
                loadTables(currentSectionId, pageNumber, pageSize);
            } else {
                toastr.error("Failed to delete Item");
            }
        },
        error: function () {
            toastr.error("Error deleting Table");
        }
    });
}

// for masstable delete

$("#deleteSelectedBtnForTables").click(function () {
    let selectedIds = [];
    $(".row-checkbox:checked").each(function () {
        selectedIds.push($(this).val());
    });

    if (selectedIds.length === 0) {
        toastr.warning("Please select at least one item.");
        return;
    }
   
    $("#deleteTablesId").val(selectedIds.join(","));
    $("#deleteTablesModal").modal("show");
});


$("#confirmDeleteBtnTables").click(function () {
    let tableIds = $("#deleteTablesId").val().split(",").map(Number);
   
    $.ajax({
        url: '/TableAndSection/SoftDeleteTables',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(tableIds),
        success: function (response) {
            if (response.success) {
                $("#deleteTablesModal").modal('hide');
                toastr.success("Table Deleted Successfully");
                let currentSectionId = $(".section-item.selected").data("id");
                let pageSize = $("#tablesPerPage").val();
                let pageNumber = $(".tablepagination-link.active").data("page");
    
                loadTables(currentSectionId, pageNumber, pageSize);
            } else {
                toastr.error("Failed to Delete Tables");
            }
        }
    });
    
});