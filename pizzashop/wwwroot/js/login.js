    $(document).ready(function () {
      $("#forgotPasswordLink").click(function () {
        var email = $("#email").val();
        $(this).attr("href", '@Url.Action("ForgotPassword", "Auth")' + '?Email=' + encodeURIComponent(email));
      });
    });

    $(document).ready(function () {
      const $passfield = $('#password');
      $('#togglePassword').on('click', function () {
        const type = $passfield.attr('type') === 'password' ? 'text' : 'password';
        $passfield.attr('type', type);
        $(this).toggleClass('fa-eye-slash');
      });
    });