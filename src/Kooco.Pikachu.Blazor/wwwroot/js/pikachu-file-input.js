window.initPikachuFileInput = function (randomId, isMultiple) {
    const container = document.getElementById(`pikachuFileInputContainer_${randomId}`);
    const uploadArea = document.getElementById(`pikachuFileUploadArea_${randomId}`);
    const fileInput = document.getElementById(`pikachuFileInput_${randomId}`);

    if (!container || !uploadArea || !fileInput) {
        console.error('PikachuFileInput: Required elements not found');
        return;
    }

    // Prevent multiple bindings
    if (container.dataset.bound === "true") return;
    container.dataset.bound = "true";

    // Store reference to prevent memory leaks
    const eventHandlers = {
        preventDefaults,
        highlight,
        unhighlight,
        handleDrop
    };

    // Prevent default drag behaviors
    ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
        container.addEventListener(eventName, eventHandlers.preventDefaults, false);
        document.body.addEventListener(eventName, eventHandlers.preventDefaults, false);
    });

    // Highlight drop area when item is dragged over it
    ['dragenter', 'dragover'].forEach(eventName => {
        container.addEventListener(eventName, eventHandlers.highlight, false);
    });

    ['dragleave', 'drop'].forEach(eventName => {
        container.addEventListener(eventName, eventHandlers.unhighlight, false);
    });

    // Handle dropped files
    container.addEventListener('drop', eventHandlers.handleDrop, false);

    // Store cleanup function for later use
    container.pikachuCleanup = function () {
        ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
            container.removeEventListener(eventName, eventHandlers.preventDefaults, false);
            document.body.removeEventListener(eventName, eventHandlers.preventDefaults, false);
        });

        ['dragenter', 'dragover'].forEach(eventName => {
            container.removeEventListener(eventName, eventHandlers.highlight, false);
        });

        ['dragleave', 'drop'].forEach(eventName => {
            container.removeEventListener(eventName, eventHandlers.unhighlight, false);
        });

        container.removeEventListener('drop', eventHandlers.handleDrop, false);
    };

    function preventDefaults(e) {
        e.preventDefault();
        e.stopPropagation();
    }

    function highlight(e) {
        uploadArea.classList.add('dragover');
    }

    function unhighlight(e) {
        if (!container.contains(e.relatedTarget) && e.relatedTarget !== null) {
            uploadArea.classList.remove('dragover');
        }
    }

    function handleDrop(e) {
        uploadArea.classList.remove('dragover');

        const dt = e.dataTransfer;
        const files = dt.files;

        if (files.length > 0) {
            // For single file mode, only take the first file
            const filesToAdd = isMultiple ? Array.from(files) : [files[0]];

            // Create a new DataTransfer object to set files
            const dataTransfer = new DataTransfer();
            filesToAdd.forEach(file => {
                dataTransfer.items.add(file);
            });

            // Set the files to the input element
            fileInput.files = dataTransfer.files;

            // Trigger the change event
            const changeEvent = new Event('change', {
                bubbles: true,
                cancelable: true
            });
            fileInput.dispatchEvent(changeEvent);
        }
    }
};

// Cleanup function for when component is disposed
window.cleanupPikachuFileInput = function (randomId) {
    const container = document.getElementById(`pikachuFileInputContainer_${randomId}`);
    if (container && container.pikachuCleanup) {
        container.pikachuCleanup();
        delete container.pikachuCleanup;
        container.removeAttribute('data-bound');
    }
};