window.downloadFile = (fileName, mimeType, fileData) => {
    const blob = new Blob([fileData], { type: mimeType });
    const link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};

// wwwroot/domain.js
window.getDomain = function () {
    return window.location.hostname;
}

// Add this JavaScript code to your Blazor app
window.viewDocument = function (fileContent, mimeType) {
    // Assuming PDF.js is loaded (you might need to include it in your project)
    const pdfData = new Uint8Array(fileContent);
    const blob = new Blob([pdfData], { type: mimeType });
    const url = URL.createObjectURL(blob);

    // Open the document in a new window/tab
    window.open(url, '_blank');
};
window.downloadFile = (fileName, content) => {
    const link = document.createElement('a');
    link.download = fileName;
    link.href = 'data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,' + content;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};

