window.downloadFile = function (data) {
    var blob = new Blob([new Uint8Array(data.byteArray)], { type: data.contentType });
    var url = window.URL.createObjectURL(blob);
    var a = document.createElement('a');
    a.href = url;
    a.download = data.fileName;
    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(url);
}