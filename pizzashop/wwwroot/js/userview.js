$(document).ready(function () {

    loadUsers();

});

let currentSortBy = "Name";
let currentSortOrder = "asc";

$(document).on("click", ".sort-link", function (e) {
    e.preventDefault();

    let sortBy = $(this).data("column");
    let pageNumber = $(this).data("page");

    if (sortBy === currentSortBy) {
        currentSortOrder = currentSortOrder === "asc" ? "desc" : "asc";
    } else {
        currentSortBy = sortBy;
        currentSortOrder = "asc";
    }

    loadUsers(
        pageNumber,
        $("#usersPerPage").val(),
        currentSortBy,
        currentSortOrder,
        $("#searchUsers").val().trim(),
    );
});


function updateSortIcons() {
    $(".sort-icons .asc, .sort-icons .desc").removeClass("active");

    let activeIcon = currentSortOrder === "asc" ? ".asc" : ".desc";
    $(".sort-link[data-column='" + currentSortBy + "']").find(activeIcon).addClass("active");
}

$("#searchUsers").on("keyup", function () {
    clearTimeout(this.delay);
    this.delay = setTimeout(function () {
        searchUsers();
    }, 300);
});

function searchUsers() {
    let searchTerm = $("#searchUsers").val().trim();
    loadUsers(1, "Id", "asc", 5, searchTerm);

}

function loadUsers(pageNumber = 1, pageSize = 5, sortby = "Name", sortOrder = "asc", searchTerm = "") {
    $.ajax({
        url: "/Users/GetUsers",
        type: "GET",
        data: { pageNumber: pageNumber, pageSize: pageSize, sortby: sortby, sortOrder: sortOrder, searchTerm: searchTerm },
        success: function (response) {
            $("#userListContainer").html(response);
            $("#usersPerPage").val(pageSize);
            $(".userspagination-link").removeClass("active");
            $(".userspagination-link[data-page='" + pageNumber + "']").addClass("active");
            updateSortIcons();
        },

        error: function () {
            toastr.error("Error fetching Users.");
        }
    });

}
// Handle pagination button clicks
$(document).on("click", ".userspagination-link", function (e) {
    e.preventDefault();
    let pageNumber = $(this).data("page");
    let pageSize = $("#usersPerPage").val();
    let searchTerm = $("#searchUsers").val().trim();

    loadUsers(pageNumber, pageSize, currentSortBy, currentSortOrder, searchTerm);
});

// Handle items per page dropdown
$(document).on("change", "#usersPerPage", function () {
    let pageSize = $(this).val();
    let searchTerm = $("#searchUsers").val().trim();
    loadUsers(1, pageSize, currentSortBy, currentSortOrder, searchTerm);
});