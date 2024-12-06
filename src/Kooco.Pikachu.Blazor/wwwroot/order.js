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

window.downloadPdfFile = function (filePath, mimeType, fileName) {
    fetch(filePath)
        .then(response => response.blob())
        .then(blob => {
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.style.display = 'none';
            a.href = url;
            a.download = fileName;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
        })
        .catch(err => console.error('Error downloading file:', err));
}

window.uncheckOtherCheckbox = function (method) {
    document.getElementById(method).checked = false;
};
window.executeScript = function (script) {
    try {
        // Use Function constructor to create a function from the script
        var func = new Function(script);
        // Execute the function
        func();
    } catch (error) {
       
    }
};
window.setCookie = function (name, value, sameSite, secure) {
    var cookieString = name + "=" + value + "; SameSite=" + sameSite + "; Secure=" + secure;
    document.cookie = cookieString;
};
window.changeFormActionAndSubmit = async function (formId, newAction) {
    var form = document.getElementById(formId);

    if (form) {
        form.action = newAction;
        form.submit();
        // Perform an AJAX request
        var formData = new FormData(form);
        //var response = await fetch(form.action, {
        //    method: form.method,
        //    body: formData
        //});

        //// Handle the response as needed (e.g., get HTML content)
        //var htmlContent = await response.text();
        //console.log(htmlContent);
    } else {
        console.error('Form not found');
    }
};
window.openPopup = function (htmlContent) {
    var popupWindow = window.open("", "_blank", "width=600,height=400,scrollbars=yes,resizable=yes");

    popupWindow.document.open();
    popupWindow.document.write(htmlContent);
    popupWindow.document.close();
};

window.openInNewTab = function (url) {
    window.open(url, '_blank');
}

function submitForm() {
    var formElement = document.getElementById('formid');
    debugger;
    var formData = new FormData(formElement);

    // Get form values using FormData object
    var name = formData.get('name');

    // Call Blazor page handler method
    DotNet.invokeMethodAsync('YourBlazorApp', 'HandleFormSubmission', name)
        .then(result => {
            // Handle the result if needed
        })
        .catch(error => {
            console.error('Error calling Blazor method:', error);
        });
}