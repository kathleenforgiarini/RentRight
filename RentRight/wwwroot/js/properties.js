function expandText(button) {
    var cardBody = button.parentElement;
    var cardText = cardBody.querySelector('.card-text');
    var fullText = cardBody.querySelector('.card-text:last-of-type');

    cardText.style.maxHeight = 'none'; // Remove a altura máxima
    fullText.style.display = 'block'; // Exibe o texto completo
    button.style.display = 'none'; // Esconde o botão "Leia mais"
}