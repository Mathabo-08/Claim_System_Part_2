document.addEventListener('DOMContentLoaded', function () {
    const form = document.querySelector('form');
    const fileInput = document.querySelector('input[type="file"]');

    form.addEventListener('submit', function (event) {
        if (fileInput.files.length > 0) {
            const fileSize = fileInput.files[0].size;

            // Check file size (5 MB limit)
            if (fileSize > 5 * 1024 * 1024) {
                alert('File size exceeds the 5 MB limit.');
                event.preventDefault(); // Prevent form submission
            }
        } else {
            alert('Please select a file to upload.');
            event.preventDefault(); // Prevent form submission
        }
    });
});
