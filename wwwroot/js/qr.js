// Renders the server-provided authenticator URI as a QR code.
window.addEventListener("load", function () {
    var dataElement = document.getElementById("qrCodeData");
    if (!dataElement) {
        return;
    }

    var uri = dataElement.getAttribute("data-url");
    if (!uri) {
        return;
    }

    new QRCode(document.getElementById("qrCode"), {
        text: uri,
        width: 160,
        height: 160
    });

    var notice = document.getElementById("qrCodeNotice");
    if (notice) {
        notice.style.display = "none";
    }
});
