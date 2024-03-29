$('#PhotoFile').on("change", function () {
    var file = $(this)[0].files[0];
    if (file) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('.preview-image').attr('src', e.target.result);
        }
        reader.readAsDataURL(file);
    } else {
        $('.preview-image').attr('src', '');
    }
});