const container = $("#container");
const registerBtn = $("#register");
const loginBtn = $("#login");
registerBtn.on("click", () => {
    container.addClass("active");
});
loginBtn.on("click", () => {
    container.removeClass("active");
    $('.loginForm').show();

});

$(".togglePassword").on('click', function () {
    $(this).toggleClass("fa-eye fa-eye-slash");
    let input = $("#password");
    if (input.attr("type") === "password") {
        input.attr("type", "text");
    } else {
        input.attr("type", "password");
    }
});
$(".togglePasswordSignUp").on('click', function () {
    $(this).toggleClass("fa-eye fa-eye-slash");
    let input = $("#passwordSignUp");
    if (input.attr("type") === "password") {
        input.attr("type", "text");
    } else {
        input.attr("type", "password");
    }
});
$(".togglePasswordConfirmSignUp").on('click', function () {
    $(this).toggleClass("fa-eye fa-eye-slash");
    let input = $("#passwordConfirmSignUp");
    if (input.attr("type") === "password") {
        input.attr("type", "text");
    } else {
        input.attr("type", "password");
    }
});


if (!showLoginForm) {
    $('#container').addClass("active");
    $('.loginForm').hide();
}


if (errorMessage !== "") {
    new Notify({
        status: 'error',
        title: errorMessage,
        position: 'x-center',
        effect: 'slide',
        autotimeout: 5000,
        speed: 200,
    })
}