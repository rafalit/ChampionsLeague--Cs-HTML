// Obsługa komunikatów po zarejestrowaniu konta
const registerParams = new URLSearchParams(window.location.search);
if (registerParams.has('successMessage')) {
    const successMessage = registerParams.get('successMessage');
    displayMessage(successMessage);
}

// Obsługa komunikatów po próbie logowania
const loginParams = new URLSearchParams(window.location.search);
if (loginParams.has('errorMessage')) {
    const errorMessage = loginParams.get('errorMessage');
    displayMessage(errorMessage);
}

// Funkcja do wyświetlania komunikatu
function displayMessage(message) {
    const errorMessageElement = document.getElementById('error-message');
    errorMessageElement.textContent = message;
    errorMessageElement.style.display = 'block';
}
