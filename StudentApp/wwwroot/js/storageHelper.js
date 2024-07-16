// storageHelper.js

// Function to set a value in local storage
function setLocalStorage(key, value) {
    localStorage.setItem(key, value);
}

// Function to get a value from local storage
function getLocalStorage(key) {
    return localStorage.getItem(key);
}

// Function to set a value in session storage
function setSessionStorage(key, value) {
    sessionStorage.setItem(key, value);
}

// Function to get a value from session storage
function getSessionStorage(key) {
    return sessionStorage.getItem(key);
}
