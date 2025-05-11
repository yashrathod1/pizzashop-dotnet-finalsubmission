$(document).ready(function () {
    var selectedCountryId = $("#Country").attr("data-selected-id");
    var selectedStateId = $("#State").attr("data-selected-id");
    var selectedCityId = $("#City").attr("data-selected-id");

    $.ajax({
        url: "/GetData/GetCountries",
        type: "GET",
        success: function (data) {
            $("#Country").empty().append('<option value="">Select Country</option>');
            $.each(data, function (index, country) {
                $("#Country").append(
                    '<option value="' + country.countryId + '">' + country.countryName + "</option>"
                );
            });

            if (selectedCountryId) {
                $("#Country").val(selectedCountryId).change();
            }
        },
    });

    $("#Country").change(function () {
        var countryId = $(this).val();
        $("#State").empty().append('<option value="">Select State</option>');
        $("#City").empty().append('<option value="">Select City</option>');

        if (!countryId) return;

        $.ajax({
            url: "/GetData/GetStates",
            type: "GET",
            data: { countryId: countryId },
            success: function (data) {
                $.each(data, function (index, state) {
                    $("#State").append(
                        '<option value="' +
                        state.stateId +
                        '">' +
                        state.stateName +
                        "</option>"
                    );
                });

                if (
                    selectedStateId &&
                    $("#State option[value='" + selectedStateId + "']").length
                ) {
                    $("#State").val(selectedStateId).change();
                } else {
                    selectedStateId = null;
                }
            },
        });
    });

    $("#State").change(function () {
        var stateId = $(this).val();
        $("#City").empty().append('<option value="">Select City</option>');

        if (!stateId) return;

        $.ajax({
            url: "/GetData/GetCities",
            type: "GET",
            data: { stateId: stateId },
            success: function (data) {
                $.each(data, function (index, city) {
                    $("#City").append(
                        '<option value="' + city.cityId + '">' + city.cityName + "</option>"
                    );
                });

                if (
                    selectedCityId &&
                    $("#City option[value='" + selectedCityId + "']").length
                ) {
                    $("#City").val(selectedCityId);
                } else {
                    selectedCityId = null;
                }
            },
        });
    });

    $("#State").click(function () {
        if (!$("#Country").val()) {
            toastr.warning("Please select a Country first!");
            $("#State").val("");
        }
    });

    $("#City").click(function () {
        if (!$("#State").val()) {
            toastr.warning("Please select a state first!");
            $("#City").val("");
        }
    });

    //change password

    $(".toggle-password").click(function () {
        var targetId = $(this).data("target");
        var input = $(targetId);
        if (input.length) {
            if (input.attr("type") === "password") {
                input.attr("type", "text");
                $(this).removeClass("fa-eye").addClass("fa-eye-slash");
            } else {
                input.attr("type", "password");
                $(this).removeClass("fa-eye-slash").addClass("fa-eye");
            }
        }
    });
});

// profile image logic

function OpenFile() {
    $("#FileInput").click();
}

$(document).ready(function () {
    $("#FileInput").on("change", function (event) {
        var file = event.target.files[0];

        if (file) {
            var allowedExtensions = ["jpg", "jpeg", "png"];
            var fileExtension = file.name.split(".").pop().toLowerCase();

            if (!allowedExtensions.includes(fileExtension)) {
                toastr.error("Only .jpg, .jpeg, and .png image files are allowed!");
                $("#ProfileImagePreview").attr("src", "/images/Default_pfp.svg.png");
                event.target.value = "";
                return;
            }

            var reader = new FileReader();
            reader.onload = function (e) {
                $("#ProfileImagePreview").attr("src", e.target.result);
            };
            reader.readAsDataURL(file);
        }
    });
});


