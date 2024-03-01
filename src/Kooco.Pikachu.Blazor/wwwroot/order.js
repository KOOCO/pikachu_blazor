window.removeSelectClass = function (elementId) {
        var element = document.getElementById(elementId);
        if (element) {
            element.classList.remove("form-select");
        }
    };
    window.removeInputClass = function (elementId) {
        var element = document.getElementById(elementId);
        if (element) {
            element.classList.remove("form-control");
        }
    };
    window.downloadFile = function (data) {
        debugger;
        var blob = new Blob([new Uint8Array(data.byteArray)], { type: data.contentType });
        var url = window.URL.createObjectURL(blob);
        var a = document.createElement('a');
        a.href = url;
        a.download = data.fileName;
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
}
window.uncheckOtherCheckbox = function (method) {
    document.getElementById(method).checked = false;
};
