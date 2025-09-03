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
//window.clipboard = {
//    copy: function (text) {
//        var textarea = document.createElement("textarea");
//        textarea.textContent = text;
//        document.body.appendChild(textarea);
//        textarea.select();
//        try {
//            return document.execCommand("copy");  // Security exception may be thrown by some browsers.
//        } catch (ex) {
//            console.warn("Copy to clipboard failed.", ex);
//            return false;
//        } finally {
//            document.body.removeChild(textarea);
//        }
//    }
//}
window.clipboard = {
    copy: async function (text) {
        try {
            await navigator.clipboard.writeText(text);
            console.log("Text copied to clipboard");
            return true;
        } catch (err) {
            console.warn("Copy to clipboard failed", err);
            return false;
        }
    }
};

function enforceLineLimit(element) {
    let lines = element.value.split('\n');
    if (lines.length > 5) {
        element.value = lines.slice(0, 5).join('\n');
    }
}

function enforceCharacterLimit(element, maxlength) {
    let characters = element.value?.length;
    if (characters > maxlength) {
        element.value = characters.slice(0, maxlength);
    }
}

/* ** File Uploader ** */

window.initUploadAreas = function () {
    const uploadAreas = document.querySelectorAll('.upload-area');

    uploadAreas.forEach(area => {

        if (area.dataset.bound === "true") return;

        area.dataset.bound = "true";

        ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
            area.addEventListener(eventName, preventDefaults, false);
        });

        ['dragenter', 'dragover'].forEach(eventName => {
            area.addEventListener(eventName, highlight, false);
        });

        ['dragleave', 'drop'].forEach(eventName => {
            area.addEventListener(eventName, unhighlight, false);
        });

        area.addEventListener('drop', handleDrop, false);
    });

    function preventDefaults(e) {
        e.preventDefault();
        e.stopPropagation();
    }

    function highlight(e) {
        e.currentTarget.classList.add('dragover');
    }

    function unhighlight(e) {
        e.currentTarget.classList.remove('dragover');
    }

    function handleDrop(e) {
        const dt = e.dataTransfer;
        const files = dt.files;
        if (files.length > 0) {
            document.getElementById('fileInput').files = files;
            document.getElementById('fileInput').dispatchEvent(new Event('change', { bubbles: true }));
        }
    }
};