/// dashboard
  $(document).ready(function () {

        LoadDashboardDetails("Current Month");

        $('#filterSelect').on('change', function () {
            const filterType = $(this).val();
            if (filterType === "Custom") {
                $('#startDate').val('');
                $('#endDate').val('');
                $('#customDateModal').modal('show');
            } else {
                LoadDashboardDetails(filterType);
            }
        });


        $('#applyDateRange').on('click', function () {
            const startDate = $('#startDate').val();
            const endDate = $('#endDate').val();

            if (!startDate || !endDate) {
                toastr.error("Please select both start and end dates.");
                return;
            }

            const start = new Date(startDate);
            const end = new Date(endDate);

            if (start > end) {
                toastr.error("Start date cannot be after end date.");
                return;
            }

            $('#customDateModal').modal('hide');

            // Send custom date range
            LoadDashboardDetails("Custom", startDate, endDate);
        });


        function LoadDashboardDetails(filterType, startDate = null, endDate = null) {
            $.ajax({
                url: '/Dashboard/GetDashboardData',
                type: 'GET',
                data: { filter: filterType, startDate: startDate, endDate: endDate },
                success: function (response) {
                    $('#totalSales').html(`&#x20b9; ${response.totalSales}`);
                    $('#totalOrders').html(response.totalOrders);
                    $('#avgOrderValue').html(`&#x20b9; ${response.averageOrderValue}`);
                    $('#waitingListCount').html(`${response.averageWaitingTime} mins`);
                    $('#avgWaitingTime').html(response.waitingListCount);
                    $('#newCustomerCount').html(response.newCustomer);

                    updateRevenueChart(response.revenueChartData);
                    updateCustomerChart(response.customerGrowthData);
                    updateSellingItems('#topSellingContainer', response.topSellingItems);
                    updateSellingItems('#leastSellingContainer', response.leastSellingItems);
                }
            });
        }


        function updateRevenueChart(data) {
            const values = data.map(d => d.value);
            const maxValue = Math.max(...values);
            const numberOfTicks = 5;

            function getRoundedStepSize(value) {
                if (value === 0) return 1;
                const exponent = Math.floor(Math.log10(value));
                const base = Math.pow(10, exponent);
                return Math.ceil(value / base) * base;
            }

            const rawStep = maxValue / numberOfTicks;
            const cleanStepSize = getRoundedStepSize(rawStep);
            const roundedMax = cleanStepSize * numberOfTicks;

            revenueChart.data.labels = data.map(d => d.label);
            revenueChart.data.datasets[0].data = values;

            revenueChart.options.scales.y.min = 0;
            revenueChart.options.scales.y.max = roundedMax;
            revenueChart.options.scales.y.ticks.stepSize = cleanStepSize;

            revenueChart.update();
        }

        function updateCustomerChart(data) {
            data.reverse();

            const values = data.map(d => d.value);
            const maxValue = Math.max(...values);
            const numberOfTicks = 5;

            function getRoundedStepSize(value) {
                if (value === 0) return 1;
                const exponent = Math.floor(Math.log10(value));
                const base = Math.pow(10, exponent);
                return Math.ceil(value / base) * base;
            }

            const rawStep = maxValue / numberOfTicks;
            const cleanStepSize = getRoundedStepSize(rawStep);
            const roundedMax = cleanStepSize * numberOfTicks;

            customerGrowthChart.data.labels = data.map(d => d.label);
            customerGrowthChart.data.datasets[0].data = values;

            customerGrowthChart.options.scales.y.min = 0;
            customerGrowthChart.options.scales.y.max = roundedMax;
            customerGrowthChart.options.scales.y.ticks.stepSize = cleanStepSize;

            customerGrowthChart.update();
        }

        function updateSellingItems(containerId, items) {
            const container = $(containerId);
            container.empty();
            if (items.length === 0) {
                container.append('<span class="text-muted">No data available.</span>');
                return;
            }
            items.forEach((item, index) => {
                container.append(`
                    <div class="d-flex justify-content-start align-items-center mb-3">
                        <span class="fw-bold">#${index + 1}</span>
                        <img src="${item.imageUrl}" height="40" width="40" class="mx-2 rounded">
                        <div class="d-flex flex-column">
                            <span>${item.name}</span>
                            <div class="d-flex align-items-center">${item.orderCount} Order${item.orderCount > 1 ? 's' : ''}</div>
                        </div>
                    </div>
                    <hr class="my-2"/>
                `);
            });
        }
    });

    // Revenue Chart
    const revenueCtx = $('#myChart')[0].getContext('2d');
    const revenueChart = new Chart(revenueCtx, {
        type: 'line',
        data: {
            labels: [],
            datasets: [{
                label: 'Revenue',
                data: [],
                borderColor: 'rgba(0, 102, 167)',
                backgroundColor: 'rgba(235,242,247,255)',
                tension: 0.4,
                fill: true
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { position: 'top' },
                title: { display: true }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: { stepSize: 1 }
                }
            }
        }
    });

    // Customer Growth Chart
    const customerCtx = $('#customerGrowthChart')[0].getContext('2d');
    const customerGrowthChart = new Chart(customerCtx, {
        type: 'line',
        data: {
            labels: [],
            datasets: [{
                label: 'Customer Growth',
                data: [],
                borderColor: 'rgba(0, 102, 167)',
                backgroundColor: 'rgba(235,242,247,255)',
                tension: 0.4,
                fill: true
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { position: 'top' },
                title: { display: true }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: { stepSize: 1 }
                }
            }
        }
    });