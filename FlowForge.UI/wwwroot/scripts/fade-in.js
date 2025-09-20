document.addEventListener('DOMContentLoaded', function () {
    const cards = document.querySelectorAll('.fade-in');
    cards.forEach((card, index) => {
        setTimeout(() => {
            card.classList.add('show');
        }, 50 * index);
    });
});