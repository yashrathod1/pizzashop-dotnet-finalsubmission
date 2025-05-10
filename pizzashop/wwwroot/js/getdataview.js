$(document).ready(function () {



    $('#togglePassword').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        const type = $('#password').attr('type') === 'password' ? 'text' : 'password';
        $('#password').attr('type', type);
        $(this).toggleClass('fa-eye-slash');
    });


    var selectedCountryId = $("#Country").attr("data-selected-id");
    var selectedStateId = $("#State").attr("data-selected-id");
    var selectedCityId = $("#City").attr("data-selected-id");


    $.ajax({
        url: "/GetData/GetCountries",
        type: "GET",
        success: function (data) {
            $("#Country").empty().append('<option value="">Select Country</option>');
            $.each(data, function (index, country) {
                $("#Country").append('<option value="' + country.countryId + '">' + country.countryName + '</option>');
            });


            if (selectedCountryId) {
                $("#Country").val(selectedCountryId).change();
            }
        }
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
                    $("#State").append('<option value="' + state.stateId + '">' + state.stateName + '</option>');
                });


                if (selectedStateId && $("#State option[value='" + selectedStateId + "']").length) {
                    $("#State").val(selectedStateId).change();
                } else {
                    selectedStateId = null;
                }
            }
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
                    $("#City").append('<option value="' + city.cityId + '">' + city.cityName + '</option>');
                });


                if (selectedCityId && $("#City option[value='" + selectedCityId + "']").length) {
                    $("#City").val(selectedCityId);
                } else {
                    selectedCityId = null;
                }
            }
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
});


$(document).on("change", ".image-input", function () {
    const file = this.files[0];
    const allowedExtensions = ['jpg', 'jpeg', 'png', 'gif', 'webp'];
    const maxSizeMB = 2;

    const container = $(this).closest(".upload-icon");
    const fileNameDisplay = container.find(".user-image-name");
    const errorDisplay = container.find(".image-error");

    fileNameDisplay.text("");
    errorDisplay.text("");

    if (file) {
        const fileExtension = file.name.split('.').pop().toLowerCase();
        const fileSizeMB = file.size / (1024 * 1024);

        if (!allowedExtensions.includes(fileExtension)) {
            errorDisplay.text("Only image files are allowed (jpg, png, gif, webp).");
            $(this).val('');
            return;
        }

        if (fileSizeMB > maxSizeMB) {
            errorDisplay.text("Image size must be less than 2 MB.");
            $(this).val('');
            return;
        }

        fileNameDisplay.text("Selected Image: " + file.name);
    }
});

// $('#FileInput').on('change', function () {
//     const fileName = this.files[0]?.name || "";
//     $('#addUserImageName').text(fileName);

//     if (fileName) {
//         $('#removeImage').removeClass('d-none');
//         $('#RemoveProfileImg').val('false');
//     }
// });

// $('#removeImage').on('click', function (e) {
//     e.stopPropagation();
//     $('#FileInput').val('');
//     $('#addUserImageName').text('');
//     $(".ProfileImagePreview").attr("src","/images/icons/cloud-arrow-up.svg");
//     $(this).addClass('d-none');
//     $('#RemoveProfileImg').val('true');
// });

// if ($('#addUserImageName').text().trim() !== '') {
//     $('#removeImage').removeClass('d-none');
// }


// $("#FileInput").on("change", function (event) {
//     var file = event.target.files[0];
//     if (file) {
//         var reader = new FileReader();
//         reader.onload = function (e) {
//             $(".ProfileImagePreview").attr("src", e.target.result);
//         };
//         reader.readAsDataURL(file);
//     }
// });


    // Handle file change and preview
    $("#FileInput").on("change", function (event) {
        var file = event.target.files[0];
        if (file) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $(".ProfileImagePreview").attr("src", e.target.result);
            };
            reader.readAsDataURL(file);

            // Show file name and remove icon
            $('#addUserImageName').text(file.name);
            $('#removeImage').removeClass('d-none');
            $('#RemoveProfileImg').val('false'); // reset flag
        }
    });

    // Remove image logic
    $('#removeImage').on('click', function (e) {
        e.stopPropagation();
        $('#FileInput').val('');
        $('#addUserImageName').text('');
        $(".ProfileImagePreview").attr("src", "/images/icons/cloud-arrow-up.svg");
        $(this).addClass('d-none');
        $('#RemoveProfileImg').val('true'); 
    });

    if ($('#addUserImageName').text().trim() !== '') {
        $('#removeImage').removeClass('d-none');
    }

