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
function preventDefaultDrag(event) {
    event.preventDefault(); // Prevent the default drag behavior
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

window.attachResizerListeners = function () {
    console.log("Re-attaching resizer event listeners...");

    const resizer = document.getElementById("resizer");
    const gridContainer = document.getElementById("gridContainer");
    const detailPanel = document.getElementById("detailPanel");

    if (!resizer || !gridContainer || !detailPanel) {
        console.error("One or more elements are missing!");
        return;
    }

    let isResizing = false;
    let startX = 0;
    let initialGridWidth = 0;
    let initialDetailWidth = 0;

    resizer.addEventListener("mousedown", function (e) {
        if (e.button !== 0) return; // Left-click only

        console.log("Left mouse button pressed on resizer!");
        isResizing = true;
        startX = e.clientX;
        initialGridWidth = gridContainer.offsetWidth;
        initialDetailWidth = detailPanel.offsetWidth;

        // Prevent text selection during resize
        document.body.style.userSelect = "none";

        document.addEventListener("mousemove", resizePanels);
        document.addEventListener("mouseup", stopResize);
    });

    function resizePanels(e) {
        if (!isResizing) return;

        let deltaX = e.clientX - startX;
        let newGridWidth = initialGridWidth + deltaX;
        let newDetailWidth = initialDetailWidth - deltaX;

        // Set minimum and maximum width constraints
        const minGridWidth = 300; // Minimum width for the grid
        const minDetailWidth = 300; // Minimum width for detail panel
        const maxGridWidth = window.innerWidth - minDetailWidth;
        const maxDetailWidth = window.innerWidth - minGridWidth;

        // Constrain the cursor movement
        if (newGridWidth < minGridWidth) {
            newGridWidth = minGridWidth;
            newDetailWidth = window.innerWidth - minGridWidth;
        } else if (newDetailWidth < minDetailWidth) {
            newDetailWidth = minDetailWidth;
            newGridWidth = window.innerWidth - minDetailWidth;
        }

        gridContainer.style.width = `${newGridWidth}px`;
        detailPanel.style.width = `${newDetailWidth}px`;

        console.log(`Resizing: Grid ${newGridWidth}px, Detail ${newDetailWidth}px`);
    }

    function stopResize() {
        if (!isResizing) return;
        console.log("Resizing stopped!");

        isResizing = false;
        document.body.style.userSelect = ""; // Re-enable text selection
        document.removeEventListener("mousemove", resizePanels);
        document.removeEventListener("mouseup", stopResize);
    }
};
window.updatePanelVisibility = function (isOpen) {
    const gridContainer = document.getElementById("gridContainer");
    const detailPanel = document.getElementById("detailPanel");
    const resizer = document.getElementById("resizer");

    if (isOpen) {
        detailPanel.classList.add("open");
        detailPanel.classList.remove("closed");
        resizer.style.display = "block";

        gridContainer.style.width = "60%";  // Default width when opening
        detailPanel.style.width = "40%";
    } else {
        detailPanel.classList.remove("open");
        detailPanel.classList.add("closed");
        resizer.style.display = "none"; // Hide the resizer when panel is closed

        gridContainer.style.width = "100%"; // Expand grid to full width
        detailPanel.style.width = "0"; // Set panel width to 0 when closed
    }
};




