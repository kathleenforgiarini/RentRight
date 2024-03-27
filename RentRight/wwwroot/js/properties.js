$('#PhotoFile').on("change", function () {
    var file = $(this)[0].files[0];
    if (file) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('.preview-image').attr('src', e.target.result);
        }
        reader.readAsDataURL(file);
    } else {
        // Se nenhum arquivo for selecionado, exibir uma imagem padrão ou limpar a imagem atual
        $('.preview-image').attr('src', 'caminho/para/imagem/padrao.jpg');
        // Ou
        // $('.preview-image').attr('src', '');
    }
});
